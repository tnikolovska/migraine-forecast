pipeline {
    agent any

    environment {
        IMAGE_NAME = "migraineapi-app"
        REGISTRY = "host.docker.internal:5001"
        APP_NAME = "migraineapi-app"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Debug Docker') {
            steps {
                sh '''
                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker run --rm hello-world
                '''
            }
        }

        stage('Verify Files') {
            steps {
                sh '''
                echo "Workspace content:"
                ls -la
                '''
            }
        }

        stage('Find Test Projects') {
                steps {
                    sh '''
                    echo "Searching for test projects..."
                    find $WORKSPACE -name "*Tests*.csproj"
                    '''
                }
            }

        // ❌ REMOVED .NET build (Docker already does it)

        stage('Docker Build') {
            steps {
                sh '''
                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker rm -f migraineapi-app-container || true

                docker build \
                  -t ${IMAGE_NAME}:${BUILD_NUMBER} \
                  backend/MigraineForecast.API
                '''
            }
        }

       /* stage('Run Services') {
            steps {
                sh '''
                docker run -d \
                  --name migraineapi-app-container \
                  -p 5050:80 \
                  ${IMAGE_NAME}:${BUILD_NUMBER}

                sleep 10
                '''
            }
        }*/

    stage('Build Frontend') {
        steps {
            sh '''
                docker build -t migraine-frontend:latest ./frontend/migraine-frontend
            '''
        }
    }

    stage('Run Services') {
    steps {
        sh '''
            docker rm -f migraine-backend || true
            docker rm -f migraine-db || true
            docker rm -f migraine_frontend || true

            docker network create migraine-net || true

            # DB
            docker run -d \
              --name migraine-db \
              --network migraine-net \
              -e POSTGRES_DB=migraine_db \
              -e POSTGRES_USER=postgres \
              -e POSTGRES_PASSWORD=password \
              postgres:15

            sleep 5

            # Backend
           docker run -d \
            --name migraine-backend \
            --network migraine-net \
            -p 5000:8080 \
            -e "ConnectionStrings__DefaultConnection=Host=migraine-db;Port=5432;Database=migraine_db;Username=postgres;Password=password" \
            -e "ConnectionStrings__Default=Host=migraine-db;Port=5432;Database=migraine_db;Username=postgres;Password=password" \
            ${IMAGE_NAME}:${BUILD_NUMBER}

            sleep 5

            # Frontend
            docker run -d \
              --name migraine_frontend \
              --network migraine-net \
              -p 5173:80 \
              migraine-frontend:latest

            sleep 10


            docker ps -a | grep migraine-backend || true

            echo "=== Running containers ==="
            docker ps

            echo "=== Backend logs ==="
            docker logs migraine-backend || true

            echo "=== Frontend logs ==="
            docker logs migraine_frontend || true
        '''
    }
}

        // ✅ Only run if you ACTUALLY have tests
     /*stage('Integration Tests') {
            steps {
                sh '''
                    set -e

                    TEST_PROJECT=$(find . -name "*Tests*.csproj" | head -n 1)

                    echo "Detected test project: $TEST_PROJECT"

                    if [ -z "$TEST_PROJECT" ]; then
                        echo "No test project found. Failing build."
                        exit 1
                    fi

                    docker run --rm \
                        --volumes-from devops-jenkins-1 \
                        -w "$WORKSPACE" \
                        mcr.microsoft.com/dotnet/sdk:9.0 \
                        dotnet test "$TEST_PROJECT" -c Release
                '''
            }
        }*/

       stage('Integration Tests') {
            steps {
                sh '''
                    set -e

                    TEST_PROJECT=$(find . -name "*Tests*.csproj" | head -n 1)

                    echo "Detected test project: $TEST_PROJECT"

                    docker run --rm \
                        --volumes-from devops-jenkins-1 \
                        -v /var/run/docker.sock:/var/run/docker.sock \
                        -e DOCKER_HOST=unix:///var/run/docker.sock \
                        -e TESTCONTAINERS_RYUK_DISABLED=true \
                        -e TESTCONTAINERS_CHECKS_DISABLE=true \
                        -e TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal \
                        -w "$WORKSPACE" \
                        mcr.microsoft.com/dotnet/sdk:9.0 \
                        dotnet test "$TEST_PROJECT" -c Release
                '''
            }
        }


        stage('Push to Nexus') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'nexus-cred', usernameVariable: 'NEXUS_USER', passwordVariable: 'NEXUS_PASS')]) {
                    sh '''
                        docker tag migraineapi-app:${BUILD_NUMBER} host.docker.internal:5001/migraineapi-app:${BUILD_NUMBER}

                        echo "$NEXUS_PASS" | docker login host.docker.internal:5001 -u "$NEXUS_USER" --password-stdin

                        docker push host.docker.internal:5001/migraineapi-app:${BUILD_NUMBER}
                    '''
                }
            }
        }

        //Blue-Green Deployment

        stage('Start Nginx Load Balancer') {
            steps {
                sh '''
                docker rm -f nginx-lb || true

                docker run -d \
                --name nginx-lb \
                -p 80:80 \
                nginx:alpine

                sleep 3

                docker cp nginx.conf nginx-lb:/etc/nginx/nginx.conf

                docker exec nginx-lb nginx -s reload
                '''
            }
        }

        stage('Determine Target Color') {
            steps {
                script {
                    // Проверка што работи моментално
                    def running = sh(script: "docker ps --format '{{.Names}}'", returnStdout: true).trim()
                    
                    if (running.contains("${APP_NAME}-blue")) {
                        env.TARGET_COLOR = "green"
                        env.OLD_COLOR = "blue"
                        env.TARGET_PORT = "5052"
                        env.OLD_PORT = "5051"
                    } else {
                        env.TARGET_COLOR = "blue"
                        env.OLD_COLOR = "green"
                        env.TARGET_PORT = "5051"
                        env.OLD_PORT = "5052"
                    }
                    echo "Deploying to ${env.TARGET_COLOR} on port ${env.TARGET_PORT}. Current active is ${env.OLD_COLOR}."
                }
            }
        }

        stage('Deploy to Inactive Environment') {
            steps {
                sh '''
                    echo "Deploying ${TARGET_COLOR}..."

                    # Remove old container if exists
                    docker rm -f migraineapi-app-${TARGET_COLOR} || true

                    # Run new version
                    docker run -d \
                    --name migraineapi-app-${TARGET_COLOR} \
                    --network migraine-net \
                    -p ${TARGET_PORT}:8080 \
                    -e "ConnectionStrings__DefaultConnection=Host=migraine-db;Port=5432;Database=migraine_db;Username=postgres;Password=password" \
                    -e "ConnectionStrings__Default=Host=migraine-db;Port=5432;Database=migraine_db;Username=postgres;Password=password" \
                    ${REGISTRY}/${APP_NAME}:${BUILD_NUMBER}

                    echo "Waiting for application to start..."
                    sleep 15

                    echo "=== Container status ==="
                    docker ps -a | grep migraineapi-app-${TARGET_COLOR} || true

                    echo "=== Container logs ==="
                    docker logs migraineapi-app-${TARGET_COLOR} || true
                '''
            }
        }

        stage('Switch Traffic (Nginx Reload)') {
            steps {
                script {
                    // Го менуваме upstream во nginx.conf од старата порта на новата порта
                    // ПРЕДУПРЕДУВАЊЕ: Осигурај се дека во nginx.conf почетната порта се совпаѓа со OLD_PORT
                    /*sh "sed -i 's/:${env.OLD_PORT}/:${env.TARGET_PORT}/g' ./nginx.conf"
                    
                    // Копирање на новиот конф и релоад
                    sh "docker cp ./nginx.conf nginx-lb:/etc/nginx/nginx.conf"
                    sh "docker exec nginx-lb nginx -s reload"
                    
                    echo "Traffic successfully switched to ${env.TARGET_COLOR}"*/

                    // Менуваме директно ВНАТРЕ во контејнерот за да го избегнеме "device busy" на хостот
                    // 1. Прво го копираме локалниот nginx.conf во контејнерот (овој пат ќе дозволи бидејќи нема -v)
                    sh "docker cp ./nginx.conf nginx-lb:/etc/nginx/nginx.conf"

                    // 2. Сега го извршуваме sed внатре во контејнерот врз копираниот фајл
                    sh "docker exec nginx-lb sed -i 's/:${env.OLD_PORT}/:${env.TARGET_PORT}/g' /etc/nginx/nginx.conf"
            
                    // 3. Релоад
                    sh "docker exec nginx-lb nginx -s reload"
            
                    echo "Traffic successfully switched to ${env.TARGET_COLOR}"
                }
            }
        }

        stage('Cleanup Old Version') {
            steps {
                script {
                    // Сега кога сообраќајот е префрлен, ја гасиме претходната верзија
                    echo "Stopping old version: ${APP_NAME}-${env.OLD_COLOR}"
                    sh "docker stop ${APP_NAME}-${env.OLD_COLOR} || true"
                    sh "docker rm ${APP_NAME}-${env.OLD_COLOR} || true"
                }
            }
        }


    }

   post {
        always {
            sh '''
                docker rm -f migraine-backend || true
                # docker rm -f migraine-db || true
                # docker rm -f migraine_frontend || true
                # docker network rm migraine-net || true
            '''
        }

        failure {
            script {
                // Ако нешто падне за време на пајплајнот, избриши го неуспешниот TARGET контејнер
                echo "Deployment failed. Cleaning up targeted container..."
                sh "docker stop ${APP_NAME}-${env.TARGET_COLOR} || true"
                sh "docker rm ${APP_NAME}-${env.TARGET_COLOR} || true"
            }
        }
    }
}
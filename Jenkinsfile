pipeline {
    agent any

    environment {
        IMAGE_NAME = "migraineapi-app"
        REGISTRY = "host.docker.internal:5001"
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

      stage('Run Services') {
            steps {
                sh '''
                    docker run --rm \
                    -v /var/run/docker.sock:/var/run/docker.sock \
                    -v "$WORKSPACE":/workspace \
                    -w /workspace \
                    docker/compose:1.29.2 \
                    down || true

                    docker run --rm \
                    -v /var/run/docker.sock:/var/run/docker.sock \
                    -v "$WORKSPACE":/workspace \
                    -w /workspace \
                    -e IMAGE_NAME=${IMAGE_NAME} \
                    -e BUILD_NUMBER=${BUILD_NUMBER} \
                    docker/compose:1.29.2 \
                    up -d

                    sleep 10

                    echo "=== Running containers ==="
                    docker ps

                    echo "=== API logs ==="
                    docker logs migraineapi-app-container || true
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
    }

    post {
            always {
                sh '''
                    docker run --rm \
                    -v /var/run/docker.sock:/var/run/docker.sock \
                    -v "$WORKSPACE":/workspace \
                    -w /workspace \
                    docker/compose:1.29.2 \
                    down || true
                '''
            }
        }
}
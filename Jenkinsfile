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

        stage('Run Services') {
            steps {
                sh '''
                docker run -d \
                  --name migraineapi-app-container \
                  -p 5050:80 \
                  ${IMAGE_NAME}:${BUILD_NUMBER}

                sleep 10
                '''
            }
        }

        // ✅ Only run if you ACTUALLY have tests
        stage('Integration Tests') {
            steps {
                sh '''
                docker run --rm \
                -v /var/jenkins_home/workspace/migraineapi-multibranch_main:/src \
                -w /src/backend \
                mcr.microsoft.com/dotnet/sdk:9.0 \
                bash -c "
                    echo 'Root:' && ls -la &&
                    echo 'Backend:' && ls -la /src/backend &&
                    echo 'Tests:' && ls -la /src/backend/MigraineForecastAPI.Tests &&
                    dotnet test /src/backend/MigraineForecastAPI.Tests/MigraineForecastAPI.Tests.csproj -c Release
                "
                '''
            }
        }
        stage('Push to Nexus') {
            steps {
                sh '''
                docker login $REGISTRY -u admin -p Securityobjectives1!

                docker tag ${IMAGE_NAME}:${BUILD_NUMBER} $REGISTRY/${IMAGE_NAME}:${BUILD_NUMBER}
                docker tag ${IMAGE_NAME}:${BUILD_NUMBER} $REGISTRY/${IMAGE_NAME}:latest

                docker push $REGISTRY/${IMAGE_NAME}:${BUILD_NUMBER}
                docker push $REGISTRY/${IMAGE_NAME}:latest
                '''
            }
        }
    }

    post {
        always {
            sh '''
            docker stop migraineapi-app-container || true
            docker rm migraineapi-app-container || true
            '''
        }
    }
}
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
                set -e

                WORKSPACE_PATH="/src"

                TEST_PROJECT=$(find $WORKSPACE_PATH -name "*Tests*.csproj" | head -n 1)

                echo "Detected test project: $TEST_PROJECT"

                docker run --rm \
                    -v $WORKSPACE:$WORKSPACE_PATH \
                    -w $WORKSPACE_PATH \
                    mcr.microsoft.com/dotnet/sdk:9.0 \
                    bash -c "
                        set -e
                        dotnet test $TEST_PROJECT -c Release
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
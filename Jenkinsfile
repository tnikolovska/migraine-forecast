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

        stage('Debug Structure') {
            steps {
                sh '''
                docker run --rm \
                -v $WORKSPACE:/src \
                mcr.microsoft.com/dotnet/sdk:9.0 \
                bash -c "ls -R /src"
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

      /*stage('Build (.NET)') {
        steps {
            sh '''
            docker run --rm -v $WORKSPACE:/src mcr.microsoft.com/dotnet/sdk:9.0 \
            bash -c "ls -R /src/backend"

            docker run --rm -v $WORKSPACE:/src mcr.microsoft.com/dotnet/sdk:9.0 \
            bash -c "find /src -name '*.sln'"

            '''
        }
        }*/

        stage('Build (.NET)') {
            steps {
                sh '''
                docker run --rm \
                -v $WORKSPACE:/src \
                -w /src \
                mcr.microsoft.com/dotnet/sdk:9.0 \
                bash -c "dotnet restore && dotnet build -c Release"
                '''
            }
        }

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

       stage('Integration Tests') {
            steps {
                sh '''
                docker run --rm \
                -v $WORKSPACE:/src \
                -w /src \
                mcr.microsoft.com/dotnet/sdk:9.0 \
                bash -c "dotnet test -c Release"
                '''
            }
        }

        stage('Push to Nexus') {
            steps {
                sh '''
                REGISTRY=host.docker.internal:5001

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
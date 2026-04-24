pipeline {
    agent any

    stages {

        stage('Checkout') {
            steps { checkout scm }
        }

        stage('Debug Docker') {
            steps {
                sh '''
                #!/bin/bash

                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker run --rm hello-world
                '''
            }
        }
                stage('Build') {
                steps {
                    sh '''
                    set -e

                    ls -R

                    cd backend

                    dotnet restore MigraineForecast.API.sln
                    dotnet build MigraineForecast.API.sln -c Release
                    '''
                }
            }

        stage('Docker Build') {
            steps {
                sh '''
                #!/bin/bash

                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker rm -f migraineapi-app-container || true
                docker build -t migraineapi-app:${BUILD_NUMBER} .
                '''
            }
        }

        stage('Run Services') {
            steps {
                sh '''
                #!/bin/bash

                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker run -d --name migraineapi-app-container -p 5050:80 migraineapi-app:${BUILD_NUMBER}
                sleep 10
                '''
            }
        }

        stage('Integration Tests') {
            steps {
                sh '''
                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker run --rm \
                --network host \
                -v $WORKSPACE:/app \
                -w /app/backend \
                mcr.microsoft.com/dotnet/sdk:9.0 \
                dotnet test MigraineForecast.API.sln -c Release
                '''
            }
        }

        stage('Push to Nexus') {
            steps {
                sh '''
                #!/bin/bash

                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                REGISTRY=host.docker.internal:5001

                docker login -u admin -p Securityobjectives1! $REGISTRY
                docker tag migraineapi-app:${BUILD_NUMBER} $REGISTRY/migraineapi-app:${BUILD_NUMBER}
                docker push $REGISTRY/migraineapi-app:${BUILD_NUMBER}
                '''
            }
        }
    }

    post {
        always {
            sh '''
            #!/bin/bash

            unset DOCKER_HOST
            unset DOCKER_TLS_VERIFY
            unset DOCKER_CERT_PATH
            unset DOCKER_CONTEXT

            docker stop migraineapi-app-container || true
            docker rm migraineapi-app-container || true
            '''
        }
    }
}
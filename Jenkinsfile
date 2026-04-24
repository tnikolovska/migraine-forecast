pipeline {
    agent any

    stages {

        stage('Checkout') {
            steps { checkout scm }
        }

        stage('Debug Docker') {
            steps {
                sh(script: '''
                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker run --rm hello-world
                ''', shell: '/bin/bash')
            }
        }

        stage('Build') {
            steps {
                sh(script: '''
                set -e

                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker run --rm \
                  -v $WORKSPACE:/app \
                  -w /app/backend \
                  mcr.microsoft.com/dotnet/sdk:9.0 \
                  dotnet restore

                docker run --rm \
                  -v $WORKSPACE:/app \
                  -w /app/backend \
                  mcr.microsoft.com/dotnet/sdk:9.0 \
                  dotnet build -c Release
                ''', shell: '/bin/bash')
            }
        }

        stage('Docker Build') {
            steps {
                sh(script: '''
                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker rm -f migraineapi-app-container || true
                docker build -t migraineapi-app:${BUILD_NUMBER} .
                ''', shell: '/bin/bash')
            }
        }

        stage('Run Services') {
            steps {
                sh(script: '''
                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker run -d --name migraineapi-app-container -p 5050:80 migraineapi-app:${BUILD_NUMBER}
                sleep 10
                ''', shell: '/bin/bash')
            }
        }

        stage('Integration Tests') {
            steps {
                sh(script: '''
                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                docker run --rm \
                  --network host \
                  -v $WORKSPACE:/app \
                  -w /app/backend \
                  mcr.microsoft.com/dotnet/sdk:9.0 \
                  dotnet test -c Release
                ''', shell: '/bin/bash')
            }
        }

        stage('Push to Nexus') {
            steps {
                sh(script: '''
                unset DOCKER_HOST
                unset DOCKER_TLS_VERIFY
                unset DOCKER_CERT_PATH
                unset DOCKER_CONTEXT

                REGISTRY=host.docker.internal:5001

                docker login -u admin -p Securityobjectives1! $REGISTRY
                docker tag migraineapi-app:${BUILD_NUMBER} $REGISTRY/migraineapi-app:${BUILD_NUMBER}
                docker push $REGISTRY/migraineapi-app:${BUILD_NUMBER}
                ''', shell: '/bin/bash')
            }
        }
    }

    post {
        always {
            sh(script: '''
            unset DOCKER_HOST
            unset DOCKER_TLS_VERIFY
            unset DOCKER_CERT_PATH
            unset DOCKER_CONTEXT

            docker stop migraineapi-app-container || true
            docker rm migraineapi-app-container || true
            ''', shell: '/bin/bash')
        }
    }
}
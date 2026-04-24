pipeline {
    // Run everything on the Jenkins container (which has our Docker mount)
    agent any

    stages {
        stage('Checkout') {
            steps { checkout scm }
        }

        /*stage('Build') {
            // Use the SDK image as a 'tool' or wrapper, 
            // but keep the execution on the main agent
            steps {
                sh 'docker run --rm -v ${WORKSPACE}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 dotnet restore'
                sh 'docker run --rm -v ${WORKSPACE}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 dotnet build --configuration Release'
            }
        }*/

       /* stage('Build') {
                steps {
                    sh "docker run --rm -v ${WORKSPACE}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 dotnet restore"
                    sh "docker run --rm -v ${WORKSPACE}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 dotnet build --configuration Release"
                }
            }*/

        stage('Build') {
            steps {
                sh '''
                    unset DOCKER_HOST
                    unset DOCKER_TLS_VERIFY
                    unset DOCKER_CERT_PATH

                    docker run --rm \
                    -v $WORKSPACE:/app \
                    -w /app \
                    mcr.microsoft.com/dotnet/sdk:9.0 \
                    dotnet restore
                '''

                sh '''
                    unset DOCKER_HOST
                    unset DOCKER_TLS_VERIFY
                    unset DOCKER_CERT_PATH

                    docker run --rm \
                    -v $WORKSPACE:/app \
                    -w /app \
                    mcr.microsoft.com/dotnet/sdk:9.0 \
                    dotnet build --configuration Release
                '''
            }
        }

        stage('Docker Build') {
            steps {
                sh 'docker rm -f migraineapi-app-container || true'
                sh "docker build -t migraineapi-app:${env.BUILD_NUMBER} ."
            }
        }

        stage('Run Services') {
            steps {
                sh 'docker run -d --name migraineapi-app-container -p 5050:80 migraineapi-app:${BUILD_NUMBER}'
                sh 'sleep 10' 
            }
        }

        stage('Integration Tests') {
            steps {
                // Same logic as Build stage
                sh 'docker run --rm --network host -v ${WORKSPACE}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 dotnet test --configuration Release'
            }
        }

        stage('Push to Nexus') {
            steps {
                script {
                    def registry = "host.docker.internal:5001"
                    sh "docker login -u admin -p Securityobjectives1! ${registry}"
                    sh "docker tag migraineapi-app:${BUILD_NUMBER} ${registry}/migraineapi-app:${BUILD_NUMBER}"
                    sh "docker push ${registry}/migraineapi-app:${BUILD_NUMBER}"
                }
            }
        }
    }

    post {
        always {
            sh 'docker stop migraineapi-app-container || true'
            sh 'docker rm migraineapi-app-container || true'
        }
    }
}
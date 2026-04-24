pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps { checkout scm }
        }

        stage('Build') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:9.0'
                    args '-u root -e DOTNET_CLI_HOME=/tmp/dotnet_home'
                }
            }
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Docker Build') {
            steps {
                sh "docker build -t migraineapi-app:${env.BUILD_NUMBER} ."
            }
        }

        stage('Run Services') {
            steps {
        	// We manually run the image we just built in the previous stage
        	// -d: detached, --name: so we can stop it later, -p: port mapping
        	sh 'docker run -d --name migraineapi-app-container -p 5050:80 migraineapi-app:${BUILD_NUMBER}'
        	sh 'sleep 10' 
   	 }
        }

        stage('Integration Tests') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:9.0'
                    // Ensure the network name matches your docker-compose network
                    args '-u root -e DOTNET_CLI_HOME=/tmp/dotnet_home -e APP_URL=http://localhost:5050 --network host'
                }
            }
            steps {
                sh 'dotnet test --configuration Release --no-build'
            }
        }

        stage('Push to Nexus') {
	    steps{
            script {
            	// host.docker.internal on port 5001 is the magic for Docker Desktop, need to recreate the host.docker.internal:5001
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
        // Stop and remove the specific container we started
        sh 'docker stop migraineapi-app-container || true'
        sh 'docker rm migraineapi-app-container || true'
    }
    }
}
pipeline {
    agent any

    environment {
        AWS_CREDENTIALS_ID = 'aws-eks'
        REGION = 'ap-south-1'
        ECR_REPO = 'microservice-demo'
        GITHUB_REPO = 'https://github.com/gholapk17/dotnet-microservice-demo.git'
    }

    stages {
        stage('Checkout Code') {
            steps {
                git branch: 'main', url: "${env.GITHUB_REPO}"
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    dockerImage = docker.build("${ECR_REPO}:${BUILD_NUMBER}")
                }
            }
        }

        stage('Login to ECR & Push') {
            steps {
                withCredentials([[$class: 'AmazonWebServicesCredentialsBinding', credentialsId: "${env.AWS_CREDENTIALS_ID}"]]) {
                    script {
                        sh """
                        aws ecr get-login-password --region ${REGION} | docker login --username AWS --password-stdin 474668409393.dkr.ecr.${REGION}.amazonaws.com
                        docker tag ${ECR_REPO}:${BUILD_NUMBER} 474668409393.dkr.ecr.${REGION}.amazonaws.com/${ECR_REPO}:${BUILD_NUMBER}
                        docker push 474668409393.dkr.ecr.${REGION}.amazonaws.com/${ECR_REPO}:${BUILD_NUMBER}
                        """
                    }
                }
            }
        }

        stage('Deploy to EKS') {
            steps {
                withCredentials([[$class: 'AmazonWebServicesCredentialsBinding', credentialsId: "${env.AWS_CREDENTIALS_ID}"]]) {
                    script {
                        sh """
                        aws eks --region ${REGION} update-kubeconfig --name hello-cluster
                        sed -i 's|image:.*|image: 474668409393.dkr.ecr.${REGION}.amazonaws.com/${ECR_REPO}:${BUILD_NUMBER}|' hello-deployment.yaml
                        kubectl apply -f hello-deployment.yaml
                        kubectl apply -f hello-service.yaml
                        kubectl apply -f hello-ingress.yaml
                        """
                    }
                }
            }
        }
    }
}


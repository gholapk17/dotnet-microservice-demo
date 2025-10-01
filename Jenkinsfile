pipeline {
    agent any

    environment {
        AWS_REGION = "ap-south-1"
        ECR_REPO = "474668409393.dkr.ecr.ap-south-1.amazonaws.com/microservice-demo"
        IMAGE_TAG = "latest"
    }

    stages {
        stage('Checkout') {
            steps {
                git credentialsId: 'github-ssh', url: 'git@github.com:gholapk17/dotnet-microservice-demo.git', branch: 'main'
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    sh 'docker build -t $ECR_REPO:$IMAGE_TAG .'
                }
            }
        }

        stage('Login to ECR') {
            steps {
                script {
                    sh '''
                        aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $ECR_REPO
                    '''
                }
            }
        }

        stage('Push to ECR') {
            steps {
                sh 'docker push $ECR_REPO:$IMAGE_TAG'
            }
        }

        stage('Deploy to EKS') {
            steps {
                script {
                    sh '''
                        aws eks --region $AWS_REGION update-kubeconfig --name hello-cluster
                        kubectl set image deployment/dotnetapi dotnetapi=$ECR_REPO:$IMAGE_TAG
                        kubectl rollout status deployment/dotnetapi
                    '''
                }
            }
        }
    }
}


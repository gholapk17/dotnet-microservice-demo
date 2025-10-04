pipeline {
    agent any

    environment {
        AWS_REGION = "ap-south-1"
        ECR_REPO = "474668409393.dkr.ecr.ap-south-1.amazonaws.com/microservice-demo"
        IMAGE_TAG = "latest"
        EKS_CLUSTER_NAME = "hello-cluster"  // Replace with your actual EKS cluster name
        DEPLOYMENT_NAME = "dotnetapi"  // Ensure this matches the name of your deployment
        KUBE_NAMESPACE = "default"  // Replace with your actual Kubernetes namespace, if different
    }

    stages {
        stage('Checkout') {
            steps {
                // Use HTTPS URL with GitHub credentials
                git credentialsId: 'github-credentials', url: 'https://github.com/gholapk17/dotnet-microservice-demo.git', branch: 'main'
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    // Build Docker image and tag it with the ECR repository and image tag
                    sh 'docker build -t $ECR_REPO:$IMAGE_TAG .'
                }
            }
        }

        stage('Login to ECR') {
            steps {
                script {
                    // Log in to AWS ECR to push the Docker image
                    sh '''
                        aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $ECR_REPO
                    '''
                }
            }
        }

        stage('Push to ECR') {
            steps {
                // Push the Docker image to ECR
                sh 'docker push $ECR_REPO:$IMAGE_TAG'
            }
        }

        stage('Deploy to EKS') {
            steps {
                script {
                    // Retry deploy to EKS up to 3 times in case of failure
                    retry(3) {
                        // Update kubeconfig for EKS cluster
                        sh '''
                            aws eks --region $AWS_REGION update-kubeconfig --name $EKS_CLUSTER_NAME
                            kubectl set image deployment/$DEPLOYMENT_NAME $DEPLOYMENT_NAME=$ECR_REPO:$IMAGE_TAG -n $KUBE_NAMESPACE
                            kubectl rollout status deployment/$DEPLOYMENT_NAME -n $KUBE_NAMESPACE
                        '''
                    }
                }
            }
        }
    }

    post {
        success {
            echo 'Deployment successful!'
        }
        failure {
            echo 'Deployment failed. Please check the logs for errors.'
        }
    }
}


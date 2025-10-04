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
                git credentialsId: 'github-credentials', url: 'https://github.com/gholapk17/dotnet-microservice-demo.git', branch: 'main'
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
                withCredentials([[$class: 'AmazonWebServicesCredentialsBinding', credentialsId: 'aws-credentials']]) {
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

        stage('Get Latest Image Digest from ECR') {
            steps {
                script {
                    withCredentials([[$class: 'AmazonWebServicesCredentialsBinding', credentialsId: 'aws-credentials']]) {
                        def imageDigest = sh(script: """
                            aws ecr describe-images --repository-name microservice-demo --region $AWS_REGION --query "imageDetails[?imageTags[0]=='latest'].imageDigest" --output text
                        """, returnStdout: true).trim()
                        
                        env.IMAGE_DIGEST = imageDigest
                        echo "Latest image digest: ${env.IMAGE_DIGEST}"
                    }
                }
            }
        }

        stage('Deploy to EKS') {
            steps {
                script {
                    retry(3) {
                        withCredentials([[$class: 'AmazonWebServicesCredentialsBinding', credentialsId: 'aws-credentials']]) {
                            sh '''
                                aws eks --region $AWS_REGION update-kubeconfig --name $EKS_CLUSTER_NAME
                                kubectl set image deployment/$DEPLOYMENT_NAME $DEPLOYMENT_NAME=$ECR_REPO@$IMAGE_DIGEST -n $KUBE_NAMESPACE
                                kubectl rollout status deployment/$DEPLOYMENT_NAME -n $KUBE_NAMESPACE
                            '''
                        }
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


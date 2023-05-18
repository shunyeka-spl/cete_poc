#!/bin/bash
dotnet restore
dotnet build --configuration Release
dotnet publish --configuration Release --runtime linux-x64 --self-contained false -p:PublishReadyToRun=true

# Login with aws-cli to push image to ecr
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 619140547673.dkr.ecr.us-east-1.amazonaws.com

# Building and Pushing Image to ecr repo 
docker build -t 619140547673.dkr.ecr.us-east-1.amazonaws.com/cete_poc .
docker push 619140547673.dkr.ecr.us-east-1.amazonaws.com/cete_poc:latest

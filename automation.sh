#!/bin/bash

# Navigate to the git directory
cd /home/ubuntu/docker/ai_chan/test_voluma/Ai-Chan

#pull new version from github
git pull

# Stop the running container
docker stop aichan

# Remove the stopped container
docker rm aichan

#remove old image
docker rmi aichan:latest

#build image
docker build -t aichan:latest . 
sleep 5
#run again
docker run -d -v /home/ubuntu/docker/ai_chan/test_voluma/Ai-Chan/persist_data:/app/data --name aichan aichan:latest

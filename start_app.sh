#!/bin/bash

docker-compose up -d

echo "Waiting for infrastructures to start..."

sleep 10

echo "Starting PlayerFeedbackService..."

docker build -t playerfeedbackserviceimg .
docker run --rm -p 5000:5000 --name playerfeedbackservice --network=host playerfeedbackserviceimg

echo "Stopping PlayerFeedbackService..."

docker stop playerfeedbackservice

echo "Stopping infrastructures..."

docker-compose stop

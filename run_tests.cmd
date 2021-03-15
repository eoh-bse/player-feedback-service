docker build -f Dockerfile.tests -t playerfeedbackservicetestsimg .

docker run --rm --name playerfeedbackservicetests playerfeedbackservicetestsimg

cd C:\Users\Drumm\git\ceabot
git pull
docker build . --file ./Bot.DockerService/Dockerfile --tag ceabot:local
docker container kill local_ceabot
docker container rm local_ceabot
docker run --name local_ceabot -d ceabot:local
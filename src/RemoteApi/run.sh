dotnet publish -c Release -o publish

cd publish

docker rm -f remote-api
docker rmi remote-api
docker build -t remote-api -f ../../../docker/Dockerfile.api .
docker run --name remote-api -d -p 5000:5000 remote-api

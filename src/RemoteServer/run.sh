dotnet publish -c Release -o publish

cd publish

docker rm -f remote-server
docker rmi remote-server
docker build -t remote-server -f ../../../docker/Dockerfile.server .
docker run --name remote-server -d -p 4040:4040 remote-server

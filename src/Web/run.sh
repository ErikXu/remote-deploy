docker rm -f remote-web
docker rmi remote-web
docker build -t remote-web -f ../../docker/Dockerfile.web .
docker run --name remote-web -d -p 8080:80 remote-web

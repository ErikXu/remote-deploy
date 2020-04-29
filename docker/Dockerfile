FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

#Install ssh-keygen and sshpass
RUN apt-get update -y &&  apt-get install openssh-server -y && apt-get install sshpass

#Install kubectl
RUN curl -LO https://storage.googleapis.com/kubernetes-release/release/`curl -s https://storage.googleapis.com/kubernetes-release/release/stable.txt`/bin/linux/amd64/kubectl
RUN chmod +x ./kubectl
RUN mv ./kubectl /usr/local/bin/kubectl

COPY . /app
WORKDIR /app
EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000/

ENTRYPOINT ["dotnet", "RemoteDeploy.dll"]
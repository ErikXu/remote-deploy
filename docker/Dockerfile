FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

RUN apt-get update -y &&  apt-get install openssh-server -y && apt-get install sshpass

COPY . /app
WORKDIR /app
EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000/

ENTRYPOINT ["dotnet", "RemoteDeploy.dll"]
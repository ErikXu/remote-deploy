FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

RUN apk add openssh-keygen && apk add sshpass

COPY . /app
WORKDIR /app
EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000/

ENTRYPOINT ["dotnet", "RemoteDeploy.dll"]
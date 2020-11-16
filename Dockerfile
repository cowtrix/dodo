FROM ubuntu:18.04

RUN apt update; apt install -y wget apt-transport-https
RUN wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb

RUN apt-get update
RUN apt-get install -y dotnet-sdk-3.1 aspnetcore-runtime-3.1

WORKDIR /app/src/DodoServer
RUN ln -s /bin/echo /usr/bin/xcopy

WORKDIR /app/src/DodoServer/src/DodoServer/
RUN ls

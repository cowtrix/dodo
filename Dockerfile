FROM ubuntu:18.04

RUN apt update; apt install -y wget apt-transport-https
RUN wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb

RUN apt-get update
RUN apt-get install -y dotnet-sdk-3.1 aspnetcore-runtime-3.1

WORKDIR /app/src/DodoServer

#
#
# https://hub.docker.com/_/microsoft-dotnet-core
# FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# WORKDIR /source
# 
# # copy csproj and restore as distinct layers
# COPY *.sln .
# COPY src/ ./src/
# COPY lib/ ./lib/
# COPY test/ ./test/
# RUN dotnet restore
# 
# # copy everything else and build app
# COPY src/. ./src/
# WORKDIR /source/src/DodoServer
# RUN dotnet publish -c release -o /app --no-restore
# 
# # final stage/image
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
# WORKDIR /app
# COPY --from=build /app ./
# ENTRYPOINT ["dotnet", "aspnetapp.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

RUN mkdir /app
RUN mkdir /build
COPY . /app
WORKDIR /app

# Fetch and install Node 10. Make sure to include the --yes parameter 
# to automatically accept prompts during install, or it'll fail.
RUN curl --silent --location https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install --yes nodejs

RUN dotnet restore "src/DodoServer/DodoServer.csproj"
RUN dotnet publish "src/DodoServer/DodoServer.csproj" -c Release --output /app/build

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim

COPY --from=build /app/dodo-frontend/build/ ./build/wwwroot/
COPY --from=build /app/build .

EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS https://*:5001 http://*:5000

ENTRYPOINT ["dotnet", "DodoServer.dll"]
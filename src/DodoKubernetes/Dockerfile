FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["DodoKubernetes/DodoKubernetes.csproj", "DodoKubernetes/"]

RUN dotnet restore "DodoKubernetes/DodoKubernetes.csproj"
COPY . .
WORKDIR "/src/DodoKubernetes"
RUN dotnet build "DodoKubernetes.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DodoKubernetes.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DodoKubernetes.dll"]
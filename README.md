Dodo is a website for organising events, made for Extinction Rebellion by a volunteer team.

# [REST API Documentation](https://documenter.getpostman.com/view/8888079/SW15xbbc?version=latest)

# Deployment

## Deploying via Docker

Dodo contains a docker build script (`~/Dockerfile`), and a docker compose yaml configuration (`~/docker-compose.yml`), which should be everything you need in order to build and run a Dodo server via Docker.

```
docker build . -t dodo:latest
docker pull mongo:latest
docker compose up
```

This will do a few things:

1. Build the docker image, which will contain a release version of the server
2. Download the MongoDB image from [here](https://hub.docker.com/_/mongo/)
3. Launch both containers and set the server up to connect to the database

By default, the `docker-compose` deployment will attempt to bind the configuration file at the root of the .git repository (`~/DodoServer_config.json`) and this is the easiest way to set the configuration of your server. Just edit the file at the root of the git repository where the docker file was built, and it should just work.

## Deploying directly

If you don't wish to use docker, you can deploy Dodo directly on a machine.

### Requirements

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet/3.1)
- [npm](https://www.npmjs.com/get-npm)

To run the project from source:

```
dotnet run -c Release --project .\src\DodoServer\DodoServer.csproj
```

Or, to build the project to a standalone executable:

```
dotnet publish --force src\DodoServer\DodoServer.csproj -o ..\build -c Release
```

## Configuring Your Server

Configuration variables are read from a JSON configuration at the root of the project directory called `DodoServer_config.json`. You can also set these variables by setting an environment variable of the same name.

Your configuration file might look something like this:

```
{
	"LogLevel":3,
	"MongoDBServerURL": "mongodb://username:password@dodo_mongo_1:27017/?authSource=admin"
	"Dodo_SupportEmail": "support@dodo.ovh",
	"NetworkConfig": {
		"SSLPort": 5001,
		"HTTPPort" : 5000,
		"Domains": [ "dodo.ovh" ],
		"LetsEncryptAutoSetup": true,
	},
	"MapBoxGeocodingService_ApiKey": "<api key>",
	"Dodo_EmailConfiguration": {
		"FromEmail": "noreply@dodo.ovh",
		"FromName": "Dodo",
		"SMTPAddress": "smtp.myemailserver.net",
		"SMTPPort": 587,
		"SMTPUsername": "<username>",
		"SMTPPassword": "<password>"
	}
}
```

# Security

Dodo is designed to be highly secure. The developers acknowledge that protest is a politically sensitive activity in many countries, and that individuals should be assured that their information is protected. The philosophy behind the security decisions of Dodo is to encrypt all relational information. This means that even in a threat scenario where the server data is compromised, an attacker will not be able to create any connection between individual users and activity, except where that information is explicitly published by the user. **This security infrastructure results in some caveats that any systems adminstrator should be aware of:**

-   Systems administrators do not have access to a lot of the information stored on the Dodo server. Wherever possible, only the user concerned will have access to information. This includes but is not limited to:
    -   Whether or not a user is an administrator of a given resource.
    -   Whether or not a user is a member of a given group.
-   When a user resets their password, they will lose access to any secure resources. This means that users will have to be re-added as administrators to any groups if they forget their password.

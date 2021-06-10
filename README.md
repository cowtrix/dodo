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

By default, the `docker-compose` deployment will attempt to bind the configuration file at the root of the .git repository (`~/DodoServer_config.json`) and this is the easiest way to set the configuration of your server. Just create and edit the file at the root of the git repository where the docker file was built, and it should just work. **Please be aware that if you do not have a DodoServer_config.json file at the root of your project, the docker compose will not work as it will try to bind a nonexistant file.**

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

You can set configuration values for your server in a few ways:

### JSON File

Configuration variables can be read from a JSON file. This is the recommended way of setting configuration values. By default, Dodo will try to load a file called `DodoServer_config.json` at the root of the solution directory. If you'd like to override the path to this configuration file, run the server with the `--config` argument like so:

`DodoServer.exe --config C:\myconfig.json`

Your configuration JSON file might look something like this:

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

### Environment Variables

You can set environment variables with the same name as a configuration key above, and it will set the configuration value. For instance, I might set the environment variable `Dodo_SupportEmail` to `test@example.com`. Please be aware that values in the JSON file have a higher priority than environment variable and will override them.

# Security

The developers acknowledge that protest is a politically sensitive activity in many countries, and that individuals should be assured that their information is protected. The philosophy behind the security decisions of Dodo is to encrypt all relational information. This means that even in a threat scenario where the server data is compromised, an attacker will not be able to create any connection between individual users and activity, except where that information is explicitly published by the user. **This security infrastructure results in some caveats that any systems adminstrator should be aware of:**

-   Systems administrators do not have access to a lot of the information stored on the Dodo server. Wherever possible, only the user concerned will have access to information. This includes but is not limited to:
    -   Whether or not a user is an administrator of a given resource.
    -   Whether or not a user is a member of a given group.
-   When a user resets their password, they will lose access to any secure resources. This means that users will have to be re-added as administrators to any groups if they forget their password.

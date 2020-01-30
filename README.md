Dodo is a tool for organising Rebellions. A Rebellion is a peaceful, occupational protest. 
Its purpose is to allow Rebels to map out the organisational structure of their protest 
team, support internal communications, recruit volunteers into open roles, and broadcast 
information about the status of the Rebellion.

**Read the [Specification Document](https://docs.google.com/document/d/1yjUkmxTiSCRLJ7weWW5JxJRUwE2LFJmo2SnO-Fwxggk/)** for more information on project goals.

# [Basecamp](https://3.basecamp.com/3559494/projects/14285600)

# [REST API Documentation](https://documenter.getpostman.com/view/8888079/SW15xbbc?version=latest)

# Code Documentation

[API: Common](https://code.organise.earth/sean/dodo/wikis/API:-Common)

[API: SimpleHttpServer](https://code.organise.earth/sean/dodo/wikis/API:-SimpleHttpServer)

[API: Dodo](https://code.organise.earth/sean/dodo/wikis/API:-Dodo)

# Getting Started

## Building From Source 

Dodo is built on .NET Core 3.0. To build from source, you must install the SDK from [here](https://dotnet.microsoft.com/download/dotnet-core/3.0).

From the root directory, run `dotnet build`. The project will be output in the `/bin/` directory.

## System Requirements

Dodo is compatible with Linux and Windows. It requires the .NET Core 3.0 runtime, which you can install for your operating system from [here](https://dotnet.microsoft.com/download/dotnet-core/3.0). 

## Configuring Your Server

Configuration variables are read from a file called `config.json`, which should be formatted as a JSON dictionary. You can find a sample config file within the project called `config.sample.json` which also serves as a list of all variables that you can set. This file needs to be in the root directory of the application. At various points in this setup, you'll need to open up this file and set a variable to the given value.

### 1 - Install or set up MongoDB

Dodo requires a MongoDB server to store its data. We strongly recommend that you host the server on hardware that you directly control, and not through a cloud service such as Azure. You can download a free, local MongoDB server [here](https://www.mongodb.com/community).

If you are locally hosting the MongoDB server, move to the next step. If not, you will need to define the configuration variable `MongoDBServerURL` to the URL of your database. If you need to authenticate to the server, you should be able to do this within the connection string (e.g. "mongodb://user1:password1@localhost/test").

### 2 - Set up a SendGrid account

Dodo uses [SendGrid](https://sendgrid.com/) to manage and send emails. To link your server to a SendGrid account, you will need to get an API key from the SendGrid website, and set the `SendGrid_APIKey` configuration value to this key. Without this, users will not receive emails from your server. Additionally, you may wish to customize the `Email_FromEmail` and `Email_FromName` fields, which will change how emails are displayed when received by users.

### 3 - Set up your SSL Certificate

// TODO: rewrite after ASP.NET
Dodo expects to load a `.pfx` file containing the SSL certificate it will use to validate HTTPS connections. The relative path to this certificate is defined with the `SSLCertificatePath` configuration variable.

### 3 - Launch the server

You're ready to go! Just run the executable Dodo.executable

# Security

Dodo is designed to be highly secure. The developers acknowledge that protest is a politically sensitive activity in many countries, and that individuals should be assured that their information is protected. The philosophy behind the security decisions of Dodo is to encrypt all relational information. This means that even in a threat scenario where the server data is compromised, an attacker will not be able to create any connection between individual users and activity, except where that information is explicitly published by the user. **This security infrastructure results in some caveats that any systems adminstrator should be aware of:**

- Systems adminstrators do not have access to a lot of the information stored on the Dodo server. Wherever possible, only the user concerned will have access to information. This includes but is not limited to:
  - Whether or not a user is an adminstrator of a given resource.
  - Whether or not a user is a member of a given group.
- When a user resets their password, they will lose access to any secure resources. This means that users will have to be readded as administrators to any groups if they forget their password.
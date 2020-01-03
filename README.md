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

## Configuring Your Server

### 1 - Install or set up MongoDB

Dodo requires a MongoDB server to store its data. We strongly recommend that you host the server on hardware that you directly control, and not through a cloud service such as Azure. You can download a free, local MongoDB server [here](https://www.mongodb.com/community).

## Security

Dodo is designed to be highly secure. The developers acknowledge that protest is a politically sensitive activity in many countries, and that individuals should be assured that their information is protected. The philosophy behind the security decisions of Dodo is to encrypt all relational information. This means that even in a threat scenario where the server data is compromised, an attacker will not be able to create any connection between individual users and activity, except where that information is explicitly published by the user. This security infrastructure results in some caveats that any systems adminstrator should be aware of:

- Systems adminstrators do not have access to a lot of the information stored on the Dodo server. Wherever possible, only the user concerned will have access to information. This includes but is not limited to:
  - Whether or not a user is an adminstrator of a given resource.
  - Whether or not a user is a member of a given group.
- When a user resets their password, they will lose access to any secure resources. This means that users will have to be readded as administrators to any groups if they forget their password.
# Infrastructure

NX.Workspace is designed to run in any cloud environment using containers to hold each sub-system.

You can see the of each container by using the ``Site Manager`` tool located in the ``System`` Start menu entry.

![image](/help/info/images/SiteManager.png)

## Containers

### Processor

The processor containers handle all the calls from the browsers.  They also run all chores.

### MongoDb

The mongodb container holds the database.

### SocketIO

The socketio container handles all realtime communications among browser and processors.

### Redis

The redis container handles shared storage among processors.

### Nginx

The nginx container handles routing of calls between browsers and processors.

## NX.Project

NX.Workspace is based in ``NX.Node``.  Documentation for thiis aspect of the system can be found [here](/help/docs/README.md).


[Home)(../README.md)

# Infrastructure

NX.Workspace is designed to run in any cloud environment using containers to hold each sub-system.

You can see the of each container by using the ``Site Manager`` tool located in the ``System`` Start menu entry.

![image](images/SiteManager.png)

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

## Docker

When running the state of the images aand cotainer should look like these.

### Images

```
root@site:~# docker images ls

REPOSITORY                              TAG                 IMAGE ID            CREATED             SIZE
nxproject/processor                     dev                 194b7c6dd3f0        19 hours ago        661MB
nxproject/perconanosql                  dev                 ac2e2d1f872e        20 hours ago        507MB
nxproject/socketio                      dev                 39ddfe9bc867        20 hours ago        126MB
nxproject/nginx                         dev                 4dc6bc8f425e        20 hours ago        133MB
nxproject/redis                         dev                 0d7276a4d336        20 hours ago        78.5MB
nxproject/base                          dev                 755d0565be75        20 hours ago        72.9MB
nxproject/dotnet                        dev                 d25045c82f0d        20 hours ago        504MB
mhart/alpine-node                       10                  7d3849b7af27        2 weeks ago         73.7MB
mcr.microsoft.com/dotnet/core/runtime   3.1                 aeda02cc2c98        4 weeks ago         190MB
```

### Containers

```
root@site:~# docker ps -a

CONTAINER ID        IMAGE                        COMMAND                  CREATED             STATUS              PORTS                                      NAMES
085b92245c9a        nxproject/perconanosql:dev   "tini -- bash /entry…"   19 hours ago        Up 19 hours         0.0.0.0:32901->27017/tcp                   site_perconanosql_94673753C91BECCCA94270AAF5F7239F
80f84fb61884        nxproject/nginx:dev          "tini -- nginx -g 'd…"   19 hours ago        Up 19 hours         0.0.0.0:80->80/tcp, 0.0.0.0:443->443/tcp   site_nginx_EE434023CF89D7DFB21F63D64FF9D74
f7fd8103e37a        nxproject/processor:dev      "dotnet NXNode.dll -…"   19 hours ago        Up 19 hours         0.0.0.0:32900->8086/tcp                    site_processor__2A5B9450495D4CB38DD7A7C155AF2F1D
d7b368602077        nxproject/redis:dev          "tini -- redis-server"   19 hours ago        Up 19 hours         0.0.0.0:32899->6379/tcp                    site_redis_86A1B97D54BF71394BF316E183E67
5a9c18866870        nxproject/socketio:dev       "node /etc/wd/server…"   19 hours ago        Up 19 hours         0.0.0.0:32898->3000/tcp                    site_socketio_4393A1A5B976BFDD99B321D2187FBE3E
```

## NX.Project

NX.Workspace is based in ``NX.Node``.  Documentation for thiis aspect of the system can be found [here](https://github.com/nxproject/node).


[Home](../README.md)

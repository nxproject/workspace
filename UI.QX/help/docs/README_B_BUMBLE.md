# NX.Node - Bumble bees

A bumble bee is a bee that provide services.  These are the predifined genomes for
bumble brees included in the system:

* [Redis](README_REDIS.md)
* [Minio](README_MINIO.md)
* [SQL](README_SQL.md)
* [NoSQL](README_NOSQL.md)
* [SocketIO](README_SOCKETIO.md)
* [NginX](README_NGINX.md)
* [Traefik](README_TRAEFIK.md)

## Naming Processes, functions and routes for bumble bees

You need to name the projects that hold processes, functions and route in the format of:
```
XX.genome
```
for example, The route for the MongoDb Bumble Bee is called **Route.MongoDB**.  This
is because the genome used is **mongodb**.

Note that capitalization is ignored.

Also route trees should be defined as:
```JavaScript
public override List<string> RouteTree => new List<string>() { RouteClass.GET(Types.Routed), "get", ":id" };
```
Note the use of **RouteClass.GET(Types.Routed)** instead of **RouteClass.GET**.  You can use 
**RouteClass.RouteClass.GET(Types.Routed | Types.Secured)** to also secure the route.

See [NginX](README_NGINX.md), section **Bumble bee access** to understand why this is required.

##  Sharing bumble bees

While the structure of the hive is designed to be self-contained, sometimes is
necessary to share resources, like databases, across multiple hives.  To accomplish
this, set an environment setting like:
```
--hive_percona marketing
```
which tells the system that this hive and the marketing hive will use the same percona
bumble bee.   This is assuming that the bee has:
```
--hive sales
```
You can include any number of hives to share in the bumble bee:
```
--hive_percona marketing --hive_percona frontoffice
```
Now the bumble bee will be shared among the tree hives, sales, marketing and frontoffice.

In order for this to work correctly, all hives entries must be the **same** in **all
the hives used**.  Changes therefore require a full recycle of all the hives.

## External bumble bees

While the hive can easily manage bees, sometimes some bumble bees are outside the hive.
An example would be a shared database that is managed from the outside, while allowing
access to the hives' bees.

You setup these bees using the following:
```
--external mongodb=10.0.199.9:9999
```
which tells the system that the DNA is **mongodb**, and it can be reached at the IP
given.  Like many other calls, you can define multiple external resources.

## Delay in creation

As the bumble bee's underlying DNA (container) may take some time to be created and started,
calls to the bumble bee's code may not produce the intended result if called too soon
after the bumble bee' creation.

To solve this issue, you can call:
```JavaScript
bumblebee.Wait();
```
to ensure availability.

External bumble bees are considered available at all times.

## Samples of bumble bees

* [Redis](README_BB_REDIS.md)
* [NginX](README_BB_NGINX.md)

[Back to top](/help/docs/README.md)

# NX.Node - NoSQL

NX.Node supports both MongoDb and Percona Server for MongoDb.

When the **Proc.MongoDb** is called for, the system will in fact instantiate
a Percona MongoDb bumble bee, but you can override this behavior by calling:
```
--nosql mongodb
```

The choices for the nosql enviroment setting are:

Value|Uses
-----|----
perconanosql|Percona Server for MongoDb 3.4
mongodb|MongoDb 3.4

## Accessing the code

If your code is going  to make use of the bumble bee:

* Add ```using Proc.MongoDb;``` to your code
* Add the DLL as a Shared Project Reference

You also want to use the NuGet Manager to get:

* MongoDb.Driver -Version 2.10.4
* MongoDb.Bson -Version 2.10.4

## The manager

The Proc.MongoDb.ManagerClass is the interface to the mongodb bumble bee.
It is reached by calling:
```JavaScript
var mgr = env.Globals.Get<Proc.MongoDb.ManagerClass>();
```
If a bumble bee is available, it will be used otherwise one will be created.

## Databases

From the manager, you can get to any database by calling:
```JavaScript
var db = mgr["databasename"];
```

## Collections

From a database, you can get to any collection by calling:
```JavaScript
var coll = db["collectionname"];
```

## Objects

You can get to any **BsonDocument** from by calling:
```JavaScript
var obj = coll.Interface.xxxx();
```

[Back to top](/help/docs/README.md)

# NX.Node - Processes

Processes allow for the modification of a worker bee.  They can also create mason bees.

When a bee is created, it will run the function defined by the **proc** environment
setting, which by default is empty.   When a value is passed by calling:
```
--proc Chores
```
or programmatically:
```JavaScript
var bee = env.Hive.MakeWorkerBee("Chores");
```
where you replace "Chores" with your own value or set of values, the bee will do a:
```JavaScript
env.Use("Proc.Chores");
```
which can customize a bee by loading routes and other code.

You can assure that a number of bees that run a given **proc** are running by calling:
```JavaScript
env.Hive.AssureWorkerBee("Chores", min, max);

## Default process

The default **proc**, which is an empty string, can also be customized.  At start, it will
call:
```JavaScript
env.Use("Proc.Default");

## Naming Processes, functions and routes for procs

You need to name the projects that hold processes, functions and route in the format of:
```
XX.proc
```
for example, The route for the Chores proc routes is called **Route.Chores**.  This
is because the proc used is **chores**.

Note that capitalization is ignored.

Also route trees should be defined as:
```JavaScript
public override List<string> RouteTree => new List<string>() { RouteClass.GET(Types.Routed), "get", ":id" };
```
Note the use of **RouteClass.GET(Types.Routed)** instead of **RouteClass.GET**.  You can use 
**RouteClass.GET(Types.Routed | Types.Secured)** to also secure the route.

See [NginX](README_NGINX.md), section **Procs** to understand why this is required.

## Process routes

While you will later learn how to use **NginX** or **Traefik** to route  HTTP calls to
customized worker bees, you can also change the route trees themselves to work with
the load balancers.

You can make routes use the **proc** environment setting by modifying the first parameter,
which is the HTTP method in the route tree, for example:
```JavaScript
public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "echo", "?opt?" };
```
is a non-proc based route as:
```
GET /echo/...
```
but:
```JavaScript
public override List<string> RouteTree => new List<string>() { RouteClass.GET(Types.Routed), "echo", "?opt?" };
```
changes the call to
```
GET /chores/echo/...
```
You can also use the same mechanism for secured routes:
```JavaScript
public override List<string> RouteTree => new List<string>() { RouteClass.GET(Types.Routed | Types.Secured), "{id}", ":name"};
```
which changes the call to:
```
POST /chore/securecode/idofbee/nameoffn
```
In the above examples, **chore** is replaced by the value of **proc**.


[Back to top](/help/docs/README.md)

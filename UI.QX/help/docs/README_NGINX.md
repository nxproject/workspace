# NX.Node - Nginx

The nginx genome creates a NginX instance, and maps all of the worker bees, bumble bees
and procs into an access path using port 80 as th entry point.

The bumble bee keeps track of changes and will regenerate the nginx

Setting|Meaning
-------|-------
routing_port|The port to use.  The default depens whether [Traefik](README_TRAEFIK.md) is used
routing_debug|Set to y if you want a debug log
routing_wb|The site for the worker bees, defaults to "workerbees"
routing_proc|Allows for routing to specific worker bees
routing_bumble|Allow for access to the bumbe bee

## Procs

Routes for procs have the format of:
```
GET /xxx/...
```
where xxx is the proc value.  In order for this to work properly, you need to:

* Setup the proc routes to be RouteClass.GET(Types.Routed) or RouteClass.GET(Types.Routed  | Types.Secured)
* Create an enviroment setting of:
```
--routing_proc xxx
```
where xxx is the proc of the worker bee.  You can then call:
```
GET /chores/get/....
```
and NginX will properly route the call to a Chores route.

If you do not use **routing_proc**, the call will go to the first available worker bee.

## Bumble bee access

If you wish to give remote access to the bumble bee you need to:

* Setup the bumble bee routes to be RouteClass.GET(Types.Routed) or RouteClass.GET(Types.Routed  | Types.Secured)
* Create an enviroment setting of:
```
--routing_bumble genome
```
where genome is the genome of the bumble bee.  For example you can call:
```
--routing_bumble mongodb
```
Which allows for call like:
```
GET /mongodb/get/....
```
and NginX will properly route the call to a MongoDB, or other genome, route.

To change the location you can use:
```
--routing_bumble genome=newlocation
```
If in the example above we used:
```
--routing_bumble mongodb=calcutta
```
The call would then be:
```
GET /calcutta/get/....
```

If you do not use **routing_bumble**, the call will go to the first available worker bee.

[Back to top](/help/docs/README.md)

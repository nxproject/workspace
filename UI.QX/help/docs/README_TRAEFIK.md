# NX.Node - Traefik

The traefik genome creates a Traefik instance.  As Traefik is used as a front-end for
multiple hives, the **hive_traefik** environment settings, as explained in
[Bumble bee](README_B_BUMBLE.md) section **Sharing bumble bees** is used to determine
which hives are to be included.  

The first hive defined is the one that will host the traefik bumble bee.

You tell the system that you want to use Traefik by using:
```
--traefik_hive hivename
```

where hivename is the name of the hive that will host the Traefik container,

## SSL

NX.Node uses [Let's Encrypt](https://letsencrypt.org/) to get the SSL certificates needed
for HTTP support.  In order to do this you need to define the three environment settings:

Setting|Meaning
-------|-------
routing_domain|The domain to be used (eg. mydomain.com)
routing_email|An email address where notifications will be sent
routing_provider|The domain provider name (Default: namecheap)
routing_catchall|The route if all else fails (Default: https:google.com)

You need to setup the DNS for the domain per [these instructions](https://letsencrypt.org/how-it-works/).

## Accessing each hive

Once you have your domain setup, each hive becomes a sub-domain, so to access hive **finance**
you would call:
```
https://finance.mydomain.com/....
```
 
[Back to top](/help/docs/README.md)

# NX.Node - DNA

You can think a DNA being the definition used to create a running container.  It
is a JSON object with the values used in the [Docker CreateContainer](https://docs.docker.com/engine/api/v1.40/#operation/ContainerCreate)
call.  You can optionally use the following additions:

Key|Meaning
---|-------
$$From|The name of the DNA that defines the basis for this DNA.  The from task is used as the template and any keys defined in the DNA definition replace those in the from DNA.  Can be recursive.
$$Unique|Set to "1" if only one bee of this type can be in the system at any one time.
$$Ports|A JSON array of port numbers to  expose.  Any port defined before a zero is treated as a private port and exposed with a dynamic port number.  Ports defined after the zero are exposed with the same port number.
$$Map|A JSON array of volumes to use.   They are the source and target, separated by a semi-colon.  Note that the order is the reverse of the way Docker defines volumes, but IMHO is easier.
$$Requires|A JSON array of DNA that must be running before this DNA can be used.  The system automatically creates a bee if necessary for any missing DMA.

The above plus an **Image** and **Cmd** entries are all that is needed to define a task.

The following DNA definitions are part of the base system:

DNA|Use
----|---
processor|The basic task in the system.  It runs a copy of the NX.Node
minio|Minio (latest)
mongodb|MoongoDb 3.4.10
nginx|NginX 1.16.0-1~bionic
percona|Percona MongoDb 4.0.10-5.bionic
redis|Redis 4.0.2
traefik|Traefik 1.7.9

All of the above originated in a different container based project, and use a variant
of Ubuntu as the OS.

This is the DNA for the processor **genome**:

```JSON
{
  "@Ports": [
    "{http_port}"
  ],
  "@Map": [
    "{shared_folder}:/etc/shared",
    "{shared_folder}/processor:/etc/processor",
    "{shared_folder}/bee/B{id}/etc/private",
    "{shared_folder}/files:/etc/files",
    "/mm/nginx/default:/etc/sdnginx",
    "{shared_folder}/_cert:/etc/cert"
  ],
  "Cmd": [
    "dotnet",
    "NXNode.dll",
    "--config",
    "{config}",
    "--id",
    "{next_id}",
    "--proc",
    "{proc}"
  ],
  "Image": "{repo_project}/processor:{tier}",
  "WorkingDir": "/etc/wd"
  }
}
```
When the system uses a DNA, it will do replacement of all **{xxx}** entries with
values found in the [environment settings](README_ENv.md).

In the example above:
```JavaScript
"Image": "{repo_project}/redis:{tier}"
```
may be converted to:
 ```JavaScript
 "Image": "classic/redis:latest"
 ```
 when the environment settings are set as:

Var|Value
---|-----
repo_project|classic
tier|latest

## @Unique

The @Unique entry is the field where the bee will be created, or in Docker speak,
which computer will host the container.  Since the DNA for the built-in services
is hardcoded, the field name used in it may not be one that you want to use.  For
this reason, the value of * is used.

When the system sees the wildcard, it will look into the environment settings for
a an entry in the form of **field_xxx**, where xxx is the genome name, so for redis
it would be:
```
--field_redis jack
```
which would create Redis in field **jack**.

If no environment setting is defined, the system will use the first field.

## @Ports

The @Port entry is which ports are to be exposed.  If the entry starts with a $, the 
port will be exposed with an external port of the same value, otherwise a dynamic port
will be assigned.

If an * entry is seen, all ports opened by the genome will be exposed.

## @Map

The @Map entry defines volume mapping.  Each entry defines a source:target pair and
it is used to generate Volums and Bindings.

##@SkipRecycle

The @SkipRecycle flag will be used in the future to detemine if a genome is to be
recycled when the site changes.  If set to "y", the genome will not be recycled.

## Adding your own DNA

Just like you can add your own [genomes](README_B_GENOME.md), you can add your own
DNA defitions to the Hive/DNA folder.

[Back to top](/help/docs/README.md)

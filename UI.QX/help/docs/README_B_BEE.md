# NX.Node - Worker bee

A worker bee is the most common form of a bee in a hive.  It responds to HTTP calls.

The difference between a worker bee and a bumble bee is the genome, which is **processor**
for the worker bee, but varies for the bumble bee.

A worker bee's behavior can be altered by the use of the **proc** parameter, as explained
in [Processes](README_PROCS.md)

Any bee can create other bees, but usually that job faals on the hive's [Queen Bee](README_B_QUEEN.md).

## Checking the health of a bee

Each bee runs a Docker **healthcheck** call 30 seconds and sets itself to be unhealthy
if it cannot be reached.   Each bee checks its follower state every five minutes and if
it not running, it will kill the bee and replace it with a clone.  The last drone
in the chain checks on the Queen's health, closing the loop.

#### Creating a bee from the command line

AIn order to start a hive, we need to have a bee:
```
--make_bee y --hive myhive --field roses=10.0.192.168
```
This creates a single bee in a hive in a field, which automatically becomes the queen.

From there on out, either by code or command line, you can add more bees to make
a working hive.
```
## Ghost bee

A ghost bee is created when you run the program in MS Windows withoy making the genome or a bee.

This bee is useful in that it behaves just like any workier bee, but it will not
become queen, as it is not tracked by the roster.   As it is meant to run in Visual Studio
Debug mode, you have access to all of the debugging features.

[Back to top](/help/docs/README.md)
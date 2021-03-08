# NX.Node - Hive

The hive is the center of the system.  It holds all of bees, regardless of field location
or DNA.  The hive is composes of a queen bee, multiple worker, mason and bumble bees.

There is no one central hive, each bee holds a copy.

## Hive hierarchy and its health

While you are familiar with bee keepers, a natural hive has no such person as relying
on a beekeeper brings the danger of single point of failure.  It is the same with
this hive.  All worker bees have at most one leader and one follower, where the queen
bee has no leader and the lowest worker bee has no follower.

The selection of each worker leader and follower is done by the hive roster, as it
keeps track of all bees.  There is no single hive, the hive is duplicated in all worker
bees.  Maintenance of the roster is done via polling each field until a redis bumble
bee is known, which is then used to synchronize bee's birth and deaths.

## Hive environment settings

The following settings are used by the hive:

Setting|Meaning
-------|-------
hive|The name of the hive
repo_name| The name of the repository.  Typically this is an URL in the form of https://cr.myco.com:8888 and is used only if there is an external repository
repo_project|The project name.  Defaults to **nxproject**
repo_username|user name to log into the repository
repo_userpwd|Password used to log into the repository
repo_useremail|E-Mail address where your company may be contacted if any issues arise

NB: The limitation in this structure is that the sample project name is used for your
local and any external repository.

## Hives via Routes

The following routes can be made available by:
```javascript
env.Use("Route.Hive")
```
The routes exposed by this allow you to:

Route|Use
-----|---
MakeBee|Makes a bee
KillBee|Kills a bee
KillBeesWithDNA|Kists all bees with a given DNA
AssureDNA|Assures that a number of bees with a given DNA are active

[Back to top](/help/docs/README.md)

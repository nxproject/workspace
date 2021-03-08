# NX.Node - Redis

It is difficult to undervalue the use of Redis in the system.  All inter-be communications
and handling of settings are done via Redis and it is done in many places in the system.

The Redis services are sharable with any party, as the keys used by the system are
prefixed with **$$**.  If this conflicts with any of the keys in use, it cal be changed
by setting the **redis_prefix** environment setting.

## Synchronized store

*NX.Node* has a special class **NX.Engine.SynchronizedStoreClass** that makes
use of Redis to keep each copy in each bee synchronized.  It does this by keeping mirror
copies of the data in memory and also in Redis.

The store has built-in behaviors to make it work:

### Creation of the Redis store

When the store is first created, the data already kept in memory by the bee is copied
to Redis.  This also happens when a ghost bee opens the store, allowing for updates
to already established stores.

### When the Redis store become available

When the store becomes available, when a Redis bumble bee is seen, any data already
saved in the store will be read into memory.  If the data is not known, it will be
copied to Redis.

### Changes to the store

Changes are always kept in memory.  If the Redis store is available, it is also copied
to the Redis store and propagated to all the other bees.

### Something about key names

The synchronized store does not allow it's keys to have a leading plus **+** or minus **-**
sign.  These two leading characters mean that the key being set is an array and that
the values are to be added or removed from the array.

### Making changes to a running hive from the outside

Remember that the environment settings are a synchronized store.  If you run NX.Node.exe
from outside the hive as a normal program, any parameters in the command line are copied
to the hive's environment settings store, thereby updating the hive.

When the change entails adding or removing fields, you should expect a fair amount of
mayhem as bees are killed in some places and added in others.  Once the changes stabilize,
the queen will take over and reconstruct the hive.

[Back to top](/help/docs/README.md)

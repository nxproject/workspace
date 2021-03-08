# NX.Node - Queen bee

Each bee is given a unique identifier at birth.  The bee with the largest identifier
becomes queen.  There are moments when there are more than one queen, especially
before Redis is available, and there could be moments when there is no queen, as
the bee may die unexpectedly.

As soon as any condition where there is no queen is detected, the hive will select
a new queen from the worker bees.

## Queen's duties

The duty of the Queen is to ensure that the required bumble bees are alive.  You
can set what the queen is required as a bumble bee by:
```
--qd_bumble minio
```
and you can have multiple requirements by:
```
--qd_bumble minio --qd_bumble percona
```
 By default, the only bumble bee needed is of DNA redis which is automatically done,
 but you can change that behavior via:
 ```
 --qd_bumble !redis
 ```

Similarly, you can also add to the Queen duties the orchestration of worker bees
by calling:
```
--qd_worker 10
```
which will ensure that ten bees are running.  You can specify which type of bee
to keep alive by:
```
--qd_worker Run:2
```
where the prefix is the process name **proc** to pass each bee.

The above checks are done every minute, unless you override it by:
```
--qd_every numberofminutes
```

You can add code tasks to the Queen's duties by calling:
```JavaScript
var id = env.Hive.Roster.AddQueenToDo(delegate() {
  ...
});
```
Which returns an ID for the todo.

You can delete the to by calling
```JavaScript
env.Hisv.Roster.RemoveQueenToDo(id);
```

Note that the Queen's duties do not start for two minutes after her ascension.
This is to handle moments where
a number of bees have come into play and things are getting sorted out.

## qd_uses vs. qd_bumble

Let's look at these tqo entries:
```
--qd_bumble mongodb
```
and
```
--qd_uses Proc.MongoDb
```

Both end up creating a genome (container) of a MongoDb server, but while the
**qd_bumble** just creates the genome, **qd_uses** loads the MongoDb client,
thus making the genome usable.

[Back to top](/help/docs/README.md)

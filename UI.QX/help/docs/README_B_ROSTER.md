# NX.Node - Roster


The roster is the most important piece of the hive, as it keeps tabs on all of the fields
and thereby, all of the bees.

The roster refreshes itself every few minutes, period which is lagrger if a redis 
bumble bee is available, at which time all of the fields can signal the roster when a change
in the numer and/or type of bee is seen.

Just like the hive, there is no central copy, each bee holds its own copy, which is the
one tasked with keeping all copies of the hive in synch.

When bees are added or removed, the roster alerts all processes with the changes in DNA count.
This changes the behavior of the system automatically, and any bumble bee that you create should 
make use of this mechanism.

[Back to top](/help/docs/README.md)

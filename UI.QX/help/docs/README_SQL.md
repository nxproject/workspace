# NX.Node - SQL

NX.Node supports both MySQL and Percona Server for MySQL.

When the **Proc.MongoDb** is called for, the system will in fact instatiate
a Percona MySQL bumble bee, but you can override this behavior by calling:
```
--sql mysql
```

The choices for the sql enviroment setting are:

Value|Uses
-----|----
perconasql|Percona Server for MySQL 8.0
mysql|MySQL 8.0


[Back to top](/help/docs/README.md)

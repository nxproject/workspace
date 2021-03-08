# NX.Node - Field

A field is an instance of the Docker deamon.  The deamon must have an exposed port so calls
can be made via the Docker API.

A hive can have any number of fields, and a feild can be shared among many hives.

You define the field as follows:
```
--field name=ip:port
```

The same ip:port combination cannot be used more than once for a hive.  The feild names do
not need to match when sharing the field among multipl hives.

## Field environment settings

The following setting is used by the hive:

Setting|Meaning
-------|-------
field|The name and IP address of the Docker API.  The syntax is name=ip, for example sales=http://cr.myco.com:8087 or test=10.0.192.99:2375.  You can enter any number of entries by repeating the --field name=ip setting.  If none are entered a=localhost:2375 is used.

[Back to top](/help/docs/README.md)

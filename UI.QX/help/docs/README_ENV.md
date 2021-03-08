# NX.Node - Environment

The environment settings are system-wide settings, easily accessible via code:
```JavaScript
env["settname"]
```
When the NX.Sever.exe starts, they are obtained from the command line:
```
NX.Sever.exe --http_threads 10 --uuid STATIONA
```

If the key is entered more than once, the setting will turn into a JSON array.

You can also place environment settings inn your OS environment values definition,
but if you do so, prefix the name with **nx_** in the OS table, so **http_threads** would
read like **nx_http_threads=10**

The settings are stored in  different places according to the resources available,
but they are
shared among he bees.  Any setting that you use when creating a bee replaces
the values previously known.

You can load settings from a file, which you can then use by calling:
```
--config pathtofile
```
This setting is not stored in the shared settings.  This file is a JSON object
with each environment setting that you want to use as the key and the value
can be a string, JSON array or JSON object.  This is an example of a config file:
```
{
	"hive": "sales",
	"fields": [
		"10.0.192.68",
		"10.0.192.69:2788"
	],
	"qd_bumble":["redis","mongodb"],
	"qd_worker": [ "Portal:2", "Chores:3" ]
}
```

The following settings are used at the present time, but not mentioned elsewhere:

Setting|Meaning
-------|-------
http_threads|The number of HTTP worker threads to run (Default: 4)
http_port|The port that the server will listen for HTTP requests (Default: 80)

[Back to top](/help/docs/README.md)

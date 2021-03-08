# NX.Node - SocketIO

NX.Node supports Socket.IO via a bumble bee.

## Accessing the code

If your code is going  to make use of the bumble bee:

* Add ```using Proc.SocketIO;``` to your code
* Add the DLL as a Shared Project Reference

You also want to use the NuGet Manager to get:

* SocketIOClient -Version 2.0.2.3

## The manager

The Proc.SocketIO.ManagerClass is the interface to the mongodb bumble bee.
It is reached by calling:
```JavaScript
var mgr = env.Globals.Get<Proc.SocketIO.ManagerClass>();
```
If a bumble bee is available, it will be used otherwise one will be created.

## Events

From the manager, you can get to any event by calling:
```JavaScript
var event = mgr["eventname"];
```

## Messages

Socket.IO messages are JSON objects.  You can create a new message from the event:
```Javacript
var msg = evt.New();
```

Once you have a message, you can access the values by:
```JavaScript
// Get value
var value = msg["key"];

// Set value
msg["key"] = "value";
```

## Sending messages

Sending a message can be done vy calling:
```JavaScript
var ok = msg.Send();
```
which returns true if the message was sent.

## Receiving messages

You can receive messages using events:
```JavaScript
event.MessageReceived += delegate (MessageClass msg)
{
	...
};

[Back to top](/help/docs/README.md)

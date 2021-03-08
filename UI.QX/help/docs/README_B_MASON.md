# NX.Node - Mason bees

There is a special type of bee, which along with its normal duties also accepts requests
via a message queue.  For this bee to exist, the **redis** bumble bee must also exist.

To turn any bee into a mason, use the following call:
```JavaScript
env.Hive.Mason.Listen("queuename", delegate(WorkMessageClass msg){
  ....
});
```
and to respond to a request:
```JavaScript
envHive..Mason.Respond(msg);
```

Any bee can call on a mason bee by:
```JavaScript
var id = env.Hive.Mason.Send(msg);
```
which will open a work item for the mason bees handling the queue and continue on.
The ID is a unique value so the requesting bee can keep track of requests.

This is how to set a handler for the request returns:
```JavaScript
env.Hive.Mason.Handle(delegate(WorkMessageClass msg){
  ....
});
```
As the mason processing is asynchronous, and may take a bit of time, it is best not to
base any processing in a given time period.  If the requesting bee dies, any requests
and responses are removed from the system.  The Handle call must be made prior to
sending any messages, otherwise the messages will be sent as **DoNotRespond**.

## Mason work message

The mason work message has the following properties:

Property|Type|Meaning
--------|----|-------
RequestorBee|string|ID of bee that sent the message
MasonBee|string|ID of the bee that processed the message
Request|Store|Data sent by the sender bee
Response|Store|Date return by the mason bee
DoNotRespond|bool|If true, no response is allowed
RequestTTL|TimeSpan|Amount of time in milliseconds that the request is valid. Zero means no expiration
ResponseTTL|TimeSpan|Amount of time in milliseconds that the response is valid. Zero means no expiration

[Back to top](/help/docs/README.md)

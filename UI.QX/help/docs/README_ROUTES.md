# NX.Node - Routes

The route is the first *NX.Node* item that you need to understand.  The following
is a basic route definition in *NX.Node* :

```javascript

using System.Collections.Generic;

using NX.Engine;
using NX.Shared;

namespace Route.System
{
    /// <summary>
    ///
    /// Echoes any URL values
    ///
    /// </summary>
    public class Echo : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "echo", "?opt?" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            call.RespondWithStore(store);
        }
    }
}
```

This route definition uses two C# modules NX.Engine and NX.Shared.  Itwill not go
into detail on what those do at this time, just keep in mind that they make this
code much simpler.

The Echo class is based in the RouteClass.  All of the built-in classes are suffixed
with the word 'Class' to  minimize conflict with other code.

The class has two items that override the defaults in the RouteClass, **RouteTree**
and **Call**:

#### Call

The Call code is what gets executed when the route is matched.  In this example, the
called gets passed a StoreClass, which can be thought as a JSON Object and responds
with the contents of the same store.  A simple echo.

#### RouteTree

The route tree defines the route.  The first entry is the HTTP method GET/POST/PUT/DELETE/PATCH
or anything you like, followed by the URL to be used.  You can have four type of
definitions:

Format|Meaning
------|-------
text|The text entered must be seen in the URL.
:key|The URL can have any text, but it must be non-empty.  The value entered will be passed to the route handler as a key/value pair in the store parameter.
?key|The URL can have any text which can be empty.  Once an optional definition is seen, all following definitions must also be optional.   If a value is entered, the rules will be passed back like the :key definition.
?key?|Same as the ?key definition, but must be the last entry in the tree.  Behaves like if multiple ?key were entered.

Next I will run through some example of the above route.  Let's start with the simplest:
```
http://localhost/echo
```
This is the smallest call that will be handled by the route, as the route only requires
the word "echo" as the first piece of the URL.  The call would produce a return of:
```JSON
{"opt":[]}
```
which tells you that opt is an optional entry but no values were defined.  This is
similar to NodeJS but not quite the same.  Lets try a more complete example:
```
http://localhost/echo/john/mary/sam/peter
```
which produces:
```JSON
{"opt":["john","mary","sam","peter"]}
```
But we are not limited by the tree itself, we can make use of the URL parameters
as well:
```
http://localhost/echo/john/paul?dept=sales&route=NY
```
Which return:
```JSON
{"dept":"sales","route":"NY","opt":["john","paul"]}
```
as all entries are made part of the store, only one place needs to be checked for values.

Following the rule that when multiple values are seen for a given key, the passed
values are turned into a JSON array, we get this:
```
http://localhost/echo/john/paul?age=34&age=47
```
which returns:
```JSON
{"age":["34","47"],"opt":["john","paul"]}
```
Note that the parameters are return before the route entries, so calling:
```
http://localhost/echo/john/paul?opt=sam&age=22&age=34&age=47
```
returns:
```JSON
{"opt":["sam","john","paul"],"age":["22","34","47"]}
```
Now to show the :key option, let's trun the route tree into:

```JavaScript
public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "echo", ":dept", "?opt?" };
```

Now let's try the first example:
```
http://localhost/echo
```
the return is now:
```JSON
{"code":500,"expl":"InternalServerError","error":"Internal error"}
```

This means that no route was found, as a "dept" entry is required.  Let's try a valid
call:
```
http://localhost/echo/sales/john/mike/alice
```
which returns:
```JSON
{"opt":["john","mike","alice"],"dept":"sales"}
```
The route tree definitions, while similar to NodeJS, create a more powerful and consistent
set of rules.

## Securing routes

There is a built-in route that allows one bee to call another using an HTTP POST call.
This is a back door into the system and back doors are trouble in the making.  This
is the code for that call:
```JavaScript

using System.Collections.Generic;

using NX.Engine;
using NX.Shared;

namespace Route.System
{
    /// <summary>
    ///
    /// A way to reach the bee in a semi-opaque way
    ///
    /// </summary>
    public class FN : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.POST(Types.Secured), "{id}", ":name"};
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Call the function and respond
            call.RespondWithStore(call.FN(store["name"], call.BodyAsStore));
        }
    }
}
```
Note that the method in the **RouteTree** is a modified POST that tell the system that
this route is only available if a secure code is passed.

The **_SECURE** changes the call form this:
```
POST  /idofbee/nameoffn
```
to:
```
POST /securecode/idofbee/nameoffn
```
The first is semi-secure, as the caller would have to know the id of the bee, but
the second becomes better secured, as the caller also has to know the secure code.

And the secure code can be changed on the fly:
```
NX.Node.exe --secure_code codeyouwant
```
This is the format to use when there no secure code already defined, the format is:
```
NX.Node.exe --secure_code oldcode=newcode
```
when there is a secure code already in place.

The change can also be done programatically by calling:
```JavaScript
env["secure_code"] = newcode;
```
The old code is not needed as the code runs in a secure environment.

And to solve the issue of forgetting to set the secure code, the secure routes are
disabled until one is set.

Note that you can change the secure code at any time without having to recycle.  It
may take a small amount of time until all nodes switch.

Note:  The secure code must not match any route entry that the system uses, as the
behavior is not guaranteed.

[Back to top](/help/docs/README.md)

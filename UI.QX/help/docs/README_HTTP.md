# NX.Node - HTTP

The settings used by HTTO are:

Setting|Meaning|Default
http_port|The port to listen to|8086
http_threads|Worker threads|4

## Multi-threading and handling of HTTP calls

While NodeJS is a single threaded process, *NX.Node*  is built with the ability to
handle multiple HTTP call processors at the same time, while letting the programmer
handle each call as its own process.

The **Do** call above has two parameters, the first is the encapsulation of the HTTP
Call via the **HTTPCallClass** and the second is the parameters in the call URL using
the **StoreClass**.

## Caching

The system automatically handles caching for all files.  Because of their nature, html
files with embedded **nxjs** will not be cached.

## HTTP authentication

You can define which authentication method to use by calling:
```JavaScript
env.HTTP.SetAuthenticationScheme("scheme");
```
 where scheme is one of the following:

Scheme|Meaning
------|-------
None|No authentication is allowed. A client requesting an HTTPCallClass object with this flag set will always receive a 403 Forbidden status. Use this flag when a resource should never be served to a client.
Digest|Specifies digest authentication.
Negotiate|Negotiates with the client to determine the authentication scheme. If both client and server support Kerberos, it is used; otherwise, NTLM is used.
Ntlm|Specifies NTLM authentication.
Basic|Specifies basic authentication.
Anonymous|Specifies anonymous authentication.
IntegratedWindowsAuthentication|Specifies Windows authentication.

When an HTTP request is received, the system will place the user name and password
in the HTTPCallClass UserInfo object.  It will then check to see if the request has
been authenticated and if it has not, the env.ValidationCallback() will be called.  The
function should return true if the validation was successful, otherwise the request
will be rejected.

The above scheme table was taken from [AuthenticationSchemes Enum]( https://docs.microsoft.com/en-us/dotnet/api/system.net.authenticationschemes?view=netcore-3.1)

NB: The authentication method applies to all calls, so if you are setting up callbacks
from other services, make sure that they are capable of handling the authentication
method that you decide upon.

[Back to top](/help/docs/README.md)

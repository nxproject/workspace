# NX.Node - Loading processes, routes and functions

While the base system has a limited number of routes and functions, *NX.Node* itself
has a few extra modules that can be made available by calling:
```javascript
env.Use("module");
```
In this mode, all processes, routes and functions are brought into the system,

You an optionally select which combination of the above to load by calling:
```javascript
env.Use("module", Profiles.Procs | Profiles.Routes | Profiles.Fns);
```
or any combination thereof.

These are the modules available:

SubSystem|Modules|Use
---------|-------|---
DEX|Route.DEX, Fn.DEX|Data exchange between sites
Dynamic|Route.Dynamic|Allows for runtime loadeing of routes and functions
Files|Route.File|Allows for the upload, download and merging of files
MongoDb|Fn.MongoDb|MongoDb support
USPS|Route.USPS Fn.USPS|USPS support

Note that modules must have a two part name and the first part must be **Fn**, **Proc**
or **Route**.  The contents do not have to math the name, you can put functions in a
DLL named Routes.  But it may help your sanity.

## Dynamic code

Dynamic code is code loaded by the user.  It is kept in a sub-folder, which defaults
to **dyn**.

As code stored in the **dyn** folder is automatically loaded at start time and since
you can get the contents of a Git repository loaded into the **dyn** folder, there
is little to do to link in any code that you or others create.  By copying the DLL
into the folder before launching the *NX.Node* does the job.

## Naming conventions for routes and functions

While we have not discussed functions, they are the second part of the *NX.Node*
system.   Both share the same naming convention, which start at the DLL level.

Route DLLs must have the name of "Route.xxx.dll" and the namespace whould be Route.xxx.
The DLL can contain multiple classes based on the RouteClass.  Each class will be
mapped using the DLL name and the class name, for example the **Echo** class above
comes from the Route.System.dll so the name would be System.Echo.  While the class
name is limited toa single word, the DLL name is not limited to two parts, so if
the **Echo** call is located in the Route.Allen.Cute.dll, the name would be Allen.Cute.Echo.


[Back to top](/help/docs/README.md)

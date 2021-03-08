# NX.Node - Functions

A long time ago, someone taught me that what a computer does is move data from point
A to point B and maybe do a bit of processing on the way.  It was true then and it
is true now.  If you look at the **Echo** route above, it takes two parameters and
returns something.  And In this case, does nothing in the way there.

But there are times where you want to do something in the way.  This is an example
of a very simple function:

```JavaScript
using NX.Engine;
using NX.Shared;

namespace Fn.System
{
    /// <summary>
    ///
    /// Evaluates an expression and returns a value
    ///
    /// Uses from passed store:
    ///
    /// expr        - The expression to be evaluated
    /// new         - If true or non-zero, return a new store
    ///
    /// Returns in return store:
    ///
    /// value       - Result of the expression
    ///
    /// </summary>
    public class Eval : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass values)
        {
            // The return store
            StoreClass c_Ans = values;
            // If not the original, make a new one
            if (XCVT.ToBoolean(values["new"])) c_Ans = new StoreClass();

            // Eval
            c_Ans["value"] = call.Env.Eval(values["expr"], values).Value;

            // ANd return the store
            return c_Ans;
        }
    }
}
```

The function definition looks a lot like the route definition form **Echo**, as all
that computers do is move data from point A to point B and maybe do a bit f processing
on the way.  But function do not have a route tree, as they are not called via an
HTTP call.

Let's look at what this function does.

The first thing it does is to move the passed store value into a local variable.
Next it checks to see if the value of the **new** entry in the passed store is true,
converting to Boolean from whatever was passed.  If this is so, it will create a
new store instead of using the passed one.

Next it sets the value of **value** in the store with the evaluation of the string
**expr** in the passed store and using the passed store as the values to be used
when evaluating the expression.

Last it will return the result store back to the caller.

Note:  I am not trying to explain you what each class and call does, I am explaining
the structure of the system, or how I think.

## Calling a function

The easiest way to make a function call is to use the HTTPCallClass **call** parameter,
so to call the **Eval** function, you would do this:

```JavaScript
var ans = call.FN("System.Eval", "expr", "1+2")["value"];
```
The **ans** variable would be a string whose value would be "3".  If you had other
values to be used as variables you could use:
```JavaScript
var ans = call.FN("System.Eval", "expr", "1+a")["value", "a", "10"];
```
Which would result in **ans** being "11";

And you could use a store instead:
```JavaScript

var passed = StoreClass.Make("a", "10", "b", "99", "expr", "a+b");

var returned = call.FN("System.Eval", passed);

var ans = returned["value"];
```
and in this case **value** would be "109".

[Back to top](/help/docs/README.md)

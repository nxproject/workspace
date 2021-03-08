# NX.Node - How do I debug my code?

Since NX.Node is designed to be a dynamic system, the typical "set breakpoint"
methodology will not work.  This is how to do the same thing in a different way.

## Compile in Debug mode

all the code that you will be using should be compiled in Debug mode, as it creates the correct
**.pdb** files that you will need.

## Setup

First, setup your **config.json** to have these entries:
```
	"uses": [ "Proc.Office" ],
	"code_folder": [ "C:\\Candid Concepts\\NX\\Office\\Proc.Office\\bin\\Debug\\netcoreapp3.1" ],

	"make_genome": "n",
	"make_bee": "n"
```
You can include any number of entries for the **uses** and **code_folder**.

When you run the NX.Node code, the code_folders entry will have all of the folders that
NX.Node typically uses to build the genome plus the entries that you specified, thus
allowing you to call:
```JavaScript
env.Use("Proc.Office");
```
which is what the **uses** entry does.

This loads your code into the system maps.

## Triggering a breakpoint

As you are running NXMode.exe but your code is in some other project, you cannot
set the breakpoints the normal way.  What you need to do is call:
```JavaScript
env.Debug();
```
somewhere in your code to trigger a Debugger breakpoint.  Once that is triggered,
you can set the breakpoints like you normally do.

## Including your code in the NX.Node project

While this is certainly allowed, and probably the way that most will do, remember
that NX.Node code will change, so you will have to reintegrate your projects every time
NX.Node changes.  May not seem like much, but it becomes a headache when the
new version has radical mods. 

[Back to top](/help/docs/README.md)

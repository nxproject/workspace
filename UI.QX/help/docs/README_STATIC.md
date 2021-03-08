# NX.Node - A static (maybe) web site

Including the **Route.UI** DLL, makes the bee into a "static" web server.  Let's
look at the code:
```JavaScript
using System.Collections.Generic;

using NX.Engine;
using NX.Shared;

namespace Route.System
{
    /// <summary>
    ///
    /// A route that allows a "regular" website support
    ///
    /// Make sure that all of the files to be served are in the #rootfolder#/ui
    /// folder and that none of the subdirectories match a defined route
    ///
    /// </summary>
    public class UI : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "?path?" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Make the folder path
            string sPath = call.Env.RootFolder.CombinePath("modulesui").CombinePath(call.Env.UI.Replace("+", ""));

            // Get the full path
            sPath = store.PathFromEntry(sPath, "path");

            if(!"".InContainer())
            {
                // Get the path
                sPath = "".WorkingDirectory();
                // Find where NX.Node is
                int iPos = sPath.IndexOf("NX.Node");
                // Make new path
                sPath = sPath.Substring(0, iPos) + "UI." + call.Env.UI;
            }

            // Assure folder
            sPath.AssurePath();

            // If not a file, then try using index.html
            if (!sPath.FileExists()) sPath = sPath.CombinePath("index.html");

            // Assume no processor
            Func<FileStream, Stream> c_Proc = null;

            // HTML?
            if (sPath.GetExtensionFromPath().IsSameValue("html") && call.Env.UI.Contains("+"))
            {
                // Make JS interpreter
                var c_Engine = new Engine(cfg => cfg.AllowClr());
                // Add the objects
                c_Engine.SetValue("call", call);
                c_Engine.SetValue("env", call.Env);
                c_Engine.SetValue("store", store);
                c_Engine.SetValue("html", new HTMLClass());

                // Make our HTML processor
                c_Proc = delegate (FileStream stream)
                {
                    // Open a reader
                    using (StreamReader c_Reader = new StreamReader(stream))
                    {
                        // Read the page
                        string sPage = c_Reader.ReadToEnd();
                        // Build the parser
                        HtmlDocument c_Page = new HtmlDocument();
                        //Parse
                        c_Page.LoadHtml(sPage);
                        // Find JS tags
                        var c_Nodes = c_Page.DocumentNode.SelectNodes("//nxjs");
                        // Any?
                        if (c_Nodes != null)
                        {
                            // Loop thru
                            foreach (HtmlNode c_Node in c_Nodes)
                            {
                                // Make new node
                                HtmlDocument c_New = new HtmlDocument();

                                // Process
                                HTMLClass c_HTML = c_Engine.Execute(c_Node.InnerText).GetValue("html").ToObject() as HTMLClass;
                                // Any?
                                if (c_HTML != null)
                                {
                                    //Parse
                                    c_New.LoadHtml(c_HTML.ToString());
                                }

                                // Replace
                                c_Node.ParentNode.ReplaceChild(c_New.DocumentNode, c_Node);
                            }
                        }

                        // Make the output stream
                        MemoryStream c_Out = new MemoryStream(c_Page.DocumentNode.OuterHtml.ToBytes());

                        return c_Out;
                    }
                };
            }

            // And deliver
            call.RespondWithUIFile(sPath, c_Proc);
        }
    }
}
```
The key is the **RouteTree**, which has no text to match, just optional.
What this causes is to create a route where if a GET request finds no matches
elsewhere, it will match whatever the requestor entered.

If what was entered does not exist as a file, **index.html** is appended.

If the resultant path is not found a 404 Not found error is returned.

And I say maybe a static web system, as I can see where with a bit of code, you can
make this route into a processor and modify the files as they are being returned.

The config for this application looks like:
```JSON
{
    "ui": "react"
}
```
which will use the React boilerplate.

The following choices are available:

Code|System
bootstrap|[Bootstrap](https://getbootstrap.com)
html|HTML based system
react|[React](https://github.com/facebook/react)
vue|[Vue](https://vuejs.org)

## nxjs

You can include JavaScript in your .html pages by the use of **nxjs** tag.  For example:
```HTML
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>Hello World!</title>
</head>
<body>
    <nxjs>
        html.Add(html.h2("Hello World!"), html.p("I am ", env.Hive.Name));
    </nxjs>
</body>
</html>
```
would produce the HTML page of:

``` ------------------------------- Top ----------------------------------```

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8">
    <title>Hello World!</title>
</head>
<body>
    <h2>Hello World!</h2><p>I am test</p>
</body>

``` ----------------------------- Bottom ---------------------------------```

The following are available in the JavaScript:

Variable|Meaning
--------|-------
env|The current environment
call|The HTTP call
html|HTML generator

To turn on this feature, call:
```
--ui react+
```
The plus sign tells the system to support nxjs tags.  You can do this with any of the UI
systems.

[Back to top](/help/docs/README.md)

///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
/// 
/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.
/// 
///--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

using NX.Engine;
using NX.Shared;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// A route that redirects /ux routes
    /// 
    /// </summary>
    public class UX : RouteClass
    {
        public override List<string> RouteTree =>null;
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Make the folder path
            string sPath = "".WorkingDirectory().CombinePath("ui." + call.Env.UI.Replace("+", "").ToLower()).CombinePath("ux").CombinePath(call.RouteTree[0]).AdjustPathToOS();

            // Get the full path
            sPath = store.PathFromEntry(sPath, "path");

            // Map?
            if (sPath.GetExtensionFromPath().IsSameValue("map"))
            {
                // Fake a date
                call.RespondLastModified = DateTime.MinValue;

                // Respond
                call.RespondIf((DateTime)call.RespondLastModified, delegate ()
                {
                    // Make readable
                    using (MemoryStream c_Stream = new MemoryStream("{'version':3,'sources':[],'names':[],'mappings':'','sourcesContent':[]}".Replace("'", "\"").ToBytes()))
                    {
                        call.RespondWithStream("default.js", null, false, c_Stream);
                    }
                });
            }
            else
            {
                // If not a file, then try using index.html
                if (!sPath.FileExists()) sPath = sPath.CombinePath("index.html");

                // And deliver
                call.RespondWithUIFile(sPath);
            }
        }

        public override void AtAdd(RouterClass router)
        {
            router.Parent.Debug();

            // Make the folder path
            string sPath = "".WorkingDirectory().CombinePath("ui." + router.Parent.UI.Replace("+", "").ToLower()).CombinePath("ux").AdjustPathToOS();

            // Get folders
            List<string> c_Folders = sPath.GetDirectoriesInPath();
            // Loop thru
            foreach (string sFolder in c_Folders)
            {
                // Map
                router.Add(this, new List<string>() { RouteClass.GET(), sFolder.GetDirectoryNameFromPath(), "?path?" });
            }

            base.AtAdd(router);
        }
    }
}
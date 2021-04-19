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
            string sPath = "".WorkingDirectory().CombinePath("ui." + call.Env.UI.Replace("+", "").ToLower()).AdjustPathToOS();

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
    }
}
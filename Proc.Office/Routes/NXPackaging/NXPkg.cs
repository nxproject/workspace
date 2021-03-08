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
using System.Text;

using NX.Engine;
using NX.Shared;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Gets many .js files and their dependencies as a single call
    /// 
    /// </summary>
    public class NXPkg : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "nxpkg" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Parse the body
            List<string> c_List = store["list"].ToJArray().ToList();
            if (c_List == null) c_List = new List<string>();

            List<string> c_NoMin = store["nomin"].ToJArray().ToList();
            if (c_NoMin == null) c_NoMin = new List<string>();

            // Make the folder path
            string sPath = "".WorkingDirectory().CombinePath("ui." + call.Env.UI.Replace("+", "").ToLower()).AdjustPathToOS();

            // Time saver
            string sLastMod = "ts{0}.lastmodified".FormatString(store["list"].MD5HashString());
            string sTS = sPath.CombinePath(sLastMod).ReadFile();
            if(!sTS.HasValue())
            {
                sTS = DateTime.MaxValue.ToDBDate();
            }
            // 
            call.RespondLastModified = sTS.FromDBDate();

            // Quick Ckeck
            call.RespondIf((DateTime)call.RespondLastModified, delegate ()
            {
                // Reset
                call.RespondLastModified = null;

                // The result list
                List<string> c_Done = new List<string>();

                // Make the buffer
                StringBuilder c_Buffer = new StringBuilder();

                // Load Support
                using (SupportClass c_Supp = new SupportClass())
                {
                    // Loop thru
                    foreach (string sFile in c_List)
                    {
                        //
                        c_Supp.GetItem(call, sPath, sFile.Replace("/", "."), c_Done, c_NoMin, c_Buffer);
                    }

                    // Handle empty
                    if (call.RespondLastModified == null)
                    {
                        call.RespondLastModified = DateTime.MinValue;
                    }

                    // Save
                    sPath.CombinePath(sLastMod).WriteFile(call.RespondLastModified.ToDBDate());

                    // Respond
                    call.RespondIf((DateTime)call.RespondLastModified, delegate ()
                    {
                        // Get code
                        string sCode = c_Buffer.ToString();

                        // Make readable
                        using (MemoryStream c_Stream = new MemoryStream(sCode.ToBytes()))
                        {
                            call.RespondWithStream("pkg.js", null, false, c_Stream);
                        }
                    });
                }
            });
        }
    }
}
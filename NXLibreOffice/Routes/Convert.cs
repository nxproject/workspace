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
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;

namespace Proc.LibreOffice
{
    /// <summary>
    /// 
    /// Uploads a file
    /// 
    /// </summary>
    public class Convert : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "libreoffice", "convert", ":source", ":target" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            //
            StoreClass c_Ans = new StoreClass();

            //
            string sSource = store["source"];
            string sTarget = store["target"];

            // Must have one
            if (sSource.HasValue() && sTarget.HasValue())
            {
                // Get the file manager
                NX.Engine.Files.ManagerClass c_Mgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

                // Protect
                try
                {
                    // Source
                    using (DocumentClass c_Source = new DocumentClass(c_Mgr, sSource))
                    {
                        // Real?
                        if (c_Source.Exists)
                        {
                            // Target
                            using (DocumentClass c_Target = new DocumentClass(c_Mgr, sTarget))
                            {
                                // Is the tarhet older?
                                if (c_Target.WrittenOn < c_Source.WrittenOn)
                                {
                                    call.Env.LogInfo("Converting {0} to {1}".FormatString(sSource, sTarget));

                                    // Remove
                                    c_Target.Delete();

                                    // Arguments
                                    List<string> c_Args = new List<string>(){
                                "-env:UserInstallation=file:///tmp",
                                "--nologo",
                                "--headless",
                                "--nocrashreport",
                                "--nodefault",
                                "--norestore",
                                "--nolockcheck",
                                "--invisible",
                                "--convert-to" ,
                                c_Target.Extension,
                                "--outdir",
                                c_Target.Location.GetDirectoryFromPath(),
                                c_Source.Location
                            };

                                    // Convert
                                    ProcessStartInfo c_Info = new ProcessStartInfo("/usr/bin/libreoffice");
                                    c_Info.Arguments = c_Args.JoinSpaces();

                                    var c_Proc = Process.Start(c_Info);
                                    c_Proc.WaitForExit();

                                    // Set the path
                                    c_Ans["path"] = c_Target.Path;
                                }
                                else
                                {
                                    // Set the path
                                    c_Ans["path"] = c_Target.Path;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    call.Env.LogException("WHile converting {0} to {1}".FormatString(sSource, sTarget), e);
                }
            }

            call.RespondWithStore(c_Ans);
        }
    }
}
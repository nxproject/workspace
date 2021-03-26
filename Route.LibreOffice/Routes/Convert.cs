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

namespace Route.LibreOffice
{
    /// <summary>
    /// 
    /// Converts a file
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
            string sSource = store["source"].FromBase64URL();
            string sTarget = store["target"].FromBase64URL();

            call.Env.LogInfo("Convert {0} to {1}".FormatString(sSource, sTarget));

            // Must have one
            if (sSource.HasValue() && sTarget.HasValue())
            {
                // Get the file manager
                NX.Engine.Files.ManagerClass c_Mgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

                // Source
                using (DocumentClass c_Source = new DocumentClass(c_Mgr, sSource))
                {
                    // Real?
                    if (c_Source.Exists)
                    {
                        // Target
                        using (DocumentClass c_Target = new DocumentClass(c_Mgr, sTarget))
                        {
                            // Set the path
                            c_Ans["path"] = c_Target.Path;

                            call.Env.LogInfo("Insuring path");

                            // Make the folder
                            c_Target.Folder.AssurePath();

                            // 
                            call.Env.LogInfo("Converting...");

                            // Remove
                            c_Target.Delete(true);

                            // Arguments
                            List<string> c_Args = new List<string>() {
                                        //"-env:UserInstallation=file:///tmp",
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
                                        c_Target.Folder.Location,
                                        c_Source.Location
                                    };

                            // Convert
                            ProcessStartInfo c_Info = new ProcessStartInfo("/usr/bin/libreoffice");
                            c_Info.Arguments = c_Args.JoinSpaces();
                            c_Info.RedirectStandardOutput = true;
                            c_Info.RedirectStandardError = true;
                            Process c_Proc = null;

                            try
                            {
                                // Run
                                c_Proc = Process.Start(c_Info);
                            }
                            catch (Exception e)
                            {
                                call.Env.LogException("While converting {0} to {1}".FormatString(sSource, sTarget), e);
                                // Copy output
                                if (c_Proc != null)
                                {
                                    call.Env.LogInfo("STDOUT: " + c_Proc.StandardOutput.ReadToEnd());
                                    call.Env.LogInfo("STDERR: " + c_Proc.StandardError.ReadToEnd());
                                }
                            }
                            finally
                            {
                                // Copy output
                                if (c_Proc != null)
                                {
                                    call.Env.LogInfo("STDOUT: " + c_Proc.StandardOutput.ReadToEnd());
                                    call.Env.LogInfo("STDERR: " + c_Proc.StandardError.ReadToEnd());
                                }
                                // End
                                c_Proc.WaitForExit();
                            }

                            // 
                            call.Env.LogInfo("Done...");
                        }
                    }
                }
            }

            call.RespondWithStore(c_Ans);
        }

        /// <summary>
        /// 
        /// Gets the --convert-to setting
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private string ConvertExtensionToFilterType(string extension, string target)
        {
            string sFilter = null;

            switch (extension.IfEmpty().ToLower())
            {
                case "doc":
                case "docx":
                case "txt":
                case "rtf":
                case "html":
                case "htm":
                case "xml":
                case "odt":
                case "wps":
                case "wpd":
                    sFilter = ":writer_{0}_Export".FormatString(target);
                    break;

                case "xls":
                case "xlsb":
                case "xlsx":
                case "ods":
                    sFilter = ":calc_{0}_Export".FormatString(target);
                    break;

                case "ppt":
                case "pptx":
                case "odp":
                    sFilter = ":impress_{0}_Export".FormatString(target);
                    break;
            }

            return target + sFilter.IfEmpty();
        }
    }
}
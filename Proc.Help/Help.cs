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

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;
using Proc.Office;

namespace Proc.Help
{
    /// <summary>
    /// 
    /// A route that allows a "regular" website support
    /// 
    /// Make sure that all of the files to be served are in the #rootfolder#/ui 
    /// folder and that none of the subdirectories match a defined route
    /// 
    /// </summary>
    public class Help : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "help", ":file" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Make the folder path
            string sPath = "".WorkingDirectory().CombinePath("ui." + call.Env.UI.Replace("+", "").ToLower()).CombinePath("help").AdjustPathToOS();

            // Get the template
            string sTemplate = sPath.CombinePath("template.html").ReadFile();

            // Get the database
            AO.ManagerClass c_DBMgr = call.Env.Globals.Get<AO.ManagerClass>();

            // Get the file name
            string sFile = store["file"].IfEmpty(c_DBMgr.DefaultDatabase.SiteInfo.HelpRoot).IfEmpty("home").GetFileNameOnlyFromPath();

            // Get the contents
            AO.ObjectClass c_Obj = c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetHelp][sFile];
            if (c_Obj != null)
            {
                // Read the file
                string sContents = c_Obj["text"];

                // Get the values
                JObject c_Values = new JObject();
                c_Values.Merge(call.Env.AsParameters);
                c_Values.Merge(c_DBMgr.DefaultDatabase.SiteInfo.AsJObject);
                c_Values.Merge(c_Obj.AsJObject);
                // Apply changes
                sContents = sContents.Handlebars(new StoreClass(c_Values), delegate (string value)
                {
                    using (Context c_Ctx = new Context(call.Env, new StoreClass(c_Values)))
                    {
                        return Expression.Eval(c_Ctx, value).Value;
                    }
                });

                // Extend
                var c_PB = new Markdig.MarkdownPipelineBuilder();
                var c_PL = Markdig.MarkdownExtensions.UseAdvancedExtensions(c_PB).Build();
                // Convert
                string sHTML = Markdig.Markdown.ToHtml(sContents, c_PL);
                // Into template
                string sFinal = sTemplate.Replace("{0}", sHTML);

                // And deliver
                using (MemoryStream c_Stream = new MemoryStream(sFinal.ToBytes()))
                {
                    call.RespondWithStream("", "text/html", false, c_Stream);
                }
            }

        }
    }
}
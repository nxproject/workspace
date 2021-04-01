﻿///--------------------------------------------------------------------------------
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
using Proc.AO;

namespace Proc.Web
{
    /// <summary>
    /// 
    /// A route that allows a "regular" website support
    /// 
    /// </summary>
    public class Web : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "web", ":file" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Get the database
            AO.ManagerClass c_DBMgr = call.Env.Globals.Get<AO.ManagerClass>();

            // Get the file name
            string sFile = store["file"];

            call.Env.LogInfo("GET: {0}".FormatString(sFile));

            // Get the contents
            AO.ObjectClass c_Obj = c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetWeb][sFile];
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

                // And deliver
                using (MemoryStream c_Stream = new MemoryStream(sContents.ToBytes()))
                {
                    call.RespondWithStream("", "text/html", false, c_Stream);
                }
            }
            else
            {
                call.Env.LogInfo("MISSING: {0}".FormatString(sFile));
            }

        }
    }
}
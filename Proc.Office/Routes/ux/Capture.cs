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

using System.Collections.Generic;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.AO;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Uploads a file
    /// 
    /// </summary>
    public class Capture : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.POST(), "ux", "capture" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            //
            string sID = store["id"];

            // Must have one
            if (sID.HasValue())
            {
                // Get the manager
                AO.ManagerClass c_DBMgr = call.Env.Globals.Get<AO.ManagerClass>();

                // Get the user dataset
                IndexItemClass c_Item = c_DBMgr.DefaultDatabase.IndexStore[sID];
                // Valid?
                if (c_Item != null && c_Item.Type.IsSameValue("upload"))
                {
                    // Make the path
                    string sPath = "/ao/" + c_Item.Values["ds"] + "/" + c_Item.Values["id"] + "/Upload";

                    // Get the manager
                    NX.Engine.Files.ManagerClass c_Mgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

                    // And upload
                    using (DocumentClass c_Doc = c_Mgr.Upload(call, null, c_Mgr, sPath))
                    { }
                }

                // And back
                call.Response.Redirect("/ux/capture?id=" + sID);

            }
        }
    }
}
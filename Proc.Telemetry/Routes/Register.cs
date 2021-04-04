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

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.AO;

namespace Proc.Docs
{
    /// <summary>
    /// 
    /// Registers a call
    /// 
    /// </summary>
    public class Register : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "z", ":id", "?path?" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            call.Env.Debug();

            // Get
            string sID = store["id"];

            // Must have an ID
            if (sID.HasValue())
            {
                // Get database manager
                Proc.AO.ManagerClass c_DBMgr = call.Env.Globals.Get<Proc.AO.ManagerClass>();

                // Make query
                using (QueryClass c_Qry = new QueryClass(c_DBMgr.DefaultDatabase[Proc.AO.DatabaseClass.DatasetTelemetry].DataCollection))
                {
                    // By ID
                    c_Qry.AddByID(sID);
                    // Get
                    List<AO.ObjectClass> c_Poss = c_Qry.FindObjects(1);
                    // Any?
                    if (c_Poss.Count == 1)
                    {
                        // Get values
                        JObject c_Values = c_Poss[0].AsJObject;
                        // Add
                        c_Values.Set("r", store.PathFromEntry("", "path"));
                        c_Values.Set("d", DateTime.Now.ToDBDate());
                        c_Values.Set("i", call.Request.RemoteEndPoint.Address.ToString());

                        // Make the _id
                        c_Values.Set("_id", c_Values.ToSimpleString().MD5HashString());
                        // Get the collection
                        Proc.AO.CollectionClass c_Coll = c_DBMgr.DefaultDatabase[Proc.AO.DatabaseClass.DatasetTelemetryData].DataCollection;
                        // Write
                        c_Coll.AddDirect(c_Values.ToSimpleString());

                        // Get path
                        List<string> c_Path = store.GetAsJArray("path").ToList();
                        // Find the route
                        RouteClass c_Route = call.Env.Router.Get(store, call.Request.HttpMethod, c_Path);
                        if (c_Route != null)
                        {
                            // Reset the tree
                            call.RouteTree = c_Path;
                            // Call the route
                            c_Route.Call(call, store);
                        }
                    }
                }
            }

        }
    }
}
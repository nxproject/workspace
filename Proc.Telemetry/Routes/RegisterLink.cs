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

namespace Proc.Telemetry
{
    /// <summary>
    /// 
    /// Registers a call
    /// 
    /// </summary>
    public class RegisterLink : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "zl", ":id", ":target", "?path?" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Get path
            string sPath = "/" + store.GetAsJArray("path").ToList().Join("/");

            try
            {
                // Get
                string sID = store["id"];
                string sUser = store["target"];

                // Must have an ID
                if (sID.HasValue())
                {
                    // Get database manager
                    Proc.AO.ManagerClass c_DBMgr = call.Env.Globals.Get<Proc.AO.ManagerClass>();

                    // Make data block
                    Proc.Telemetry.DataClass c_Data = new Telemetry.DataClass(c_DBMgr.DefaultDatabase, sID);
                    // And to data
                    c_Data.Via = "Link";

                    // Add
                    c_Data.AddTransaction(sUser, true,
                                            sPath,
                                            call.Request.RemoteEndPoint.Address.ToString());

                    // Do we have one?
                    if (!c_Data.IsBroadcast)
                    {
                        using (AO.AccessClass c_AE = new AccessClass(c_DBMgr, c_Data.To))
                        {
                            c_AE.UpdateContactIn(c_Data.Template.IfEmpty(), c_Data.Via.IfEmpty(), c_Data.Campaign.IfEmpty());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                call.Env.LogException("At RegisterLink", e);
            }

            // Process
            call.Env.HTTP.Process(call.Context, sPath);

        }
    }
}
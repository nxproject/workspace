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
    /// Creates an account
    /// 
    /// </summary>
    public class Subscribe : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "zs", ":id" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Get payload
            string sID = store["id"];
            if (sID.HasValue())
            {
                // Encode
                string sOID = sID.MD5HashString();

                // Get manager
                AO.ManagerClass c_Mgr = call.Env.Globals.Get<AO.ManagerClass>();

                // Make the record
                using (AO.ObjectClass c_Obj = c_Mgr.DefaultDatabase[AO.DatabaseClass.DatasetBillAccess][sOID])
                {
                    // Set
                    c_Obj["name"] = sID;
                    c_Obj["subscribedon"] = DateTime.Now.ToDBDate();
                    c_Obj["optouton"] = "";

                    // Save
                    c_Obj.Save();
                }
            }

            call.RespondWithOK();
        }
    }
}
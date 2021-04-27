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

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.AO;
using Proc.Communication;

namespace Proc.Stripe
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class AskPaymnt : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), SupportClass.Route, "askpayment", ":id" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Get the params
            string sID = store["id"];

            string sURL = call.Env.ReachableURL + "/stripe/fail.html";

            // Must have one
            if (sID.HasValue())
            {
                //
                AO.ManagerClass c_DBMgr = call.Env.Globals.Get<AO.ManagerClass>();

                // Get Site Info
                AO.SiteInfoClass c_SI = c_DBMgr.DefaultDatabase.SiteInfo;

                // Make the query
                using (AO.QueryClass c_Qry = new QueryClass(c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetBiilInvoice].DataCollection))
                {
                    c_Qry.Add("code", QueryElementClass.QueryOps.Eq, sID);

                    // Get
                    foreach (AO.ObjectClass c_Inv in c_Qry.FindObjects(1))
                    {
                        // Must have a balance
                        double dBalance = c_Inv["billed"].ToDouble(0) - c_Inv["payment"].ToDouble(0);
                        if (dBalance > 0)
                        {
                            // Make params
                            JObject c_Params = new JObject();
                            c_Params.Set("id", sID);

                            // Redirect
                            sURL = call.Env.ReachableURL + "/stripe/index.html".URLQuery(c_Params);
                        }
                    }
                }
            }

            // Redirect
            call.Redirect(sURL);
        }
    }
}
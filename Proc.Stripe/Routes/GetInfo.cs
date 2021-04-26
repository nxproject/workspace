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
    public class GetInfo : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), SupportClass.Route, "getinfo", ":id" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Get the params
            string sID = store["id"];
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
                            // Make the context
                            using (ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, null, null, call.UserInfo.Name))
                            {
                                // Do handlebars
                                HandlebarDataClass c_HData = new HandlebarDataClass(call.Env);
                                // Add the object
                                c_HData.Merge(c_Inv.Explode(ExplodeMakerClass.ExplodeModes.Yes, c_Ctx));

                                // Make message
                                using (eMessageClass c_Msg = eMessageClass.FromStore(call.Env, store, c_HData))
                                {
                                    // Get the body
                                    string sMsg = c_Msg.FormatMessage("", c_SI.PayMakeTemplate, "PaymentTemplate.html", "EMail", true);

                                    //
                                    JObject c_Resp = new JObject();
                                    c_Resp.Set("publicKey", c_SI.StripePublic);

                                    // TBD
                                    c_Resp.Set("amount", "{0:#0.00}".FormatString(dBalance));
                                    c_Resp.Set("body", sMsg);

                                    call.RespondWithJSON(c_Resp);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
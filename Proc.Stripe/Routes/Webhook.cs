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
    public class Webhook : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.POST(), SupportClass.Route, "wh" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Get the params
            JObject c_Params = call.BodyAsJObject;

            // Get working
            JObject c_Wkg = c_Params.AssureJObject("data").AssureJObject("object");

            // Get reference
            string sRefID = c_Wkg.Get("client_reference_id");
            string sRet = null;

            // Any?
            if(sRefID.HasValue())
            {
                // Parse
                string[] asPieces = sRefID.Split(":");
                // Must have two
                if(asPieces.Length == 2)
                {
                    // Validate token
                    using(Proc.Office.TokenClass c_Token = new Office.TokenClass(asPieces[1]))
                    {
                        //
                        if(c_Token.IsValid)
                        {
                            // Process payment
                            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();
                            sRet = c_Mgr.Pay(asPieces[0], c_Wkg.Get("amount_total").IfEmpty().ToDouble(0) / 100);
                            if(sRet.HasValue())
                            {
                                sRet = "STRIPE: {0}".FormatString(sRet);
                            }
                        }
                        else
                        {
                            sRet = "STRIPE: Invalid token";
                        }
                    }
                }
                else
                {
                    sRet = "STRIPE: Client reference id has improper format '{0}'".FormatString(sRefID);
                }
            }
            else
            {
                sRet = "STRIPE: Missing client reference id";
            }

            //
            if (sRet.HasValue())
            {
                call.Env.LogError(sRet);
                call.RespondWithError(sRet);
            }
            else
            {
                //
                call.RespondWithOK();
            }
        }
    }
}
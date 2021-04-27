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
using Stripe;
using Stripe.Checkout;

using NX.Shared;
using NX.Engine;
using Proc.AO;

namespace Proc.Stripe
{
    public class ManagerClass : ChildOfClass<EnvironmentClass>
    {
        #region Constructor
        public ManagerClass(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Holds the settings
        /// 
        /// </summary>
        private AO.SiteInfoClass SiteInfo { get; set; }

        /// <summary>
        /// 
        /// Has the stripe sub-sys been initialized?
        /// </summary>
        private bool IsInit { get; set; }
        #endregion

        #region Methods
        private void Initialize()
        {
            // Only once
            if (!this.IsInit)
            {
                // Flag
                this.IsInit = true;

                //
                AO.ManagerClass c_Mgr = this.Parent.Globals.Get<AO.ManagerClass>();
                //
                this.SiteInfo = c_Mgr.DefaultDatabase.SiteInfo;

                //
                if (this.SiteInfo.StripeSecurity.HasValue())
                {
                    // Make
                    StripeConfiguration.ApiKey = this.SiteInfo.StripeSecurity;

                    // Get the URL
                    string sURL = this.Parent.ReachableURL.CombinePath(SupportClass.Route, "wh");

                    // Assume new
                    bool bIsNew = true;

                    // Create service
                    var c_Svc = new WebhookEndpointService();
                    // List
                    foreach(var c_EP in c_Svc.List())
                    {
                        // URL Match?
                        if(sURL.IsSameValue(c_EP.Url))
                        {
                            bIsNew = false;
                        }
                    }

                    // If new, add
                    if (bIsNew)
                    {

                        // Create the endpoint
                        var c_Options = new WebhookEndpointCreateOptions
                        {
                            Url = sURL,
                            EnabledEvents = new List<string>
                          {
                                "checkout.session.completed"
                          }
                        };

                        var c_Ans = c_Svc.Create(c_Options);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// Creates a checkput session
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Checkout(string id)
        {
            string sAns = null;

            // Initialize
            this.Initialize();

            //
            AO.ManagerClass c_DBMgr = this.Parent.Globals.Get<AO.ManagerClass>();

            // Get Site Info
            AO.SiteInfoClass c_SI = c_DBMgr.DefaultDatabase.SiteInfo;

            // Make the query
            using (AO.QueryClass c_Qry = new QueryClass(c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetBiilInvoice].DataCollection))
            {
                c_Qry.Add("code", QueryElementClass.QueryOps.Eq, id);

                // Get
                foreach (AO.ObjectClass c_Inv in c_Qry.FindObjects(1))
                {
                    // Must have a balance
                    double dBalance = c_Inv["billed"].IfEmpty().ToDouble(0) - c_Inv["payment"].IfEmpty().ToDouble(0);
                    if (dBalance > 0)
                    {
                        //
                        using (Proc.Office.TokenClass c_Token = new Office.TokenClass(""))
                        {
                            SessionCreateOptions c_Options = new SessionCreateOptions()
                            {
                                PaymentMethodTypes = new System.Collections.Generic.List<string>() { "card" },
                                LineItems = new System.Collections.Generic.List<SessionLineItemOptions>()
                            {
                                new SessionLineItemOptions()
                                {
                                    PriceData = new SessionLineItemPriceDataOptions()
                                    {
                                        UnitAmount = (long)(dBalance * 100),
                                        Currency = "USD",
                                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                                        {
                                            Name = c_Inv["desc"]
                                        }
                                    },
                                    Quantity=1
                                }
                            },
                                Mode = "payment",
                                SuccessUrl = this.Parent.ReachableURL.CombineURL("stripe/done.html"),
                                CancelUrl = this.Parent.ReachableURL.CombineURL("stripe/fail.html"),
                                ClientReferenceId = id + ":" + c_Token.Till()
                            };
                            var c_Svc = new SessionService();
                            Session c_Session = c_Svc.Create(c_Options);

                            //
                            JObject c_Resp = new JObject();
                            c_Resp.Set("sessionId", c_Session.Id);
                            c_Resp.Set("publicKey", c_SI.StripePublic);

                            // 
                            sAns = c_Resp.ToSimpleString();
                        }
                    }
                }
            }

            return sAns.IfEmpty("{}");
        }

        /// <summary>
        ///  
        /// Processes payment
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amt"></param>
        /// <returns></returns>
        public string Pay(string id, double amt)
        {
            string sAns = null;

            // Initialize
            this.Initialize();

            //
            AO.ManagerClass c_DBMgr = this.Parent.Globals.Get<AO.ManagerClass>();

            // Get Site Info
            AO.SiteInfoClass c_SI = c_DBMgr.DefaultDatabase.SiteInfo;

            // Make the query
            using (AO.QueryClass c_Qry = new QueryClass(c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetBiilInvoice].DataCollection))
            {
                c_Qry.Add("code", QueryElementClass.QueryOps.Eq, id);

                //
                sAns = "Payment not found for '{0}'".FormatString(id);

                // Get
                foreach (AO.ObjectClass c_Inv in c_Qry.FindObjects(1))
                {
                    // Must have a balance
                    double dBalance = c_Inv["billed"].IfEmpty().ToDouble(0) - c_Inv["payment"].IfEmpty().ToDouble(0);
                    if (amt > 0 && dBalance > 0 && amt <= dBalance)
                    {
                        // Update
                        double dPay = c_Inv["payment"].IfEmpty().ToDouble(0) + amt;
                        c_Inv["payment"] = dPay.ToString();
                        c_Inv["paidon"] = DateTime.Now.ToDBDate();
                        c_Inv.Save();

                        sAns = null;
                    }
                    else
                    {
                        sAns = "Invoice already paid or invalid amount for '{0}'.  Balance: {1}, amount: {2}".FormatString(id, dBalance, amt);
                    }
                }
            }

            return sAns;
        }
        #endregion
    }
}
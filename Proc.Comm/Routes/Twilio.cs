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
using Twilio;
using Twilio.TwiML;

using NX.Engine;
using NX.Shared;
using Proc.AO;

namespace Proc.Communication
{
    /// <summary>
    /// 
    /// Handles inbound Twilio
    /// 
    /// </summary>
    public class TwilioR : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.POST(), "twilio", ":nxmode", ":nxuser" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // response
            TwiML c_Resp = null;

            // Make the environment
            using (TwilioEnvironmentClass c_Env = new TwilioEnvironmentClass(call.Env, store, call.BodyAsJObject))
            {
                // If we have a task
                if(c_Env.Task.HasValue())
                {
                    // Call
                    using (TaskParamsClass c_Params = new TaskParamsClass(call.Env))
                    {
                        c_Params.Task = c_Env.Task;

                        if (c_Env.Caller != null) c_Params.AddObject("caller", c_Env.Caller);
                        if (c_Env.Called != null) c_Params.AddObject("called", c_Env.Caller);

                        c_Params.AddStore("passed", c_Env.Params);

                        c_Params.Call();
                    }
                }
                // According to type
                switch (store["nxmode"])
                {
                    case "voice":
                        c_Resp = this.HandleVoice(c_Env);
                        break;

                    case "sms":
                        c_Resp = this.HandleSMS(c_Env);
                        break;
                }
            }

            //
            string sResponse = "";
            if (c_Resp != null)
            {
                sResponse = c_Resp.ToString();
            }

            //
            using (MemoryStream c_Stream = new MemoryStream(sResponse.ToBytes()))
            {
                call.RespondWithStream("", "text/xml", false, c_Stream);
            }
        }

        #region Methods
        /// <summary>
        /// 
        /// Process voice calls
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        private TwiML HandleVoice(TwilioEnvironmentClass env)
        {
            //
            TwiML c_Ans = null;

            // TBD

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Process SMS calls
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>TwilioEnvironmentClass env)
        private TwiML HandleSMS(TwilioEnvironmentClass env)
        {
            //
            TwiML c_Ans = null;

            // TBD

            return c_Ans;
        }
        #endregion
    }

    public class TwilioEnvironmentClass : ChildOfClass<EnvironmentClass>
    {
        #region Constructor
        public TwilioEnvironmentClass(EnvironmentClass env, StoreClass passed, JObject body)
            : base(env)
        {
            //
            this.DBManager = this.Parent.Globals.Get<AO.ManagerClass>();

            // Get the task
            this.Task = this.Database.SiteInfo.TwilioTask;
            // Only if a task was defined
            if (this.Task.HasValue())
            {
                // Decode user
                string sUser = passed["nxuser"].FromBase64URL();
                // Valid?
                if (AO.UUIDClass.IsValid(sUser))
                {
                    //
                    using (AO.UUIDClass c_UUID = new AO.UUIDClass(this.Database, sUser))
                    {
                        // Get
                        this.Called = c_UUID.AsObject;
                    }
                }

                // Load rest
                passed.LoadFrom(body);
                this.Params = passed;

                // Get the caller hone
                string sCaller = this.Params["From"].ToPhone();

                // Get caller dataset
                string sCallerDS = this.Database.SiteInfo.PhoneAccessDS;
                // Must have one
                if (sCallerDS.HasValue())
                {
                    // Get the dataset
                    AO.DatasetClass c_DS = this.Database[sCallerDS];
                    // Open the dataset definition
                    AO.Definitions.DatasetClass c_DSDef = c_DS.Definition;
                    // Get the fields
                    List<string> c_LUFields = c_DSDef.AccessPhoneFields;
                    // Loop until we have one
                    foreach (string sLUField in c_LUFields)
                    {
                        // Look up
                        using (QueryClass c_Qry = new QueryClass(c_DS.DataCollection))
                        {
                            c_Qry.Add(sLUField, QueryElementClass.QueryOps.Eq, sCaller);

                            // Get the objects
                            List<AO.ObjectClass> c_Poss = c_Qry.FindObjects(1);
                            // Any?
                            if (c_Poss.Count > 0)
                            {
                                this.Caller = c_Poss[0];
                            }
                        }

                        // Once we got one, we are done
                        if (this.Caller != null) break;
                    }
                }
            }
        }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            //
            this.DBManager.Dispose();

            base.Dispose();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The database manager
        /// 
        /// </summary>
        public AO.ManagerClass DBManager { get; private set; }

        /// <summary>
        /// 
        /// The database
        /// 
        /// </summary>
        public AO.DatabaseClass Database { get { return this.DBManager.DefaultDatabase; } }

        /// <summary>
        /// 
        /// The task to run
        /// 
        /// </summary>
        public string Task { get; private set; }

        /// <summary>
        /// 
        /// The user object
        /// 
        /// </summary>
        public AO.ObjectClass Called { get; private set; }

        /// <summary>
        /// 
        /// The caller object
        /// 
        /// </summary>
        public AO.ObjectClass Caller { get; private set; }

        /// <summary>
        /// 
        /// Params
        /// 
        /// </summary>
        public StoreClass Params { get; private set; }
        #endregion
    }
}
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

/// Packet Manager Requirements
/// 
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;

namespace Proc.AO
{
    public class Names
    {
        public const string Passed = "passed";
        public const string Original = "params";
        public const string Local = "l";
        public const string Obj = "obj";
        public const string Portal = "portal";

        public const string Sys = "sys";
        public const string Changes = "changes";
        public const string User = "user";
    }

    public class ExtendedContextClass : Context
    {
        #region Constructor
        public ExtendedContextClass(EnvironmentClass env, StoreClass store, AO.ObjectClass obj, string user)
            : base(env, store)
        {
            //
            this.DocumentManager = this.Parent.Globals.Get<NX.Engine.Files.ManagerClass>();
            this.DBManager = this.Parent.Globals.Get<AO.ManagerClass>();

            //
            this.Objects = new ContextStoreClass<AO.ObjectClass>();

            this.Stores = new ContextStoreClass<StoreClass>();
            this.Documents = new ContextStoreClass<NX.Engine.Files.DocumentClass>();

            // Did we get a store?
            if (store != null)
            {
                this.Stores[Names.Passed] = store;
                this.Stores.Use(Names.Passed);

                //
                this.SenderUUID = store["_uuid"];
                this.WinID = store["_winid"];
            }

            // Did we get an object?
            if (obj != null)
            {
                this.Objects[Names.Passed] = obj;
                this.Objects.Use(Names.Passed);
            }

            // Add sys
            this.Stores[Names.Sys] = new StoreClass(this.SiteInfo.AsJObject);

            // Add user
            string sUser = user;
            if (!sUser.HasValue() && store != null) sUser = store["_user"];
            if (sUser.HasValue())
            {
                this.User = new ExtendedUserClass(this.GetObject(AO.DatabaseClass.DatasetUser, sUser).AsJObject, this.DBManager.DefaultDatabase.SiteInfo);
                this.Stores[Names.User] = this.User;
            }

            this.Callback = this.ObjectMap;
        }
        #endregion

        #region Indexer
        public string this[string key]
        {
            get
            {
                using (DatumClass c_Datum = new DatumClass(this, key))
                {
                    return c_Datum.Value;
                }
            }
            set
            {
                using (DatumClass c_Datum = new DatumClass(this, key))
                {
                    c_Datum.Value = value;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The manager
        /// 
        /// </summary>
        public NX.Engine.Files.ManagerClass DocumentManager { get; internal set; }

        /// <summary>
        /// 
        /// The database manager
        /// 
        /// </summary>
        public AO.ManagerClass DBManager { get; internal set; }

        /// <summary>
        /// 
        /// The database being used
        /// 
        /// </summary>
        public AO.DatabaseClass Database { get { return this.DBManager.DefaultDatabase; } }

        /// <summary>
        /// 
        /// The current user
        /// 
        /// </summary>
        public ExtendedUserClass User { get; set; }

        /// <summary>
        /// 
        /// UUID of sender
        /// 
        /// </summary>
        public string SenderUUID { get; private set; }

        /// <summary>
        /// 
        /// The window that sent the call
        /// 
        /// </summary>
        public string WinID { get; private set; }

        /// <summary>
        /// 
        /// The site information
        /// 
        /// </summary>
        public SiteInfoClass SiteInfo { get { return this.DBManager.DefaultDatabase.SiteInfo; } }

        /// <summary>
        /// 
        /// Storage
        /// 
        /// </summary>
        public ContextStoreClass<AO.ObjectClass> Objects { get; internal set; }
        #endregion

        #region Database
        /// <summary>
        /// 
        /// Gets an object
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public AO.ObjectClass GetObject(AO.DatasetClass ds, string id)
        {
            return ds[id];
        }

        public AO.ObjectClass GetObject(string ds, string id)
        {
            return this.GetObject(this.Database[ds], id);
        }

        public AO.ObjectClass GetObject(AO.UUIDClass uuid)
        {
            return this.GetObject(uuid.Dataset, uuid.ID);
        }

        /// <summary>
        /// 
        /// Gets a list of objects
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public List<AO.ObjectClass> Query(AO.DatasetClass ds, params string[] values)
        {
            using(AO.QueryClass c_Qry = new QueryClass(ds.DataCollection))
            {
                for(int i=0;i < values.Length;i+=2)
                {
                    c_Qry.Add(values[i], QueryElementClass.QueryOps.Eq, values[i + 1]);
                }

                // Do
                return c_Qry.FindObjects();
            }
        }
        #endregion

        #region DateTime
        public DateTime Today()
        {
            return DateTime.Today.AdjustTimezone(this.SiteInfo.Timezone);
        }

        public DateTime Now()
        {
            return DateTime.Now.AdjustTimezone(this.SiteInfo.Timezone);
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Handles the mapping of objects
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ObjectMap(ExprCBParams value)
        {
            // Assume none
            string sAns = null;

            // Get the name
            string sName = value.Prefix.IfEmpty(Names.Passed);

            // Is there an object?
            if(this.Objects.Has(sName))
            {
                sAns = this.Objects[sName][value.Field];
            }
            else if (this.Stores.Has(sName))
            {
                sAns = this.Stores[sName].GetAsString(value.Field);
            }

            return sAns.IfEmpty();
        }

        public override string ToString()
        {
            StringBuilder c_Buffer = new StringBuilder();

            c_Buffer.AppendLine("USER\r\n" + this.User.ToString());
            c_Buffer.AppendLine("SITE\r\n " + this.SiteInfo.ToString());

            foreach(string sStore in this.Stores.Keys)
            {
                c_Buffer.Append("STORE {0}\r\n".FormatString(sStore) + this.Stores[sStore].ToStringSafe());
            }

            return c_Buffer.ToString();
        }
        #endregion

        #region SIO
        public void SendQM(string user, string mesaage, List<string> attachments)
        {
            SIO.ManagerClass c_Mgr = this.Parent.Globals.Get<SIO.ManagerClass>();

            // Attachments
            string sAtt = new JArray(attachments).ToSimpleString();

            //
            using(SIO.MessageClass c_Msg = new SIO.MessageClass(c_Mgr, false, "$$noti", "to", user, "type", "QM", "msg", mesaage, "att", sAtt))
            {
                c_Msg.Send();
            }
        }
        #endregion
    }
}
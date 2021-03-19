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
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;
using Proc.AO;

namespace Route.ADEX
{
    /// <summary>
    /// 
    /// Sends a store to another site and processes return
    /// 
    /// </summary>
    public class Do : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.POST(), Constants.Route, "sync" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Use the body
            RequestClass c_Data = new RequestClass(call.Env, call.BodyAsStore);

            // Assume failure
            bool bOK = false;

            // Validate
            if (this.Validate(c_Data))
            {
                // Do by fn
                switch (c_Data.Function)
                {
                    case "get":
                        bOK = this.Get(c_Data, c_Ans);
                        break;

                    case "put":
                        bOK = this.Put(c_Data, c_Ans);
                        break;

                    case "create":
                        bOK = this.Create(c_Data, c_Ans);
                        break;

                    case "next":
                        bOK = this.Next(c_Data, c_Ans);
                        break;

                    case "delta":
                        bOK = this.Delta(c_Data, c_Ans);
                        break;

                    case "list":
                        bOK = this.List(c_Data, c_Ans);
                        break;

                    case "map.get":
                        bOK = this.MapGet(c_Data, c_Ans);
                        break;

                    case "map.put":
                        bOK = this.MapPut(c_Data, c_Ans);
                        break;

                    case "map.delete":
                        bOK = this.MapDelete(c_Data, c_Ans);
                        break;

                    case "doc.get":
                        bOK = this.DocGet(c_Data, c_Ans);
                        break;

                    case "doc.put":
                        bOK = this.DocPut(c_Data, c_Ans);
                        break;

                    case "doc.list":
                        bOK = this.DocList(c_Data, c_Ans);
                        break;

                    case "folder.list":
                        bOK = this.FolderList(c_Data, c_Ans);
                        break;
                }
            }

            // Failed?
            if (!bOK)
            {
                // Tell user
                call.RespondWithFail();
            }
            else
            {
                // No callback, return response
                call.RespondWithStore(c_Ans);
            }
        }

        #region Methods
        /// <summary>
        /// 
        /// Assures that the user is valid and can do the call
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool Validate(RequestClass source)
        {
            // Assume failure
            bool bAns = false;

            // Do we have what we need?
            if(source.Function.HasValue() &&
                source.Name.HasValue() &&
                source.Password.HasValue() &&
                source.Dataset.HasValue())
            {
                // Get the user dataset
                DatasetClass c_DS = source.DBManager.DefaultDatabase[DatabaseClass.DatasetUser];
                // Get the user ID
                string sID = source.Name.MD5HashString();
                // Get the user
                Proc.AO.Definitions.UserClass c_User = Proc.AO.Definitions.UserClass.Get(source.Env, source.Name);
                // Is it real?
                bAns = c_User.IsValid;
                // Is it valid?
                if (bAns) bAns = Proc.AO.Definitions.UserClass.ValidatePassword(c_User.Password, source.Password);

                // TBD - Validate rights against dataset
            }

            return bAns;
        }

        public bool AssureID(RequestClass source, bool isoptional = false)
        {
            // TBD

            return true;
        }

        /// <summary>
        /// 
        /// Assures that a query is present
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool AssureQuery(RequestClass source)
        {
            return source.Query.HasValue();
        }

        /// <summary>
        /// 
        /// Assures that a map is present
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool AssureMap(RequestClass source)
        {
            return source.Map.HasValue();
        }

        /// <summary>
        /// 
        /// Assures that a doc is present
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool AssureDoc(RequestClass source)
        {
            return this.AssureID(source) && source.Doc.HasValue();
        }
        #endregion

        #region Calls
        private bool Get(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if(this.AssureID(source))
            { }

            return bAns;
        }

        private bool Put(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureID(source))
            { }

            return bAns;
        }

        private bool Create(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureID(source, true))
            { }

            return bAns;
        }

        private bool Next(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureID(source, true))
            { }

            return bAns;
        }

        private bool Delta(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureID(source, true))
            { }

            return bAns;
        }

        private bool List(RequestClass source, StoreClass target)
        { 
            // Assume failure
            bool bAns = false;

            // TBD

            return bAns;
        }

        private bool MapGet(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureMap(source))
            {
                // Get the dataset
                Proc.AO.DatasetClass c_DS = source.DBManager.DefaultDatabase["_dex_map"];

                // Get the object
                Proc.AO.ObjectClass c_Map = c_DS[source.Map];

                // Set the query
                source.Query = c_Map["query"];
            }

            return bAns;
        }

        private bool MapPut(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureMap(source) && this.AssureQuery(source))
            {
                // Get the dataset
                Proc.AO.DatasetClass c_DS = source.DBManager.DefaultDatabase["_dex_map"];

                // Get the object
                Proc.AO.ObjectClass c_Map = c_DS[source.Map];

                // Set the query
                c_Map["query"] = source.Query;

                // Save
                c_Map.Save();
            }

            return bAns;
        }

        private bool MapDelete(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureMap(source))
            { 
                // Get the dataset
                Proc.AO.DatasetClass c_DS = source.DBManager.DefaultDatabase["_dex_map"];

                // Get the object
                Proc.AO.ObjectClass c_Map = c_DS[source.Map];

                // Delete
                c_Map.Delete();
            }

            return bAns;
        }

        private bool DocGet(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureDoc(source))
            {
                // Get the data
                target.Set("data", source.DocumentObject.ValueAsBytes.ToBase64());
                // Flag
                bAns = true;
            }

            return bAns;
        }

        private bool DocPut(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureDoc(source))
            {
                // Save the data
                source.DocumentObject.ValueAsBytes = source.Content.FromBase64Bytes();
                // Flag
                bAns = true;
            }

            return bAns;
        }

        private bool DocList(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureID(source))
            {
                // Make room
                JArray c_Docs = new JArray();
                // Loop thru
                foreach(var c_Doc in source.FolderObject.Files)
                {
                    // Add
                    c_Docs.Add(c_Doc.Name);
                }
                // Get the data
                target.Set("docs", c_Docs);
                // Flag
                bAns = true;
            }

            return bAns;
        }

        private bool FolderList(RequestClass source, StoreClass target)
        {
            // Assume failure
            bool bAns = false;

            if (this.AssureID(source))
            {
                // Make room
                JArray c_Docs = new JArray();
                // Loop thru
                foreach (NX.Engine.Files.FolderClass c_Folder in source.FolderObject.Folders)
                {
                    // Add
                    c_Docs.Add(c_Folder.Name);
                }
                // Get the data
                target.Set("folders", c_Docs);
                // Flag
                bAns = true;
            }

            return bAns;
        }
        #endregion
    }
}
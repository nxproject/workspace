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

using NX.Engine;
using NX.Shared;
using Proc.AO;

namespace Route.ADEX
{
    public class RequestClass : ChildOfClass<StoreClass>
    {
        #region Constructor
        public RequestClass(EnvironmentClass env, StoreClass store)
            : base(store)
        { }
        #endregion

        #region Properties
        public string Function
        {
            get { return this.Parent["fn"].IfEmpty().ToLower(); }
            set { this.Parent["fn"] = value; }
        }

        public string Name
        {
            get { return this.Parent["name"].IfEmpty(); }
        }

        public string Password
        {
            get { return this.Parent["pwd"].IfEmpty(); }
        }

        public string Dataset
        {
            get { return this.Parent["ds"].IfEmpty(); }
        }

        public string ID
        {
            get { return this.Parent["id"].IfEmpty(); }
            set { this.Parent["id"] = value; }
        }

        public string Query
        {
            get { return this.Parent["query"].IfEmpty(); }
            set { this.Parent["query"] = value; }
        }

        public string Map
        {
            get { return this.Parent["map"].IfEmpty(); }
            set { this.Parent["map"] = value; }
        }

        public string Folder
        {
            get { return this.Parent["folder"].IfEmpty(); }
            set { this.Parent["folder"] = value; }
        }

        public string Doc
        {
            get { return this.Parent["doc"].IfEmpty(); }
            set { this.Parent["doc"] = value; }
        }

        public string Content
        {
            get { return this.Parent["content"].IfEmpty(); }
            set { this.Parent["content"] = value; }
        }
        #endregion

        #region Computed
        /// <summary>
        /// 
        /// The query as a MongoDb filter
        /// 
        /// </summary>
        public QueryClass QueryObject
        {
            get
            {
                QueryClass c_Ans = null;

                string sWkg = this.Parent["query"];
                if (sWkg.HasValue())
                {
                    c_Ans = new QueryClass(null, this.DBManager.DefaultDatabase[this.Dataset].DataCollection, sWkg);
                }

                return c_Ans;
            }
        }

        /// <summary>
        /// 
        /// The document
        /// 
        /// </summary>
        public NX.Engine.Files.FolderClass FolderObject
        {
            get { return this.Object.Folder.SubFolder(this.Folder); }
        }

        /// <summary>
        /// 
        /// The document
        /// 
        /// </summary>
        public NX.Engine.Files.DocumentClass DocumentObject
        {
            get { return this.Object.Folder.SubFolder(this.Folder)[this.Doc]; }
        }

        /// <summary>
        /// 
        /// The current environment
        /// 
        /// </summary>
        public EnvironmentClass Env { get; private set; }

        /// <summary>
        /// 
        /// The MongoDb interface
        /// 
        /// </summary>
        public Proc.AO.ManagerClass DBManager { get { return this.Env.Globals.Get<Proc.AO.ManagerClass>(); } }

        /// <summary>
        /// 
        /// The MongoDb interface
        /// 
        /// </summary>
        public NX.Engine.Files.ManagerClass DocManager
        {
            get { return this.Env.Globals.Get<NX.Engine.Files.ManagerClass>(); }
        }

        /// <summary>
        /// 
        ///  The working object
        ///  
        /// </summary>
        private Proc.AO.ObjectClass IObject { get; set; }
        public Proc.AO.ObjectClass Object
        {
            get
            {
                if(this.IObject == null)
                {
                    this.IObject = this.DBManager.DefaultDatabase[this.Dataset][this.ID];
                }

                return this.IObject;
            }
        }
        #endregion
    }
}
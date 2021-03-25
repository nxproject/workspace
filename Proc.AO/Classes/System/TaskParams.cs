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
using System.Text;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public class TaskParamsClass : ChildOfClass<EnvironmentClass>
    {
        #region Constants
        private const string KeyTask = "__task";
        private const string KeyObjects = "__objects";
        private const string KeyStore = "__stores";
        #endregion

        #region Constructor
        public TaskParamsClass(EnvironmentClass env)
            : base(env)
        {
            //
            this.Values = new StoreClass();
            this.Values.Set(KeyObjects, new JObject());
            this.Values.Set(KeyStore, new JObject());
        }

        public TaskParamsClass(EnvironmentClass env, StoreClass values)
            : base(env)
        {
            //
            this.Values = values;
            if (this.Values == null) this.Values = new StoreClass();
        }
        #endregion

        #region Indexer
        public string this[string key]
        {
            get { return this.Values[key]; }
            set { this.Values[key] = value; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The store to be passed
        /// 
        /// </summary>
        private StoreClass Values { get; set; }

        /// <summary>
        ///  
        /// List of all the keys in the key/value chain
        /// 
        /// </summary>
        public List<string> Keys
        {
            get
            {
                //
                List<string> c_Ans = this.Values.Keys;
                // Loop thru
                for(int i=c_Ans.Count -1;i>=0;i--)
                {
                    if (c_Ans[i].StartsWith("__")) c_Ans.RemoveAt(i);
                }

                return c_Ans;
            }
        }

        /// <summary>
        /// 
        /// A list of the objects
        /// 
        /// </summary>
        public List<string> Objects
        {
            get { return this.Values.GetAsJObject(KeyObjects).Keys(); }
        }

        /// <summary>
        /// 
        /// A list of the stores
        /// 
        /// </summary>
        public List<string> Stores
        {
            get { return this.Values.GetAsJObject(KeyStore).Keys(); }
        }

        /// <summary>
        /// 
        /// The task to run
        /// 
        /// </summary>
        public string Task
        {
            get { return this.Values[KeyTask]; }
            set { this.Values[KeyTask] = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Calls the task start
        /// 
        /// </summary>
        /// <returns></returns>
        public StoreClass Call()
        {
            return this.Parent.FN("Task.Start", this.Values);
        }

        /// <summary>
        /// 
        /// Get the object UUID
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetObject(string name)
        {
            // Get
            return this.Values.GetAsJObject(KeyObjects).Get(name);
        }

        /// <summary>
        /// 
        /// Adds an object
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void AddObject(string name, AO.ObjectClass obj)
        {
            // Get
            JObject c_Values = this.Values.GetAsJObject(KeyObjects);
            // Add uuid
            c_Values.Set(name, obj.UUID.ToString());
            // And back
            this.Values.Set(KeyObjects, c_Values);
        }

        /// <summary>
        /// 
        /// Get the store
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StoreClass GetStore(string name)
        {
            // Get
            return this.Values.GetAsJObject(KeyStore).GetAs<StoreClass>(name);
        }

        /// <summary>
        /// 
        /// Adds an object
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="uuid"></param>
        public void AddObject(string name, AO.UUIDClass uuid)
        {
            // Get
            JObject c_Values = this.Values.GetAsJObject(KeyObjects);
            // Add uuid
            c_Values.Set(name, uuid.ToString());
            // And back
            this.Values.Set(KeyObjects, c_Values);
        }

        /// <summary>
        /// 
        /// Adds a store
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="store"></param>
        public void AddStore(string name, StoreClass store)
        {
            // Get
            JObject c_Values = this.Values.GetAsJObject(KeyStore);
            // Add uuid
            c_Values.Set(name, store);
            // And back
            this.Values.Set(KeyStore, c_Values);
        }
        #endregion
    }
}
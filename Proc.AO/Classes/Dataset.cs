///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
/// 
/// This work is covered by GPL v3 as defined in https://www.gnu.org/licenses/gpl-3.0.en.html
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
/// Install-Package MongoDb.Bson -Version 2.11.0
/// 

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MongoDB.Bson;

using NX.Shared;
using NX.Engine;
using Proc.AO.BuiltIn;

namespace Proc.AO
{
    public class DatasetClass : ChildOfClass<DatabaseClass>
    {
        #region Constructor
        internal DatasetClass(DatabaseClass mgr, string name)
            : base(mgr)
        {
            //
            this.Name = name;

            // Default
            this.Define();

            // Loop thru fields
            foreach (string sFld in this.Definition.FieldNames)
            {
                // Index?
                if (this.Definition[sFld].UseIndex)
                {
                    this.DataCollection.CreateIndex(sFld);
                }
            }
        }
        #endregion

        #region Indexer
        public ObjectClass this[string id]
        {
            get { return new ObjectClass(this, id); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The name of the dataset
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// The collection
        /// 
        /// </summary>
        public CollectionClass DataCollection
        {
            get
            {
                // Make name
                string sName = CollectionClass.MakeName(this, "", "");

                //
                return new CollectionClass(this, "");

                //// Already setup?
                //if (!this.Collections.Contains(sName))
                //{
                //    // Make
                //    this.Collections[sName] = new CollectionClass(this, "");
                //}

                //return this.Collections[sName];
            }
        }

        /// <summary>
        /// 
        /// The collection
        /// 
        /// </summary>
        public CollectionClass SettingsCollection
        {
            get
            {
                // Make name
                string sName = CollectionClass.MakeName(this, "#", "");

                //
                return new CollectionClass(this, "#");
            }
        }

        /// <summary>
        /// 
        /// A storage collection
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CollectionClass NamedCollection(string name)
        {
            // Adjust
            name = "_" + name.IfEmpty();

            // Make name
            string sName = CollectionClass.MakeName(this, "", name);

            //
            return new CollectionClass(this, "", name);
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Creates a new object
        /// 
        /// </summary>
        /// <returns></returns>
        public ObjectClass New(string id = null, ExtendedContextClass ctx = null)
        {
            //
            ObjectClass c_Ans = null;

            //
            if (!id.HasValue())
            {
                c_Ans = new ObjectClass(this, ctx);
            }
            else
            {
                // Make
                c_Ans = this[id];
            }

            return c_Ans;
        }
        #endregion

        #region Definition
        private Definitions.DatasetClass IDefinition { get; set; }
        public Definitions.DatasetClass Definition
        {
            get
            {
                if (this.IDefinition == null)
                {
                    this.IDefinition = new Definitions.DatasetClass(this);
                }

                return this.IDefinition;
            }
        }
        #endregion

        #region Views
        public List<string> Views
        {
            get 
            { 
                List<string> c_Ans = this.SettingsCollection.Names(Definitions.ViewClass.Prefix);
                // Must jave default
                if (!c_Ans.Contains("default")) c_Ans.Add("default");

                return c_Ans;
            }
        }

        /// <summary>
        /// 
        /// The views
        /// 
        /// </summary>
        private NamedListClass<Definitions.ViewClass> ViewCache { get; set; } = new NamedListClass<Definitions.ViewClass>();

        /// <summary>
        /// 
        /// Returns a view definition
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Definitions.ViewClass View(string name, bool localize = false)
        {
            // Holding area
            Definitions.ViewClass c_Ans = null;

            // Local
            if (localize)
            {
                c_Ans = new Definitions.ViewClass(this, name);
            }
            else
            {
                // Do we know it?
                if (!this.ViewCache.ContainsKey(name))
                {
                    // Add
                    this.ViewCache[name] = new Definitions.ViewClass(this, name);
                }

                c_Ans = this.ViewCache[name];
            }

            return c_Ans;
        }
        #endregion

        #region Tasks
        public List<string> Tasks
        {
            get { return this.SettingsCollection.Names(Definitions.TaskClass.Prefix); }
        }

        /// <summary>
        /// 
        /// The tasks
        /// 
        /// </summary>
        private NamedListClass<Definitions.TaskClass> TaskCache { get; set; } = new NamedListClass<Definitions.TaskClass>();

        /// <summary>
        /// 
        /// Returns a task definition
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Definitions.TaskClass Task(string name)
        {
            // Do we know it?
            if (!this.TaskCache.ContainsKey(name))
            {
                // Add
                this.TaskCache[name] = new Definitions.TaskClass(this, name);
            }

            return this.TaskCache[name];
        }

        /// <summary>
        /// 
        /// Get a shadow task entry
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="task"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        //public AO.Extended.GroupClass GetTaskShadows(UUIDClass uuid, string task, string name)
        //{
        //    return new AO.Extended.GroupClass(Extended.GroupClass.Types.Task, this, uuid, task, name);
        //}
        #endregion

        #region Workflows
        public List<string> Workflows
        {
            get { return this.SettingsCollection.Names(Definitions.WorkflowClass.Prefix); }
        }

        /// <summary>
        /// 
        /// The tasks
        /// 
        /// </summary>
        private NamedListClass<Definitions.WorkflowClass> WorkflowCache { get; set; } = new NamedListClass<Definitions.WorkflowClass>();

        /// <summary>
        /// 
        /// Returns a task definition
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Definitions.WorkflowClass Workflow(string name)
        {
            // Do we know it?
            if (!this.WorkflowCache.ContainsKey(name))
            {
                // Add
                this.WorkflowCache[name] = new Definitions.WorkflowClass(this, name);
            }

            return this.WorkflowCache[name];
        }

        /// <summary>
        /// 
        /// Get a shadow workflow entry
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="task"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        //public AO.Extended.GroupClass GetWorkflowShadows(UUIDClass uuid, string wf, string name)
        //{
        //    return new AO.Extended.GroupClass(Extended.GroupClass.Types.Workflow, this, uuid, wf, name);
        //}
        #endregion

        #region Pick List
        public List<string> PickLists
        {
            get { return this.SettingsCollection.Names(Definitions.PickListClass.Prefix); }
        }

        /// <summary>
        /// 
        /// The pick lists
        /// 
        /// </summary>
        private NamedListClass<Definitions.PickListClass> PickListCache { get; set; } = new NamedListClass<Definitions.PickListClass>();

        /// <summary>
        /// 
        /// Returns a pick list definition
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Definitions.PickListClass PickList(string name)
        {
            // Holding area
            Definitions.PickListClass c_Ans = null;

            // Do we know it?
            if (!this.PickListCache.ContainsKey(name))
            {
                // Add
                this.PickListCache[name] = new Definitions.PickListClass(this, name);
            }

            c_Ans = this.PickListCache[name];

            return c_Ans;
        }
        #endregion

        #region Method
        /// <summary>
        /// 
        /// Removes a view from cache
        /// 
        /// </summary>
        public void RemoveFromCache(string name)
        {
            // Do we have it?
            if (this.ViewCache.ContainsKey(name))
            {
                // Add
                this.ViewCache.Remove(name);
            }
        }
        #endregion
    }
}
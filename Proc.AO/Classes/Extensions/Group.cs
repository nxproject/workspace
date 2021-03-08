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

using MongoDB.Bson;

using NX.Shared;
using NX.Engine;
using Proc.AO.BuiltIn;

namespace Proc.AO.Extended
{
    public class GroupClass : ChildOfClass<DatabaseClass>
    {
        #region Constants
        private const string KeyID = "_groupid";
        private const string KeyDS = "_groupds";
        private const string KeyType = "_grouptype";
        private const string KeyUUID = "_groupuuid";
        private const string KeyFlow = "_groupflow";
        private const string KeyInstance = "_groupinst";
        private const string KeyOnError = "_grouponerror";
        private const string KeyOnOverdue = "_grouponoverdue";
        #endregion

        #region Constructor
        public GroupClass(DatabaseClass db, Types type, UUIDClass uuid, string taskname, string instancename)
            : this(db, (type + ":" + taskname + ":" + instancename + uuid.ToString()).MD5HashString())
        {
            //
            if (!this.FlowName.HasValue())
            {
                this.Type = type;
                this.FlowName = taskname;
                this.InstanceName = instancename;
                this.UUID = uuid;

                this.Values.Save();
            }
        }

        internal GroupClass(DatabaseClass db, string id)
            : base(db)
        {
            // Make the ID
            this.ID = id;

            // Get
            this.Values = this.Parent[DatabaseClass.DatasetGroup][this.ID];
            this.Values.Volatile();

            //
            switch (this.Type)
            {
                case Types.Task:
                    this.Flow = this.UUID.Dataset.Task(this.FlowName);
                    break;

                case Types.Workflow:
                    this.Flow = this.UUID.Dataset.Workflow(this.FlowName);
                    break;
            }

        }
        #endregion

        #region Enums
        public enum Types
        {
            Task,
            Workflow
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The group ID
        /// 
        /// </summary>
        private string ID { get; set; }
        /// <summary>
        /// 
        /// Internal values
        /// 
        /// </summary>
        private ObjectClass Values { get; set; }

        /// <summary>
        /// 
        /// The type of flow
        /// 
        /// </summary>
        public Types Type
        {
            get { return (Types)Enum.Parse(typeof(Types), this.Values[KeyType], true); }
            private set { this.Values[KeyType] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// The name of the workflow
        /// 
        /// </summary>
        public string FlowName
        {
            get { return this.Values[KeyFlow]; }
            private set { this.Values[KeyFlow] = value; }
        }

        /// <summary>
        /// 
        /// The instance name
        /// 
        /// </summary>
        public string InstanceName
        {
            get { return this.Values[KeyInstance]; }
            private set { this.Values[KeyInstance] = value; }
        }

        /// <summary>
        /// 
        /// The step to call if unhandled error
        /// 
        /// </summary>
        public string OnError
        {
            get { return this.Values[KeyOnError]; }
            set { this.Values[KeyOnError] = value; this.Values.Save(); }
        }

        /// <summary>
        /// 
        /// The step to call if unhandled overdue
        /// 
        /// </summary>
        public string OnOverdue
        {
            get { return this.Values[KeyOnOverdue]; }
            set { this.Values[KeyOnOverdue] = value; this.Values.Save(); }
        }

        /// <summary>
        /// 
        /// The object UUID
        /// 
        /// </summary>
        public UUIDClass UUID
        {
            get { return new UUIDClass(this.Parent, this.Values[KeyUUID]); }
            private set { this.Values[KeyUUID] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// The flow definition
        /// 
        /// </summary>
        public Definitions.ElsaClass Flow { get; set; }

        /// <summary>
        /// 
        /// The object
        /// 
        /// </summary>
        public ObjectClass Object
        {
            get { return this.UUID.Dataset[this.UUID.ID]; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Creates a new entry
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="patt"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public CronEntryClass New(string fn, StoreClass data, DateTime? next, string patt = null, DateTime? start = null, DateTime? end = null)
        {
            using (CronManagerClass c_Mgr = new CronManagerClass(this.Parent))
            {
                CronEntryClass c_Ans = c_Mgr.New(fn, data, next, patt, start, end);
                this.Map(c_Ans);

                return c_Ans;
            }
        }

        /// <summary>
        /// 
        /// Generates a cron entry data block
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public StoreClass ToGroupData(params string[] values)
        {
            StoreClass c_Ans = new StoreClass();

            // Predefined
            c_Ans[KeyID] = this.Values.UUID.ToString();

            // User
            for (int i = 0; i < values.Length; i += 2)
            {
                c_Ans[values[1]] = values[i + 1];
            }

            return c_Ans;
        }

        /// <summary>
        ///  
        /// Maps the group into the entry
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public void Map(CronEntryClass entry)
        {
            //
            entry[AO.ObjectClass.FieldParent] = this.UUID.ToString();

            switch (this.Type)
            {
                case Types.Task:
                    break;

                case Types.Workflow:
                    entry[WorkflowClass.FieldWFType] = this.Type.ToString();
                    entry[WorkflowClass.FieldWFUUIID] = this.UUID.ToString();
                    entry[WorkflowClass.FieldWFFlowName] = this.FlowName;
                    entry[WorkflowClass.FieldWFInstance] = this.InstanceName;
                    break;
            }
        }

        /// <summary>
        /// 
        /// End the group
        /// 
        /// </summary>
        public void End()
        {
            // Find all CRON entries
            using (CronManagerClass c_Cron = new CronManagerClass(this.Parent))
            {
                using (QueryClass c_Qry = c_Cron.OpenQuery())
                {
                    // Add filters
                    c_Qry.Add(WorkflowClass.FieldWFType, QueryElementClass.QueryOps.Eq, this.Type.ToString());
                    c_Qry.Add(WorkflowClass.FieldWFUUIID, QueryElementClass.QueryOps.Eq, this.UUID.ToString());
                    c_Qry.Add(WorkflowClass.FieldWFFlowName, QueryElementClass.QueryOps.Eq, this.FlowName);
                    c_Qry.Add(WorkflowClass.FieldWFInstance, QueryElementClass.QueryOps.Eq, this.InstanceName);

                    // Loop thru
                    foreach(AO.ObjectClass c_Obj in c_Qry.FindObjects())
                    {
                        // See if it an activity
                        string sActID = c_Obj[AO.Extended.WorkflowClass.FieldWFActivityUUIID];
                        // Is it?
                        if(sActID.HasValue())
                        {
                            //
                            using(UUIDClass c_AUUID = new UUIDClass(this.Parent, sActID))
                            {
                                // Get the outcome field
                                string sOutcomeFld = c_Obj[AO.Extended.WorkflowClass.FieldWFOutcome];
                                // Get the outcome value
                                if (!c_Obj[sOutcomeFld].HasValue())
                                {
                                    // Delete if not done
                                    c_Obj.Delete();
                                }
                            }
                        }

                        // Delete
                        c_Qry.Delete();
                    }
                }
            }

            // Find all activities
            if (this.Type == Types.Workflow)
            {
                // TBD
            }

            // Delete
            this.Values.Delete();
        }

        /// <summary>
        /// 
        /// Marks the completion of a step
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void MarkCompletion(string id)
        {
            this.Values[id] = DateTime.Now.ToDBDate();
            this.Values.Save();
        }

        /// <summary>
        /// 
        /// Returns true if the step is done
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsDone(string id)
        {
            return this.Values[id].HasValue();
        }
        #endregion

        #region Statics
        public static GroupClass FromGroupData(DatabaseClass db, StoreClass data)
        {
            //
            GroupClass c_Ans = null;

            //
            string sID = data[KeyID];
            if (sID.HasValue())
            {
                //
                DatasetClass c_DS = db[DatabaseClass.DatasetGroup];
                if (QueryClass.ByID(c_DS.DataCollection, sID).Contains())
                {
                    c_Ans = new GroupClass(db, sID);
                }
            }

            return c_Ans;
        }
        #endregion
    }
}
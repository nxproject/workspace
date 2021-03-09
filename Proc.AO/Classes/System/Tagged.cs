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
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
///

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using MongoDB.Driver;
using MongoDB.Bson;

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public class TaggedClass : ChildOfClass<DatabaseClass>
    {
        #region Constructor
        public TaggedClass(DatabaseClass db)
            : base(db)
        { }
        #endregion

        #region Indexer
        public TagClass this[string user, string type, UUIDClass info]
        {
            get { return new TagClass(this, user, type, info); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Returns all tags
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<TagClass> AllTags(string user= null, string include = null, string exclude = null)
        {
            // Assume none
            List<TagClass> c_Ans = new List<TagClass>();

            // Get the dataset
            DatasetClass c_DS = this.Parent[DatabaseClass.DatasetTagged];
            // Make the query
            QueryClass c_Qry = new QueryClass(c_DS.DataCollection);
            if(user.HasValue()) c_Qry.Add(TagClass.FldUser, QueryElementClass.QueryOps.Eq, user);
            if (include.HasValue()) c_Qry.Add(TagClass.FldType, QueryElementClass.QueryOps.Eq, include);
            if (exclude.HasValue()) c_Qry.Add(TagClass.FldType, QueryElementClass.QueryOps.Ne, exclude);
            // Get
            foreach (ObjectClass c_Doc in c_Qry.FindObjects())
            {
                TagClass c_Tag = new TagClass(this, c_Doc);
                c_Ans.Add(c_Tag);
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Returns only active tags
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TagClass> ActiveTags(string user = null, string include = null, string exclude = null)
        {
            // Assume none
            List<TagClass> c_Ans = this.AllTags(user, include, exclude);

            // 
            for (int i = c_Ans.Count; i > 0; i--)
            {
                if (c_Ans[i - 1].Status == TagClass.Statuses.Ended) c_Ans.RemoveAt(i - 1);
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Deletes all tags
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ds"></param>
        public void DeleteAll(string user = null, string ds=null)
        {
            // Get the dataset
            DatasetClass c_DS = this.Parent[DatabaseClass.DatasetTagged];
            // Make the query
            QueryClass c_Qry = new QueryClass(c_DS.DataCollection);
            if (user.HasValue()) c_Qry.Add(TagClass.FldUser, QueryElementClass.QueryOps.Eq, user);
            if (ds.HasValue()) c_Qry.Add(TagClass.FldObj, QueryElementClass.QueryOps.Like, "::" + ds + ":*");
            // Get
            foreach (ObjectClass c_Doc in c_Qry.FindObjects())
            {
                TagClass c_Tag = new TagClass(this, c_Doc);
                c_Tag.Delete();
            }
        }
        #endregion

        #region Details
        private TagDetailClass IDetails { get; set; }
        public TagDetailClass Details
        {
            get
            {
                if (this.IDetails == null)
                {
                    this.IDetails = new TagDetailClass(this);
                }

                return this.IDetails;
            }
        }
        #endregion
    }

    public class TagClass : ChildOfClass<TaggedClass>
    {
        #region Constants
        internal const string FldUser = "user";
        internal const string FldType = "type";
        internal const string FldObj = "obj";
        internal const string FldTS = "ts";
        internal const string FldStatus = "status";
        internal const string FldSegments = "segs";

        internal const string FldFrom = "from";
        internal const string FldTo = "to";
        #endregion

        #region Constructor
        public TagClass(TaggedClass tags, string user, string type, UUIDClass obj)
            : this(tags, (user.IfEmpty() + ":" + type.IfEmpty() + ":" + obj.ToString()).MD5HashString())
        {
            //
            this.User = user;
            this.Type = type;
            this.ObjUUID = obj;
        }

        public TagClass(TaggedClass tags, string id)
            : base(tags)
        {
            // Save
            this.ID = id;

            // Read
            this.Object = this.Parent.Parent[DatabaseClass.DatasetTagged][this.ID];

            // Set the start timestamp
            if (this.IsNew)
            {
                this.Object[FldTS] = DateTime.Now.ToDBDate();
                this.Object[FldFrom] = this.Object[FldTS];
            }
        }

        public TagClass(TaggedClass tags, ObjectClass obj)
            : base(tags)
        {
            this.Object = obj;
        }
        #endregion

        #region Enum 
        public enum Statuses
        {
            Active,
            Frozen,
            Ended
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// UUID
        /// 
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 
        /// Reference object
        /// 
        /// </summary>
        internal ObjectClass Object { get; set; }

        /// <summary>
        /// 
        /// User that owns the tag
        /// 
        /// </summary>
        public string User
        {
            get { return this.Object[FldUser]; }
            set { this.Object[FldUser] = value.IfEmpty(); }
        }

        /// <summary>
        /// 
        /// Tag type
        /// 
        /// </summary>
        public string Type
        {
            get { return this.Object[FldType]; }
            set { this.Object[FldType] = value.IfEmpty(); }
        }

        /// <summary>
        /// 
        /// UUID of reference object
        /// 
        /// </summary>)
        public UUIDClass ObjUUID
        {
            get { return new UUIDClass(this.Object.Parent.Parent, this.Object[FldObj]); }
            private set { this.Object[FldObj] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// The timestamp
        /// 
        /// </summary>
        public DateTime TS
        {
            get { return this.Object[FldTS].FromDBDate(); }
        }

        /// <summary>
        /// 
        /// Age of tag
        /// 
        /// </summary>
        public double Age { get { return DateTime.Now.Subtract(this.TS).TotalSeconds; } }

        /// <summary>
        /// 
        /// Is the tag new?
        /// 
        /// </summary>
        public bool IsNew { get { return this.Object.IsNew; } }

        /// <summary>
        /// 
        /// Rqetunrs the object used by the web broswer
        /// 
        /// </summary>
        public JObject AsJObject
        {
            get
            {
                JObject c_Ans = this.Object.AsJObject;

                // Update reference
                using (ObjectClass c_Ref = this.ObjUUID.AsObject)
                {
                    c_Ans["desc"] = c_Ref.ObjectDescription;
                    c_Ans["icon"] = c_Ref.Dataset.Definition.Icon;
                    c_Ans["ds"] = c_Ref.Dataset.Name;
                    c_Ans["id"] = c_Ref.ID;
                }

                // Compute totla segments size
                double dSize = 0;
                foreach (TagSegmentClass c_Seg in this.Segments)
                {
                    dSize += c_Seg.Size;
                }
                c_Ans["size"] = dSize.ToString();

                return c_Ans;
            }
        }

        /// <summary>
        /// 
        /// Status
        /// 
        /// </summary>
        public Statuses Status
        {
            get
            {
                Statuses eAns = Statuses.Active;

                try
                {
                    eAns = (Statuses)Enum.Parse(typeof(Statuses), this.Object[FldStatus], true);
                }
                catch { }

                return eAns;
            }
            private set { this.Object[FldStatus] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// Are we stopped?
        /// 
        /// </summary>
        public bool IsEnded { get { return this.Status == Statuses.Ended; } }

        public string Explain
        {
            get
            {
                StringBuilder c_Buffer = new StringBuilder();

                double dSecs = 0;

                c_Buffer.AppendLine("Status: {0}".FormatString(this.Status));

                if (this.Status == Statuses.Active)
                {
                    c_Buffer.AppendLine("Started On: {0}".FormatString(this.TS));
                    dSecs += this.Age;
                }

                string sLabel = "Duration";

                List<TagSegmentClass> c_Segs = this.Segments;
                if (c_Segs.Count > 0)
                {
                    c_Buffer.AppendLine("Segments");
                    sLabel = "Total duration";

                    foreach (TagSegmentClass c_Seg in c_Segs)
                    {
                        c_Buffer.Append("   {0}".FormatString(c_Seg.StartedOn.FromDBDate()));
                        c_Buffer.Append("-{0}".FormatString(c_Seg.EndedOn.FromDBDate()));
                        c_Buffer.AppendLine(" for {0}".FormatString(c_Seg.Size.ToDuration()));

                        dSecs += c_Seg.Size;
                    }
                }

                c_Buffer.AppendLine(" {1}: {0}".FormatString(dSecs.ToDuration(), sLabel));

                return c_Buffer.ToString();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Saves the tag
        /// 
        /// </summary>
        public void Save()
        {
            // Put back
            this.Object.Save();
        }

        /// <summary>
        /// 
        /// Deletes the tag
        /// 
        /// </summary>
        public void Delete()
        {
            //
            this.Object.Delete();
        }

        /// <summary>
        /// 
        /// Starts
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        public string Start(bool reset = false, bool frozen = false)
        {
            string sAns = "";

            // Are we frozen?
            if (this.IsFrozen)
            {
                // 
                if (!frozen)
                {
                    sAns = this.Unfreeze();
                }
                else
                {
                    sAns = "alreaddy frozen";
                }
            }
            else
            {
                this.Status = frozen ? Statuses.Frozen : Statuses.Active;
                if (reset) this.SegmentsAsJArray = new JArray();

                sAns = "started" + (frozen ? " frozen" : "");
            }
            // Save
            this.Save();

            return sAns;
        }

        /// <summary>
        /// 
        /// Ends the tracking
        /// 
        /// </summary>
        public string End()
        {
            if (this.Status == Statuses.Active)
            {
                this.Freeze("Ended");
            }

            this.Status = Statuses.Ended;
            this.Object[FldTo] = DateTime.Now.ToDBDate();

            this.Save();

            return "ended";
        }
        #endregion

        #region Multiple steps
        /// <summary>
        /// 
        /// Are we frozen?
        /// 
        /// </summary>
        public bool IsFrozen { get { return this.Status == Statuses.Frozen; } }

        /// <summary>
        /// 
        /// Freezes the time tracking
        /// 
        /// </summary>
        /// <param name="reason"></param>
        public string Freeze(string reason)
        {
            string sAns = "Already frozen";

            // Are we frozen?
            if (!this.IsFrozen)
            {
                this.Status = Statuses.Frozen;

                DateTime c_Now = DateTime.Now;

                TagSegmentClass c_Seg = new TagSegmentClass(this, new JObject());
                c_Seg.StartedOn = this.TS.ToDBDate();
                c_Seg.EndedOn = c_Now.ToDBDate();
                c_Seg.Reason = reason;
                c_Seg.By = this.User;
                c_Seg.Size = c_Now.Subtract(this.TS).TotalSeconds;

                JArray c_Segs = this.SegmentsAsJArray;
                c_Segs.Add(c_Seg.Values);
                this.SegmentsAsJArray = c_Segs;

                this.Object[FldTS] = DateTime.Now.ToDBDate();

                this.Save();

                this.Parent.Details.AddSegment(c_Seg);

                sAns = "frozen";
            }

            return sAns;
        }

        /// <summary>
        /// 
        /// Continues the timetracking
        /// 
        /// </summary>
        public string Unfreeze()
        {
            string sAns = "not frozen";

            //
            if (this.IsFrozen)
            {
                this.Object[FldTS] = DateTime.Now.ToDBDate();
                this.Status = Statuses.Active;
                this.Save();

                sAns = "continued";
            }

            return sAns;
        }

        /// <summary>
        /// 
        /// The browser displayble
        /// 
        /// </summary>
        public JArray SegmentsAsJArray
        {
            get { return this.Object[FldSegments].ToJArray(); }
            set { this.Object[FldSegments] = value.ToSimpleString(); }
        }

        /// <summary>
        /// 
        /// Gets the list of segments
        /// 
        /// </summary>
        public List<TagSegmentClass> Segments
        {
            get
            {
                List<TagSegmentClass> c_Ans = new List<TagSegmentClass>();

                // Get the array
                JArray c_List = this.Object[FldSegments].ToJArray();

                // Loop thru
                for (int i = 0; i < c_List.Count; i++)
                {
                    c_Ans.Add(new TagSegmentClass(this, c_List.GetJObject(i)));
                }

                return c_Ans;
            }
        }
        #endregion
    }

    public class TagSegmentClass : ChildOfClass<TagClass>
    {
        #region Constants
        private const string FldStart = "start";
        private const string FldEnd = "end";
        private const string FldSize = "size";
        private const string FldReason = "reason";
        private const string FldBy = "by";
        #endregion

        #region Constructor
        internal TagSegmentClass(TagClass tag, JObject values)
            : base(tag)
        {
            //
            this.Values = values;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Holding area
        /// 
        /// </summary>
        internal JObject Values { get; set; }

        /// <summary>
        /// 
        /// The date that the segment started
        /// 
        /// </summary>
        public string StartedOn
        {
            get { return this.Values.Get(FldStart); }
            set { this.Values.Set(FldStart, value); }
        }

        /// <summary>
        /// 
        /// The date that the segmen ended
        /// 
        /// </summary>
        public string EndedOn
        {
            get { return this.Values.Get(FldEnd); }
            set { this.Values.Set(FldEnd, value); }
        }

        /// <summary>
        /// 
        /// Number of seconds
        /// 
        /// </summary>
        public double Size
        {
            get { return this.Values.Get(FldSize).ToFloat(0); }
            set { this.Values.Set(FldSize, value.ToString()); }
        }

        /// <summary>
        /// 
        /// The reason of the segment
        /// 
        /// </summary>
        public string Reason
        {
            get { return this.Values.Get(FldReason); }
            set { this.Values.Set(FldReason, value); }
        }

        /// <summary>
        /// 
        /// The user of the segment
        /// 
        /// </summary>
        public string By
        {
            get { return this.Values.Get(FldBy); }
            set { this.Values.Set(FldBy, value); }
        }
        #endregion
    }

    public class TagDetailClass : ChildOfClass<TaggedClass>
    {
        #region Constants
        internal const string FldDS = "ods";
        internal const string FldDesc = "odesc";
        internal const string FldPDS = "pds";
        internal const string FldPDesc = "pdesc";
        internal const string FldStart = "start";
        internal const string FldEnd = "end";
        internal const string FldReason = "reason";
        internal const string FldBy = "by";
        internal const string FldSize = "size";
        #endregion

        #region Constructor
        public TagDetailClass(TaggedClass tag)
            : base(tag)
        { }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Adds a segment to the detail dataset
        /// 
        /// </summary>
        /// <param name="seg"></param>
        public void AddSegment(TagSegmentClass seg)
        {
            //
            TagClass c_Tag = seg.Parent;
            DatabaseClass c_DB = this.Parent.Parent;

            //
            ObjectClass c_Obj = c_DB[DatabaseClass.DatasetTaggedDetail].New();

            // Fill
            c_Obj[FldDS] = c_Tag.Object.Dataset.Definition.Caption;
            c_Obj[FldDesc] = c_Tag.Object.ObjectDescription;

            c_Obj[FldStart] = seg.StartedOn;
            c_Obj[FldEnd] = seg.EndedOn;
            c_Obj[FldReason] = seg.Reason;
            c_Obj[FldBy] = seg.By;
            c_Obj[FldSize] = seg.Size.ToString();

            string sParent = c_Tag.Object[ObjectClass.FieldParent];
            if (sParent.HasValue())
            {
                UUIDClass c_PUUID = new UUIDClass(c_DB, sParent);
                ObjectClass c_Parent = c_PUUID.AsObject;

                c_Obj[FldPDS] = c_Parent.Dataset.Definition.Caption;
                c_Obj[FldPDesc] = c_Parent.ObjectDescription;
            }

            c_Obj.Save();
        }
        #endregion
    }
}
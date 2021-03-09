//--------------------------------------------------------------------------------
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

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;
using MongoDB.Bson;

using NX.Shared;
using Proc.AO.BuiltIn;

namespace Proc.AO.Definitions
{
    public class DatasetClass : ExtraClass
    {
        #region Constructor
        internal DatasetClass(Proc.AO.DatasetClass ds)
            : base(ds, "def")
        {
            // Assure
            this.Fields = this.Object.GetAsBObject("fields");
        }
        #endregion

        #region Indexer
        public DatasetFieldClass this[string name]
        {
            get
            {
                // New?
                if (!this.Cache.ContainsKey(name))
                {
                    // Create
                    this.Cache[name] = new DatasetFieldClass(this, name);
                }

                return this.Cache[name];
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The field definitions
        /// 
        /// </summary>
        public BsonDocument Fields { get; private set; }

        public List<string> FieldNames { get { return new List<string>(this.Fields.Names); } }

        /// <summary>
        /// 
        /// The cache
        /// 
        /// </summary>
        private Dictionary<string, DatasetFieldClass> Cache { get; set; } = new Dictionary<string, DatasetFieldClass>();

        /// <summary>
        /// 
        /// Release of defnition
        /// 
        /// </summary>
        public string Release
        {
            get { return this.Object["release"]; }
            private set { this.Object["release"] = value; }
        }

        /// <summary>
        /// 
        /// Returns the next release, if valid
        /// 
        /// </summary>
        public string NextRelease
        {
            get
            {
                // Use current as default
                string sAns = this.Release.IfEmpty();
                // Proper format?
                if (Regex.Match(sAns, @"^\d{4}\x2E\d{2}\x2E\d{2}\x2E\d+$").Success)
                {
                    // Find end piece
                    int iPos = sAns.LastIndexOf(".") + 1;
                    // And increment
                    sAns = sAns.Substring(0, iPos) + (1 + sAns.Substring(iPos).ToInteger(0)).ToString();
                }

                return sAns;
            }
        }

        /// <summary>
        /// 
        /// The alias of the _id field
        /// 
        /// </summary>
        public string IDAlias
        {
            get { return this.Object["idalias"]; }
            set { this.Object["idalias"] = value; }
        }

        /// <summary>
        /// 
        /// The displayable description
        /// 
        /// </summary>
        public string Placeholder
        {
            get { return this.Object["placeholder"]; }
            set { this.Object["placeholder"] = value; }
        }

        /// <summary>
        /// 
        /// Can the dataset be displayed in start menu?
        /// 
        /// </summary>
        public bool Hidden
        {
            get { return this.Object["hidden"].FromDBBoolean(); }
            set { this.Object["hidden"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The order displayed
        /// 
        /// </summary>
        public string SortOrder
        {
            get { return this.Object["sortOrder"]; }
            set { this.Object["sortOrder"] = value; }
        }

        /// <summary>
        /// 
        /// The caption for the dataset
        /// 
        /// </summary>
        public string Caption
        {
            get { return this.Object["caption"]; }
            set { this.Object["caption"] = value; }
        }

        /// <summary>
        /// 
        /// The icon for the dataset
        /// 
        /// </summary>
        public string Icon
        {
            get { return this.Object["icon"]; }
            set { this.Object["icon"] = value; }
        }

        /// <summary>
        /// 
        /// The default privileges for the dataset
        /// 
        /// </summary>
        public string Privileges
        {
            get { return this.Object["privileges"]; }
            set { this.Object["privileges"] = value; }
        }

        /// <summary>
        /// 
        /// Can the dataset be displayed only once?
        /// 
        /// </summary>
        public bool IsUnique
        {
            get { return this.Object["isUnique"].FromDBBoolean(); }
            set { this.Object["isUnique"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// Is the dataset a workflow activity?
        /// 
        /// </summary>
        public bool IsWorkflow
        {
            get { return this.Object["isWorkfow"].FromDBBoolean(); }
            set { this.Object["isWorkfow"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The task to run at save
        /// 
        /// </summary>
        public string TaskAtSave
        {
            get { return this.Object["taskatsave"]; }
            set { this.Object["taskatsave"] = value; }
        }

        /// <summary>
        /// 
        /// The selector needed
        /// 
        /// </summary>
        public string Selector
        {
            get { return this.Object["selector"]; }
            set { this.Object["selector"] = value; }
        }

        #region Private
        /// <summary>
        /// 
        /// Is the privacy allowed?
        /// 
        /// </summary>
        public bool PrivacyAllow
        {
            get { return this.Object["privAllow"].FromDBBoolean(); }
            set { this.Object["privAllow"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// Is the dataset tied to the user?
        /// 
        /// </summary>
        public string PrivateField
        {
            get { return this.Object["privField"]; }
            set { this.Object["privField"] = value; }
        }
        #endregion

        #region Organizer
        /// <summary>
        /// 
        /// Is the organizer allowed?
        /// 
        /// </summary>
        public bool OrganizerAllow
        {
            get { return this.Object["orgAllow"].FromDBBoolean(); }
            set { this.Object["orgAllow"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The fields to sort
        /// 
        /// </summary>
        public string OrganizeBy
        {
            get { return this.Object["orgby"]; }
            set { this.Object["orgby"] = value; }
        }

        /// <summary>
        /// 
        /// Only the by fields
        /// 
        /// </summary>
        public bool OrganizeByOnly
        {
            get { return this.Object["orgbyo"].FromDBBoolean(); }
            set { this.Object["orgbyo"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// Exclude
        /// 
        /// </summary>
        public bool ExcludeFromOrganizer
        {
            get { return this.Object["orgexc"].FromDBBoolean(); }
            set { this.Object["orgexc"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// Alphabetic
        /// 
        /// </summary>
        public bool OrganizeByAlpha
        {
            get { return this.Object["orgalpha"].FromDBBoolean(); }
            set { this.Object["orgalpha"] = value.ToDBBoolean(); }
        }
        #endregion

        #region Calendar
        /// <summary>
        /// 
        /// Is the calendar allowed?
        /// 
        /// </summary>
        public bool CalendarAllow
        {
            get { return this.Object["calAllow"].FromDBBoolean(); }
            set { this.Object["calAllow"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The start date/time
        /// 
        /// </summary>
        public string CalendarStart
        {
            get { return this.Object["calstart"]; }
            set { this.Object["calstart"] = value; }
        }

        /// <summary>
        /// 
        /// The end date/time
        /// 
        /// </summary>
        public string CalendarEnd
        {
            get { return this.Object["calend"]; }
            set { this.Object["calend"] = value; }
        }

        /// <summary>
        /// 
        /// The displayable description
        /// 
        /// </summary>
        public string CalendarSubject
        {
            get { return this.Object["calsubj"]; }
            set { this.Object["calsubj"] = value; }
        }

        /// <summary>
        /// 
        /// The by fields
        /// 
        /// </summary>
        public string CalendarBy
        {
            get { return this.Object["calby"]; }
            set { this.Object["calby"] = value; }
        }
        #endregion

        #region Workflows
        /// <summary>
        /// 
        /// Is the workflow allowed?
        /// 
        /// </summary>
        public bool WorkflowAllow
        {
            get { return this.Object["wfAllow"].FromDBBoolean(); }
            set { this.Object["wfAllow"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The dataset that holds the activities
        /// 
        /// </summary>
        public string WorkflowDataset
        {
            get { return this.Object["wfDS"]; }
            set { this.Object["wfDS"] = value; }
        }

        /// <summary>
        /// 
        /// The description field
        /// 
        /// </summary>
        public string WorkflowDescription
        {
            get { return this.Object["wfDescription"]; }
            set { this.Object["wfDescription"] = value; }
        }

        /// <summary>
        /// 
        /// The date field
        /// 
        /// </summary>
        public string WorkflowStartedOn
        {
            get { return this.Object["wfStartedOn"]; }
            set { this.Object["wfStartedOn"] = value; }
        }

        /// <summary>
        /// 
        /// The date the activity is expected to end field
        /// 
        /// </summary>
        public string WorkflowExpectedOn
        {
            get { return this.Object["wfExpectedOn"]; }
            set { this.Object["wfExpectedOn"] = value; }
        }

        /// <summary>
        /// 
        /// The date the activity ended field
        /// 
        /// </summary>
        public string WorkflowEndedOn
        {
            get { return this.Object["wfEndedOn"]; }
            set { this.Object["wfEndedOn"] = value; }
        }

        /// <summary>
        /// 
        /// The message field
        /// 
        /// </summary>
        public string WorkflowMessage
        {
            get { return this.Object["wfMessage"]; }
            set { this.Object["wfMessage"] = value; }
        }

        /// <summary>
        /// 
        /// The group assigment field
        /// 
        /// </summary>
        public string WorkflowAssignedTo
        {
            get { return this.Object["wfAssignedTo"]; }
            set { this.Object["wfAssignedTo"] = value; }
        }

        /// <summary>
        /// 
        /// The user done by field
        /// 
        /// </summary>
        public string WorkflowDoneBy
        {
            get { return this.Object["wfDoneBy"]; }
            set { this.Object["wfDoneBy"] = value; }
        }

        /// <summary>
        /// 
        /// The date the outcome field
        /// 
        /// </summary>
        public string WorkflowOutcome
        {
            get { return this.Object["wfOutcome"]; }
            set { this.Object["wfOutcome"] = value; }
        }
        #endregion

        #region Analyzer
        /// <summary>
        /// 
        /// Is the analyzer allowed?
        /// 
        /// </summary>
        public bool AnalyzerAllow
        {
            get { return this.Object["anaAllow"].FromDBBoolean(); }
            set { this.Object["anaAllow"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The by fields
        /// 
        /// </summary>
        public string AnalyzerBy
        {
            get { return this.Object["anaby"]; }
            set { this.Object["anaby"] = value; }
        }

        /// <summary>
        /// 
        /// The by fields
        /// 
        /// </summary>
        public string AnalyzerPick
        {
            get { return this.Object["anapick"]; }
            set { this.Object["anapick"] = value; }
        }
        #endregion

        #region Time track
        /// <summary>
        /// 
        /// Is time tracking allowed?
        /// 
        /// </summary>
        public bool TimetrackAllow
        {
            get { return this.Object["ttAllow"].FromDBBoolean(); }
            set { this.Object["ttAllow"] = value.ToDBBoolean(); }
        }
        #endregion

        #region Chat
        /// <summary>
        /// 
        /// Is tchat allowed?
        /// 
        /// </summary>
        public bool ChatAllow
        {
            get { return this.Object["chatAllow"].FromDBBoolean(); }
            set { this.Object["chatAllow"] = value.ToDBBoolean(); }
        }
        #endregion

        ///// <summary>
        ///// 
        ///// keep pick list open?
        ///// 
        ///// </summary>
        //public bool PickKeepOpen
        //{
        //    get { return this.Object["keepOpen").FromDBBoolean(); }
        //    set { this.Object["keepOpen", value.ToDBBoolean()); }
        //}

        /// <summary>
        /// 
        /// Command to use as default in window
        /// 
        /// </summary>
        public string DefaultCommand
        {
            get { return this.Object["defaultCommand"]; }
            set { this.Object["defaultCommand"] = value; }
        }

        /// <summary>
        /// 
        /// The Start group for the dataset
        /// 
        /// </summary>
        public string StartGroup
        {
            get { return this.Object["startgroup"]; }
            set { this.Object["startgroup"] = value; }
        }

        /// <summary>
        /// 
        /// The Start priority for the dataset
        /// 
        /// </summary>
        public string StartPriority
        {
            get { return this.Object["startpriority"]; }
            set { this.Object["startpriority"] = value; }
        }

        /// <summary>
        /// 
        /// The Start index for the dataset
        /// 
        /// </summary>
        public string StartIndex
        {
            get { return this.Object["startindex"]; }
            set { this.Object["startindex"] = value; }
        }

        /// <summary>
        /// 
        /// The SIO events when an object is saved
        /// 
        /// </summary>
        public string SIOEventsAtSave
        {
            get { return this.Object["sioeventsatsave"]; }
            set { this.Object["sioeventsatsave"] = value; }
        }

        /// <summary>
        /// 
        /// 
        /// Is the dataset hidden in root menu?
        /// 
        /// </summary>
        public bool IsHidden { get { return this.StartIndex.IsSameValue("hidden"); } }

        /// <summary>
        /// 
        /// The child DS map
        /// 
        /// </summary>
        public string ChildDSs
        {
            get { return this.Object["childdss"]; }
            set { this.Object["childdss"] = value; }
        }

        /// <summary>
        /// 
        /// Are the key fields set?
        /// 
        /// </summary>
        public bool IsValid { get { return this.Placeholder.HasValue() && this.Privileges.HasValue() && this.Caption.HasValue(); } }

        /// <summary>
        /// 
        /// The document folder
        /// 
        /// </summary>
        private NX.Engine.Files.FolderClass IFolder { get; set; }
        public NX.Engine.Files.FolderClass Folder
        {
            get
            {
                if (this.IFolder == null)
                {
                    this.IFolder = new NX.Engine.Files.FolderClass(this.Parent.Parent.FileManager, "/ao/" + this.Parent.Name.AsDatasetName());
                }

                return this.IFolder;
            }
        }

        /// <summary>
        /// 
        /// The shared document foldeer
        /// 
        /// </summary>
        public NX.Engine.Files.FolderClass SharedDocuments
        {
            get
            {
                return this.Folder.SubFolder("_ds");
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Saves definition
        /// 
        /// </summary>
        public void Save()
        {
            //
            this.Object.SetAsBObject("fields", this.Fields);

            // Create known indexes
            this.Parent.DataCollection.CreateIndex(ObjectClass.FieldSearch);
            this.Parent.DataCollection.CreateIndex(ObjectClass.FieldParent);

            // Loop thru
            foreach (string sField in this.Fields.ToJObject().Keys())
            {
                // Get
                Definitions.DatasetFieldClass c_Field = this[sField];

                // Indexing
                switch (c_Field.Type)
                {
                    case DatasetFieldClass.FieldTypes.Link:
                        this.Parent.DataCollection.CreateIndex(sField);
                        break;
                }

                // Assure grid views
                string sGV = c_Field.GridView;
                if (sGV.HasValue())
                {
                    // Get list of current
                    List<string> c_Current = this.Parent.Views;

                    // Into array
                    List<string> c_Views = sGV.SplitSpaces();
                    // Loop thu
                    foreach (string sView in c_Views)
                    {
                        // Exists?
                        if (!c_Current.Contains(sView))
                        {
                            // Map
                            Definitions.ViewClass c_View = this.Parent.View(sView);
                            // Save
                            c_View.Save();
                        }
                    }
                }

            }

            // Save
            this.Object.Save(force: true);

            this.Parent.Parent.RemoveFromCache(this.Parent.Name);

            // Signal
            this.Parent.Parent.Parent.SignalChange(this.Parent);

            // Reload
            var x = this.Parent[this.Name];
        }

        /// <summary>
        /// 
        /// Checks to see if the release changed
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <returns></returns>
        public bool ReleaseChanged(string release)
        {
            // Is it different?
            bool bAns = !release.IsSameValue(this.Release);
            // Is it private?
            if (bAns) bAns = !this.Release.IfEmpty().Contains("private");
            // If changed, sabve new
            if (bAns) this.Release = release;

            return bAns;
        }

        /// <summary>
        /// 
        /// Deletes a dataset
        /// 
        /// </summary>
        public void Delete()
        {

            //
            this.Parent.DataCollection.Delete();
            this.Parent.SettingsCollection.Delete();

            //
            this.Parent.Parent.Parent.SignalChange(this.Parent, true);

            this.Parent.Parent.RemoveFromCache(this.Parent.Name);

            //
            this.Folder.Delete();

            // If system, recreate
            if (this.Parent.Name.StartsWith("_"))
            {
                // Make
                AO.DatasetClass c_DS = this.Parent.Parent[this.Parent.Name];
                // Save
                c_DS.Definition.Save();
                // All the views
                foreach (string sView in c_DS.Views)
                {
                    c_DS.View(sView).Save();
                }
            }
            else
            {
                // Delete tags
                this.Parent.Parent.Tagged.DeleteAll(null, this.Name);
            }
        }

        /// <summary>
        /// 
        /// Clear all the fields
        /// 
        /// </summary>
        public void ClearFields()
        {
            // 
            this.Fields.Clear();
        }

        /// <summary>
        /// 
        /// Clear a field
        /// 
        /// </summary>
        public void RemoveField(string name)
        {
            // 
            this.Fields.Remove(name);
        }

        /// <summary>
        /// 
        /// Load from JSON object 
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadFrom(JObject data)
        {
            //
            this.Object.LoadFrom(data);
            this.Fields = this.Object.GetAsBObject("fields");
        }
        #endregion
    }
}
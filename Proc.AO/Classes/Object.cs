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
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public class ObjectClass : ChildOfClass<DatasetClass>
    {
        #region Constants
        public const string FieldID = "_id";
        public const string FieldDataset = "_ds";
        public const string FieldVersion = "_ver";
        public const string FieldCreatedOn = "_cre";
        public const string FieldChanges = "_changes";
        public const string FieldDescription = "_desc";
        public const string FieldSearch = "_search";
        public const string FieldUUID = "_uuid";
        public const string FieldCalendarStart = "_calstart";
        public const string FieldCalendarEnd = "_calend";
        public const string FieldCalendarSubject = "_calsubj";
        public const string FieldParent = "_parent";
        public const string FieldGeo = "_geo";
        public const string FieldComputed = "_computed";

        public const string FieldToken = "_token";
        public const string FieldSIO = "_sio";
        public const string FieldAccount = "_account";

        private const string SIOSaved = "$$object.saved";
        private const string SIOData = "$$object.data";
        #endregion

        #region Constructor
        internal ObjectClass(DatasetClass ds, string id)
            : base(ds)
        {
            // Missing ID?
            if (!id.HasValue())
            {
                // Get one
                id = ObjectId.GenerateNewId().ToString();
            }

            // Save
            this.ID = id;

            // Get
            this.Load();
        }

        internal ObjectClass(DatasetClass ds, BsonDocument doc)
            : base(ds)
        {
            // Save
            this.Document = doc;
            this.ID = doc.GetField("_id");

            // Handle new
            this.SetDefaultValues();

            // Flag
            this.IsValid = true;
        }

        internal ObjectClass(DatasetClass ds, ExtendedContextClass ctx)
             : base(ds)
        {
            //
            this.ID = ObjectId.GenerateNewId().ToString();

            // Get
            this.Load(ctx);
        }
        #endregion

        #region Indexer
        public string this[string field]
        {
            get
            {
                // Get raw
                string sAns = this.Document.GetField(field);

                // Are we a data object?
                if (this.IsData)
                {
                    // Assure correct format
                    Definitions.DatasetFieldClass c_Field = this.Dataset.Definition[field];
                    // Ad-hoc?
                    if (c_Field != null)
                    {
                        // Adjust
                        sAns = c_Field.StandarizeDBValue(sAns);
                    }
                }

                return sAns;
            }
            set
            {
                // Mark
                this.MarkChanged(field);

                // Are we a data object?
                if (this.IsData)
                {
                    // Assure correct format
                    Definitions.DatasetFieldClass c_Field = this.Dataset.Definition[field];
                    // Ad-hoc?
                    if (c_Field != null)
                    {
                        // Adjust
                        value = c_Field.StandarizeDBValue(value);
                    }
                }

                // Save
                this.Document.SetField(field, value);

                // Handle volatile
                if (this.IsVolatile)
                {
                    // Open the manager
                    SIO.ManagerClass c_Mgr = this.Parent.Parent.Parent.Parent.Globals.Get<SIO.ManagerClass>();

                    // Make a message
                    using (SIO.MessageClass c_Msg = new SIO.MessageClass(c_Mgr,
                                                                            SIO.MessageClass.Modes.Both,
                                                                            "$$object.data",
                                                                            "winid", "ao_{0}_{1}".FormatString(this.Dataset.Name, this.ID),
                                                                            "aoFld", field,
                                                                            "value", value))
                    {
                        // Send
                        c_Msg.Send();
                    }
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The object ID
        /// 
        /// </summary>
        public string ID { get; private set; }

        public bool IsData { get { return !this.ID.StartsWith("#"); } }

        /// <summary>
        /// 
        /// From the object itself
        /// 
        /// </summary>
        public string ObjectID { get { return this[FieldID]; } }
        public string ObjectDataset { get { return this[FieldDataset].IfEmpty(this.Parent.Name); } }
        public string ObjectVersion { get { return this[FieldVersion]; } }
        public string ObjectDescription { get { return this[FieldDescription]; } set { this[FieldDescription] = value; } }
        public string ObjectSearch { get { return this[FieldSearch]; } }

        /// <summary>
        /// 
        /// The collection where the object resides
        /// 
        /// </summary>
        public CollectionClass Collection
        {
            get
            {
                CollectionClass c_Ans = null;

                if (this.IsData)
                {
                    c_Ans = this.Dataset.DataCollection;
                }
                else
                {
                    c_Ans = this.Dataset.SettingsCollection;
                }

                return c_Ans;
            }
        }

        /// <summary>
        /// 
        /// The dataset where the object resides
        /// 
        /// </summary>
        public DatasetClass Dataset { get { return this.Parent; } }

        /// <summary>
        /// 
        /// The underlying document
        /// 
        /// </summary>
        private BsonDocument Document { get; set; }

        /// <summary>
        /// 
        /// Is the object valid?
        /// 
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// 
        /// Is the object new?
        /// 
        /// </summary>
        public bool IsNew { get; private set; }

        /// <summary>
        /// 
        /// Returns the document as a JSON object
        /// 
        /// </summary>
        public JObject AsJObject
        {
            get { return this.Document.ToJObject(); }
        }

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
                    this.IFolder = new NX.Engine.Files.FolderClass(this.Parent.Parent.FileManager, "/ao/" + this.Parent.Name.AsDatasetName() + "/" + this.ID);
                }

                return this.IFolder;
            }
        }

        /// <summary>
        /// 
        /// Returns the field names
        /// 
        /// </summary>
        public List<string> Fields
        {
            get { return this.Document.Keys(); }
        }

        /// <summary>
        /// 
        /// The unique identifier for the object
        /// 
        /// </summary>
        public UUIDClass UUID
        {
            get { return new UUIDClass(this.Parent.Parent, this.ObjectDataset, this.ID, ""); }
        }

        private WalletClass IWallet { get; set; }
        public WalletClass Wallet
        {
            get
            {
                if (this.IWallet == null)
                {
                    this.IWallet = new WalletClass(this);
                }
                return this.IWallet;
            }
        }

        /// <summary>
        /// 
        /// List of fileds that have changed
        /// 
        /// </summary>
        public List<string> Changes { get { return this[FieldChanges].IfEmpty().ToJArrayOptional().ToList(); } }

        /// <summary>
        /// 
        /// Callback when changed
        /// 
        /// </summary>
        private Action<string, string> OnChanged { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Makes a displayable representation
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Document.ToString();
        }

        /// <summary>
        /// 
        /// Sets the default values
        /// 
        /// </summary>
        private void SetDefaultValues(ExtendedContextClass ctx = null)
        {
            // New?
            if (this.IsNew && !this.ID.StartsWith("#") && !this.Dataset.Name.StartsWith("_"))
            {
                // Setup context
                if (ctx == null)
                {
                    ctx = new ExtendedContextClass(this.Parent.Parent.Parent.Parent, null, this, "");
                }
                else
                {
                    ctx = new ExtendedContextClass(ctx.Parent, ctx.Stores[Names.Passed], this, ctx.User.Name);
                }
                // Loop thru
                foreach (string sField in this.Dataset.Definition.FieldNames)
                {
                    // Get the field
                    Definitions.DatasetFieldClass c_Field = this.Dataset.Definition[sField];
                    // Does it have a default value?
                    if (c_Field.DefaultValue.HasValue())
                    {
                        using (DatumClass c_Datum = new DatumClass(ctx, c_Field.DefaultValue))
                        {
                            // According to type
                            switch (c_Datum.Type)
                            {
                                case DatumClass.Types.Data:
                                case DatumClass.Types.Expression:
                                case DatumClass.Types.Value:
                                    this[sField] = c_Datum.Value;
                                    break;
                                default:
                                    this[sField] = c_Field.DefaultValue;
                                    break;
                            }
                        }
                    }
                }
            }

            this.SetSysValues();
        }

        private void SetSysValues()
        {
            // Set system values
            if (!this.Document.Contains(FieldID)) this.Document[FieldID] = this.ID;
            if (!this.Document.Contains(FieldDataset)) this.Document[FieldDataset] = this.Parent.Name;
            if (!this.Document.Contains(FieldCreatedOn)) this.Document[FieldCreatedOn] = ObjectId.GenerateNewId().ToString();

            // Set dynamic
            this.Document.Set("_documents", "/f/ao/{0}/{1}".FormatString(this.Dataset.Name, this.ID));
        }

        /// <summary>
        /// 
        /// Loads the object from database
        /// 
        /// </summary>
        public void Load(ExtendedContextClass ctx = null)
        {
            // Make the filter
            using (QueryClass c_Filter = new QueryClass(this.Collection))
            {
                // By ID
                c_Filter.AddByID(this.ID);

                // Get the list
                var c_List = c_Filter.Find(1);
                // Any?
                if (c_List != null && c_List.Count > 0)
                {
                    // Pick first
                    this.Document = c_List[0];

                    // Flag
                    this.IsValid = true;
                }
            }

            // If none, make one
            if (this.Document == null)
            {
                // Make
                this.Document = new BsonDocument();

                // Flag
                this.IsNew = true;

                // Handle new
                this.SetDefaultValues(ctx);
            }

            // Assure
            this.SetSysValues();
        }

        /// <summary>
        /// 
        /// Copies store to an object
        /// 
        /// </summary>
        /// <param name="to">Target store</param>
        public void LoadFrom(JObject to)
        {
            //
            if (to != null)
            {
                // Move
                this.Document.FromJObject(to);
                // Flag fields
                foreach (string sField in to.Keys())
                {
                    this.MarkChanged(sField);
                }
            }
        }

        /// <summary>
        /// 
        /// Copies store to an object
        /// 
        /// </summary>
        /// <param name="to">Target store</param>
        public void LoadFrom(StoreClass to)
        {
            this.LoadFrom(to.SynchObject);
        }

        /// <summary>
        /// 
        /// Saves the changes
        /// 
        /// </summary>
        public bool Save(bool force = false, string user = "", bool runtask = false, bool signal = false, JObject orig = null)
        {
            //
            bool bAns = true;

            // Get the changes
            List<string> c_Changes = this[FieldChanges].ToJArrayOptional().ToList();

            // Do we have an ID alias?
            if (this.IsData && this.Dataset.Definition.IDAlias.HasValue())
            {
                // Missign alias?
                if (!this[this.Dataset.Definition.IDAlias].HasValue())
                {
                    // Fill
                    this[this.Dataset.Definition.IDAlias] = this.ID;

                    //
                    c_Changes.Add(this.Dataset.Definition.IDAlias);
                }

                // Force the _id
                this[FieldID] = this[this.Dataset.Definition.IDAlias];
            }

            // Do we force?
            if (force)
            {
                // All
                c_Changes = this.Fields;
            }

            // SIO flags
            List<string> c_SIO = new List<string>();
            // Do we have an SIO field?
            if (this.Fields.Contains(FieldSIO))
            {
                // Get
                c_SIO = this[FieldSIO].SplitSpaces();
            }

            // Remove system
            c_Changes.Remove(FieldUUID);
            c_Changes.Remove(FieldChanges);
            c_Changes.Remove(FieldToken);
            c_Changes.Remove(FieldSIO);
            if (this.IsData) c_Changes.Remove(FieldAccount);

            // Prepare
            if (orig == null) orig = this.AsJObject;
            // Make changes object
            JObject c_CData = new JObject();
            // Loop thru
            foreach (string sField in c_Changes)
            {
                // Save
                c_CData.Set(sField, this[sField]);
            }

            // Do pre process
            if (this.IsData && this.Dataset.Definition.SavePreProcess != null)
            {
                this.Dataset.Definition.SavePreProcess(orig, c_CData);
            }

            // Assume normal
            bool bSave = true;

            // Call task
            string sTask = this.Dataset.Definition.TaskAtSave;
            if (sTask.HasValue() && runtask && this.IsData)
            {
                // Call
                using (TaskParamsClass c_Params = new TaskParamsClass(this.Parent.Parent.Parent.Parent))
                {
                    c_Params.Task = sTask;

                    c_Params.AddObject("passed", this);
                    c_Params.AddStore("changes", new StoreClass(c_CData));
                    c_Params.AddStore("current", new StoreClass(orig));
                    c_Params["_user"] = user;

                    StoreClass c_Resp = c_Params.Call();

                    if (c_Params.CancelSave)
                    {
                        // Cancel the save
                        bSave = false;
                    }
                }
            }

            // Do we save?
            if (bSave)
            {
                // Only if changes
                if (c_Changes.Count > 0)
                {
                    // Make SIO
                    using (SIO.ManagerClass c_SIOM = this.Parent.Parent.Parent.Parent.Globals.Get<SIO.ManagerClass>())
                    {
                        // Make the window ID
                        string sWinID = "ao_{0}_{1}".FormatString(this.Dataset.Name, this.ID);

                        // Make the filter
                        FilterDefinition<BsonDocument> c_Filter = Builders<BsonDocument>.Filter.Eq(FieldID, this.Document.GetValue(FieldID));

                        // Set a new version
                        UpdateDefinition<BsonDocument> c_Updates = Builders<BsonDocument>.Update.Set(FieldVersion, ObjectId.GenerateNewId().ToString());
                        // Loop thru
                        foreach (string sField in c_Changes)
                        {
                            // Cannot change the id
                            if (sField.HasValue() && !sField.IsSameValue(FieldID))
                            {
                                //
                                bool bDo = true;

                                // Only if data
                                if (this.IsData)
                                {
                                    // Get the type
                                    if (this.Dataset != null && this.Dataset.Definition != null)
                                    {
                                        // Do we signal
                                        if (signal)
                                        {
                                            // Make a message
                                            using (SIO.MessageClass c_Msg = new SIO.MessageClass(c_SIOM,
                                                                                                    SIO.MessageClass.Modes.Both,
                                                                                                    "$$object.data",
                                                                                                    "winid", sWinID,
                                                                                                    "aoFld", sField,
                                                                                                    "value", this[sField]))
                                            {
                                                // Send
                                                c_Msg.Send();
                                            }
                                        }

                                        // Get the field
                                        Definitions.DatasetFieldClass c_FDef = this.Dataset.Definition[sField];
                                        if (c_FDef != null)
                                        {
                                            // Save the value
                                            string sValue = this[sField];

                                            // According to type
                                            switch (c_FDef.Type)
                                            {
                                                case Definitions.DatasetFieldClass.FieldTypes.Password:
                                                    // Must have value
                                                    if (sValue.HasValue())
                                                    {
                                                        // Create
                                                        c_Updates = c_Updates.Set(sField, sValue.MD5HashString());
                                                    }
                                                    bDo = false;
                                                    break;
                                                case Definitions.DatasetFieldClass.FieldTypes.Boolean:
                                                    // Must have value
                                                    if (sValue.HasValue())
                                                    {
                                                        // Assure
                                                        c_Updates = c_Updates.Set(sField, sValue.FromDBBoolean().ToDBBoolean());
                                                    }
                                                    else
                                                    {
                                                        c_Updates = c_Updates.Set(sField, sValue);
                                                    }
                                                    bDo = false;
                                                    break;

                                                case Definitions.DatasetFieldClass.FieldTypes.Account:
                                                    this.UpdateAccount(sField, orig.Get(sField), c_CData.Get(sField));
                                                    break;

                                                case Definitions.DatasetFieldClass.FieldTypes.TwilioPhone:
                                                    using (StoreClass c_Info = new StoreClass())
                                                    {
                                                        c_Info["phone"] = sValue;
                                                        c_Info["ref"] = this.UUID.ToString();
                                                        this.Parent.Parent.Parent.Parent.FN("Communication.TwilioRegister", c_Info);
                                                    }
                                                    break;
                                            }
                                        }

                                    }
                                }

                                // Normal
                                if (bDo)
                                {
                                    // Add
                                    c_Updates = c_Updates.Set(sField, this.Document.GetValue(sField));
                                }
                            }
                        }

                        // Data collection?
                        if (this.IsData)
                        {
                            // Create context
                            using (ExtendedContextClass c_Ctx = new ExtendedContextClass(this.Parent.Parent.Parent.Parent,
                                                                                            null,
                                                                                            this,
                                                                                            user.IfEmpty()))
                            {
                                // Process placeholder
                                string sPH = this.ProcessExtended(c_Ctx, this.Parent.Definition.Placeholder);
                                c_Updates = c_Updates.Set(FieldDescription, sPH);
                                c_Updates = c_Updates.Set(FieldSearch, sPH.ToUpper());

                                // Process calendar
                                c_Updates = c_Updates.Set(FieldCalendarStart,
                                    this.ProcessExtended(c_Ctx, this.Parent.Definition.CalendarStart, delegate (string value)
                                    {
                                        return "d" + value.FromDBDate().AdjustTimezone().ToDBDate();
                                    }));
                                c_Updates = c_Updates.Set(FieldCalendarEnd,
                                     this.ProcessExtended(c_Ctx, this.Parent.Definition.CalendarEnd, delegate (string value)
                                    {
                                        return "d" + value.FromDBDate().AdjustTimezone().ToDBDate();
                                    }));
                                c_Updates = c_Updates.Set(FieldCalendarSubject,
                                     this.ProcessExtended(c_Ctx, this.Parent.Definition.CalendarSubject));

                                // Get SIO from dataset
                                if (this.Dataset != null)
                                {
                                    // Get the SIO
                                    string sSIO = this.Dataset.Definition.SIOEventsAtSave;
                                    // Any?
                                    if (sSIO.HasValue())
                                    {
                                        // Add
                                        c_SIO.AddRange(sSIO.SplitSpaces());
                                    }
                                }

                                // Add fixed
                            }
                        }

                        // Always
                        c_SIO.Add(SIOSaved);

                        // Options
                        UpdateOptions c_Opts = new UpdateOptions();
                        c_Opts.IsUpsert = true;

                        // Update
                        this.Collection.Documents.UpdateOne(c_Filter, c_Updates, c_Opts);

                        // Is it an activity?
                        if (this[AO.Extended.WorkflowClass.FieldWFActivity].FromDBBoolean())
                        {
                            // Get the outcome
                            string sOutcome = this[AO.Extended.WorkflowClass.FieldWFOutcome];
                            if (sOutcome.HasValue())
                            {
                                // Get the cron manager
                                using (CronManagerClass c_Cron = new CronManagerClass(this.Parent.Parent))
                                {
                                    // Get params
                                    string sFlow = this[AO.Extended.WorkflowClass.FieldWFFlowName];
                                    string sInstance = this[AO.Extended.WorkflowClass.FieldWFInstance];
                                    AO.UUIDClass c_UUID = new UUIDClass(this.Parent.Parent, this[AO.Extended.WorkflowClass.FieldWFShadow]);
                                    // Open the group
                                    using (AO.Extended.GroupClass c_Group = new AO.Extended.GroupClass(this.Dataset.Parent, Extended.GroupClass.Types.Workflow, this.UUID, sFlow, sInstance))
                                    {
                                        // Does the shadow exist?
                                        if (c_Cron.Contains(c_UUID.ID))
                                        {
                                            // Get
                                            AO.Extended.WorkflowClass c_Shadow = c_Cron.Get(c_UUID.ID) as AO.Extended.WorkflowClass;
                                            // Get the next step
                                            string sNext = (sOutcome.IsSameValue("Fail") ? c_Shadow[AO.Extended.WorkflowClass.FieldWFIfFail] : c_Shadow[AO.Extended.WorkflowClass.FieldWFIfDone]);
                                            // And delete
                                            c_Shadow.Delete();

                                            // Any?
                                            if (sNext.HasValue())
                                            {
                                                // Make args
                                                StoreClass c_Args = new StoreClass();
                                                c_Args["ds"] = this.Dataset.Name;
                                                c_Args["id"] = this.UUID.ID;
                                                c_Args["wf"] = sFlow;
                                                c_Args["instance"] = sInstance;
                                                c_Args["at"] = sNext;
                                                this.Parent.Parent.Parent.Parent.FN("Workflow.Continue", c_Args);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Reset changes
                        this[FieldChanges] = "";
                    }
                }

                // Any SIO?
                if (c_SIO != null && c_SIO.Count > 0)
                {
                    // Get the Dataset and ID
                    string sDS = this.UUID.Dataset.Name;
                    string sID = this.UUID.ID;
                    string sWindID = "ao_{0}_{1}".FormatString(sDS, sID);

                    // Open the manager
                    SIO.ManagerClass c_Mgr = this.Parent.Parent.Parent.Parent.Globals.Get<SIO.ManagerClass>();

                    // Loop thru
                    foreach (string sFN in c_SIO)
                    {
                        // Make a message
                        using (SIO.MessageClass c_Msg = new SIO.MessageClass(c_Mgr, SIO.MessageClass.Modes.Both, sFN,
                            "ds", sDS,
                            "id", sID,
                            "winid", sWindID))
                        {
                            // Send
                            c_Msg.Send();
                        }
                    }
                }
            }
            else
            {
                // Flag out
                bAns = false;
            }

            return bAns;
        }

        /// <summary>
        /// 
        /// Updates account Access table
        /// 
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="newvalue"></param>
        private void UpdateAccount(string field, string orig, string newvalue)
        {
            // Get the UUID of ourselves
            string sOUUID = this.UUID.ToString();

            // Assure
            orig = orig.IfEmpty();
            newvalue = newvalue.IfEmpty();

            // Get new
            ObjectClass c_New = null;
            if (newvalue.HasValue())
            {
                // The UUID of the new
                string sNUUID = sOUUID.CombinePath(newvalue).MD5HashString();

                using (UUIDClass c_UUID = new UUIDClass(this.Dataset.Parent, DatabaseClass.DatasetBillAccess, sNUUID))
                {
                    c_New = c_UUID.AsObject;
                }
            }

            // Changed?
            if (orig.IsSameValue(newvalue) || !orig.HasValue())
            {
                // Set
                c_New["name"] = newvalue;
                c_New["actual"] = sOUUID;
                c_New.Save();
            }
            else
            {
                // The UUID of the original
                string sXUUID = sOUUID.CombinePath(orig).MD5HashString();

                // Get original
                using (UUIDClass c_UUID = new UUIDClass(this.Dataset.Parent, DatabaseClass.DatasetBillAccess, sXUUID))
                {
                    ObjectClass c_Orig = c_UUID.AsObject;

                    // If not a delete
                    if (c_New != null)
                    {
                        // Copy
                        c_Orig.CopyTo(c_New);
                        // Set
                        c_New["name"] = newvalue;
                        c_New["actual"] = sOUUID;
                        // save
                        c_New.Save();
                    }

                    // Delete original
                    c_Orig.Delete();
                }
            }
        }

        /// <summary>
        /// 
        /// Evaluates a string
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        private string ProcessExtended(ExtendedContextClass ctx, string expr, Func<string, string> post = null)
        {
            // Assume none
            string sAns = "";

            // Defined?
            if (expr.HasValue())
            {
                try
                {
                    // Remake description
                    sAns = Expression.Evaluate(ctx, expr).Value;
                    // Post process
                    if (sAns.HasValue() && post != null)
                    {
                        sAns = post(sAns);
                    }
                }
                catch { }
            }

            return sAns;
        }

        /// <summary>
        /// 
        /// Handler to map passed object to store
        /// 
        /// </summary>
        /// <param name="cbp"></param>
        /// <returns></returns>
        private string HandleParams(ExprCBParams cbp)
        {
            // Assume nothing
            string sAns = null;

            // Set ourselves as the passed object
            cbp.Parent.Locals["passed"] = this;

            // Get the object to be used
            string sObjName = cbp.Prefix.IfEmpty(cbp.Parent.Locals.Default).IfEmpty("passed");
            // Any?
            if (sObjName.HasValue())
            {
                // Prefix only?
                if (!cbp.Field.HasValue())
                {
                    // Make UUID
                    using (UUIDClass c_UUID = new UUIDClass(this.Parent.Parent, cbp.Value))
                    {
                        // Add object
                        cbp.Parent.Locals[sObjName] = new ObjectClass(c_UUID.Dataset, c_UUID.ID);
                    }
                }
                else
                {
                    // Get
                    ObjectClass c_Obj = cbp.Parent.Locals[sObjName] as ObjectClass;
                    // Any?
                    if (c_Obj != null)
                    {
                        // Set?
                        if (cbp.Mode == ExprCBParams.Modes.Set)
                        {
                            // Do
                            c_Obj[cbp.Field] = cbp.Value;
                        }
                        else
                        {
                            // Get
                            sAns = c_Obj[cbp.Field];
                        }
                    }
                }
            }

            return sAns.IfEmpty();
        }

        /// <summary>
        /// 
        /// Marks a field as changed
        /// 
        /// </summary>
        /// <param name="field"></param>
        public void MarkChanged(string field)
        {
            // Only user fields
            if (field.HasValue() && (!field.StartsWith("_") || field.IsSameValue(FieldParent)))
            {
                // Get
                JArray c_Changes = this[FieldChanges].IfEmpty().ToJArrayOptional();
                // Set in changes
                if (!c_Changes.Contains(field))
                {
                    // Only if new
                    c_Changes.Add(field);
                    // Save
                    this[FieldChanges] = c_Changes.ToSimpleString();
                }
            }
        }

        /// <summary>
        /// 
        /// Gets a field as a JSON Object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JObject GetAsJObject(string name)
        {
            return this.Document.GetValue(name).AsBsonDocument.ToJObject();
        }

        /// <summary>
        /// 
        /// Sets a field as a JSON object
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAsJObject(string field, JObject value)
        {
            // Make
            BsonDocument c_Doc = new BsonDocument();

            // Loop thru
            foreach (string sKey in value.Keys())
            {
                // Set
                c_Doc.Set(sKey, value.Get(sKey));
            }

            // Save
            this.Document.Set(field, c_Doc);

            // Mark
            this.MarkChanged(field);
        }

        /// <summary>
        /// 
        /// Gets a field as a JSON Object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JArray GetAsJArray(string name)
        {
            return this.Document.GetValue(name).AsBsonArray.ToJArray();
        }

        /// <summary>
        /// 
        /// Sets a field as a JSON object
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAsJArray(string field, JArray value)
        {
            // Make
            BsonArray c_Doc = new BsonArray();

            // Loop thru
            for (int i = 0; i < value.Count; i++)
            {
                // Set
                c_Doc.Add(value.Get(i));
            }

            // Save
            this.Document.Set(field, c_Doc);

            // Mark
            this.MarkChanged(field);
        }

        /// <summary>
        /// 
        /// Gets a field as a BSON object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BsonDocument GetAsBObject(string name)
        {
            // Assume none
            BsonDocument c_Ans = null;

            try
            {
                // Make if not there
                if (!this.Document.Contains(name))
                {
                    this.Document.Set(name, new BsonDocument());
                }

                // Get
                c_Ans = this.Document.GetValue(name).AsBsonDocument;
            }
            catch
            {
                // Make a new one
                c_Ans = new BsonDocument();
                // Save
                this.Document.Set(name, c_Ans);
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Sets a field as a BSON object
        /// 
        /// </summary>
        /// <param name="field">The field</param>
        /// <param name="doc">The BSON object</param>
        public void SetAsBObject(string field, BsonDocument doc)
        {
            // Set
            this.Document.Set(field, doc);
        }

        /// <summary>
        /// 
        /// Deletes a field
        /// 
        /// </summary>
        /// <param name="name">The field name</param>
        public void Delete(string name)
        {
            // Any?
            if (this.Document.Contains(name))
            {
                // Delete
                this.Document.Remove(name);
                // And delete the folder
                this.Folder.Delete();
            }
        }

        /// <summary>
        /// 
        /// deletes the document
        /// 
        /// </summary>
        public void Delete()
        {
            // Make the filter
            using (QueryClass c_Filter = new QueryClass(this.Collection))
            {
                // By ID
                c_Filter.Add(FieldID, QueryElementClass.QueryOps.Eq, this.ID);

                // Delete the list
                c_Filter.Delete();

                // Delete documents
                this.Folder.Delete();

                // Delete acces points
                using (QueryClass c_AP = new QueryClass(this.Parent.Parent[DatabaseClass.DatasetBillAccess].DataCollection))
                {
                    // Any one related to us
                    c_AP.Add("actual", this.UUID.ToString());
                    // Bye
                    c_AP.Delete(true);
                }

                // Get the Dataset and ID
                string sDS = this.UUID.Dataset.Name;
                string sID = this.UUID.ID;

                // Open te manager
                SIO.ManagerClass c_Mgr = this.Parent.Parent.Parent.Parent.Globals.Get<SIO.ManagerClass>();

                // Make a message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(c_Mgr, SIO.MessageClass.Modes.Both, "$$object.deleted", "ds", sDS, "id", sID))
                {
                    // Send
                    c_Msg.Send();
                }

                // Get the children dss
                var cds = this.Dataset.Definition.ChildDSs;
                if (cds.HasValue())
                {
                    //
                    string sOID = this.UUID.ToString();

                    // Make list
                    List<string> c_DSS = cds.SplitSpaces();
                    // Loop thru
                    foreach (string sCDS in c_DSS)
                    {
                        // Get te dataset
                        DatasetClass c_CDSD = this.Parent.Parent[sCDS];

                        // Make the query
                        using (QueryClass c_Qry = new QueryClass(c_CDSD.DataCollection))
                        {
                            //
                            c_Qry.Add(ObjectClass.FieldParent, QueryElementClass.QueryOps.Eq, sOID);

                            // Loop thru
                            foreach (ObjectClass c_Child in c_Qry.FindObjects())
                            {
                                // Delete
                                c_Child.Delete();
                            }
                        }
                    }
                }

                // User?
                if (this.UUID.Dataset.Name.IsSameValue(DatabaseClass.DatasetUser))
                {
                    // Delete tags
                    this.UUID.Dataset.Parent.Tagged.DeleteAll(this.UUID.ID);
                }
            }
        }

        /// <summary>
        /// 
        /// Copies object to store
        /// 
        /// </summary>
        /// <param name="to">Target store</param>
        public void CopyTo(StoreClass to)
        {
            if (to != null)
            {
                // Loop thru
                foreach (string sField in this.Fields)
                {
                    // Set
                    to[sField] = this[sField];
                }
            }
        }

        /// <summary>
        /// 
        /// Loads an object
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void CopyFrom(JObject values)
        {
            // Loop thru
            foreach (string sField in values.Keys())
            {
                // Set
                this[sField] = values.Get(sField);
            }
        }

        /// <summary>
        /// 
        /// Are we autoupdating?
        /// 
        /// </summary>
        /// <param name="to">Target store</param>
        /// 
        private bool IsVolatile { get; set; }

        /// <summary>
        /// 
        /// Sets the auto-update 
        /// 
        /// </summary>
        /// <param name="cb"></param>
        public void Volatile(Action<string, string> cb = null)
        {
            // Save
            this.OnChanged = cb;

            // Open the manager
            SIO.ManagerClass c_Mgr = this.Parent.Parent.Parent.Parent.Globals.Get<SIO.ManagerClass>();

            this.Parent.Parent.Parent.Parent.LogInfo("Auto updating setup for {0}".FormatString(this.UUID.ToString()));

            // Set the callback
            c_Mgr.MessageReceived += delegate (SIO.MessageClass msg)
            {
                //this.Parent.Parent.Parent.Parent.Parent.LogInfo("Object: Received message: {0}".FormatString(msg.ToString()));

                //this.Parent.Parent.Parent.Parent.Parent.LogInfo("Object: Fn {0}".FormatString(msg.Fn));
                //this.Parent.Parent.Parent.Parent.Parent.LogInfo("Object: Message {0}".FormatString(msg.Values.ToSimpleString()));

                // From message
                string sMWID = msg["winid"];
                string sMDS = msg["ds"];
                string sMID = msg["id"];

                if (sMWID.HasValue() && !sMDS.HasValue() && !sMID.HasValue())
                {
                    // Fix
                    sMWID = sMWID.Replace("__", "_-");
                    // Split
                    string[] asPieces = sMWID.Split("_");
                    if (asPieces.Length > 2)
                    {
                        sMDS = asPieces[1].Replace("-", "_");
                        sMID = asPieces[2].Replace("-", "_");
                    }
                }

                // Get the Dataset and ID
                string sDS = this.UUID.Dataset.Name;
                string sID = this.UUID.ID;

                // Us?
                if (sDS.IsSameValue(sMDS) && sID.IsSameValue(sMID))
                {
                    //
                    //this.Parent.Parent.Parent.Parent.Parent.LogInfo("Auto updating {0}".FormatString(this.UUID.ToString()));

                    switch (msg.Fn)
                    {
                        case SIOSaved:
                            // Reload
                            this.Load();
                            // If callback, do
                            if (this.OnChanged != null) this.OnChanged(null, null);
                            break;

                        case SIOData:
                            this.IsVolatile = false;
                            this[msg["aofld"]] = msg["value"];
                            this.IsVolatile = true;
                            // If callback, do
                            if (this.OnChanged != null) this.OnChanged(msg["aofld"], msg["value"]);
                            break;
                    }
                }
            };

            this.IsVolatile = true;
        }

        /// <summary>
        /// 
        /// Copies one object to another
        /// 
        /// </summary>
        /// <param name="to"></param>
        public void CopyTo(ObjectClass to)
        {
            // Loop thru
            foreach (string sField in this.Fields)
            {
                // Not any system
                if (!sField.StartsWith("_"))
                {
                    // Set
                    to[sField] = this[sField];
                }
            }
        }

        /// <summary>
        /// 
        /// Evaluates an expresssion using the object as the default store
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string Eval(string expr)
        {
            // Create context
            using (ExtendedContextClass c_Ctx = new ExtendedContextClass(this.Parent.Parent.Parent.Parent,
                                                                            null,
                                                                            this,
                                                                            ""))
            {
                return DatumClass.Eval(c_Ctx, expr);
            }
        }

        /// <summary>
        /// 
        /// Copies object to store
        /// 
        /// </summary>
        /// <param name="store"></param>
        public void ToStore(StoreClass store)
        {
            this.Document.ToStore(store);
        }

        /// <summary>
        /// 
        /// Get the account field from parent
        /// 
        /// </summary>
        public void FloatAccount()
        {
            // Data?
            if (this.IsData)
            {
                // Do we have an account?
                if (this.Dataset.Definition.AccountFields.Count == 0)
                {
                    // Get 
                    string sAcct = this.GetAccount();
                    // Any?
                    if (sAcct.HasValue())
                    {
                        this.Document.SetField(FieldAccount, sAcct);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// Returns the account
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAccount()
        {
            // Assume none
            string sAns = null;

            //this.Parent.Parent.Parent.Parent.LogInfo();

            // Do we have an account field?
            List<string> c_AF = this.Dataset.Definition.AccountFields;
            // Any?
            if (c_AF.Count == 0)
            {
                this.Parent.Parent.Parent.Parent.LogInfo("NO ACCT FIELDS");
                // Get parent
                string sParent = this[FieldParent];
                if (sParent.HasValue())
                {
                    this.Parent.Parent.Parent.Parent.LogInfo("MOVING TO PARENT");
                    //
                    using (UUIDClass c_UUID = new UUIDClass(this.Parent.Parent, sParent))
                    {
                        using (ObjectClass c_Parent = c_UUID.AsObject)
                        {
                            // Get
                            sAns = c_Parent.GetAccount();
                        }
                    }
                }
            }
            else
            {
                foreach (string sFld in c_AF)
                {
                    this.Parent.Parent.Parent.Parent.LogInfo("CHEKING " + sFld);
                    sAns = this[sFld];
                    if (sAns.HasValue()) break;
                }
            }

            return sAns;
        }
        #endregion

        #region Workflow
        public ObjectClass MakeWorkflowActivity(AO.Extended.GroupClass group, Definitions.ElsaActivityClass def)
        {
            // Make an object
            ObjectClass c_Ans = null;

            // Get the dataset definition
            Definitions.DatasetClass c_Def = this.Dataset.Definition;

            // Do we have a workflow dataset?
            if (c_Def.WorkflowDataset.HasValue())
            {
                // Get it
                DatasetClass c_WDS = this.Dataset.Parent[c_Def.WorkflowDataset];
                // Mke a new object
                c_Ans = c_WDS.New();

                // Link to us
                c_Ans[AO.ObjectClass.FieldParent] = this.UUID.ToString();
                // Create context
                using (ExtendedContextClass c_Ctx = new ExtendedContextClass(this.Parent.Parent.Parent.Parent,
                                                                                null,
                                                                                this,
                                                                                ""))
                {
                    // Today
                    string sToday = DatumClass.Eval(c_Ctx, "#today([*sys:timezone])#");

                    // Fill
                    c_Ans[AO.Extended.WorkflowClass.FieldWFActivity] = "y";

                    c_Ans[c_Def.WorkflowDescription] = DatumClass.Eval(c_Ctx, def["subject"]);
                    c_Ans[AO.Extended.WorkflowClass.FieldWFDesc] = c_Def.WorkflowDescription;

                    c_Ans[c_Def.WorkflowStartedOn] = sToday;
                    c_Ans[AO.Extended.WorkflowClass.FieldWFStartOn] = c_Def.WorkflowStartedOn;

                    c_Ans[c_Def.WorkflowExpectedOn] = DateTime.Parse(sToday).Add(TimeSpan.FromSeconds(DatumClass.Eval(c_Ctx, def["duration"]).ToSeconds())).ToDBDate();
                    c_Ans[AO.Extended.WorkflowClass.FieldWFExpectedOn] = c_Def.WorkflowExpectedOn;

                    c_Ans[AO.Extended.WorkflowClass.FieldWFEndedOn] = c_Def.WorkflowEndedOn;

                    c_Ans[c_Def.WorkflowStartedOn] = DatumClass.Eval(c_Ctx, def["message"]);
                    c_Ans[AO.Extended.WorkflowClass.FieldWFMessage] = c_Def.WorkflowMessage;

                    c_Ans[c_Def.WorkflowStartedOn] = DatumClass.Eval(c_Ctx, def["assignedTo"]);
                    c_Ans[AO.Extended.WorkflowClass.FieldWFAssignedTo] = c_Def.WorkflowAssignedTo;

                    c_Ans[AO.Extended.WorkflowClass.FieldWFBy] = c_Def.WorkflowDoneBy;

                    c_Ans[AO.Extended.WorkflowClass.FieldWFOutcome] = c_Def.WorkflowOutcome;

                    // Create data
                    StoreClass c_Data = group.ToGroupData(
                        AO.Extended.WorkflowClass.FieldWFActivityUUIID, c_Ans.UUID.ToString(),
                        AO.Extended.WorkflowClass.FieldWFIfOverdue, def.Outcomes["OnOverdue"].IfEmpty(group.OnOverdue)
                    );

                    // Make entry
                    AO.Extended.WorkflowClass c_Shadow = group.New("Workflow.Overdue", c_Data, c_Ans[c_Def.WorkflowExpectedOn].FromDBDate()) as AO.Extended.WorkflowClass;

                    // Link back
                    c_Ans[AO.Extended.WorkflowClass.FieldWFShadow] = c_Shadow.UUID.ToString();
                    c_Ans[AO.Extended.WorkflowClass.FieldWFFlowName] = group.FlowName;
                    c_Ans[AO.Extended.WorkflowClass.FieldWFInstance] = group.InstanceName;

                    // Save
                    c_Ans.Save();

                    // And save
                    c_Shadow.Save();
                }
            }

            return c_Ans;
        }
        #endregion
    }
}
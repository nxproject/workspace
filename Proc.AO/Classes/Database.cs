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

using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;

using NX.Shared;

namespace Proc.AO
{
    public class DatabaseClass : ChildOfClass<ManagerClass>
    {
        #region Constants
        public const string DatasetSys = "_sys";
        public const string DatasetHelp = "_help";
        public const string DatasetUser = "_user";
        public const string DatasetAccount = "_account";
        public const string DatasetAllowed = "_allowed";
        public const string DatasetCookies = "_cookies";
        public const string DatasetIndexStore = "_indexstore";
        public const string DatasetWallet = "_wallet";
        public const string DatasetCron = "_cron";
        public const string DatasetGroup = "_group";
        public const string DatasetTagged = "_tagged";
        public const string DatasetTaggedDetail = "_taggeddet";

        public const string DatasetDingDong = "_dingdong";

        public const string DatasetIOTAgent = "iotagent";
        public const string DatasetIOTClient = "iotclient";
        public const string DatasetIOTUnit = "iotunit";
        public const string DatasetIOTKeyboard = "iotkeyboard";
        public const string DatasetIOTVerb = "iotverb";
        public const string DatasetIOTMacro = "iotmacro";
        #endregion

        #region Constructor
        public DatabaseClass(ManagerClass mgr, string name)
            : base(mgr)
        {
            //
            this.Name = name;
        }
        #endregion

        #region Indexer
        public DatasetClass this[string name]
        {
            get
            {
                // Fix
                name = name.IfEmpty().AsDatasetName();

                // Do we have it?
                if (!this.Cache.ContainsKey(name))
                {
                    // Add
                    this.Cache[name] = new DatasetClass(this, name);
                }

                return this.Cache[name];
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The name of the database
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// The database
        /// 
        /// </summary>
        private IMongoDatabase IDatabase { get; set; }
        public IMongoDatabase Database
        {
            get
            {
                if (this.IDatabase == null && this.Parent.DBInterface.Client != null)
                {
                    this.IDatabase = this.Parent.DBInterface.Client.GetDatabase(this.Name);
                }

                return this.IDatabase;
            }
        }

        /// <summary>
        /// 
        /// Dataset cache
        /// 
        /// </summary>
        private NamedListClass<DatasetClass> Cache { get; set; } = new NamedListClass<DatasetClass>();

        /// <summary>
        /// 
        /// Listing of all datasets
        /// 
        /// </summary>
        public List<string> AllDatasets
        {
            get
            {
                // Get the collections
                return this.Database.ListCollectionNames().ToList();
            }
        }

        /// <summary>
        /// 
        /// List of user accessible datasets
        /// 
        /// </summary>
        public List<string> Datasets
        {
            get
            {
                // Get all
                List<string> c_Ans = this.AllDatasets;

                // Flags
                bool bIOT = this.SiteInfo.IOTEnabled;

                // Loop thru
                for (int i = c_Ans.Count; i > 0; i--)
                {
                    // Flag for removal
                    bool bRemove = true;

                    // Get the name
                    string sName = c_Ans[i - 1];

                    // Remove if system
                    if (sName.StartsWith("#") && !sName.Contains("_"))
                    {
                        // Get name
                        string sDS = sName.Substring(1);
                        // Get dataset
                        DatasetClass c_DS = this[sDS];
                        // And use start index
                        bRemove = c_DS.Definition.IsHidden;
                        // 
                        c_Ans[i - 1] = sDS;

                        // 
                        switch (c_DS.Definition.Selector)
                        {
                            case "IOT":
                                bRemove = !bIOT;
                                break;
                        }
                    }

                    //
                    if (bRemove)
                    {
                        c_Ans.RemoveAt(i - 1);
                    }
                }

                // Assure _allowed
                this.AssureDataset(DatabaseClass.DatasetAllowed, c_Ans);
                // Assure users
                this.AssureDataset(DatabaseClass.DatasetUser, c_Ans);
                // Assure users
                this.AssureDataset(DatabaseClass.DatasetAccount, c_Ans);
                // Assure _help
                this.AssureDataset(DatabaseClass.DatasetHelp, c_Ans);

                //
                this.Initialize();

                return c_Ans;
            }
        }

        /// <summary>
        /// 
        /// The file manager
        /// 
        /// </summary>
        private NX.Engine.Files.ManagerClass IFileManager { get; set; }
        public NX.Engine.Files.ManagerClass FileManager
        {
            get
            {
                if (this.IFileManager == null)
                {
                    this.IFileManager = new NX.Engine.Files.ManagerClass(this.Parent.Parent);

                    // Link changes
                    this.IFileManager.FileSystemChanged += delegate (NX.Engine.Files.FileSystemParamClass param)
                    {
                        //this.Parent.Parent.LogInfo("ACTION: {0} - PATH: {1}".FormatString(param.Action, param.Path));

                        // Available?
                        if (this.Parent.Synch != null)
                        {
                            // Only certain actions
                            switch (param.Action)
                            {
                                case NX.Engine.Files.FileSystemParamClass.Actions.Write:
                                case NX.Engine.Files.FileSystemParamClass.Actions.Delete:
                                case NX.Engine.Files.FileSystemParamClass.Actions.CreatePath:

                                    // Parse
                                    string sDS = param.Path.PathToObjectDataset();
                                    string sID = param.Path.PathToObjectID();
                                    bool bDeleted = param.Action == NX.Engine.Files.FileSystemParamClass.Actions.Delete;

                                    // Valid?
                                    if (sDS.HasValue() && sID.HasValue())
                                    {
                                        // Make message
                                        using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Parent.Synch, SIO.MessageClass.Modes.Internal, "$$changed.document",
                                                                                                            "ds", sDS,
                                                                                                            "id", sID,
                                                                                                            "deleted", bDeleted.ToDBBoolean()))
                                        {
                                            // Send
                                            c_Msg.Send();
                                        }
                                    }
                                    break;
                            }
                        }
                    };
                }
                return this.IFileManager;
            }
        }

        /// <summary>
        /// 
        /// Tagged dataset
        /// 
        /// </summary>
        private TaggedClass ITagged { get; set; }
        public TaggedClass Tagged
        {
            get
            {
                if (this.ITagged == null)
                {
                    this.ITagged = new TaggedClass(this);
                }

                return this.ITagged;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IndexStoreClass IIndexStore { get; set; }
        public IndexStoreClass IndexStore
        {
            get
            {
                if (this.IIndexStore == null)
                {
                    this.IIndexStore = new IndexStoreClass(this);

                }
                return this.IIndexStore;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private DingDongsClass IDingDongs { get; set; }
        public DingDongsClass DingDongs
        {
            get
            {
                if (this.IDingDongs == null)
                {
                    this.IDingDongs = new DingDongsClass(this);

                }
                return this.IDingDongs;
            }
        }
        #endregion

        #region System Items
        private SiteInfoClass ISiteInfo { get; set; }
        public SiteInfoClass SiteInfo
        {
            get
            {
                if (this.ISiteInfo == null)
                {
                    this.ISiteInfo = new SiteInfoClass(this);

                    // Load from environment
                    if (!this.ISiteInfo.SynchObject["proccount"].HasValue())
                    {
                        this.ISiteInfo.ProcessorCount = 1;
                    }
                    this.Parent.Parent.LogInfo("Processor count is set to {0}".FormatString(this.ISiteInfo.ProcessorCount));

                    if (!this.ISiteInfo.SynchObject["domain"].HasValue())
                    {
                        this.ISiteInfo.Domain = this.Parent.Parent.Domain;
                    }
                    this.Parent.Parent.LogInfo("Domain is set to {0}".FormatString(this.ISiteInfo.Domain));

                    if (!this.ISiteInfo.SynchObject["certemail"].HasValue())
                    {
                        this.ISiteInfo.CertEMail = this.Parent.Parent["certbot_email"].IfEmpty();
                    }
                    this.Parent.Parent.LogInfo("Certificate EMail is set to {0}".FormatString(this.ISiteInfo.CertEMail));

                    this.Parent.Parent.LogInfo("Reachable URL is set to {0}".FormatString(this.Parent.Parent.ReachableURL));

                    this.ISiteInfo.UpdateEnv(false);
                }

                return this.ISiteInfo;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Assures that a dataset exists and isin list
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="done"></param>
        private void AssureDataset(string ds, List<string> done)
        {
            // Chack
            if (!done.Contains(ds))
            {
                // Add to list
                done.Add(ds);

                // Assure
                var c_DS = this[ds];
            }
        }

        /// <summary>
        /// 
        /// Removes a dataset from the cache
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void RemoveFromCache(string name)
        {
            // Do we have it?
            if (this.Cache.ContainsKey(name))
            {
                // Add
                this.Cache.Remove(name);
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// 
        /// Has the initialize logic run
        /// 
        /// </summary>
        private static bool HasInitialized { get; set; }

        /// <summary>
        /// 
        /// Initilaize logic
        /// 
        /// </summary>
        public void Initialize()
        {
            // Only once
            if (!HasInitialized)
            {
                //
                HasInitialized = true;

                //
                var x = this.SiteInfo;

            }
        }
        #endregion
    }
}
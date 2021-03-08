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

using System.Collections.Generic;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Database interface
    /// 
    /// </summary>
    public class ManagerClass : ChildOfClass<EnvironmentClass>
    {
        #region Constructor
        public ManagerClass(EnvironmentClass env)
            : base(env)
        {
            // Setup
            this.DBInterface = env.Globals.Get<NX.Engine.BumbleBees.MongoDb.ManagerClass>();

            // Setup the file manager
            this.FileInterface = this.Parent.Globals.Get<NX.Engine.Files.ManagerClass>();
            // And the events
            this.FileInterface.FileSystemChanged += delegate (NX.Engine.Files.FileSystemParamClass param)
            {
                // Only certain actions
                switch (param.Action)
                {
                    case NX.Engine.Files.FileSystemParamClass.Actions.Write:
                    case NX.Engine.Files.FileSystemParamClass.Actions.Delete:
                    case NX.Engine.Files.FileSystemParamClass.Actions.CreatePath:

                        // Available?
                        if (this.Synch != null)
                        {
                            // Parse
                            string[] asPieces = param.Path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
                            bool bDeleted = param.Action == NX.Engine.Files.FileSystemParamClass.Actions.Delete;

                            if (asPieces.Length > 3 && asPieces[0].IsSameValue("ao"))
                            {
                                // Make message
                                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, false, "$$changed.document",
                                                                                                    "path", param.Path,
                                                                                                    "ds", asPieces[1],
                                                                                                    "id", asPieces[2],
                                                                                                    "deleted", bDeleted.ToDBBoolean()))
                                {
                                    // Send
                                    c_Msg.Send();
                                }
                            }
                        }
                        break;
                }
            };

            // Synch support
            this.Synch = env.Globals.Get<SIO.ManagerClass>();
            this.Synch.MessageReceived += delegate (SIO.MessageClass msg)
            {
                // Handle by fn
                switch (msg.Fn)
                {
                    case "$$changed.dataset":
                        // Get the dataset
                        string sDS = msg["ds"];
                        // Any?
                        if (sDS.HasValue())
                        {
                            // In cache?
                            if (this.Cache.ContainsKey(sDS))
                            {
                                // Remove
                                this.Cache.Remove(sDS);
                            }
                        }
                        break;
                }
            };

            // Assure bee
            this.DBInterface.CheckForAvailability();
        }
        #endregion

        #region Indexer
        public DatabaseClass this[string name]
        {
            get
            {
                // If no name
                if (!name.HasValue()) name = this.Parent.Hive.Name;

                // Do we have it?
                if (!this.Cache.ContainsKey(name))
                {
                   // Add
                    this.Cache[name] = new DatabaseClass(this, name);
                }

                return this.Cache[name];
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The MongoDb client
        /// 
        /// </summary>
        public NX.Engine.BumbleBees.MongoDb.ManagerClass DBInterface { get; private set; }

        /// <summary>
        /// 
        /// The synch
        /// 
        /// </summary>
        public SIO.ManagerClass Synch { get; private set; }

        /// <summary>
        /// 
        /// The file manager
        /// 
        /// </summary>
        public NX.Engine.Files.ManagerClass FileInterface { get; set; }

        /// <summary>
        /// 
        /// Database cache
        /// 
        /// </summary>
        private NamedListClass<DatabaseClass> Cache { get; set; } = new NamedListClass<DatabaseClass>();

        /// <summary>
        /// 
        /// The default database
        /// 
        /// </summary>
        private DatabaseClass IDefaultDatabase { get; set; }
        public DatabaseClass DefaultDatabase 
        { 
            get 
            {
                if(this.IDefaultDatabase == null)
                {
                    // Create 
                    this.IDefaultDatabase = this[null];

                    //
                    //var x = this.IDefaultDatabase.SystemDataset;

                    //// Build default datasets
                    //var j1 = this.IDefaultDatabase[DatabaseClass.DatasetSys];
                    //var j2 = this.IDefaultDatabase[DatabaseClass.DatasetUser];
                    //var j3 = this.IDefaultDatabase[DatabaseClass.DatasetAllowed];
                    //var j4 = this.IDefaultDatabase[DatabaseClass.DatasetIndexStore];
                }
                return this.IDefaultDatabase;
            } 
        }
        #endregion

        #region Methods
        public void SignalChange(DatasetClass ds, bool deleted = false)
        {
            // Available?
            if (this.Synch != null)
            {
                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, false, "$$changed.dataset", 
                                                                                    "ds", ds.Name,
                                                                                    "deleted", deleted.ToDBBoolean()))
                {
                    // Send
                    c_Msg.Send();
                }

                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, true, "$$changed.dataset",
                                                                                    "ds", ds.Name,
                                                                                    "deleted", deleted.ToDBBoolean()))
                {
                    // Send
                    c_Msg.Send();
                }
            }
        }

        public void SignalChange(Definitions.ViewClass view, bool deleted = false )
        {
            // Available?
            if (this.Synch != null)
            {
                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, false, "$$changed.view", 
                                                                                    "ds", view.Parent.Name, 
                                                                                    "view", view.Name.AsViewName(),
                                                                                    "deleted", deleted.ToDBBoolean()))
                {
                    // Send
                    c_Msg.Send();
                }

                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, true, "$$changed.view",
                                                                                    "ds", view.Parent.Name,
                                                                                    "view", view.Name.AsViewName(),
                                                                                    "deleted", deleted.ToDBBoolean()))
                {
                    // Send
                    c_Msg.Send();
                }
            }
        }

        public void SignalChange(Definitions.PickListClass pl, bool deleted = false)
        {
            // Available?
            if (this.Synch != null)
            {
                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, false, "$$changed.picklist",
                                                                                    "ds", pl.Parent.Name,
                                                                                    "picklist", pl.Name.AsKeyword(),
                                                                                    "deleted", deleted.ToDBBoolean()))
                {
                    // Send
                    c_Msg.Send();
                }

                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, true, "$$changed.picklist",
                                                                                    "ds", pl.Parent.Name,
                                                                                    "picklist", pl.Name.AsKeyword(),
                                                                                    "deleted", deleted.ToDBBoolean()))
                {
                    // Send
                    c_Msg.Send();
                }
            }
        }

        public void SignalChange(Definitions.ElsaClass pl, bool deleted = false)
        {
            // Available?
            if (this.Synch != null && pl.Dataset.HasValue())
            {
                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, false, "$$changed.{0}".FormatString(pl.Type),
                                                                                    "ds", pl.Dataset,
                                                                                    "task", pl.Name.AsKeyword(),
                                                                                    "deleted", deleted.ToDBBoolean()))
                {
                    // Send
                    c_Msg.Send();
                }
            }
        }
        #endregion
    }
}
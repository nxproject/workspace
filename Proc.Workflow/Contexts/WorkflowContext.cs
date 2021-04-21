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

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Proc.AO;

namespace Proc.Workflow
{
    public class WorkflowContextClass : ExtendedContextClass
    {
        #region Constructor
        public WorkflowContextClass(ManagerClass mgr,
                                InstanceClass inst,
                                AO.Extended.GroupClass group,
                                string user,
                                StoreClass store,
                                AO.ObjectClass obj)
            : base(mgr.Parent, store, obj, user)
        {
            //
            this.Manager = mgr;
            this.Instance = inst;

            //
            this.Stores = new ContextStoreClass<StoreClass>();
            this.Documents = new ContextStoreClass<NX.Engine.Files.DocumentClass>();
        }
        #endregion

        #region Indexer
        public string this[string key]
        {
            get
            {
                using (DatumClass c_Datum = new DatumClass(this, key))
                {
                    return c_Datum.Value;
                }
            }
            set
            {
                using (DatumClass c_Datum = new DatumClass(this, key))
                {
                    c_Datum.Value = value;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The manager
        /// 
        /// </summary>
        public ManagerClass Manager { get; internal set; }

        /// <summary>
        ///  
        /// The running process
        /// 
        /// </summary>
        public InstanceClass Instance { get; set; }

        /// <summary>
        /// 
        /// The extended group
        /// 
        /// </summary>
        public AO.Extended.GroupClass Group { get; set; }
        #endregion

        #region Methods
        public StoreClass CallFN(string fn, StoreClass store)
        {
            return this.Env.FN(fn, store);
        }
        #endregion

        #region Database
        public AO.ObjectClass GetObject(AO.DatasetClass ds, string id)
        {
            return ds[id];
        }

        public AO.ObjectClass GetObject(string ds, string id)
        {
            return this.GetObject(this.Database[ds], id);
        }

        public AO.ObjectClass GetObject(AO.UUIDClass uuid)
        {
            return this.GetObject(uuid.Dataset, uuid.ID);
        }

        public List<AO.UUIDClass> Query(string ds, params string[] values)
        {
            // TBD
            return null;
        }
        #endregion

        #region SIO
        public void SendSIO(string user, string mesaage, List<string> attachments)
        {
            // TBD
        }
        #endregion

        #region DateTime
        public DateTime Today()
        {
            return DateTime.Today.AdjustTimezone(this.SiteInfo.Timezone);
        }

        public DateTime Now()
        {
            return DateTime.Now.AdjustTimezone(this.SiteInfo.Timezone);
        }
        #endregion

        #region Outcomes
        /// <summary>
        /// 
        /// The exit points
        /// 
        /// </summary>
        public NamedListClass<string> Outcomes { get; private set; }
        #endregion

        #region Trace
        /// <summary>
        /// 
        /// Starts trace mode
        /// 
        /// </summary>
        public void StartTrace()
        {
            // If not already
            if (this.TraceBuffer == null) this.TraceBuffer = new StringBuilder();
        }

        /// <summary>
        /// 
        /// End the trace
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public void EndTrace(WorkflowContextClass ctx)
        {
            // If tracing
            if (this.TraceBuffer != null)
            {
                // Send it
                this.SendTrace(ctx);

                // Delete
                this.TraceBuffer = null;
            }
        }

        /// <summary>
        /// 
        /// Send the trace to the user
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public void SendTrace(WorkflowContextClass ctx)
        {
            // If tracing
            if (this.TraceBuffer != null)
            {
                // TBD
            }
        }

        /// <summary>
        /// 
        /// The trace buffer
        /// 
        /// </summary>
        private StringBuilder TraceBuffer { get; set; }

        /// <summary>
        /// 
        /// Is the thread being traced?
        /// 
        /// </summary>
        public bool IsTracing { get { return this.TraceBuffer != null; } }
        #endregion

    }
}
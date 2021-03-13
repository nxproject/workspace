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

namespace Proc.Task
{
    public class TaskContextClass : ExtendedContextClass
    {
        #region Constructor
        public TaskContextClass(EnvironmentClass env,
                                string user,
                                Action<TaskContextClass> cb)
            : base(env, null, null, user)
        {
            //
            this.Arrays = new ContextStoreClass<ArrayClass>();
            this.Charges = new ContextStoreClass<ChargesClass>();

            this.Stores = new ContextStoreClass<StoreClass>();
            this.Documents = new ContextStoreClass<NX.Engine.Files.DocumentClass>();

            this.ObjectLists = new ContextStoreClass<ObjectListClass>();
            this.DocumentLists = new ContextStoreClass<DocumentListClass>();

            this.HTTP = new ContextStoreClass<HTTPClientClass>();
            this.FTP = new ContextStoreClass<FTPClientClass>();

            if (cb != null) cb(this);
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
        /// Storage
        /// 
        /// </summary>
        public ContextStoreClass<ArrayClass> Arrays { get; internal set; }
        public ContextStoreClass<ChargesClass> Charges { get; internal set; }

        public ContextStoreClass<ObjectListClass> ObjectLists { get; set; }
        public ContextStoreClass<DocumentListClass> DocumentLists { get; set; }

        public ContextStoreClass<HTTPClientClass> HTTP { get; internal set; }
        public ContextStoreClass<FTPClientClass> FTP { get; internal set; }

        public ContextStoreClass<MemoClass> Memos { get; internal set; }
        public ContextStoreClass<TextClass> Texts { get; internal set; }
        public ContextStoreClass<TaskQueryClass> Queries { get; internal set; }
        #endregion

        #region Methods
        public void Initialize(ManagerClass mgr, InstanceClass inst)
        { 
            //
            this.Manager = mgr;
            this.Instance = inst;

        }

        /// <summary>
        /// 
        /// Calls a function
        /// 
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public StoreClass CallFN(string fn, StoreClass store)
        {
            return base.Parent.FN(fn, store);
        }

        /// <summary>
        ///  
        /// Copies context to serve as params
        /// 
        /// </summary>
        /// <param name="tp"></param>
        public void AsParams(AO.TaskParamsClass tp)
        {
            // Stores
            foreach(string sKey in this.Stores.Keys)
            {
                // Copy
                tp.AddStore(sKey, this.Stores[sKey]);
            }

            // Objects
            foreach(string sKey in this.Objects.Keys)
            {
                // Copy
                tp.AddObject(sKey, this.Objects[sKey]);
            }
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
        public void EndTrace(TaskContextClass ctx)
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
        public void SendTrace(TaskContextClass ctx)
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

        #region On
        public string OnError { get; set; }
        #endregion

        #region SIO
        public void SIO(string fn, params string[] values)
        {
            // Open the manager
            Proc.SIO.ManagerClass c_Mgr = this.Parent.Globals.Get<Proc.SIO.ManagerClass>();

            // Make a message
            using (SIO.MessageClass c_Msg = new SIO.MessageClass(c_Mgr, Proc.SIO.MessageClass.Modes.Internal, fn, values))
            {
                // Send
                c_Msg.Send();
            }
        }
        #endregion
    }
}
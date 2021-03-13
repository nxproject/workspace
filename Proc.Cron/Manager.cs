///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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
using Proc.AO;
using Proc.SIO;

namespace Proc.Cron
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
            this.AOInterface = env.Globals.Get<Proc.AO.ManagerClass>();
            this.Dataset = this.AOInterface.DefaultDatabase[AO.DatabaseClass.DatasetCron];

            // Synch support
            this.Synch = env.Globals.Get<Proc.SIO.ManagerClass>();
            this.Synch.MessageReceived += delegate (SIO.MessageClass msg)
            {
                // Handle by fn
                switch (msg.Fn)
                {
                    case "$$cron.set":
                        // TBD
                        break;

                    case "$$cron.delete":
                        // TBD
                        break;
                }
            };

            // Setup queen logic
            this.Parent.Hive.QueenChanged += delegate()
            {
                // Are we the queen?
                if(this.Parent.Hive.Roster.IsQueen)
                {
                    this.StartProcess();
                }
                else
                {
                    this.StopProcess();
                }
            };

            // Handle startup
            if(this.Parent.Hive.Roster.IsQueen)
            {
                this.StartProcess();
            }
        }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            // Kill the thread
            this.StopProcess();

            base.Dispose();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The MongoDb client
        /// 
        /// </summary>
        public Proc.AO.ManagerClass AOInterface { get; private set; }

        /// <summary>
        /// 
        /// The dataset
        /// 
        /// </summary>
        public AO.DatasetClass Dataset { get; private set; }

        /// <summary>
        /// 
        /// The synch
        /// 
        /// </summary>
        public Proc.SIO.ManagerClass Synch { get; private set; }

        /// <summary>
        /// 
        ///The cron thread
        ///
        /// </summary>
        private string ThreadID { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Satrts the cron process
        /// 
        /// </summary>
        private void StartProcess()
        {
            if (!this.ThreadID.HasValue())
            {
                // Start the thread
                this.ThreadID = "".GUID();
                SafeThreadManagerClass.StartThread(this.ThreadID, new System.Threading.ParameterizedThreadStart(this.ProcessThread));

                this.Parent.LogInfo("CRON process started");
            }
        }

        /// <summary>
        /// 
        /// Stops the cron process
        /// 
        /// </summary>
        public void StopProcess()
        {
            // Kill the thread
            if (this.ThreadID.HasValue())
            {
                SafeThreadManagerClass.StopThread(this.ThreadID);
                this.ThreadID = null;

                this.Parent.LogInfo("CRON process ended");
            }
        }

        /// <summary>
        /// 
        /// The thread that checks for jobs
        /// 
        /// </summary>
        /// <param name="status"></param>
        private void ProcessThread(object status)
        {
            // TBD
        }

        /// <summary>
        /// 
        /// Signal a cron process
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <param name="next"></param>
        public void SignalChange(CronEntryClass entry)
        {
            // Available?
            if (this.Synch != null)
            {
                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, MessageClass.Modes.Internal, "$$cron.{0}".FormatString(entry.SIOCode),
                                                                                    "id", entry.Name,
                                                                                    "value", entry.SIOValues,
                                                                                    "next", entry.NextOn.ToDBDate()))
                {
                    // Send
                    c_Msg.Send();
                }
            }
        }
        #endregion
    }
}
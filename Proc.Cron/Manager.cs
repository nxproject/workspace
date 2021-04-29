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

using System;
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
            this.Parent.Hive.QueenChanged += delegate ()
            {
                // Are we the queen?
                if (this.Parent.Hive.Roster.IsQueen)
                {
                    this.StartProcess();
                }
                else
                {
                    this.StopProcess();
                }
            };

            // Handle startup
            if (this.Parent.Hive.Roster.IsQueen)
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
        public Proc.AO.ManagerClass DBManager { get; private set; }

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
            //
            SafeThreadStatusClass c_Status = status as SafeThreadStatusClass;

            // Last run date
            DateTime c_Last = DateTime.Now;

            //
            this.Parent.LogInfo("Starting CRON process...");

            //
            while (c_Status.IsActive)
            {
                // Flag starting time
                DateTime c_Now = DateTime.Now;

                // Get the database manager
                this.DBManager = this.Parent.Globals.Get<AO.ManagerClass>();

                // Open a query
                using (AO.QueryClass c_Qry = new QueryClass(this.DBManager.DefaultDatabase[AO.DatabaseClass.DatasetCron].DataCollection))
                {
                    // Get already marked
                    c_Qry.Add("next", QueryElementClass.QueryOps.Lte, c_Now.ToDBDate());

                    // Loop theu
                    foreach (AO.ObjectClass c_Obj in c_Qry.FindObjects())
                    {
                        // Wrap
                        using (CronEntryClass c_Entry = new CronEntryClass(c_Obj))
                        {
                            // Run
                            c_Entry.Run(this);
                        }
                    }
                }

                // Has the date changed?
                if (c_Last.DayOfYear != c_Now.DayOfYear)
                {
                    //
                    this.DailyHousekeeping();
                }

                // Has the month changed?
                if (c_Last.Month != c_Now.Month)
                {
                    this.MonthlyHousekeeping();
                }

                // Has the year changed?
                if (c_Last.Year != c_Now.Year)
                {
                    this.YearlyHousekeeping();
                }

                // Reset
                c_Last = c_Now;

                // Wait a few minutes
                c_Status.WaitFor(3.MinutesAsTimeSpan());
            }

            //
            this.Parent.LogInfo("Ending CRON process...");
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

        #region Housekeeping
        /// <summary>
        /// 
        /// Stuff we have to do everyday
        /// 
        /// </summary>
        private void DailyHousekeeping()
        {
            //
            this.Parent.LogInfo("Starting daily housekeeping...");

            // Make a query
            using (AO.QueryClass c_Query = new QueryClass(this.DBManager.DefaultDatabase[AO.DatabaseClass.DatasetBitly].DataCollection))
            {
                // Days old
                int iDays = this.DBManager.DefaultDatabase.SiteInfo.BitlyDays;
                if (iDays <= 0) iDays = 7;
                // Set the filter
                c_Query.Add("creon", QueryElementClass.QueryOps.Lt, DateTime.Now.AddDays(iDays - iDays).ToDBDate());
                // Delete
                c_Query.Delete(true);
            }


            // DO we need to update?
            if (this.DBManager.DefaultDatabase.SiteInfo.AutoUpdate)
            {
                // Run
                this.Parent.FN("Updater.Run");
            }

            //
            this.Parent.LogInfo("Ending daily housekeeping...");
        }

        /// <summary>
        ///  Stuff we have to do every month
        ///  
        /// </summary>
        private void MonthlyHousekeeping()
        {
            //
            this.Parent.LogInfo("Starting monthly housekeeping...");

            //
            this.Parent.LogInfo("Ending monthly housekeeping...");
        }

        /// <summary>
        /// 
        /// Stuff we have to do every year
        /// 
        /// </summary>
        private void YearlyHousekeeping()
        { //
            this.Parent.LogInfo("Starting yearly housekeeping...");

            //
            this.Parent.LogInfo("Ending yearly housekeeping...");
        }
        #endregion
    }
}
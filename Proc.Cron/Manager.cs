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

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;
using Proc.AO;
using Proc.SIO;
using Proc.Communication;

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

            // Help out a bit
            AO.DatabaseClass c_DB = this.DBManager.DefaultDatabase;
            AO.SiteInfoClass c_SI = c_DB.SiteInfo;

            // Cleanup bitly
            using (AO.QueryClass c_Query = new QueryClass(c_DB[AO.DatabaseClass.DatasetBitly].DataCollection))
            {
                // Assure days
                int iDays = c_SI.BitlyDays;
                if (iDays <= 0) iDays = 7;
                // Set the filter
                c_Query.Add("creon", QueryElementClass.QueryOps.Lt, DateTime.Now.AddDays(iDays - iDays).ToDBDate());
                // Delete
                c_Query.Delete(true);
            }

            // Do subscriptions
            using (AO.QueryClass c_Query = new QueryClass(c_DB[AO.DatabaseClass.DatasetBiilSubscription].DataCollection))
            {
                // Everything till today
                c_Query.Add("nexton", QueryElementClass.QueryOps.Lt, DateTime.Today.AddDays(1).ToDBDate());
                // Loop
                foreach (AO.ObjectClass c_Sub in c_Query.FindObjects())
                {
                    // Must have a next on
                    string sNextOn = c_Sub["nexton"].IfEmpty();
                    if (sNextOn.HasValue())
                    {
                        // Create charge
                        AO.ObjectClass c_Charge = c_DB[AO.DatabaseClass.DatasetBiilCharge].New();
                        // Fill
                        c_Charge["code"] = c_Sub["code"];
                        c_Charge["desc"] = c_Sub["code"];
                        c_Charge["acct"] = c_Sub["code"];
                        c_Charge["at"] = c_Sub["code"];
                        c_Charge["units"] = c_Sub["code"];
                        c_Charge["rate"] = c_Sub["code"];
                        c_Charge["price"] = c_Sub["code"];
                        c_Charge["disc"] = c_Sub["code"];
                        c_Charge["total"] = c_Sub["code"];
                        c_Charge["taxable"] = c_Sub["code"];
                        // Save
                        c_Charge.Save();

                        // Compute next
                        DateTime c_NextOn = sNextOn.FromDBDate();
                        switch (c_Sub["interval"])
                        {
                            case "daily":
                                c_NextOn = c_NextOn.AddDays(1);
                                break;
                            case "weekly":
                                c_NextOn = c_NextOn.AddDays(7);
                                break;
                            case "twomonths":
                                c_NextOn = c_NextOn.AddMonths(2);
                                break;
                            case "quarterly":
                                c_NextOn = c_NextOn.AddMonths(3);
                                break;
                            case "yearly":
                                c_NextOn = c_NextOn.AddYears(1);
                                break;
                            default:
                                c_NextOn = c_NextOn.AddMonths(17);
                                break;
                        }
                        // Update
                        c_Sub["nexton"] = c_NextOn.ToDBDate();
                        c_Sub.Save();
                    }
                }
            }

            // Do invoicing
            int iDOM = c_SI.InvoiceDOM;
            if (iDOM != 0)
            {
                // Adjust
                if (iDOM < 0)
                {
                    iDOM = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) + 1 + iDOM;
                }
                // Is today the day?
                if (DateTime.Today.Day == iDOM)
                {
                    // List of done
                    List<string> c_Done = new List<string>();

                    // Make filter
                    using (AO.QueryClass c_Query = new QueryClass(c_DB[AO.DatabaseClass.DatasetBiilCharge].DataCollection))
                    {
                        // Only if no invoice
                        c_Query.Add("invon", QueryElementClass.QueryOps.Eq, "");

                        // Loop thru
                        foreach (AO.ObjectClass c_Charge in c_Query.FindObjects())
                        {
                            // Get reference
                            string sRef = c_Charge["acct"].IfEmpty() + "-" + c_Charge["at"].IfEmpty();
                            // Done?
                            if (!c_Done.Contains(sRef))
                            {
                                // Flag
                                c_Done.Add(sRef);

                                // Create chain
                                JArray c_Queries = new JArray();
                                JObject c_Entry = new JObject();
                                c_Entry.Set("field", "acct");
                                c_Entry.Set("value", c_Charge["acct"].IfEmpty());
                                c_Queries.Add(c_Entry);
                                c_Entry = new JObject();
                                c_Entry.Set("field", "at");
                                c_Entry.Set("value", c_Charge["at"].IfEmpty());
                                c_Queries.Add(c_Entry);
                                JObject c_Chain = new JObject();
                                c_Chain.Set("queries", c_Queries);

                                //Make invoice
                                AO.ObjectClass c_Inv = c_DB[AO.DatabaseClass.DatasetBiilCharge].New(chain: c_Chain);
                                // Does it have a total?
                                if (c_Inv["total"].ToDouble(0) != 0)
                                {
                                    // Get the accout UUID
                                    using (AO.UUIDClass c_AUUID = new UUIDClass(c_DB, c_Inv["acct"]))
                                    {
                                        // Get the account
                                        using (AO.ObjectClass c_Acct = c_AUUID.AsObject)
                                        {
                                            // Get the account
                                            string sAcct = c_Acct["name"];
                                            string sSubject = "Payment request";
                                            string sMsg = (sAcct.IsFormattedPhone() ? "Click on the link to view invoice" : "Click on pay button to complete transaction");

                                            using (StoreClass c_Params = new StoreClass())
                                            {
                                                // Make the context
                                                using (ExtendedContextClass c_Ctx = new ExtendedContextClass(this.Parent, null, null, ""))
                                                {
                                                    // Do handlebars
                                                    HandlebarDataClass c_HData = new HandlebarDataClass(this.Parent);
                                                    // Add the object
                                                    c_HData.Merge(c_Inv.Explode(ExplodeMakerClass.ExplodeModes.Yes, c_Ctx));

                                                    // Fill store
                                                    c_Params[eMessageClass.KeyTo] = sAcct;
                                                    c_Params[eMessageClass.KeyCommand] = "email";
                                                    c_Params[eMessageClass.KeySubj] = c_SI.PayReqSubject.IfEmpty(sSubject);
                                                    c_Params[eMessageClass.KeyMsg] = c_SI.PayReqMessage.IfEmpty(sMsg);
                                                    c_Params["user"] = "";
                                                    c_Params[eMessageClass.KeyEMailTemplate] = c_SI.PayReqTemplate;
                                                    c_Params[eMessageClass.KeyInvoice] = c_Inv["code"];

                                                    // Make message
                                                    using (eMessageClass c_Msg = eMessageClass.FromStore(this.Parent, c_Params, c_HData))
                                                    {
                                                        c_Msg.Send(false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //
                                    c_Inv["reqon"] = DateTime.Now.ToDBDate();
                                    // And save
                                    c_Inv.Save();
                                    // Send
                                    // TBD
                                }
                            }
                        }
                    }
                }
            }

            // Do we need to update?
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
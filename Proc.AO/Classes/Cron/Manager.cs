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

using NX.Engine;
using NX.Shared;
using Proc.SIO;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Database interface
    /// 
    /// </summary>
    public class CronManagerClass : ChildOfClass<DatabaseClass>
    {
        #region Constructor
        public CronManagerClass(DatabaseClass db)
            : base(db)
        {
            // Setup
            this.Dataset = this.Parent[AO.DatabaseClass.DatasetCron];

            // Synch support
            this.Synch = this.Parent.Parent.Parent.Globals.Get<Proc.SIO.ManagerClass>();
            this.Synch.MessageReceived += delegate (SIO.MessageClass msg)
            {
                //
                CronEntryClass c_Entry = null;
                if (!msg["id"].HasValue())
                {
                    c_Entry = this.Get(msg["id"]);
                }
                else
                {
                    StoreClass c_Data = null;
                    if (msg["data"].HasValue()) c_Data = new StoreClass(msg["data"].ToJObject());

                    c_Entry = this.New(msg["fn"], c_Data, msg["start"].FromDBDate());
                }

                // Handle by fn
                switch (msg.Fn)
                {
                    case "$$cron.set":
                        c_Entry.Save();
                        break;

                    case "$$cron.delete":
                        c_Entry.Delete();
                        break;
                }
            };

            // Start
            this.StartProcess();
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

        /// <summary>
        /// 
        /// Return true if entry is found
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(string id)
        {
            // Make a query
            using (QueryClass c_Qry = new QueryClass(this.Dataset.DataCollection))
            {
                // By ID
                c_Qry.AddByID(id);
                // Get the count
                return c_Qry.Find(1).Count > 0;
            }
        }

        /// <summary>
        /// 
        /// Gets an entry
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CronEntryClass Get(string id)
        {
            return new CronEntryClass(this.Dataset.New(id));
        }

        /// <summary>
        /// 
        /// Creates a new entry
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="patt"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public CronEntryClass New(string fn, StoreClass data, DateTime? next, string patt = null, DateTime? start = null, DateTime? end = null)
        {
            // Make
            CronEntryClass c_Ans = new CronEntryClass(this.Dataset.New());
            // Fill
            c_Ans.Fn = fn;
            c_Ans.Pattern = patt.IfEmpty();
            c_Ans.Timezone = this.Parent.SiteInfo.Timezone;

            if (data != null)
            {
                c_Ans.Data = data;
            }

            //
            if (start != null)
            {
                c_Ans.StartOn = (DateTime)start;
            }
            else
            {
                c_Ans.StartOn = DateTime.Now;
            }
            //
            if (end != null)
            {
                c_Ans.EndOn = (DateTime)end;
            }
            else
            {
                c_Ans.EndOn = DateTime.MaxValue;
            }
            //
            if (next != null)
            {
                c_Ans.NextOn = (DateTime)next;
            }
            else if (c_Ans.Pattern.HasValue())
            {
                // Use expression
                using (CronExpressionClass c_Exp = new CronExpressionClass(c_Ans))
                {
                    //
                    c_Exp.Compute();
                }
            }
            else
            {
                c_Ans.NextOn = DateTime.Now;
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Return the next extry to be processed
        /// 
        /// </summary>
        public CronEntryClass Next
        {
            get
            {
                CronEntryClass c_Ans = null;

                using (QueryClass c_Qry = new QueryClass(this.Dataset.DataCollection))
                {
                    // Next must be before now
                    c_Qry.Add(CronEntryClass.KeyNext, QueryElementClass.QueryOps.Lte, DateTime.Now.ToDBDate());
                    // Get object
                    ObjectClass c_Obj = c_Qry.PopObject(CronEntryClass.KeyNext);
                    if (c_Obj != null)
                    {
                        c_Ans = new CronEntryClass(c_Obj);

                        // Repeating?
                        if (c_Ans.Pattern.HasValue())
                        {
                            // Use expression
                            using (CronExpressionClass c_Exp = new CronExpressionClass(c_Ans))
                            {
                                //
                                c_Exp.Compute();
                                // Still valid?
                                if (c_Ans.NextOn < c_Ans.EndOn)
                                {
                                    // Save
                                    c_Ans.Save();
                                }
                            }
                        }
                    }
                }

                return c_Ans;
            }
        }

        public QueryClass OpenQuery()
        {
            return new QueryClass(this.Dataset.DataCollection);
        }
        #endregion

        #region Processor
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

                this.Parent.Parent.Parent.LogInfo("CRON process started");
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

                this.Parent.Parent.Parent.LogInfo("CRON process ended");
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

            // Loop
            while (c_Status.IsActive)
            {
                // Get th next entry
                CronEntryClass c_Entry = this.Next;

                // Do we have one?
                if (c_Entry != null)
                {
                    // Call
                    StoreClass c_Ret = this.Parent.Parent.Parent.FN(c_Entry.Fn, c_Entry.Data);
                    // Compute next
                    if (c_Entry.Pattern.HasValue())
                    {
                        // Use expression
                        using (CronExpressionClass c_Exp = new CronExpressionClass(c_Entry))
                        {
                            //
                            c_Exp.Compute();
                            // Dne?
                            if (c_Entry.NextOn >= c_Entry.EndOn)
                            {
                                // No more
                                c_Entry.Delete();
                            }
                            else
                            {
                                // Save the data
                                c_Entry.Data = c_Ret;
                                // Save
                                c_Entry.Save();
                            }
                        }
                    }
                    else
                    {
                        // One shot
                        c_Entry.Delete();
                    }
                }
                else
                {
                    // Wait a while
                    c_Status.WaitFor(20.SecondsAsTimeSpan());
                }
            }

            // Clean up
            c_Status.End();
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
        public void SignalChange(CronEntryClass entry, string code, string value)
        {
            // Available?
            if (this.Synch != null)
            {
                // Make message
                using (SIO.MessageClass c_Msg = new SIO.MessageClass(this.Synch, MessageClass.Modes.Internal, "$$cron.{0}".FormatString(code),
                                                                                    "id", entry.Name,
                                                                                    "value", value,
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
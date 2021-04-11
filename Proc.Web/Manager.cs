///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
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

namespace Proc.Web
{
    public class ManagerClass : ChildOfClass<EnvironmentClass>
    {
        #region Constructor
        /// <summary>
        /// 
        /// Constructor
        /// 
        /// </summary>
        /// <param name="env">The current environment</param>
        public ManagerClass(EnvironmentClass env)
            : base(env)
        {
            // Start cleanup
            this.CleanupThreadID = SafeThreadManagerClass.StartThread("".GUID(), new System.Threading.ParameterizedThreadStart(this.CleanupThread));
        }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            // Kill thread
            if (this.CleanupThreadID.HasValue())
            {
                SafeThreadManagerClass.StopThread(this.CleanupThreadID);
                this.CleanupThreadID = null;
            }

            base.Dispose();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The ID of the cleanup thread
        /// 
        /// </summary>
        private string CleanupThreadID { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Maks sure that items expire
        /// 
        /// </summary>
        /// <param name="status"></param>
        private void CleanupThread(object status)
        {
            SafeThreadStatusClass c_Status = status as SafeThreadStatusClass;

            // Wait a while
            c_Status.WaitFor(2.MinutesAsTimeSpan());

            // Loop
            while (c_Status.IsActive)
            {
                // Get the manager
                AO.ManagerClass c_Mgr = this.Parent.Globals.Get<AO.ManagerClass>();
                // Make a query
                using (AO.QueryClass c_Query = new QueryClass(c_Mgr.DefaultDatabase[AO.DatabaseClass.DatasetBitly].DataCollection))
                {
                    // Days old
                    int iDays = c_Mgr.DefaultDatabase.SiteInfo.BitlyDays;
                    if (iDays <= 0) iDays = 7;
                    // Set the filter
                    c_Query.Add("creon", QueryElementClass.QueryOps.Lt, DateTime.Now.AddDays(iDays-iDays).ToDBDate());
                    // Delete
                    c_Query.Delete(true);
                }

                // Once a day is fine
                c_Status.WaitFor(1.DaysAsTimeSpan());
            }
        }
        #endregion
    }
}
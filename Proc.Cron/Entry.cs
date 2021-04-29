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

using NX.Shared;
using NX.Engine;
using Proc.AO;

namespace Proc.Cron
{
    public class CronEntryClass : ObjectWrapperClass
    {
        #region Constructor
        public CronEntryClass(ObjectClass obj)
            : base(obj)
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The user name
        /// 
        /// </summary>
        public string Name
        {
            get { return this["_id"]; }
        }

        /// <summary>
        /// 
        /// The cron pattern
        /// 
        /// </summary>
        public string Pattern
        {
            get { return this["patt"]; }
            set { this["patt"] = value; }
        }

        /// <summary>
        /// 
        /// The next date and time to run
        /// 
        /// </summary>
        public DateTime NextOn
        {
            get { return this["next"].FromDBDate(); }
            set { this["next"] = value.ToDBDate(); }
        }

        /// <summary>
        /// 
        /// The mode to open child windows
        /// 
        /// </summary>
        public bool Enabled
        {
            get { return this["enabled"].FromDBBoolean(); }
            set { this["enabled"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The function
        /// 
        /// </summary>
        public string Fn
        {
            get { return this["fn"]; }
            set { this["fn"] = value; }
        }

        /// <summary>
        /// 
        /// The values used in the SIO callback
        /// 
        /// </summary>
        public string SIOValues
        {
            get { return this["siovalue"]; }
            set { this["siovalue"] = value; }
        }

        /// <summary>
        /// 
        /// The default pick height
        /// 
        /// </summary>
        public string SIOCode
        {
            get { return this["siocode"]; }
            set { this["siocode"] = value; }
        }
        #endregion

        #region Methods
        public void Run(ManagerClass mgr)
        {
            // Assume one shot
            bool bKill = !this.Fn.HasValue();

            // Do we kill?
            if(bKill)
            {
                this.Delete();
            }
            else
            {
                // Compute next
                using(ExpressionClass c_Expr = new ExpressionClass(this))
                {
                    c_Expr.Compute(mgr.DBManager.DefaultDatabase.SiteInfo.Timezone.GetTimezone(), 1);

                    // All done?
                    if(this.NextOn != DateTime.MaxValue)
                    {
                        // Put back
                        this.Save();

                        // And run
                        mgr.Parent.FN(this.Fn);
                    }
                    else
                    {
                        this.Delete();
                    }
                }
            }
        }
        #endregion
    }
}
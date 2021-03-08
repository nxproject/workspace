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

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public class CronEntryClass : ObjectWrapperClass
    {
        #region Constants
        internal const string KeyPattern = "_cronpatt";
        internal const string KeyStart = "_cronstart";
        internal const string KeyEnd = "_cronend";
        internal const string KeyNext = "_cronnext";
        internal const string KeyNextList = "_cronnextl";
        internal const string KeyFn = "_cronfn";
        internal const string KeyData = "_crondata";
        internal const string KeyTimezone = "_crontz";
        #endregion

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
        /// The cron patter
        /// 
        /// </summary>
        public string Pattern
        {
            get { return this[KeyPattern]; }
            set { this[KeyPattern] = value; }
        }

        /// <summary>
        /// 
        /// The date and time to start
        /// 
        /// </summary>
        public DateTime StartOn
        {
            get { return this[KeyStart].FromDBDate(); }
            set { this[KeyStart] = value.ToDBDate(); }
        }

        /// <summary>
        /// 
        /// The date and time to end
        /// 
        /// </summary>
        public DateTime EndOn
        {
            get { return this[KeyEnd].FromDBDate(); }
            set { this[KeyEnd] = value.ToDBDate(); }
        }

        /// <summary>
        /// 
        /// The next date and time to run
        /// 
        /// </summary>
        public DateTime NextOn
        {
            get { return this[KeyNext].FromDBDate(); }
            set { this[KeyNext] = value.ToDBDate(); }
        }

        /// <summary>
        /// 
        /// The list of when to run next
        /// 
        /// </summary>
        public string ScheduledOn
        {
            get { return this[KeyNextList]; }
            set { this[KeyNextList] = value; }
        }

        /// <summary>
        /// 
        /// The timezone
        /// 
        /// </summary>
        public string Timezone
        {
            get { return this[KeyTimezone]; }
            set { this[KeyTimezone] = value; }
        }

        /// <summary>
        /// 
        /// The function
        /// 
        /// </summary>
        public string Fn
        {
            get { return this[KeyFn]; }
            set { this[KeyFn] = value; }
        }

        /// <summary>
        /// 
        /// Passed data
        /// 
        /// </summary>
        public StoreClass Data
        {
            get { return new StoreClass( this[KeyData].ToJObject()); }
            set { this[KeyData] = value.ToString(); }
        }
        #endregion
    }
}
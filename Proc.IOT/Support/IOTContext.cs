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

namespace Proc.IOTIF
{
    public class IOTContextClass : ExtendedContextClass
    {
        #region Constructor
        public IOTContextClass(ManagerClass mgr,
                                StoreClass store)
            : base(mgr.Parent, store, null, "")
        {
            //
            this.Manager = mgr;
            this.Params = this.Stores[AO.Names.Passed];
            if (this.Params == null) this.Params = new StoreClass();
        }
        #endregion

        #region Indexer
        public new string this[string key]
        {
            get { return this.Params[key]; }
            set { this.Params[key] = value; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The manager
        /// 
        /// </summary>
        public ManagerClass Manager { get; internal set; }

        public StoreClass Params { get; private set; }

        //
        public string Code { get { return this.Params[IOT.Code.IOTCodeDefinitions.CodeCode].IfEmpty("objectid"); } }
        public string Option { get { return this.Params[IOT.Code.IOTCodeDefinitions.CodeOption]; } }
        public string Verb { get { return this.Params[IOT.Code.IOTCodeDefinitions.CmdVerb]; } }
        #endregion
    }
}
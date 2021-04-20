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

using NX.Engine;
using NX.Shared;

namespace Proc.Office
{
    public class AsToolClass : ChildOfClass<HTTPCallClass>
    {
        #region Constructor
        public AsToolClass(HTTPCallClass call, StoreClass store)
            : base(call)
        {
            //
            this.Passed = store;

            this.Data = new StoreClass(this.Passed["data"].ToJObject());

            this.Return = new StoreClass();
            this.ReturnMessageType = ReturnMessageTypes.OK;
        }
        #endregion

        #region Enums
        public enum ReturnMessageTypes
        {
            Info,
            Error,
            Warning,
            OK
        }
        #endregion

        #region Indexer
        public string this[string field]
        {
            get { return this.Data[field]; }
            set { this.Data[field] = value; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The passed store
        /// 
        /// </summary>
        private StoreClass Passed { get; set; }

        /// <summary>
        /// 
        /// The dataset
        /// 
        /// </summary>
        public string ObjDS { get { return this.Passed["ds"]; } }

        /// <summary>
        /// 
        /// The object ID
        /// 
        /// </summary>
        public string ObjID { get { return this.Passed["id"]; } }

        /// <summary>
        /// 
        /// The in-memory object when the tool is called
        /// 
        /// </summary>
        public StoreClass Data { get; private set; }

        /// <summary>
        /// 
        /// The return
        /// 
        /// </summary>
        public StoreClass Return { get; private set; }

        /// <summary>
        /// 
        /// The message to return
        /// 
        /// </summary>
        public string ReturnMessage
        {
            get { return this.Return["msg"]; }
            set { this.Return["msg"] = value; }
        }

        /// <summary>
        /// 
        /// The message type
        /// 
        /// </summary>
        public ReturnMessageTypes ReturnMessageType
        {
            get { return (ReturnMessageTypes)Enum.Parse(typeof(ReturnMessageTypes), this.Return["msgtype"], true); }
            set { this.Return["msgtype"] = value.ToString(); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Sets a error return
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void ReturnError(string msg)
        {
            this.ReturnMessageType = EntryPointNotFoundException.er;
            this.ReturnMessage = msg;
        }
        #endregion
    }
}
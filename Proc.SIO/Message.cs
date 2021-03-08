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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Proc.SocketIO;

namespace Proc.SIO
{
    public class MessageClass : ChildOfClass<SIO.ManagerClass>
    {
        #region Constructor
        public MessageClass(SIO.ManagerClass mgr, bool useacct, string fn, params string[] payload)
            : base(mgr)
        {
            // Make the message
            if (useacct)
            {
                this.Message = this.Parent.AccountEvent.New();
            }
            else
            {
                this.Message = this.Parent.InternalEvent.New();
            }

            // Fill payload
            for (int i = 0; i < payload.Length; i += 2)
            {
                // Save
                this[payload[i]] = payload[i + 1];
            }

            // Make payload
            this.Fn = fn;
            this.Message["uuid"] = "$$sys";
        }

        public MessageClass(SIO.ManagerClass mgr, SocketIO.MessageClass msg)
            : base(mgr)
        {
            // Save the message
            this.Message = msg;
            // And gt the payload
            this.Values = this.Message.GetJObject("message");
            if(this.Values == null)
            {
                this.Values = this.Message["message"].ToJObject();
            }
        }
        #endregion

        #region Indexer
        public string this[string key]
        {
            get { return this.Values.Get(key); }
            set { this.Values.Set(key, value); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The mesage object
        /// 
        /// </summary>
        public JObject Values { get; set; } = new JObject();

        /// <summary>
        /// 
        /// The underlying message
        /// 
        /// </summary>
        public Proc.SocketIO.MessageClass Message { get; private set; }

        /// <summary>
        /// 
        /// The function
        /// 
        /// </summary>
        public string Fn
        {
            get { return this.Message["fn"]; }
            set { this.Message["fn"] = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Adds a key/value pair
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            this[key] = value;
        }

        /// <summary>
        /// 
        /// Send the message
        /// 
        /// </summary>
        public void Send()
        {
            // Make the message
            this.Message["message"] = this.Values.ToSimpleString();
            // Send
            this.Message.Send();
        }

        /// <summary>
        /// 
        /// Sends message to specific browser
        /// 
        /// </summary>
        /// <param name="winid"></param>
        public void SendToUUID(string uuid)
        {
            this.Message["toUUID"] = uuid;
            this.Send();
        }

        /// <summary>
        /// 
        /// Sends message to specific window in a specific browser
        /// 
        /// </summary>
        /// <param name="winid"></param>
        public void SendToWinID(string uuid, string winid)
        {
            this.Message["toWinID"] = winid;
            this.SendToUUID(uuid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Message.ToString();
        }
        #endregion
    }
}
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
        public MessageClass(SIO.ManagerClass mgr, Modes mode, string fn, params string[] payload)
            : base(mgr)
        {
            //
            this.Mode = mode;

            // Fill payload
            for (int i = 0; i < payload.Length; i += 2)
            {
                // Save
                this[payload[i]] = payload[i + 1];
            }

            // Make payload
            this.Fn = fn;
        }

        public MessageClass(SIO.ManagerClass mgr, SocketIO.MessageClass msg)
            : base(mgr)
        {
            // Save the message
            this.Fn = msg["fn"];
            // And gt the payload
            this.Values = msg.GetJObject("message");
            if (this.Values == null)
            {
                this.Values = msg["message"].ToJObject();
            }
        }
        #endregion

        #region Enums
        public enum Modes
        {
            Internal,
            Account,
            Both
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
        /// The routing for the message
        /// 
        /// </summary>
        private Modes Mode { get; set; }

        /// <summary>
        /// 
        /// The function
        /// 
        /// </summary>
        public string Fn { get; private set; }

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
        //public Proc.SocketIO.MessageClass Message { get; private set; }
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
        public void Send(string uuid = null, string winid = null)
        {
            // Make the message
            if (this.Mode == Modes.Account || this.Mode == Modes.Both)
            {
                this.FillAndSend(this.Parent.AccountEvent.New(), uuid, winid);
            }

            if (this.Mode == Modes.Internal)
            {
                this.FillAndSend(this.Parent.InternalEvent.New(), uuid, winid);
            }
        }

        /// <summary>
        /// 
        /// Handles the processor
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="uuid"></param>
        /// <param name="winid"></param>
        private void FillAndSend(SocketIO.MessageClass msg, string uuid, string winid)
        {
            // Set the function
            msg["fn"] = this.Fn;
            // And who we are
            msg["uuid"] = "$$sys";
            // Optional
            if (uuid.HasValue()) msg["toUUID"] = uuid;
            if (winid.HasValue()) msg["toWinID"] = winid;
            // Make the message
            msg["message"] = this.Values.ToSimpleString();
            // Send
            msg.Send();
        }
        #endregion
    }
}
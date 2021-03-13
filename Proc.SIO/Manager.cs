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

using NX.Shared;
using NX.Engine;
using NX.Engine.SocketIO;

namespace Proc.SIO
{
    public class ManagerClass : NX.Engine.SocketIO.ManagerClass
    {
        #region Constructor
        public ManagerClass(EnvironmentClass env)
            : base(env)
        {
            //
            this.AvailabilityChanged += delegate (bool isavailable)
            {
                if (isavailable)
                {
                    this.InternalEvent = new EventClass(this, InternalCode);
                    this.InternalEvent.MessageReceived += delegate (NX.Engine.SocketIO.MessageClass msg)
                    {
                        if (this.Enabled)
                        {
                            SIO.MessageClass c_Msg = new SIO.MessageClass(this, msg);

                            this.MessageReceived?.Invoke(c_Msg);
                        }
                    };

                    //
                    this.AccountEvent = new EventClass(this, AccountCode);
                    this.AccountEvent.MessageReceived += delegate (NX.Engine.SocketIO.MessageClass msg)
                    {
                        if (this.Enabled)
                        {
                            SIO.MessageClass c_Msg = new SIO.MessageClass(this, msg);

                            this.MessageReceived?.Invoke(c_Msg);
                        }
                    };
                }
            };

            this.CheckForAvailability();
        }

        private void ManagerClass_AvailabilityChanged(bool isavailable)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The internal channel
        /// 
        /// </summary>
        public EventClass InternalEvent { get; private set; }

        /// <summary>
        /// 
        /// The account channel
        /// 
        /// </summary>
        public EventClass AccountEvent { get; private set; }

        /// <summary>
        /// 
        /// Can we send/receive messages?
        /// 
        /// </summary>
        public bool Enabled { get; set; } = true;
        #endregion

        #region Events
        /// <summary>
        /// 
        /// The delegate for the AvailabilityChanged event
        /// 
        /// </summary>
        /// <param name="msg">The message</param>
        public delegate void OnReceivedHandler(SIO.MessageClass msg);

        /// <summary>
        /// 
        /// Defines the event to be raised when a message is received
        /// 
        /// </summary>
        public event OnReceivedHandler MessageReceived;
        #endregion

        #region Codes
        /// <summary>
        /// 
        /// The internal code (MD5 of hive name)
        /// 
        /// </summary>
        public static string InternalCode { get; set; }

        /// <summary>
        /// 
        /// The account code. (MD5 of internal code)
        /// </summary>
        public static string AccountCode { get; set; }
        #endregion
    }
}
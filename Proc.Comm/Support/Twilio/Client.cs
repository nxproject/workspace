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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Comm.TwilioIF
{
    public class ClientClass : ChildOfClass<AO.ExtendedContextClass>
    {
        #region Constructor
        public ClientClass(AO.ExtendedContextClass ctx)
            : base(ctx)
        {
            //
            string sAcctSID = this.Parent.SiteInfo.TwilioAccount;
            string sAuthToken = this.Parent.SiteInfo.TwilioAccount;

            //
            TwilioClient.Init(sAcctSID, sAuthToken);
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Sends an MMS
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="msg"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public string SendSMS(string to, string from, string msg, List<DocumentClass> attachments)
        {
            string sAns = null;

            try
            {
                CreateMessageOptions c_Opts = new CreateMessageOptions(new PhoneNumber(to.PhoneNumberAse164()));

                c_Opts.From = new PhoneNumber(from.PhoneNumberAse164());

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (DocumentClass c_File in attachments)
                    {
                        if (msg.HasValue()) msg += "\r\n" + "\r\n";
                        msg += c_File.Name + " " + c_File.URL;
                    }
                }

                c_Opts.Body = msg;

                MessageResource c_Resp = MessageResource.Create(c_Opts);

                if (c_Resp != null) sAns = c_Resp.ErrorMessage;
            }
            catch (Exception e)
            {
                sAns = e.GetAllExceptions();
            }

            return sAns;
        }

        /// <summary>
        /// 
        /// Makes a voice call
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public string MakeCall(string to, string from, string via)
        {
            string sAns = null;

            try
            {
                // Create a TwiML voice response

                VoiceResponse c_TwiML = new VoiceResponse();
                c_TwiML.Say("Connecting, please wait");
                c_TwiML.Dial(to.PhoneNumberAse164());

                CallResource c_Resp = CallResource.Create(
                    twiml: new Twiml(c_TwiML.ToString()),
                    to: new PhoneNumber(from.PhoneNumberAse164()),
                    from: new PhoneNumber(via.PhoneNumberAse164())
                    );
            }
            catch (Exception e)
            {
                sAns = e.GetAllExceptions();
            }

            return sAns;
        }
        #endregion
    }
}
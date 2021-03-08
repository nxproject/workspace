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
using System.Text;

using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Comm
{
    public class eMessageClass : ChildOfClass<AO.ExtendedContextClass>
    {
        #region Constants
        private const string KeySubj = "subj";
        private const string KeyMsg = "msg";
        private const string KeyTo = "to";
        private const string KeyAtt = "att";
        private const string KeyCO = "co";
        #endregion

        #region Constructor
        public eMessageClass(AO.ExtendedContextClass ctx)
            : base(ctx)
        {
            this.Values = new JObject();
        }

        public eMessageClass(AO.ExtendedContextClass ctx, string value)
            : base(ctx)
        {
            this.Values = value.ToJObject();
            if (this.Values == null) this.Values = new JObject();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The working values
        /// 
        /// </summary>
        internal JObject Values { get; set; }

        /// <summary>
        /// 
        /// Subject text for message
        /// 
        /// </summary>
        public string Subject { get { return this.Values.Get(KeySubj); } set { this.Values.Set(KeySubj, value); } }

        /// <summary>
        /// 
        /// The message body
        /// 
        /// </summary>
        public string Message { get { return this.Values.Get(KeyMsg); } set { this.Values.Set(KeyMsg, value); } }

        /// <summary>
        /// 
        /// The care/of
        /// 
        /// </summary>
        public string CO { get { return this.Values.Get(KeyCO); } set { this.Values.Set(KeyCO, value); } }

        /// <summary>
        /// 
        /// Recipients
        /// 
        /// </summary>
        private eAddressesClass ITo { get; set; }
        public eAddressesClass To
        {
            get
            {
                if (this.ITo == null)
                {
                    this.ITo = new eAddressesClass(this, KeyTo);
                }

                return this.ITo;
            }
        }

        /// <summary>
        /// 
        /// Attachments
        /// 
        /// </summary>
        private eAttachmentListClass IAttachments { get; set; }
        public eAttachmentListClass Attachments
        {
            get
            {
                if (this.IAttachments == null)
                {
                    this.IAttachments = new eAttachmentListClass(this, KeyAtt);
                }

                return this.IAttachments;
            }
        }

        // Callbacks
        public Action<eAddressClass, eReturnClass> UserCB { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return this.Values.ToSimpleString();
        }

        public eReturnClass Send()
        {
            eReturnClass c_Ans = new eReturnClass();

            foreach (eAddressClass sTo in this.To.Users.Values)
            {
                if (this.UserCB != null)
                {
                    this.UserCB(sTo, c_Ans);
                }
                else
                {
                    this.SendQM(sTo, c_Ans);
                }
            }

            foreach (eAddressClass sTo in this.To.EMail.Values)
            {
                this.SendEMail(sTo, c_Ans);
            }

            foreach (eAddressClass sTo in this.To.SMS.Values)
            {
                this.SendSMS(sTo, c_Ans);
            }

            foreach (eAddressClass sTo in this.To.Voice.Values)
            {
                this.MakeCall(sTo, c_Ans);
            }

            foreach (eAddressClass sTo in this.To.FedEx.Values)
            {
                this.SendFedEx(sTo, c_Ans);
            }

            return c_Ans;
        }

        private void SendQM(eAddressClass to, eReturnClass result)
        {
            this.Parent.SendQM(to.To, this.Message, this.Attachments.Paths);

            result.Log(to);
        }

        private void SendEMail(eAddressClass to, eReturnClass result)
        {
            try
            {
                string sEMailLogin = this.Parent.User.EMailName;
                string sEMailPwd = this.Parent.User.EMailPassword;
                string sEMailProvider = this.Parent.User.EMailProvider;
                string sFriendly = this.Parent.User.Displayable;
                string sEMailFooter = this.Parent.User.CommFooter;

                //Validate
                if (sEMailLogin.HasValue() && sEMailPwd.HasValue())
                {
                    string sFinal = this.Message.TextToHTML();

                    using (EMailIF.EngineClass c_Client = new EMailIF.EngineClass(sFriendly,
                                                                                    sEMailLogin,
                                                                                    sEMailPwd,
                                                                                    sEMailProvider))
                    {
                        string sResp = c_Client.SendHTML(this.Parent, to.To,
                                        this.Subject,
                                        sFinal,
                                        sEMailFooter,
                                        this.Attachments.Documents,
                                        false
                                        );

                        if (sResp.HasValue())
                        {
                            result.Log(to, sResp);
                        }
                        else
                        {
                            result.Log(to);
                        }
                    }
                }
                else
                {
                    result.Log(to, "EMail has not been setup");
                }
            }
            catch (Exception e)
            {
                result.Log(to, "Error while sending email", e);
            }
        }

        private void SendSMS(eAddressClass to, eReturnClass result)
        {
            //
            using (TwilioIF.ClientClass c_Client = new TwilioIF.ClientClass(this.Parent))
            {
                string sFriendly = this.Parent.User.Displayable;
                string sUserPhone = this.Parent.User.TwilioPhone;

                //
                string sResp = c_Client.SendSMS(
                                        to.To,
                                        sUserPhone,
                                        this.Message,
                                        this.Attachments.Documents
                                        );

                if (sResp.HasValue())
                {
                    result.Log(to, sResp);
                }
                else
                {
                    result.Log(to);
                }
            }
        }

        private void MakeCall(eAddressClass to, eReturnClass result)
        {
            //
            using (TwilioIF.ClientClass c_Client = new TwilioIF.ClientClass(this.Parent))
            {
                //
                string sResp = c_Client.MakeCall(
                                        to.To,
                                        this.Parent.User.Phone,
                                        this.Parent.User.TwilioPhone
                                        );

                if (sResp.HasValue())
                {
                    result.Log(to, sResp);
                }
                else
                {
                    result.Log(to);
                }
            }
        }

        private void SendFedEx(eAddressClass to, eReturnClass result)
        {
            this.SendEMail(new eAddressClass("printandgo@fedex.com", eAddressClass.AddressTypes.EMail), result);
        }

        public void SendViaSendGrid(eAddressClass to, eReturnClass result, string html, AO.SiteInfoClass si)
        {
            SendGridClient c_Client = new SendGridClient(si.SendGridAPI);

            var from = new EmailAddress(si.SendGridEmail, si.SendGridFriendlyName);
            EmailAddress c_To = new EmailAddress(to.To);
            var msg = MailHelper.CreateSingleEmail(from, c_To, this.Subject, "", html);
            Response c_Resp = c_Client.SendEmailAsync(msg).Result;

            result.Log(to, c_Resp.IsSuccessStatusCode ? "" : "Error while sending via SendGrid: {0}".FormatString(c_Resp.StatusCode));
        }
        #endregion
    }
}
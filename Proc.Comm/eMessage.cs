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

namespace Proc.Communication
{
    public class eMessageClass : ChildOfClass<AO.ExtendedContextClass>
    {
        #region Constants
        private const string KeySubj = "subject";
        private const string KeyMsg = "message";
        private const string KeyPost = "post";
        private const string KeyFooter = "footer";
        private const string KeySite = "site";
        private const string KeyURL = "url";
        #endregion

        #region Constructor
        public eMessageClass(AO.ExtendedContextClass ctx)
            : base(ctx)
        {
            this.Values = new HandlebarDataClass();

            this.Initialize();
        }

        public eMessageClass(AO.ExtendedContextClass ctx, string value)
            : base(ctx)
        {
            this.Values = new HandlebarDataClass();
            this.Values.Merge(value.ToJObject());

            this.Initialize();
        }

        private void Initialize()
        {
            // 
            this.Subject = "A message for you";
            this.Message = "";
            this.Post = "";
            this.Site = this.Parent.Database.SiteInfo.Name;
            this.URL = this.Parent.Parent.LoopbackURL;
            this.Footer = this.Parent.User.CommFooter;
            
        }
        #endregion

        #region Indexer
        public string this[string key]
        {
            get { return this.Values.GetAsString(key); }
            set { this.Values.Set(key, value); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The working values
        /// 
        /// </summary>
        internal HandlebarDataClass Values { get; set; }

        /// <summary>
        /// 
        /// Subject text for message
        /// 
        /// </summary>
        public string Subject { get { return this.Values.GetAsString(KeySubj); } set { this.Values.Set(KeySubj, value); } }

        /// <summary>
        /// 
        /// The message body
        /// 
        /// </summary>
        public string Message { get { return this.Values.GetAsString(KeyMsg); } set { this.Values.Set(KeyMsg, value); } }

        /// <summary>
        /// 
        /// The post text
        /// 
        /// </summary>
        public string Post { get { return this.Values.GetAsString(KeyPost); } set { this.Values.Set(KeyPost, value); } }

        /// <summary>
        /// 
        /// The message footer
        /// 
        /// </summary>
        public string Footer { get { return this.Values.GetAsString(KeyFooter); } set { this.Values.Set(KeyFooter, value); } }

        /// <summary>
        /// 
        /// The site name
        /// 
        /// </summary>
        public string Site { get { return this.Values.GetAsString(KeySite); } set { this.Values.Set(KeySite, value); } }

        /// <summary>
        /// 
        /// The site URL
        /// 
        /// </summary>
        public string URL { get { return this.Values.GetAsString(KeyURL); } set { this.Values.Set(KeyURL, value); } }

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
                    this.ITo = new eAddressesClass(this);
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
                    this.IAttachments = new eAttachmentListClass(this);
                }

                return this.IAttachments;
            }
        }

        /// <summary>
        /// 
        /// Actions
        /// 
        /// </summary>
        private eActiontListClass IActions { get; set; }
        public eActiontListClass Actions
        {
            get
            {
                if (this.IActions == null)
                {
                    this.IActions = new eActiontListClass(this);
                }

                return this.IActions;
            }
        }

        /// <summary>
        /// 
        /// The data object
        /// 
        /// </summary>
        public AO.ObjectClass Object { get; set; }

        /// <summary>
        /// 
        /// The template to use for emails
        /// 
        /// </summary>
        public string EMailTemplate { get; set; }

        private string IEMailHTML { get; set; }
        public string EMailHTML
        {
            get
            {
                if(this.IEMailHTML == null)
                {
                    this.IEMailHTML = this.FormatEMail(this.EMailTemplate);
                }

                return this.IEMailHTML;
            }
        }

        /// <summary>
        /// 
        /// The template to use for SMS
        /// 
        /// </summary>
        public string SMSTemplate { get; set; }

        private string ISMSHTML { get; set; }
        public string SMSHTML
        {
            get
            {
                if (this.ISMSHTML == null)
                {
                    this.ISMSHTML = this.FormatSMS(this.SMSTemplate);
                }

                return this.ISMSHTML;
            }
        }

        // Callbacks
        public Action<eAddressClass, eReturnClass> UserCB { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Converts message to string
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Values.ToString();
        }

        /// <summary>
        /// 
        /// Generic send
        /// 
        /// </summary>
        /// <returns></returns>
        public eReturnClass Send()
        {
            eReturnClass c_Ans = new eReturnClass();

            // Rset messages
            this.IEMailHTML = null;
            this.ISMSHTML = null;

            foreach (eAddressClass sTo in this.To.Users.Values)
            {
                if (this.UserCB != null)
                {
                    this.UserCB(sTo, c_Ans);
                }
                else
                {
                    this.SendNotification(sTo, c_Ans);
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

        /// <summary>
        /// 
        /// Sends a notification
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void SendNotification(eAddressClass to, eReturnClass result)
        {
            List<string> c_Pieces = new List<string>();

            if (this.Subject.HasValue()) c_Pieces.Add(this.Subject);
            if (this.Message.HasValue()) c_Pieces.Add(this.Message);

            this.Parent.SendNotification(to.To,c_Pieces.Join(" - "), this.Attachments.AsDocuments);

            result.Log(to);
        }

        /// <summary>
        /// 
        /// Sends an email
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void SendEMail(eAddressClass to, eReturnClass result)
        {
            try
            {
                string sEMailLogin = this.Parent.User.EMailName;
                string sEMailPwd = this.Parent.User.EMailPassword;
                string sEMailProvider = this.Parent.User.EMailProvider;
                string sFriendly = this.Parent.User.Displayable;

                // Format
                string sHTML = this.EMailHTML;

                //Validate
                if (sEMailLogin.HasValue() && sEMailPwd.HasValue())
                {
                    using (EMailIF.EngineClass c_Client = new EMailIF.EngineClass(sFriendly,
                                                                                    sEMailLogin,
                                                                                    sEMailPwd,
                                                                                    sEMailProvider))
                    {
                        string sResp = c_Client.SendHTML(this.Parent, to.To,
                                        this.Subject,
                                        sHTML
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
                    if (!this.SendViaSendGrid(to, result, sHTML))
                    {
                        result.Log(to, "EMail has not been setup");
                    }
                    else
                    {
                        result.Log(to);
                    }
                }
            }
            catch (Exception e)
            {
                result.Log(to, "Error while sending email", e);
            }
        }

        /// <summary>
        /// 
        /// Sends an SMS
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void SendSMS(eAddressClass to, eReturnClass result)
        {
            //
            using (Communication.TwilioClientClass c_Client = new Communication.TwilioClientClass(this.Parent))
            {
                string sFriendly = this.Parent.User.Displayable;
                string sUserPhone = this.Parent.User.TwilioPhone;

                // Format
                string sHTML = this.SMSHTML;

                //
                string sResp = c_Client.SendSMS(
                                        to.To,
                                        sUserPhone,
                                        sHTML
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

        /// <summary>
        /// 
        /// Makes a phone call
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void MakeCall(eAddressClass to, eReturnClass result)
        {
            //
            using (Communication.TwilioClientClass c_Client = new Communication.TwilioClientClass(this.Parent))
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

        /// <summary>
        /// 
        /// Send FedEx print request
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void SendFedEx(eAddressClass to, eReturnClass result)
        {
            this.SendEMail(new eAddressClass("printandgo@fedex.com", eAddressClass.AddressTypes.EMail), result);
        }

        /// <summary>
        /// 
        /// Sends an email via SendGrid
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        private bool SendViaSendGrid(eAddressClass to, eReturnClass result, string html)
        {
            // Assume failure
            bool bAns = false;

            // Get site info
            AO.SiteInfoClass c_SI = this.Parent.Database.SiteInfo;

            // Must be setup
            if (c_SI.SendGridAPI.HasValue())
            {
                SendGridClient c_Client = new SendGridClient(c_SI.SendGridAPI);

                var from = new EmailAddress(c_SI.SendGridEmail, c_SI.SendGridFriendlyName.IfEmpty(c_SI.Name));
                EmailAddress c_To = new EmailAddress(to.To);
                var msg = MailHelper.CreateSingleEmail(from, c_To, this.Subject, "", html);
                Response c_Resp = c_Client.SendEmailAsync(msg).Result;

                //
                bAns = c_Resp.IsSuccessStatusCode;

                result.Log(to, bAns ? "" : "Error while sending via SendGrid: {0}".FormatString(c_Resp.StatusCode));
            }

            return bAns;
        }
        #endregion

        #region Formatters
        /// <summary>
        /// 
        /// Formats the message in HTML format
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private string FormatEMail(string template)
        {
            this.Parent.Parent.Debug();

            // Start with nothing
            string sTemplate = null;
            // Did we get a template name?
            if(template.HasValue())
            {
                // Fetch
                using(AO.ObjectClass c_Template = this.Parent.DBManager.DefaultDatabase[AO.DatabaseClass.DatasetEMailTemplates][template])
                {
                    // Get the text
                    string sText = c_Template["text"];
                    // Any?
                    if(sText.HasValue())
                    {
                        // Format
                        sTemplate = this.GetResource("EMailHolder.html").FromBytes().Replace("{0}", sText);
                    }
                }
            }

            // Handle missing
            if (!template.HasValue())
            {
                sTemplate = this.GetResource("EMailTemplate.html").FromBytes();
            }

            // Build attachment list
            if (this.Attachments != null && this.Attachments.Count > 0)
            {
                JArray c_Attachments = new JArray();
                
                // Loop thru
                foreach (eAttachmenClass c_File in this.Attachments.Documents)
                {
                    //
                    JObject c_Entry = new JObject();

                    c_Entry.Set("color", "#3498db");
                    c_Entry.Set("href", c_File.Document.URL);
                    c_Entry.Set("caption", c_File.Label.IfEmpty(c_File.Document.Name).ToUpper());

                    c_Attachments.Add(c_Entry);
                }

                this.Values.Set("attachments", c_Attachments);
            }

            // Build actions list
            if(this.Actions != null && this.Actions.Count > 0)
            {
                JArray c_Actions = new JArray();

                // Loop thru
                foreach (eActionClass c_Action in this.Actions.Actions)
                {
                    //
                    JObject c_Entry = new JObject();

                    c_Entry.Set("color", c_Action.Color);
                    c_Entry.Set("href", c_Action.URL);
                    c_Entry.Set("caption", c_Action.Caption.ToUpper());

                    c_Actions.Add(c_Entry);
                }

                this.Values.Set("actions", c_Actions);
            }

            // Predined
            this.Values.Set("sys", this.Parent.Database.SiteInfo.AsJObject);
            this.Values.Set("user", this.Parent.User.SynchObject);

            // Data
            if(this.Object != null)
            {
                // Get
                JObject c_Data = this.Object.AsJObject;
                // Get linked
                this.FillLinks(c_Data);

                // Save
                this.Values.Set("data", c_Data);
            }

            return sTemplate.Handlebars(this.Values, delegate (string value, object thisvalue)
            {
                // Save this
                this.Values.Set("this", thisvalue);

                // Eval
                using (Context c_Ctx = new Context(this.Parent.Parent, vars: this.Values))
                {
                    return Expression.Eval(c_Ctx, value).Value;
                }
            });
        }

        /// <summary>
        /// 
        /// Formats the message in HTML format
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private string FormatSMS(string template)
        {
            string sTemplate = template;
            if (!sTemplate.HasValue()) sTemplate = this.GetResource("SMSTemplate.html").FromBytes();

            return this.FormatEMail(sTemplate);
        }

        /// <summary>
        /// 
        /// Build data tree
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void FillLinks(JObject obj)
        {
            // Loop thru
            foreach(string sKey in obj.Keys())
            {
                // Get the value
                string sValue = obj.Get(sKey);
                // Is it a link?
                if(AO.UUIDClass.IsValid(sValue))
                {
                    // Make UUID
                    using (AO.UUIDClass c_UUID = new AO.UUIDClass(this.Parent.Database, sValue))
                    {
                        // 
                        using (AO.ObjectClass c_Child = c_UUID.AsObject)
                        {
                            // Build
                            JObject c_CData = c_Child.AsJObject;
                            // Fill
                            this.FillLinks(c_CData);
                            // Get
                            obj.Set(sKey, c_CData);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
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
using Proc.Telemetry;
using Proc.Web;

namespace Proc.Communication
{
    public class eMessageClass : ChildOfClass<AO.ExtendedContextClass>
    {
        #region Constants
        private const string KeySubj = "subject";
        private const string KeyMsg = "message";
        private const string KeyPost = "post";
        private const string KeyFooter = "footer";

        private const string KeyTo = "to";
        private const string KeyTelemetry = "telemetry";
        private const string KeyCampaign = "campaign";
        private const string KeyEMailTemplate = "template";
        private const string KeySMSTemplate = "smstemplate";
        private const string KeyCommand = "cmd";
        private const string KeyUser = "user";
        private const string KeyAttachments = "att";
        private const string KeyMessageLink = "mlink";
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
        /// The message ID
        /// 
        /// </summary>
        //private string ID { get; set; }

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
        /// The pre message link text
        /// 
        /// </summary>
        public string MessageLink { get { return this.Values.GetAsString(KeyMessageLink); } set { this.Values.Set(KeyMessageLink, value); } }

        /// <summary>
        /// 
        /// Is telemetry allowed?
        /// 
        /// </summary>
        public bool Telemetry { get { return this.Values.GetAsString(KeyTelemetry).FromDBBoolean(); } set { this.Values.Set(KeyTelemetry, value.ToDBBoolean()); } }

        /// <summary>
        /// 
        /// The user
        /// 
        /// </summary>
        public string User { get { return this.Values.GetAsString(KeyUser); } set { this.Values.Set(KeyUser, value); } }

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
        public string EMailTemplate { get { return this.Values.GetAsString(KeyEMailTemplate); } set { this.Values.Set(KeyEMailTemplate, value); } }

        /// <summary>
        /// 
        /// The template to use for SMS
        /// 
        /// </summary>
        public string SMSTemplate { get { return this.Values.GetAsString(KeySMSTemplate); } set { this.Values.Set(KeySMSTemplate, value); } }

        /// <summary>
        /// 
        /// The command
        /// 
        /// </summary>
        public string Command { get { return this.Values.GetAsString(KeyCommand).IfEmpty("email"); } set { this.Values.Set(KeyCommand, value); } }

        /// <summary>
        /// 
        /// The campaign
        /// 
        /// </summary>
        public string Campaign { get { return this.Values.GetAsString(KeyCampaign); } set { this.Values.Set(KeyCampaign, value); } }

        // Callbacks
        public Action<eAddressClass, bool> UserCB { get; set; }
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
        /// Returns a telemetry object
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="via"></param>
        /// <returns></returns>
        private Telemetry.DataClass GetTelemetry(string template, string via)
        {
            //
            return new Telemetry.DataClass(this.Parent.DBManager.DefaultDatabase, template, via, this.Campaign);
        }

        /// <summary>
        /// 
        /// Handles notification of result to user
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void HandleNotify(eAddressClass.AddressTypes type, eReturnClass result)
        {
            if (result.ToString().HasValue())
            {
                string sQM = "Result of {0} request: {1}".FormatString(type, result.ToString());
                this.Parent.SendNotification(this.Parent.User.Name, sQM, null);
            }
        }

        /// <summary>
        /// 
        /// Returns a save value
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToSave()
        {
            // Clone the values
            JObject c_Wkg = this.Values.ToString().ToJObject();

            // Fix
            c_Wkg.Set(KeyTo, c_Wkg.Get(KeyTo).SplitSpaces().ToJArray());
            c_Wkg.Set(KeyAttachments, c_Wkg.Get(KeyAttachments).SplitSpaces().ToJArray());

            return c_Wkg.ToSimpleString().Compress();
        }
        #endregion

        #region Senders
        /// <summary>
        /// 
        /// Generic send
        /// 
        /// </summary>
        /// <returns></returns>
        public void Send(bool notify)
        {
            //
            List<string> c_To = new List<string>();

            // Loop thru
            foreach (eAddressClass sTo in this.To.Users.Values)
            {
                c_To.Add(sTo.ToString());
            }

            foreach (eAddressClass sTo in this.To.EMail.Values)
            {
                c_To.Add(sTo.ToString());
            }

            foreach (eAddressClass sTo in this.To.SMS.Values)
            {
                c_To.Add(sTo.ToString());
            }

            foreach (eAddressClass sTo in this.To.Voice.Values)
            {
                c_To.Add(sTo.ToString());
            }

            foreach (eAddressClass sTo in this.To.FedEx.Values)
            {
                c_To.Add(sTo.ToString());
            }

            // Save the to list
            this.Values.Set(KeyTo, c_To.JoinSpaces());

            // Attachments
            List<string> c_Att = new List<string>();

            // Build attachment list
            if (this.Attachments != null && this.Attachments.Count > 0)
            {
                // Loop thru
                foreach (eAttachmenClass c_File in this.Attachments.Documents)
                {
                    // Save
                    c_Att.Add(c_File.Document.Path);
                }
            }

            this.Values.Set(KeyAttachments, c_Att.JoinSpaces());

            // Loop thru
            foreach (eAddressClass sTo in this.To.Users.Values)
            {
                if (this.UserCB != null)
                {
                    this.UserCB(sTo, notify);
                }
                else
                {
                    this.SendNotification(sTo);
                }
            }

            foreach (eAddressClass sTo in this.To.EMail.Values)
            {
                this.SendEMail(sTo, notify);
            }

            foreach (eAddressClass sTo in this.To.SMS.Values)
            {
                this.SendSMS(sTo, notify);
            }

            foreach (eAddressClass sTo in this.To.Voice.Values)
            {
                this.MakeCall(sTo, notify);
            }

            foreach (eAddressClass sTo in this.To.FedEx.Values)
            {
                this.SendFedEx(sTo, notify);
            }
        }

        /// <summary>
        /// 
        /// Sends a notification
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void SendNotification(eAddressClass to)
        {
            List<string> c_Pieces = new List<string>();

            if (this.Subject.HasValue()) c_Pieces.Add(this.Subject);
            if (this.Message.HasValue()) c_Pieces.Add(this.Message);

            this.Parent.SendNotification(to.To, c_Pieces.Join(" - "), this.Attachments.AsDocuments);
        }

        /// <summary>
        /// 
        /// Sends an email
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void SendEMail(eAddressClass to, bool notify)
        {
            eReturnClass c_Result = new eReturnClass();

            try
            {
                string sEMailLogin = this.Parent.User.EMailName;
                string sEMailPwd = this.Parent.User.EMailPassword;
                string sEMailProvider = this.Parent.User.EMailProvider;
                string sFriendly = this.Parent.User.Displayable;

                // Format
                string sHTML = this.FormatMessage(to.To, this.EMailTemplate, "EMailHolder.html", "EMail", false);

                this.Parent.Parent.LogInfo("EMAIL\r\n" + sHTML + "\r\n");

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
                            c_Result.Log(to, sResp);
                        }
                        else
                        {
                            c_Result.Log(to);
                        }
                    }
                }
                else
                {
                    if (!this.SendViaSendGrid(to, sHTML, notify))
                    {
                        c_Result.Log(to, "EMail has not been setup");
                    }
                    else
                    {
                        c_Result.Log(to);

                        // Update contact out
                        this.UpdateLstOut(this.GetTelemetry(this.EMailTemplate, "EMail"), to.To);
                    }

                    // Only once
                    notify = false;
                }
            }
            catch (Exception e)
            {
                c_Result.Log(to, "Error while sending email", e);
            }

            if (notify) this.HandleNotify(eAddressClass.AddressTypes.EMail, c_Result);
        }

        /// <summary>
        /// 
        /// Sends an SMS
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void SendSMS(eAddressClass to, bool notify)
        {
            eReturnClass c_Result = new eReturnClass();

            //
            using (Communication.TwilioClientClass c_Client = new Communication.TwilioClientClass(this.Parent))
            {
                string sFriendly = this.Parent.User.Displayable;
                string sUserPhone = this.Parent.User.TwilioPhone;

                // Format
                string sHTML = this.FormatMessage(to.To, this.SMSTemplate, "SMSTemplate.html", "SMS", true);

                // Telemetry?
                if(this.Telemetry)
                {
                    // Get
                    Proc.Telemetry.DataClass c_Tele = this.GetTelemetry("SMS", "SMS");
                    // Make URL
                    string sURL = this.Parent.Parent.ReachableURL.CombinePath("zm", this.ToSave(), to.To.Compress());
                    // Bitly
                    sURL = sURL.Bitly(this.Parent.Parent);
                    // Add
                    sHTML += "\r\n\r\n{0}: ".FormatString(this.MessageLink.IfEmpty("To see the message")) + sURL;
                }

                //
                string sResp = c_Client.SendSMS(
                                        to.To,
                                        sUserPhone,
                                        sHTML
                                        );

                if (sResp.HasValue())
                {
                    c_Result.Log(to, sResp);
                }
                else
                {
                    c_Result.Log(to);

                    // Update contact out
                    this.UpdateLstOut(this.GetTelemetry(this.SMSTemplate, "SMS"), to.To);
                }
            }

            if (notify) this.HandleNotify(eAddressClass.AddressTypes.SMS, c_Result);
        }

        /// <summary>
        /// 
        /// Makes a phone call
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void MakeCall(eAddressClass to, bool notify)
        {
            eReturnClass c_Result = new eReturnClass();

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
                    c_Result.Log(to, sResp);
                }
                else
                {
                    c_Result.Log(to);

                    // Update contact out
                    this.UpdateLstOut(this.GetTelemetry("", "Voice"), to.To);
                }
            }

            if (notify) this.HandleNotify(eAddressClass.AddressTypes.Voice, c_Result);
        }

        /// <summary>
        /// 
        /// Send FedEx print request
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        private void SendFedEx(eAddressClass to, bool notify)
        {
            this.SendEMail(new eAddressClass("printandgo@fedex.com", eAddressClass.AddressTypes.EMail), notify);
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
        private bool SendViaSendGrid(eAddressClass to, string html, bool notify)
        {
            // Assume failure
            bool bAns = false;

            eReturnClass c_Result = new eReturnClass();

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

                c_Result.Log(to, bAns ? "" : "Error while sending via SendGrid: {0}".FormatString(c_Resp.StatusCode));
            }

            if (notify) this.HandleNotify(eAddressClass.AddressTypes.EMail, c_Result);

            return bAns;
        }

        /// <summary>
        /// 
        /// Updates the account last out info
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="via"></param>
        private void UpdateLstOut(Telemetry.DataClass data, string user)
        {
            // Get the manage
            AO.ManagerClass c_Mgr = this.Parent.Parent.Globals.Get<AO.ManagerClass>();
            // And update
            using (AO.AccessClass c_AE = new AO.AccessClass(c_Mgr, user))
            {
                c_AE.UpdateContactOut(data.Template.IfEmpty(), data.Via, data.Campaign.IfEmpty());
            }

            // Change the via
            if (!data.Via.EndsWith(" Sent")) data.Via = data.Via + " Sent";

            // Set the to
            data.AddTransaction(user, false);
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
        public string FormatMessage(string to, string template, string defaulttemplate, string via, bool usebitly, bool webformat = false)
        {
            // Setup for telemetry
            bool bTelemetry = this.Telemetry && this.Parent.SiteInfo.TelemetryEnabled;
            Telemetry.DataClass c_Tele = this.GetTelemetry(template, via);

            // Start with nothing
            string sTemplate = null;
            // Did we get a template name?
            if (template.HasValue())
            {
                // Fetch
                using (AO.ObjectClass c_Template = this.Parent.DBManager.DefaultDatabase[AO.DatabaseClass.DatasetEMailTemplates][template])
                {
                    // Get the text
                    sTemplate = c_Template["text"];
                    // Full?
                    if (webformat && sTemplate.HasValue() && !sTemplate.StartsWith("<!DOCTYPE html>"))
                    {
                        // Get holder
                        string sHolder = this.GetResource("EMailHolder.html").FromBytes();
                        // Fill
                        sTemplate = sHolder.Replace("{message}", sTemplate);
                    }
                }
            }

            // Handle missing
            if (!sTemplate.HasValue())
            {
                sTemplate = this.GetResource(defaulttemplate).FromBytes();
            }

            // Build attachment list
            if (this.Attachments != null && this.Attachments.Count > 0)
            {
                JArray c_Attachments = new JArray();

                // Loop thru
                foreach (eAttachmenClass c_File in this.Attachments.Documents)
                {
                    // Get the URL
                    string sURL = c_File.Document.URL;

                    // Do we do telemetry?
                    if (bTelemetry)
                    {
                        // Convert
                        sURL = c_Tele.AddTelemetry("zd", sURL, usebitly, to);
                    }

                    //
                    JObject c_Entry = new JObject();

                    c_Entry.Set("color", "#3498db");
                    c_Entry.Set("href", sURL);
                    c_Entry.Set("caption", c_File.Label.IfEmpty(c_File.Document.Name).ToUpper());

                    c_Attachments.Add(c_Entry);
                }

                this.Values.Set("attachments", c_Attachments);
            }

            // Build actions list
            if (this.Actions != null && this.Actions.Count > 0)
            {
                JArray c_Actions = new JArray();

                // Loop thru
                foreach (eActionClass c_Action in this.Actions.Actions)
                {
                    // Get the URL
                    string sURL = c_Action.URL;

                    // Do we do telemetry?
                    if (bTelemetry)
                    {
                        // Convert
                        sURL = c_Tele.AddTelemetry("zr", sURL, usebitly, to);
                    }

                    //
                    JObject c_Entry = new JObject();

                    c_Entry.Set("color", c_Action.Color);
                    c_Entry.Set("href", sURL);
                    c_Entry.Set("caption", c_Action.Caption.ToUpper());

                    c_Actions.Add(c_Entry);
                }

                this.Values.Set("actions", c_Actions);
            }

            // Predined
            this.Values.Set("publicurl", this.Parent.Parent.ReachableURL);
            this.Values.Set("sys", this.Parent.Database.SiteInfo.AsJObject);
            this.Values.Set("user", this.Parent.User.SynchObject);
            this.Values.Set("env", this.Parent.Parent.AsParameters);

            // Data
            if (this.Object != null)
            {
                // Get
                JObject c_Data = this.Object.AsJObject;
                // Get linked
                this.FillLinks(c_Data);

                // Save
                this.Values.Set("data", c_Data);
            }

            // Preprocess telemetry
            if (bTelemetry)
            {
                //Handle links
                sTemplate = sTemplate.Replace("href=\"http", "href=\"{0}/zl/{1}/http".FormatString("".PublicURL(), c_Tele.ID));
                // And the social media icons
                sTemplate = c_Tele.Shorten(sTemplate, "zt", @"/viewers/automizy/images".PublicURL(), usebitly, to);
                // And the telemetry link
                sTemplate = c_Tele.Replace(sTemplate, "./images/social-nxproject.gif", "zt", @"/social-nxproject.gif".PublicURL(), usebitly, to);
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
            }).Replace("".PublicURL(), this.Parent.Parent.ReachableURL);
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
            foreach (string sKey in obj.Keys())
            {
                // Get the value
                string sValue = obj.Get(sKey);
                // Is it a link?
                if (AO.UUIDClass.IsValid(sValue))
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

        #region Statics
        /// <summary>
        /// 
        /// Creates a message from a store
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public static eMessageClass FromStore(EnvironmentClass env, StoreClass store)
        {
            // Get the params
            string sTo = store[KeyTo];
            string sFn = store[KeyCommand];
            string sSubj = store[KeySubj];
            string sMsg = store[KeyMsg];
            string sAtt = store[KeyAttachments];
            string sTemplate = store[KeyEMailTemplate];
            string sCampaign = store[KeyCampaign];
            string sTelemetry = store[KeyTelemetry];
            string sUser = store[KeyUser];
            string sPost = store[KeyPost];
            string sFooter = store[KeyFooter];

            // Handle to
            JArray c_To = sTo.ToJArrayOptional();
            // Handle attachments
            JArray c_Att = sAtt.ToJArrayOptional();

            // Create preference
            eAddressClass.AddressTypes eType = eAddressClass.AddressTypes.User;
            switch (sFn)
            {
                case "voice":
                    eType = eAddressClass.AddressTypes.Voice;
                    break;

                case "sms":
                    eType = eAddressClass.AddressTypes.SMS;
                    break;

                case "email":
                    eType = eAddressClass.AddressTypes.EMail;
                    break;

                case "fedex":
                    eType = eAddressClass.AddressTypes.FedEx;
                    break;
            }

            // Make the context
            AO.ExtendedContextClass c_Ctx = new AO.ExtendedContextClass(env, null, null, sUser);

            // Make the message
            eMessageClass c_Msg = new eMessageClass(c_Ctx);

            c_Msg.Telemetry = !sTelemetry.IsSameValue("n");
            c_Msg.Post = sPost;
            c_Msg.Footer = sFooter;
            c_Msg.Command = sFn;
            c_Msg.User = sUser;
            c_Msg.MessageLink = store[KeyMessageLink];

            // Set the template
            c_Msg.EMailTemplate = sTemplate;
            // And the campaign
            c_Msg.Campaign = sCampaign;

            // Process recipients
            for (int i = 0; i < c_To.Count; i++)
            {
                string sWkg = c_To.Get(i);
                if (sWkg.HasValue()) c_Msg.To.Add(sWkg, eType);
            }

            // Fill
            c_Msg.Subject = sSubj;
            c_Msg.Message = sMsg;

            for (int i = 0; i < c_Att.Count; i++)
            {
                string sWkg = c_Att.Get(i);
                if (sWkg.HasValue()) c_Msg.Attachments.Add(sWkg);
            }

            return c_Msg;
        }

        /// <summary>
        /// 
        /// Creates message from saved value
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static eMessageClass FromSave(EnvironmentClass env, string value)
        {
            // Do
            return eMessageClass.FromStore(env, new StoreClass(value.Decompress().ToJObject()));
        }
        #endregion
    }
}
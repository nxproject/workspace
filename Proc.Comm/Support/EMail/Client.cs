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
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;
using OpenPop;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Communication.EMailIF
{
    public class ClientClass : IDisposable
    {
        #region Constants
        public List<string> NoReply = new List<string> { "noreply", "no-reply" };
        #endregion 

        #region Constructor
        public ClientClass(string friendly, string name, string pwd, ProviderClass prov)
        {
            //
            this.EMailAddress = name;
            this.Friendly = friendly;

            this.SMTP = new SmtpClient(prov.SMTP.Server, prov.SMTP.Port);
            this.SMTP.EnableSsl = prov.SMTP.Connection != ProviderClass.Types.None;
            this.SMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
            this.SMTP.UseDefaultCredentials = false;
            this.SMTP.Credentials = new NetworkCredential(name, pwd);

            this.POP = new OpenPop.Pop3.Pop3Client();
            this.POP.Connect(prov.POP.Server, prov.POP.Port, prov.POP.Connection != ProviderClass.Types.None);
            this.POP.Authenticate(name, pwd);
        }
        #endregion

        #region Indexer
        public Inbound.MessageClass this[int index]
        {
            get
            {
                Inbound.MessageClass c_Ans = null;

                if (index >= 0 && index < this.Count)
                {
                    c_Ans = new Inbound.MessageClass(this, index);
                }

                return c_Ans;
            }
        }
        #endregion

        #region Properties
        private string EMailAddress { get; set; }
        private string Friendly { get; set; }
        protected SmtpClient SMTP { get; set; }
        public OpenPop.Pop3.Pop3Client POP { get; set; }

        public int Count
        {
            get { return this.POP.GetMessageCount(); }
        }

        private long Timeout { get; set; }
        #endregion

        #region Methods
        public void Dispose()
        {
            if (this.SMTP != null)
            {
                this.SMTP.Dispose();
                this.SMTP = null;
            }

            if (this.POP != null)
            {
                this.POP.Dispose();
                this.POP = null;
            }
        }

        public string Send(string subject, string message)
        {
            return Send("", "", "", subject, message, (List<Outbound.AttachmentClass>)null);
        }

        public string Send(string to, string subject, string message)
        {
            return Send(to, "", "", subject, message, (List<Outbound.AttachmentClass>)null);
        }

        public string Send(
                                            string to,
                                            string cc,
                                            string bcc,
                                            string subject,
                                            string message,
                                            string attname,
                                            byte[] attvalue)
        {
            List<Outbound.AttachmentClass> c_Att = null;

            if (attname != null && attvalue != null)
            {
                c_Att = new List<Outbound.AttachmentClass>();
                c_Att.Add(new Outbound.AttachmentClass(attname, attvalue));
            }

            return Send(
                                to,
                                cc,
                                bcc,
                                subject,
                                message,
                                c_Att);
        }

        public string Send(
                                            string to,
                                            string cc,
                                            string bcc,
                                            string subject,
                                            string message,
                                            List<Outbound.AttachmentClass> att
                                            )
        {
            string sAns = null;

            try
            {
                if (to != null && to.Trim().Length > 0)
                {
                    MailMessage c_Msg = new MailMessage();

                    c_Msg.From = new MailAddress(this.EMailAddress, this.Friendly);
                    c_Msg.To.Add(to);
                    if (cc.HasValue()) c_Msg.CC.Add(cc);
                    if (bcc.HasValue()) c_Msg.Bcc.Add(bcc);

                    if (subject.HasValue()) c_Msg.Subject = subject;

                    if (message.HasValue())
                    {
                        c_Msg.IsBodyHtml = true;
                        c_Msg.Body = message;
                    }

                    if (att != null)
                    {
                        foreach (Outbound.AttachmentClass c_Att in att)
                        {
                            if (c_Att != null && c_Att.Name.HasValue() && c_Att.Value != null)
                            {
                                c_Msg.Attachments.Add(c_Att.AsAttachment);
                            }
                        }
                    }

                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    this.SMTP.Send(c_Msg);
                }
            }
            catch (Exception e)
            {
                sAns = e.GetAllExceptions();
            }

            return sAns;
        }
        #endregion

        #region Statics
        public static ProviderClass ProviderFromString(string prov)
        {
            ProviderClass.Providers eProv = ProviderClass.Providers.GMail;

            if (prov.HasValue())
            {
                try
                {
                    eProv = (ProviderClass.Providers)Enum.Parse(typeof(ProviderClass.Providers), prov, true);
                }
                catch { }
            }

            return new ProviderClass(eProv);
        }
        #endregion
    }
}
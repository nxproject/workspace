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
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Communication.EMailIF
{
    public class EngineClass : IDisposable
    {
        #region Constants
        #endregion

        #region Constructor
        public EngineClass(string friendly, string name, string pwd, string prov)
        {
            //
            this.Friendly = friendly;
            this.Name = name;
            this.Pwd = pwd;
            this.Provider = prov;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public string Friendly { get; internal set; }
        public string Name { get; internal set; }
        public string Pwd { get; internal set; }
        public string Provider { get; internal set; }
        #endregion

        #region Methods
        public string SendHTML(AO.ExtendedContextClass ctx,
                                string to, 
                                string subj, 
                                string msg, 
                                string footer, 
                                List<DocumentClass> attach,
                                bool attachasfile)
        {
            string sMsg = this.GetResource("EMailTemplate.html").FromBytes();
            sMsg = sMsg.Replace("{owner}", ctx.SiteInfo.Name);
            sMsg = sMsg.Replace("{nxofficeurl}", ctx.Parent.LoopbackURL);

            StringBuilder c_Attachments = new StringBuilder();
            StringBuilder c_Actions = new StringBuilder();
            List<Outbound.AttachmentClass> c_Files = null;

            if (attachasfile)
            {
                c_Files = new List<Outbound.AttachmentClass>();
                for (int iLoop = 0; iLoop < attach.Count; iLoop++)
                {
                    DocumentClass c_File = attach[iLoop];
                    if (c_File != null)
                    {
                        string sName = c_File.Name.GetFileNameFromPath();
                        byte[] abContents = c_File.ValueAsBytes;

                        if (abContents != null)
                        {
                            Outbound.AttachmentClass c_Att = new Outbound.AttachmentClass(sName, abContents, "application/octect-stream");
                            c_Files.Add(c_Att);
                        }
                    }
                }
            }
            else if (attach != null && attach.Count > 0)
            {
                string sFmt = "<tr><td style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: {2}; border-radius: 5px; text-align: center;\"> <a href=\"{1}\" target=\"_blank\" style=\"display: inline-block; color: #ffffff; background-color: {2}; border: solid 1px {2}; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 14px; font-weight: bold; margin: 2; padding: 12px 25px; text-transform: capitalize; border-color: {2};\">{0}</a> </td></tr>";
                //string sAction = "<script type=\"application/ld+json\">{\"@context\":\"http://schema.org\",\"@type\":\"EmailMessage\",\"potentialAction\":{\"@type\":\"ViewAction\",\"target\":\"{1}\",\"name\":\"{0}\"},\"description\":\"View file {0}\"}</script>";

                c_Attachments.Append("<hr/>");
                c_Attachments.Append("<h3>Attachments</h3>");
                c_Attachments.Append("<hr/>");

                for (int iLoop = 0; iLoop < attach.Count; iLoop++)
                {
                    DocumentClass c_File = attach[iLoop];

                    string sColor = "#3498db";
                    string sURL = c_File.URL;

                    c_Attachments.Append(sFmt.FormatString(c_File.Name.ToUpper(), c_File.URL, sColor));
                }

                c_Attachments.Append("<hr/>");
            }

            sMsg = sMsg.Replace("{1}", c_Attachments.ToString());
            sMsg = sMsg.Replace("{4}", c_Actions.ToString());
            sMsg = sMsg.Replace("{0}", msg);
            sMsg = sMsg.Replace("{2}", "");
            sMsg = sMsg.Replace("{3}", footer);

            using (ClientClass c_Client = new ClientClass(this.Friendly,
                                                                this.Name,
                                                                this.Pwd,
                                                                ClientClass.ProviderFromString(this.Provider)))
            {
                return c_Client.Send(to,
                                "",
                                "",
                                subj,
                                sMsg,
                                c_Files
                                );
            }
        }
        #endregion
    }
}
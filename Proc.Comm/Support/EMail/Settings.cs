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
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Comm.EMailIF
{
    public class ProviderClass : IDisposable
    {
        #region Constants
        #endregion

        #region Constructor
        public ProviderClass(string prov)
            : this(prov.IsSameValue("GMail") ? Providers.GMail :
                  (prov.IsSameValue("Yahoo") ? Providers.Yahoo :
                  (prov.IsSameValue("GoDaddy") ? Providers.GoDaddy :
                  (prov.IsSameValue("HushMail") ? Providers.HushMail : Providers.Office365 ))))
        { }

        public ProviderClass(Providers prov)
        {
            //
            switch (prov)
            {
                case Providers.GMail:
                    this.SMTP = new Values("smtp.gmail.com", 587, Types.SSL);
                    this.POP = new Values("pop.gmail.com", 995, Types.SSL);
                    this.IMAP = new Values("imap.gmail.com", 993, Types.SSL);
                    break;

                case Providers.Live:
                case Providers.Office365:
                case Providers.Hotmail:
                    this.SMTP = new Values("smtp.live.com", 587, Types.TLS);
                    this.POP = new Values("pop3.live.com", 995, Types.SSL);
                    this.IMAP = new Values("imap-mail.outlook.com", 993, Types.SSL);
                    break;

                case Providers.Yahoo:
                    this.SMTP =  new Values( "smtp.mail.yahoo.com", 465, Types.SSL);
                    this.POP = new Values("pop.mail.yahoo.com", 995, Types.SSL);
                    this.IMAP = new Values("imap.mail.yahoo.com", 993, Types.SSL);
                    break;

                case Providers.GoDaddy:
                    this.SMTP = new Values( "smtpout.secureserver.net", 80, Types.None);
                    this.POP = new Values("pop.secureserver.net", 995, Types.SSL);
                    this.IMAP = new Values("imap.secureserver.net", 993, Types.SSL);
                    break;

                case Providers.HushMail:
                    this.SMTP = new Values("smtp.hushmail.com", 465, Types.SSL);
                    this.POP = new Values("pop.hushmail.com", 995, Types.SSL);
                    this.IMAP = new Values("imap.hushmail.com", 993, Types.SSL);
                    break;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Enums
        public enum Providers
        {
            GMail,
            Live,
            Office365,
            Hotmail,
            Yahoo,
            GoDaddy,
            HushMail
        }

        public enum Types
        {
            None,
            SSL,
            TLS
        }
        #endregion

        #region Properties
        public Values SMTP { get; internal set; }
        public Values POP { get; internal set; }
        public Values IMAP { get; internal set; }
        #endregion

        #region Methods
        #endregion

        public class Values
        {
            #region Constructor
            public Values(string server, int port, Types type)
            {
                //
                this.Server = server;
                this.Port = port;
                this.Connection = type;
            }
            #endregion

            #region Properties
            public string Server { get; internal set; }
            public int Port { get; internal set; }
            public Types Connection { get; internal set; }
            #endregion
        }
    }
}
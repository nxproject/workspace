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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

using NX.Shared;
using NX.Engine;

namespace Proc.Task.EAMS
{
    public class HTTPSession : IDisposable
    {
        #region Logging
        //public static string LogFile = null;
        #endregion

        #region Workarea
        private List<string> mc_Keep = new List<string>();
        private Dictionary<string, string> mc_Header = new Dictionary<string, string>();
        //private bool mbFollowRedirect = false;
        #endregion

        #region Constructor
        public HTTPSession()
        {
            //mc_Conn.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:15.0) Gecko/20120830 Firefox/15.0";
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
        #endregion

        #region Properties
        private Dictionary<string, string> IFields { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, string> IHeaders { get; set; } = new Dictionary<string, string>();
        private List<string> IKeep { get; set; } = new List<string>();
        private CookieContainer ICookies { get; set; } = new CookieContainer();

        public string URL { get; set; }

        private HttpWebResponse IResponse { get; set; }

        public string Result { get; private set; }

        public int ResultCode { get; private set; }
        #endregion

        #region Methods
        public void Log(string path)
        {
        }

        public void Keep(string fld)
        {
            if (!this.IKeep.Contains(fld)) this.IKeep.Add(fld);
        }

        public void NewRequest()
        {
            //
            this.IHeaders = new Dictionary<string, string>();
            this.IFields = new Dictionary<string, string>();

            if (this.IResponse != null)
            {
                foreach (string sFld in this.IKeep)
                {
                    this.AddHeader(sFld, this.IResponse.Headers[sFld]);
                }
            }
        }

        public void AddField(string fld, string value)
        {
            if (this.IFields.ContainsKey(fld))
            {
                this.IFields[fld] = value;
            }
            else
            {
                this.IFields.Add(fld, value);
            }
        }

        public void AddHeader(string header, string value)
        {
            if (this.IHeaders.ContainsKey(header))
            {
                this.IHeaders[header] = value;
            }
            else
            {
                this.IHeaders.Add(header, value);
            }
        }

        public string GetHeader(string header)
        {
            string sAns = null;

            if (this.IHeaders.ContainsKey(header)) sAns = this.IHeaders[header];

            return sAns;
        }

        public bool Request()
        {
            return this.Request(true);
        }

        public bool Request(bool checkstatus)
        {
            bool bAns = false;

            try
            {
                string sMethod = "GET";
                string sURL = this.URL;

                if (this.IFields != null && this.IFields.Count > 0)
                {
                    sMethod = "POST";

                    StringBuilder c_Out = new StringBuilder();
                    foreach (string sField in this.IFields.Keys)
                    {
                        if (c_Out.Length != 0) c_Out.Append("&");
                        c_Out.Append(WebUtility.UrlEncode(sField));
                        c_Out.Append("=");
                        c_Out.Append(WebUtility.UrlEncode(this.IFields[sField]));
                    }
                    sURL += "?" + c_Out.ToString();
                }

                HttpWebRequest c_Req = (HttpWebRequest)HttpWebRequest.Create(sURL);
                c_Req.Method = sMethod;
                c_Req.KeepAlive = true;
                c_Req.CookieContainer = this.ICookies;

                foreach (string sHeader in this.IHeaders.Keys)
                {
                    c_Req.Headers.Add(sHeader, this.IHeaders[sHeader]);
                }


                this.IResponse = (HttpWebResponse)c_Req.GetResponse();

                this.ResultCode = (int)this.IResponse.StatusCode;

                using (Stream c_Stream = this.IResponse.GetResponseStream())
                {
                    using (StreamReader c_Reader = new StreamReader(c_Stream))
                    {
                        this.Result = c_Reader.ReadToEnd();
                    }
                }

                bAns = true;
            }
            catch { }

            return bAns;
        }

        public string GetMatch(string key)
        {
            string sAns = null;

            Match c_Match = Regex.Match(this.Result, string.Format(@"(?<value>{0}[^\x22\x27]+)", key));
            if (c_Match.Success)
            {
                sAns = c_Match.Groups["value"].Value;
            }

            return sAns;
        }

        public List<string> GetMatches(string key)
        {
            List<string> c_Ans = new List<string>();

            MatchCollection c_Matches = Regex.Matches(this.Result, string.Format(@"(?<value>{0}[^\x22\x27]+)", key));
            foreach (Match c_Match in c_Matches)
            {
                c_Ans.Add(c_Match.Groups["value"].Value);
            }

            return c_Ans;
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        { }
        #endregion
    }
}

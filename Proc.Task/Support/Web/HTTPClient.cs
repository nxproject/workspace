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
using System.Net;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class HTTPClientClass : WebClient
    {
        #region Constructor
        public HTTPClientClass()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public HTTPClientClass(CookieContainer container)
            : this()
        {
            this.CookieContainer = container;
        }
        #endregion

        #region Properties
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();
        #endregion

        #region Methods
        public string Get(string url)
        {
            string sAns = null;

            try
            {
                sAns = this.DownloadString(url);
            }
            catch (Exception e)
            {
                var a = e;
            }

            return sAns;
        }

        public string Post(string url, params string[] values)
        {
            string sAns = null;

            try
            {
                this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                sAns = this.UploadString(url, HTTPClientClass.MakeURL(null, values));
            }
            catch (Exception e)
            {
                var a = e;
            }

            return sAns;
        }

        public string Post(string url, JObject values)
        {
            string sAns = null;

            try
            {
                this.Headers[HttpRequestHeader.ContentType] = "application/json";
                sAns = this.UploadString(url, values.ToSimpleString());
            }
            catch (Exception e)
            {
                var a = e;
            }

            return sAns;
        }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = this.CookieContainer;
            }
            return r;
        }

        //protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        //{
        //    WebResponse response = base.GetWebResponse(request, result);
        //    ReadCookies(response);
        //    return response;
        //}

        //protected override WebResponse GetWebResponse(WebRequest request)
        //{
        //    WebResponse response = base.GetWebResponse(request);
        //    ReadCookies(response);
        //    return response;
        //}

        //private void ReadCookies(WebResponse r)
        //{
        //    var response = r as HttpWebResponse;
        //    if (response != null)
        //    {
        //        CookieCollection cookies = response.Cookies;
        //        this.CookieContainer.Add(cookies);
        //    }
        //}
        #endregion

        #region Statics
        public static string MakeURL(string url, StoreClass values)
        {
            string sDelim = "?";

            if (values != null)
            {
                foreach (string sKey in values.Keys)
                {
                    url += sDelim + sKey + "=" + values[sKey].URLEncode();
                    sDelim = "&";
                }
            }

            return url;
        }

        public static string MakeURL(string url, params string[] values)
        {
            string sDelim = url.HasValue() ? "?" : "";

            if (values != null)
            {
                for (int i = 0; i < values.Length; i += 2)
                {
                    url += sDelim + values[i] + "=" + values[i + 1].URLEncode();
                    sDelim = "&";
                }
            }

            return url;
        }

        public static StoreClass Parse(string value)
        {
            JObject c_Ans = value.ToJObject();
            if (c_Ans == null)
            {
                string[] asValues = value.IfEmpty().Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

                c_Ans = new JObject();
                foreach (string sLine in asValues)
                {
                    int iPos = sLine.IndexOf("=");
                    if (iPos != -1)
                    {
                        c_Ans.Set(sLine.Substring(0, iPos), sLine.Substring(iPos + 1));
                    }
                    else
                    {
                        c_Ans.Set(sLine, "");
                    }
                }
            }

            return new StoreClass(c_Ans);
        }
        #endregion
    }
}

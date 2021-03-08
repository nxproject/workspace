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
using System.Text.RegularExpressions;
using System.Collections.Specialized;

using Newtonsoft.Json.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.awt.geom;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Task
{
    public static class ExtensionsC
    {
        #region Statics
        public static bool IsSiteNameValid(this string value)
        {
            return Regex.Match(value, @"^([a-z]\w{1,})$").Success;
        }
        #endregion

        #region HTTP
        public static bool IsRemoteIP(this string ip)
        {
            return !Regex.Match(ip, @"(^localhost)|(^127\.)|(^192\.168\.)|(^10\.)|(^172\.1[6-9]\.)|(^172\.2[0-9]\.)|(^172\.3[0-1]\.)|(^::1$)|(^[fF][cCdD])").Success;
        }

        public static bool IsIPAddress(this string ip)
        {
            return Regex.Match(ip, @"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$").Success;
        }

        public static void TlsSet(this string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls
                                                        | System.Net.SecurityProtocolType.Tls11
                                                        | System.Net.SecurityProtocolType.Tls12
                                                        | System.Net.SecurityProtocolType.Ssl3;
        }

        private static NameValueCollection ToCollection(this NamedListClass<string> list)
        {
            NameValueCollection c_Ans = new NameValueCollection();

            foreach(string sKey in list.Keys)
            {
                c_Ans.Add(sKey, list[sKey]);
            }

            return c_Ans;
        }

        public static string URLGet(this string url, params string[] headers)
        {
            string sAns = "";

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    for (int i = 0; i < headers.Length; i += 2)
                    {
                        c_Client.Headers.Add(headers[i], headers[i + 1]);
                    }

                    byte[] abWkg = c_Client.DownloadData(url);
                    if (abWkg != null)
                    {
                        sAns = abWkg.FromBytes();
                    }
                }
            }
            catch { }

            return sAns;
        }

        public static byte[] URLGetBytes(this string url)
        {
            byte[] abAns = null;

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    abAns = c_Client.DownloadData(url);
                }
            }
            catch { }

            if (abAns == null) abAns = new byte[0];

            return abAns;
        }

        public static string URLPost(this string url, string value)
        {
            return url.URLPost(value.ToBytes());
        }

        public static string URLPost(this string url, byte[] value)
        {
            string sAns = "";

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    byte[] abWkg = c_Client.UploadData(url, value);
                    if (abWkg != null)
                    {
                        sAns = abWkg.FromBytes();
                    }
                }
            }
            catch { }

            return sAns;
        }

        public static string URLPost(this string url, NamedListClass<string> value)
        {
            string sAns = "";

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    byte[] abWkg = c_Client.UploadValues(url, value.ToCollection());
                    if (abWkg != null)
                    {
                        sAns = abWkg.FromBytes();
                    }
                }
            }
            catch { }

            return sAns;
        }

        public static byte[] URLPostBytes(this string url, string value)
        {
            byte[] abAns = null;

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    string sWkg = c_Client.UploadString(url, value);
                    if (sWkg.HasValue()) abAns = sWkg.ToBytes();
                }
            }
            catch { }

            if (abAns == null) abAns = new byte[0];

            return abAns;
        }

        public static byte[] URLPostBytes(this string url, byte[] value)
        {
            byte[] abAns = null;

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    abAns = c_Client.UploadData(url, value);
                }
            }
            catch { }

            if (abAns == null) abAns = new byte[0];

            return abAns;
        }

        public static byte[] URLPostBytes(this string url, NamedListClass<string> value)
        {
            byte[] abAns = null;

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    abAns = c_Client.UploadValues(url, value.ToCollection());
                }
            }
            catch { }

            return abAns;
        }

        public static bool URLGetFile(this string url, string path)
        {
            bool bAns = false;

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    System.IO.Path.GetDirectoryName(path).AssurePath();
                    if (System.IO.File.Exists(path))
                    {
                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch { }
                    }

                    //path.GetDirectoryFromPath().AssurePath();

                    c_Client.DownloadFile(url, path);
                    bAns = true;
                }
            }
            catch { }

            return bAns;
        }

        public static string URLEncode(this string value)
        {
            return System.Uri.EscapeDataString(value.IfEmpty()); //.Replace(Delimiter, "");
        }

        public static string URLDecode(this string value)
        {
            return System.Uri.UnescapeDataString(value.IfEmpty()); //.Replace(Delimiter, "");
        }

        public static string HTMLEncode(this string value)
        {
            return System.Net.WebUtility.HtmlEncode(value.IfEmpty());
        }

        public static string HTMLDecode(this string value)
        {
            return System.Net.WebUtility.HtmlDecode(value.IfEmpty());
        }

        public static string WebDecode(this string value, bool fromweb)
        {
            string sAns = value;
            if (fromweb)
            {
                sAns = value.IfEmpty().Replace("+", " ");
            }

            return sAns;
        }
        #endregion

        #region PDF
        public static void CopyPage(this Document outdoc, PdfReader reader, int page, PdfWriter writer)
        {
            if (page <= reader.NumberOfPages)
            {
                outdoc.NewPage();
                PdfContentByte c_CB = writer.DirectContent;

                PdfImportedPage c_Page = writer.GetImportedPage(reader, page);
                Rectangle c_PageSize = reader.GetPageSizeWithRotation(page);
                float oWidth = c_PageSize.Width;
                float oHeight = c_PageSize.Height;

                float scale = getScale(oWidth, oHeight);
                float scaledWidth = oWidth * scale;
                float scaledHeight = oHeight * scale;
                int rotation = reader.GetPageRotation(page);

                AffineTransform c_Trans = new AffineTransform(scale, 0, 0, scale, 0, 0);

                switch (rotation)
                {
                    case 0:
                        c_CB.AddTemplate(c_Page, c_Trans);
                        break;

                    case 90:
                        AffineTransform c_T90 = new AffineTransform(0, -1f, 1f, 0, 0, scaledHeight);
                        c_T90.Concatenate(c_Trans);
                        c_CB.AddTemplate(c_Page, c_T90);
                        break;

                    case 180:
                        AffineTransform c_T180 = new AffineTransform(-1f, 0, 0, -1f, scaledWidth,
                            scaledHeight);
                        c_T180.Concatenate(c_Trans);
                        c_CB.AddTemplate(c_Page, c_T180);
                        break;

                    case 270:
                        AffineTransform c_T270 = new AffineTransform(0, 1f, -1f, 0, scaledWidth, 0);
                        c_T270.Concatenate(c_Trans);
                        c_CB.AddTemplate(c_Page, c_T270);
                        break;

                    default:
                        c_CB.AddTemplate(c_Page, scale, 0, 0, scale, 0, 0);
                        break;
                }
            }

        }

        private static float getScale(float width, float height)
        {
            float scaleX = PageSize.LETTER.Width / width;
            float scaleY = PageSize.LETTER.Height / height;
            return width > height ? Math.Min(scaleX, scaleY) : Math.Max(scaleX,
                scaleY);
        }
        #endregion
    }
}
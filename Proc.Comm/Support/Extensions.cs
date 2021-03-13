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

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Communication
{
    public static class ExtensionsC
    {
        #region Statics
        public static bool IsEMailAddress(this string value)
        {
            return Regex.Match(value.IfEmpty().Trim(), @"^[\w\.=-]+@[\w\.-]+\.[\w]{2,}$").Success;
        }

        public static bool IsFormattedPhone(this object value)
        {
            bool bAns = false;

            try
            {
                string sPoss = value.ToString();
                bAns = Regex.Match(sPoss, @"\([0-9]{3}\)\s\d{3}\-\d{4}").Success;
            }
            catch { }

            return bAns;
        }

        public static string PhoneNumberAse164(this string value)
        {
            return "+1" + Regex.Replace(value.IfEmpty(), @"[^\d]", "");
        }

        public static bool IsSiteNameValid(this string value)
        {
            return Regex.Match(value, @"^([a-z]\w{1,})$").Success;
        }
        #endregion

        #region Docs
        public enum Extensions
        {
            Unknown,
            Text,
            PDF,
            FDF,
            XML,
            Doc,
            Docx,
            XLS,
            PPT,
            WPD,
            RTF,
            JPEG,
            GIF,
            TIFF,
            BMP,
            PNG,
            CSV,
            HTML,
            MHTML,
            ODT,
            CSS
        }

        public static Extensions GetExtensionTypeFromPath(this string path)
        {
            Extensions eAns = Extensions.Unknown;

            path = path.GetExtensionFromPath();

            switch (path.ToLower())
            {
                case "css":
                    eAns = Extensions.CSS;
                    break;

                case "odt":
                    eAns = Extensions.ODT;
                    break;

                case "html":
                case "htm":
                    eAns = Extensions.HTML;
                    break;

                case "mhtml":
                    eAns = Extensions.MHTML;
                    break;

                case "csv":
                    eAns = Extensions.CSV;
                    break;

                case "jpg":
                case "jpeg":
                    eAns = Extensions.JPEG;
                    break;

                case "gif":
                    eAns = Extensions.GIF;
                    break;

                case "tif":
                case "tiff":
                    eAns = Extensions.TIFF;
                    break;

                case "bmp":
                    eAns = Extensions.BMP;
                    break;

                case "png":
                    eAns = Extensions.PNG;
                    break;

                case "json":
                case "txt":
                case "cs":
                case "js":
                case "java":
                case "bat":
                case "cmd":
                    eAns = Extensions.Text;
                    break;

                case "pdf":
                    eAns = Extensions.PDF;
                    break;

                case "fdf":
                    eAns = Extensions.FDF;
                    break;

                case "doc":
                    eAns = Extensions.Doc;
                    break;

                case "docx":
                    eAns = Extensions.Docx;
                    break;

                case "xls":
                case "xlsx":
                    eAns = Extensions.XLS;
                    break;

                case "ppt":
                    eAns = Extensions.PPT;
                    break;

                case "wpd":
                    eAns = Extensions.WPD;
                    break;

                case "rtf":
                    eAns = Extensions.RTF;
                    break;

                case "xml":
                    eAns = Extensions.XML;
                    break;
            }

            return eAns;
        }

        public static string ContentTypeFromExtension(this Extensions type)
        {
            string sContent = "";

            switch (type)
            {
                case Extensions.CSS:
                    sContent = "text/css";
                    break;

                case Extensions.BMP:
                case Extensions.JPEG:
                case Extensions.GIF:
                case Extensions.TIFF:
                case Extensions.PNG:
                    sContent = "image/" + type;
                    break;

                case Extensions.Text:
                case Extensions.CSV:
                    sContent = "text/plain";
                    break;

                case Extensions.PDF:
                    sContent = "application/pdf";
                    break;

                case Extensions.FDF:
                    sContent = "application/vnd.fdf";
                    break;

                case Extensions.Doc:
                case Extensions.Docx:
                case Extensions.ODT:
                    sContent = "application/msword";
                    break;

                case Extensions.XLS:
                    sContent = "application/vnd.ms-excel";
                    break;

                case Extensions.PPT:
                    sContent = "application/mspowerpoint";
                    break;

                case Extensions.WPD:
                    sContent = "application/wordperfect";
                    break;

                case Extensions.RTF:
                    sContent = "application/rtf";
                    break;

                case Extensions.HTML:
                    sContent = "text/html";
                    break;

                case Extensions.MHTML:
                    sContent = "text/mhtml";
                    break;

                case Extensions.XML:
                    sContent = "text/xml";
                    break;

                default:
                    sContent = "application/octect-stream";
                    break;
            }

            return sContent;
        }
        #endregion

        #region Statics
        // From: https://stackoverflow.com/questions/3991840/simple-text-to-html-conversion
        public static string TextToHTML(this string value)
        {
            string sAns = value;

            if (sAns.StartsWith("<") && sAns.EndsWith(">"))
            { }
            else
            {
                value = value.IfEmpty().HTMLEncode();

                value = value.Replace("\r\n", "\r");
                value = value.Replace("\n", "\r");
                value = value.Replace("\r", "<br>\r\n");
                value = value.Replace("  ", " &nbsp;");

                return value;
            }
            return sAns;
        }
        #endregion
    }
}
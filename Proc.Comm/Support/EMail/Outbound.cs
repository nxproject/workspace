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
using System.Drawing;
using System.Drawing.Imaging;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Comm.EMailIF.Outbound
{
    public class AttachmentClass
    {
        #region Constructor
        public AttachmentClass(string name, byte[] value, string ct = null)
        {
            this.Name = name.GetFileNameFromPath();
            this.Value = value;

            this.ContentType = ct.IfEmpty(this.Name.GetExtensionTypeFromPath().ContentTypeFromExtension());
            this.ContentEncoding = "8BIT";
        }

        public AttachmentClass(string name, string value, string ct = null)
        {
            this.Name = name.GetFileNameFromPath();
            this.Value = this.GetBytes(value);

            this.ContentType = ct.IfEmpty("text/plain");
            this.ContentEncoding = "8BIT";
        }

        public AttachmentClass(string name, Image value)
        {
            this.Name = name;
            try
            {
                System.IO.MemoryStream c_Stream = new System.IO.MemoryStream();
                value.Save(c_Stream, ImageFormat.Tiff);
                this.Value = this.GetBytes(Convert.ToBase64String(c_Stream.ToArray()));
            }
            catch
            {
            }

            this.ContentType = "image/tiff";
            this.ContentEncoding = "base64";
        }
        #endregion

        #region Proprties
        public string Name { get; set; }
        public byte[] Value { get; set; }
        public bool IsImage
        {
            get { return this.ContentType.StartsWith("image/"); }
        }

        public Image AsImage
        {
            get
            {
                Image c_Ans = null;

                if (this.ContentType.StartsWith("image/"))
                {
                    string sWkg = this.AsString;

                    if (sWkg.HasValue())
                    {
                        try
                        {
                            System.IO.MemoryStream c_Stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(sWkg));
                            c_Ans = Image.FromStream(c_Stream);
                        }
                        catch { }
                    }
                }

                return c_Ans;
            }
        }

        public string AsString
        {
            get
            {
                string sAns = null;

                if (this.ContentEncoding.IsSameValue("base64"))
                {
                    sAns = Convert.ToBase64String(this.Value);
                }
                else
                {
                    sAns = this.GetString(this.Value);
                }

                return sAns;
            }
        }

        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }

        public System.Net.Mail.Attachment AsAttachment
        {
            get
            {
                this.ContentType = this.ContentType.IfEmpty(this.Name.GetExtensionTypeFromPath().ContentTypeFromExtension());
                return new System.Net.Mail.Attachment(new System.IO.MemoryStream(this.Value), this.Name, this.ContentType);
            }
        }
        #endregion

        #region Encoders
        private byte[] GetBytes(string value)
        {
            return System.Text.ASCIIEncoding.ASCII.GetBytes(value);
        }

        private string GetString(byte[] value)
        {
            return System.Text.ASCIIEncoding.ASCII.GetString(value);
        }
        #endregion
    }
}
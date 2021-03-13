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
using OpenPop;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Communication.EMailIF.Inbound
{
    public class MessageClass : BasedObjectClass
    {
        #region Constructor
        internal MessageClass(ClientClass client, int index)
            : base(client)
        {
            //
            this.Index = index;
            this.Message = this.Client.POP.GetMessage(this.Index);
        }
        #endregion

        #region Indexer
        public AttachmentClass this[int index]
        {
            get
            {
                AttachmentClass c_Ans = null;

                if (index >= 0 && index < this.AttachmentCount)
                {
                    c_Ans = new AttachmentClass(this.IAttachments[index]);
                }

                return c_Ans;
            }
        }
        #endregion

        #region Properties
        private ClientClass Client { get { return this.Root as ClientClass; } }
        private int Index { get; set; }
        private OpenPop.Mime.Message Message { get; set; }

        public string From
        {
            get { return this.Message.Headers.From.DisplayName; }
        }

        public string FromAddress
        {
            get { return this.Message.Headers.From.Address; }
        }

        public DateTime On
        {
            get { return this.Message.Headers.DateSent; }
        }

        public string Subject
        {
            get { return this.Message.Headers.Subject; }
        }

        public string Body
        {
            get { return this.Message.MessagePart.Body.FromBytes(); }
        }

        private List<OpenPop.Mime.MessagePart> IAttachments { get; set; }
        public int AttachmentCount
        {
            get
            {
                if (this.IAttachments == null)
                {
                    this.IAttachments = this.Message.FindAllAttachments();
                }

                return this.IAttachments.Count;
            }
        }

        public List<string> AttachmentNames
        {
            get
            {
                List<string> c_Ans = new List<string>();

                for (int iLoop = 0; iLoop < this.AttachmentCount; iLoop++)
                {
                    c_Ans.Add(this[iLoop].Name);
                }

                return c_Ans;
            }
        }

        public List<string> To
        {
            get
            {
                List<string> c_Ans = new List<string>();

                foreach(var c_Addr in this.Message.Headers.To)
                {
                    c_Ans.Add(c_Addr.Address);
                }

                return c_Ans;
            }
        }

        public List<string> CC
        {
            get
            {
                List<string> c_Ans = new List<string>();

                foreach (var c_Addr in this.Message.Headers.Cc)
                {
                    c_Ans.Add(c_Addr.Address);
                }

                return c_Ans;
            }
        }

        public List<string> BCC
        {
            get
            {
                List<string> c_Ans = new List<string>();

                foreach (var c_Addr in this.Message.Headers.Bcc)
                {
                    c_Ans.Add(c_Addr.Address);
                }

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public bool Delete()
        {
            bool bAns = false;

            try
            {
                this.Client.POP.DeleteMessage(this.Index);
            }
            catch { }

            return bAns;
        }
        #endregion
    }

    public class AttachmentClass : BasedObjectClass
    {
        #region Constructor
        internal AttachmentClass(OpenPop.Mime.MessagePart att)
            : base(att)
        { }
        #endregion

        #region Properties
        private OpenPop.Mime.MessagePart Message
        {
            get { return this.Root as OpenPop.Mime.MessagePart; }
        }

        public string Name
        {
            get { return this.Message.FileName; }
        }

        public byte[] Contents
        {
            get { return this.Message.Body; }
        }
        #endregion
    }
}
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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Communication
{
    public class eAttachmentListClass : ChildOfClass<eMessageClass>
    {
        #region Constructor
        public eAttachmentListClass(eMessageClass msg)
            : base(msg)
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The list of documents
        /// 
        /// </summary>
        public List<eAttachmenClass> Documents { get; private set; } = new List<eAttachmenClass>();

        /// <summary>
        /// 
        /// The number of documents
        /// 
        /// </summary>
        public int Count
        {
            get { return this.Documents.Count; }
        }

        /// <summary>
        /// 
        /// Returns a list of the documents
        /// 
        /// </summary>
        public List<DocumentClass> AsDocuments
        {
            get
            {
                List<DocumentClass> c_Ans = new List<DocumentClass>();

                
                foreach(eAttachmenClass c_File in this.Documents)
                {
                    c_Ans.Add(c_File.Document);
                }

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public void Add(List<string> docs)
        {
            foreach (string sPath in docs)
            {
                this.Add(sPath);
            }
        }

        public void Add(string path, string name=null)
        {
            DocumentClass c_Doc = new DocumentClass(this.Parent.Parent.DocumentManager, path);
            this.Add(c_Doc);
        }

        public void Add(List<DocumentClass> docs)
        {
            foreach (DocumentClass c_Doc in docs)
            {
                this.Add(c_Doc);
            }
        }

        public void Add(DocumentClass doc, string name = null)
        {
            this.Documents.Add(new eAttachmenClass(doc, name));
        }
        #endregion
    }

    public class eAttachmenClass : IDisposable
    {
        #region Constructor
        public eAttachmenClass(DocumentClass doc, string caption)
        {
            //
            this.Document = doc;
            this.Caption = caption;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public string Caption { get; set; }
        public DocumentClass Document { get; set; }
        #endregion
    }
}
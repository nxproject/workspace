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

namespace Proc.Comm
{
    public class eAttachmentListClass : ChildOfClass<eMessageClass>
    {
        #region Constructor
        public eAttachmentListClass(eMessageClass msg, string keys)
            : base(msg)
        {
            //
            JArray c_Values = this.Parent.Values.AssureJArray(keys);

            // Make the attachments
            for (int iLoop = 0; iLoop < c_Values.Count; iLoop++)
            {
                this.Add(c_Values.Get(iLoop));
            }
        }
        #endregion

        #region Properties        
        internal List<DocumentClass> Documents { get; set; } = new List<DocumentClass>();
        public List<string> Paths
        {
            get
            {
                List<string> c_Ans = new List<string>();

                foreach (DocumentClass c_Doc in this.Documents)
                {
                    c_Ans.Add(c_Doc.Path);
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

        public void Add(string path)
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

        public void Add(DocumentClass doc)
        {
            this.Documents.Add(doc);
        }

        public void Attach(string doc)
        {
            // TBD
        }
        #endregion
    }
}
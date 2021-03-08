///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.AO
{
    public class OrgDocumentsClass : OrgPointTableClass, IDisposable
    {
        #region Constructor
        internal OrgDocumentsClass()
        { }

        internal OrgDocumentsClass(OrgContextClass env, OrgObjectClass obj)
            : this(env, new FolderClass(env, obj.Object), obj)
        { }

        internal OrgDocumentsClass(OrgContextClass env, FolderClass folder, OrgObjectClass obj)
        {
            //
            this.Env = env;

            // Do sub-folders
            List<string> folders = folder.Folders;
            foreach (string sDir in folders)
            {
                if (!sDir.StartsWith("_"))
                {
                    using (OrgDocumentsClass c_Sub = new OrgDocumentsClass(this.Env, folder.GetFolder(sDir), obj))
                    {
                        this.Merge(c_Sub);
                    }
                }
            }

            // Do files
            List<string> c_Files = folder.Files;
            foreach (string sFile in c_Files)
            {
                //env.Env.LogInfo("FILE: {0}".FormatString(sFile));

                // Add the file
                try
                {
                    DocReferenceClass c_Doc = folder.GetDocument(sFile);
                    //if(c_Doc != null) env.Env.LogInfo("DOCX: {0}".FormatString(c_Doc.Path));
                    this.Add(c_Doc, obj);
                }
                catch { }
                //(Exception e)
                //{
                //    env.Env.LogException("DOCS: {0}".FormatString(sFile), e);
                //}
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public OrgContextClass Env { get; internal set; }
        public new List<OrgDocumentClass> Sorted
        {
            get
            {
                List<OrgDocumentClass> c_Ans = new List<OrgDocumentClass>();

                foreach (OrgPointClass c_Pt in base.Sorted) c_Ans.Add(c_Pt as OrgDocumentClass);

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public OrgDocumentClass Add(DocumentClass doc, OrgObjectClass obj)
        {
            OrgDocumentClass c_Ans = null;

            if (doc.Exists)
            {
                // Use the PDF!
                DocumentClass c_PDF = doc.PDF();
                bool bAdd = true;

                // Open it and check creator
                try
                {
                    PdfReader c_Reader = new PdfReader(c_PDF.Value);
                    if (c_Reader.Info.ContainsKey("Creator"))
                    {
                        bAdd = !OrganizerGeneratorClass.Creator.IsSameValue(c_Reader.Info["Creator"]);
                    }
                }
                catch { }

                if (bAdd)
                {
                    OrgDocumentClass c_Doc = new OrgDocumentClass(this.Env, c_PDF, obj);
                    c_Ans = this.Add(c_Doc);
                }
            }

            return c_Ans;
        }

        private OrgDocumentClass Add(OrgDocumentClass entry)
        {
            OrgDocumentClass c_Ans = entry;

            if (base.ContainsKey(entry.ID))
            {
                c_Ans = base[entry.ID] as OrgDocumentClass;
            }
            else
            {
                base.Add(entry);
            }

            // Add use
            c_Ans.AddUse(entry);

            return c_Ans;
        }

        public bool Merge(OrgDocumentsClass map)
        {
            bool bAns = false;

            foreach (string sKey in map.Keys)
            {
                this.Add(map[sKey] as OrgDocumentClass);
                bAns = true;
            }

            return bAns;
        }

        public List<OrgDocumentClass> OwnedBy(OrgObjectClass obj)
        {
            List<OrgDocumentClass> c_Ans = this.Sorted;

            for (int iDoc = c_Ans.Count - 1; iDoc >= 0; iDoc--)
            {
                if (obj.ID.CompareTo(c_Ans[iDoc].Object.ID) != 0) c_Ans.RemoveAt(iDoc);
            }

            return c_Ans;
        }
        #endregion
    }
}
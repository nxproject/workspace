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
    public class OrgDocumentClass : OrgPointClass, IComparable
    {
        #region Constructor
        public OrgDocumentClass(OrgContextClass env, DocReferenceClass doc, OrgObjectClass obj)
            : base(env, doc, doc.ShortName, OrgDocumentClass.MakeID(doc))
        {
            //
            this.Object = obj;
        }
        #endregion

        #region Properties
        public DocReferenceClass Document { get { return this.Root as DocReferenceClass; } }
        public OrgObjectClass Object { get; internal set; }
        public string Group { get { return this.Document.ShortName.Substring(0, 1).ToUpper(); } }

        public List<OrgDocumentClass> Uses { get; internal set; } = new List<OrgDocumentClass>();
        public List<OrgObjectClass> ObjectUses
        {
            get
            {
                Dictionary<string, OrgObjectClass> c_Uses = new Dictionary<string, OrgObjectClass>();
                List<string> c_Done = new List<string>();

                this.GetObjectUses(c_Uses, c_Done);

                return new List<OrgObjectClass>(c_Uses.Values);
            }
        }
        public void GetObjectUses(Dictionary<string, OrgObjectClass> uses, List<string> done)
        {
            if (done.IndexOf(this.ID) == -1)
            {
                done.Add(this.ID);

                if (!uses.ContainsKey(this.Object.ID))
                {
                    uses.Add(this.Object.ID, this.Object);
                }

                foreach (OrgDocumentClass c_Doc in this.Uses)
                {
                    c_Doc.GetObjectUses(uses, done);
                }
            }
        }
        #endregion

        #region Methods
        public void AddUse(OrgDocumentClass doc)
        {
            this.Uses.Add(doc);
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            int iAns = 0;

            OrgDocumentClass c_Other = obj as OrgDocumentClass;
            if (c_Other != null)
            {
                //iAns = this.Object.ID.CompareTo(c_Other.Object.ID);
                //if (iAns == 0)
                //{
                iAns = this.Document.ShortName.CompareTo(c_Other.Document.ShortName);
                //}
            }

            return iAns;
        }
        #endregion

        #region Statics
        public static string MakeID(DocReferenceClass doc)
        {
            return "D" + doc.Hash;
        }
        #endregion
    }
}
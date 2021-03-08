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
    public class OrgObjectClass : OrgPointClass, IComparable
    {
        #region Constructor
        public OrgObjectClass(OrgContextClass env, AO.ObjectClass obj)
            : base(env, obj, obj.LocalizedDescription, OrgObjectClass.MakeID(obj.UUID))
        { }
        #endregion

        #region Properties
        public AO.ObjectClass Object { get { return this.Root as AO.ObjectClass; } }

        public bool IsLocal { get; set; }
        public OrgDocumentsClass Documents { get; set; }
        public JObject Values { get; set; }
        #endregion

        #region Methods
        public string Get(string fld)
        {
            string sAns = null;

            if (fld.IsSameValue("desc"))
            {
                sAns = this.Object.LocalizedDescription;
            }
            else if (this.Values != null)
            {
                sAns = this.Values.Get(fld);
            }
            else
            {
                sAns = this.Object[fld];
            }

            return sAns.IfEmpty();
        }

        public bool HasBy(List<string> flds)
        {
            bool bAns = false;

            foreach (string sFld in flds)
            {//        if (this.Get(sFld).HasValue())
                {
                    bAns = true;
                    break;
                }
            }

            return bAns;
        }

        public string ApplyFormat(string fmt)
        {
            string sAns = "";

            List<string> c_Fmts = fmt.SplitSpaces();
            foreach (string sFmt in c_Fmts)
            {
                string sValue = null;

                if ((sFmt.StartsWith("'") && sFmt.EndsWith("'")) ||
                    (sFmt.StartsWith("\"") && sFmt.EndsWith("\"")))
                {
                    sValue = sFmt.Substring(1, sFmt.Length - 2);
                }
                else
                {
                    sValue = this.Get(sFmt);
                }

                if (sValue.HasValue())
                {
                    if (sAns.Length > 0) sAns += " ";
                    sAns += sValue;
                }
            }

            return sAns;
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            int iAns = 0;

            OrgObjectClass c_Other = obj as OrgObjectClass;
            if (c_Other != null) iAns = this.Object.Description.CompareTo(c_Other.Object.Description);

            return iAns;
        }
        #endregion

        #region Statics
        public static string MakeID(AO.UUIDClass uuid)
        {
            return "O" + uuid.ToString().HashString();
        }
        #endregion
    }
}

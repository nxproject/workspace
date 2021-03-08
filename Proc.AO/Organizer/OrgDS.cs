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
    public class OrgDSClass : BasedObjectClass, IComparable
    {
        #region Constructor
        internal OrgDSClass(OrgContextClass env, AO.DatasetClass ds)
            : base(env, true)
        {
            //
            this.Dataset = ds.Name;
            this.Description = ds.Definition.Caption;
            this.Objects.ReverseSort = ds.DSSelectType.IsSameValue("Reverse List");
            this.ByFields = ds.Definition.OrganizeBy.SplitSpaces();
            this.ByOnly = ds.Definition.OrganizeByOnly;

            this.ByLabels = new List<string>();
            this.ByFormat = new List<string>();
            for (int iFld = 0; iFld < this.ByFields.Count; iFld++)
            {
                string sFld = this.ByFields[iFld];

                string sFmt = "d";
                int iPos = sFld.IndexOf(":");
                if (iPos != -1)
                {
                    sFmt = sFld.Substring(iPos + 1);
                    sFld = sFld.Substring(0, iPos);
                }

                string sLabel = sFld;

                AO.Definitions.DatasetFieldClass c_Def = ds.Definition[sFld];
                if (c_Def != null) sLabel = c_Def.Label;

                this.ByLabels.Add(sLabel.IfEmpty(sFld));
                this.ByFormat.Add(sFmt);
                this.ByFields[iFld] = sFld;
            }
        }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            foreach (OrgPointClass c_Entry in this.Objects.Values)
            {
                OrgObjectClass c_Obj = c_Entry as OrgObjectClass;

                if (c_Obj != null && c_Obj.IsLocal)
                {
                    this.Env.Associate.FreeObject(c_Obj.Object);
                }
            }
        }
        #endregion

        #region Indexer
        public OrgObjectClass this[AO.UUIDClass uuid]
        {
            get { return this.Objects[OrgObjectClass.MakeID(uuid)] as OrgObjectClass; }
        }
        #endregion

        #region Properties
        public OrgContextClass Env { get { return this.Root as OrgContextClass; } }
        public string Dataset { get; internal set; }
        public string Description { get; internal set; }

        private OrgPointTableClass Objects { get; set; } = new OrgPointTableClass();

        public List<OrgObjectClass> Sorted
        {
            get
            {
                List<OrgObjectClass> c_Ans = new List<OrgObjectClass>();

                foreach (OrgPointClass c_Pt in this.Objects.Sorted) c_Ans.Add(c_Pt as OrgObjectClass);

                return c_Ans;
            }
        }
        public List<string> ByFields { get; internal set; }
        public List<string> ByLabels { get; internal set; }
        public List<string> ByFormat { get; internal set; }
        public bool ByOnly { get; internal set; }

        public Dictionary<string, Dictionary<string, List<OrgObjectClass>>> By(bool uuid)
        {
            Dictionary<string, Dictionary<string, List<OrgObjectClass>>> c_Ans = new Dictionary<string, Dictionary<string, List<OrgObjectClass>>>();

            foreach (OrgObjectClass c_Obj in this.Sorted)
            {
                for (int iFld = 0; iFld < this.ByFields.Count; iFld++)
                {
                    string sFld = this.ByFields[iFld];

                    string sValue = c_Obj.Get(sFld);
                    if (sValue.HasValue())
                    {
                        if (sValue.IsISODate())
                        {
                            DateTime c_On = sValue.FromDBDate().AdjustTimezone();

                            switch (this.ByFormat[iFld])
                            {
                                case "w":
                                    int iDays = c_On.DayOfWeek - DayOfWeek.Monday;
                                    DateTime c_Start = c_On.AddDays(-iDays);
                                    DateTime c_End = c_Start.AddDays(6);
                                    sValue = c_Start.ToString("yyyyMMdd") + "\t" + c_Start.ToString("MM/dd/yyyy") + " - " + c_End.ToString("MM/dd/yyyy");
                                    break;

                                case "m":
                                    sValue = c_On.ToString("yyyyMM") + "\t" + c_On.ToString("MMMM yyyy");
                                    break;

                                case "y":
                                    sValue = c_On.ToString("yyyy") + "\t" + c_On.ToString("yyyy");
                                    break;

                                default:
                                    sValue += "\t" + c_On.ToString("MM/dd/yyyy");
                                    break;
                            }
                        }

                        if (!c_Ans.ContainsKey(this.ByLabels[iFld])) c_Ans.Add(this.ByLabels[iFld], new Dictionary<string, List<OrgObjectClass>>());
                        Dictionary<string, List<OrgObjectClass>> c_At = c_Ans[this.ByLabels[iFld]];

                        if (!c_At.ContainsKey(sValue)) c_At.Add(sValue, new List<OrgObjectClass>());

                        c_At[sValue].Add(c_Obj);
                    }
                }
            }

            return c_Ans;
        }
        #endregion

        #region Methods
        public OrgObjectClass Add(AO.ObjectClass obj, bool islocal)
        {
            OrgObjectClass c_Ans = this.Objects.Add(new OrgObjectClass(this.Env, obj)) as OrgObjectClass;

            c_Ans.IsLocal = islocal;

            return c_Ans;
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            int iAns = 0;

            OrgDSClass c_Other = obj as OrgDSClass;
            if (c_Other != null) iAns = this.Description.CompareTo(c_Other.Description);

            return iAns;
        }
        #endregion
    }
}
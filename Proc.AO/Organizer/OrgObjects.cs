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
    public class OrgObjectsClass : BasedObjectClass
    {
        #region Constructor
        public OrgObjectsClass(OrgContextClass env)
            : base(env, true)
        { }
        #endregion

        #region Indexer
        public OrgDSClass this[string ds]
        {
            get
            {
                OrgDSClass c_Ans = null;

                if (this.Objects.ContainsKey(ds)) c_Ans = this.Objects[ds];

                return c_Ans;
            }
        }
        #endregion

        #region Properties
        public OrgContextClass Env { get { return this.Root as OrgContextClass; } }

        private Dictionary<string, OrgDSClass> Objects { get; set; } = new Dictionary<string, OrgDSClass>();
        public OrgObjectClass RootObject { get; internal set; }

        public List<string> Datasets
        {
            get
            {
                List<string> c_Ans = new List<string>();

                List<OrgDSClass> c_DSS = new List<OrgDSClass>(this.Objects.Values);
                c_DSS.Sort();
                foreach (OrgDSClass c_DS in c_DSS)
                {
                    c_Ans.Add(c_DS.Dataset);
                }

                return c_Ans;
            }
        }

        public List<OrgObjectClass> ObjectList
        {
            get
            {
                List<OrgObjectClass> c_Ans = new List<OrgObjectClass>();

                foreach (string sDS in this.Objects.Keys)
                {
                    OrgDSClass c_DS = this.Objects[sDS];
                    c_Ans.AddRange(c_DS.Sorted);
                }

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public OrgObjectClass Add(AO.UUIDClass uuid)
        {
            return this.Add(this.Env.Associate.FetchObject(uuid, AO.ObjectClass.Types.Raw));
        }

        public OrgObjectClass Add(AO.ObjectClass obj)
        {
            //
            string sDS = obj.UUID.Dataset;

            // Do we have a dataset?
            if (!this.Objects.ContainsKey(sDS))
            {
                this.Objects.Add(sDS, new OrgDSClass(this.Env, this.Env.Associate.Datasets[sDS]));
            }

            // Is this the first?
            bool bFirst = this.RootObject == null;

            // Add
            OrgObjectClass c_Ans = this.Objects[sDS].Add(obj, !bFirst);

            // If first, save
            if (bFirst) this.RootObject = c_Ans;

            //
            return c_Ans;
        }
        #endregion
    }
}
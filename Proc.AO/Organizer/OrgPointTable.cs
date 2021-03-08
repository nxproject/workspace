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
    public class OrgPointTableClass : Dictionary<string, OrgPointClass>
    {
        #region Indexer
        public new OrgPointClass this[string id]
        {
            get
            {
                OrgPointClass c_Ans = null;

                if (this.ContainsKey(id)) c_Ans = base[id];

                return c_Ans;
            }
        }
        #endregion

        #region Properties
        public bool ReverseSort { get; set; }
        public List<OrgPointClass> Sorted
        {
            get
            {
                List<OrgPointClass> c_Ans = new List<OrgPointClass>(this.Values);

                c_Ans.Sort();
                if (this.ReverseSort)
                {
                    c_Ans.Reverse();
                }

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public OrgPointClass Add(OrgPointClass pt)
        {
            if (!this.ContainsKey(pt.ID))
            {
                this.Add(pt.ID, pt);
            }
            else
            {
                pt = this[pt.ID];
            }

            return pt;
        }
        #endregion
    }
}
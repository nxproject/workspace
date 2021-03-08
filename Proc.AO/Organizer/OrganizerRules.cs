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
    public class OrganizerRulesClass : IDisposable
    {
        #region Constructor
        public OrganizerRulesClass()
        {
            this.IncludeAllInOrganizer = true;
            this.IncludeAllDocuments = true;
        }

        public OrganizerRulesClass(params string[] ds)
        {
            foreach (string sEntry in ds)
            {
                string sDS = sEntry;
                string sOptions = "";

                int iPos = sEntry.IndexOf(":");
                if (iPos != -1)
                {
                    sOptions = sEntry.Substring(iPos);
                    sDS = sEntry.Substring(0, iPos);
                }

                OrganizerEntryClass c_Entry = new OrganizerEntryClass(sDS);

                if (!sOptions.HasValue())
                {
                    c_Entry.IncludeInOrganizer = true;
                    c_Entry.IncludeDocuments = true;
                }
                else
                {
                    c_Entry.IncludeInOrganizer = sOptions.IndexOf("o") != -1;
                    c_Entry.IncludeDocuments = sOptions.IndexOf("d") != -1;
                }

                this.Add(sDS, c_Entry);
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Indexer
        public OrganizerEntryClass this[string ds]
        {
            get
            {
                OrganizerEntryClass c_Ans = null;

                if (this.Entries.ContainsKey(ds)) c_Ans = this.Entries[ds];

                return c_Ans;
            }
        }
        #endregion

        #region Properties
        internal Dictionary<string, OrganizerEntryClass> Entries { get; set; } = new Dictionary<string, OrganizerEntryClass>();

        public bool IncludeAllInOrganizer { get; set; }
        public bool IncludeAllDocuments { get; set; }
        #endregion

        #region Methods
        public void Add(string ds, OrganizerEntryClass rules)
        {
            if (this.Entries.ContainsKey(ds))
            {
                this.Entries[ds] = rules;
            }
            else
            {
                this.Entries.Add(ds, rules);
            }
        }

        public bool IncludeInOrganizer(string ds)
        {
            bool bAns = this.IncludeAllInOrganizer;

            if (!bAns)
            {
                OrganizerEntryClass c_Entry = this[ds];
                if (c_Entry != null) bAns = c_Entry.IncludeInOrganizer;
            }

            return bAns;
        }

        public bool IncludeDocuments(string ds)
        {
            bool bAns = this.IncludeAllDocuments;

            if (!bAns)
            {
                OrganizerEntryClass c_Entry = this[ds];
                if (c_Entry != null) bAns = c_Entry.IncludeDocuments;
            }

            return bAns;
        }
        #endregion

        #region Statics
        public static OrganizerRulesClass Make(string options)
        {
            OrganizerRulesClass c_Ans = null;

            if (options.HasValue())
            {
                c_Ans = new OrganizerRulesClass(options.SplitSpaces(true).ToArray());
            }

            return c_Ans;
        }
        #endregion
    }
}
//--------------------------------------------------------------------------------
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
/// Install-Package MongoDb.Bson -Version 2.11.0
///  

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MongoDB.Bson;

using NX.Shared;

namespace Proc.AO.Definitions
{
    public class ViewClass : ExtraClass
    {
        #region Constants
        public const string Prefix = "view_";
        #endregion

        #region Constructor
        internal ViewClass(Proc.AO.DatasetClass ds, string name)
            : base(ds, Prefix + name)
        {
            //
            this.OriginalName = name;

            // Assure fields
            this.Fields = this.Object.GetAsBObject("fields");
        }
        #endregion 

        #region Indexer
        public ViewFieldClass this[string name]
        {
            get
            {
                // New?
                if (!this.Cache.ContainsKey(name))
                {
                    // Create
                    this.Cache[name] = new ViewFieldClass(this, name);
                }

                return this.Cache[name];
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The passed name
        /// 
        /// </summary>
        private string OriginalName { get; set; }

        /// <summary>
        /// 
        /// The fields
        /// 
        /// </summary>
        public BsonDocument Fields { get; private set; }

        /// <summary>
        /// 
        /// Release of defnition
        /// 
        /// </summary>
        public string Release
        {
            get { return this.Object["release"]; }
            private set { this.Object["release"] = value; ; }
        }

        /// <summary>
        /// 
        /// Based on defnition name
        /// 
        /// </summary>
        public string BasedOn
        {
            get { return this.Object["basedon"]; }
            set { this.Object["basedon"] = value; ; }
        }

        /// <summary>
        /// 
        /// Exclude fields
        /// 
        /// </summary>
        //public string Exclude
        //{
        //    get { return this.Object["exclude"]; }
        //    set { this.Object["exclude"]=value;; }
        //}

        /// <summary>
        /// 
        /// Caption if tab
        /// 
        /// </summary>
        public string Caption
        {
            get { return this.Object["caption"]; }
            set { this.Object["caption"] = value; ; }
        }

        /// <summary>
        /// 
        /// Selector that must be active when view used in a tab
        /// 
        /// </summary>
        public string Selector
        {
            get { return this.Object["selector"]; }
            set { this.Object["selector"] = value; ; }
        }

        /// <summary>
        /// 
        /// The cache
        /// 
        /// </summary>
        private Dictionary<string, ViewFieldClass> Cache { get; set; } = new Dictionary<string, ViewFieldClass>();

        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Saves the definition
        /// 
        /// </summary>
        public void Save()
        {
            // Must have a dataset
            if (this.Parent.Parent.AllDatasets.Contains(this.Parent.Name))
            {
                //
                this.Object.SetAsBObject("fields", this.Fields);

                // Save
                this.Object.Save(force: true);

                // Signal
                this.Parent.Parent.Parent.SignalChange(this);

                this.Parent.Parent.RemoveFromCache(this.OriginalName);
            }
        }

        /// <summary>
        /// 
        /// Checks to see if the release changed
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <returns></returns>
        public bool ReleaseChanged(string release)
        {
            // Is it different?
            bool bAns = !release.IsSameValue(this.Release);
            // Is it private?
            if (bAns) bAns = !this.Release.IsSameValue("private");
            // If changed, sabve new
            if (bAns) this.Release = release;

            return bAns;
        }

        /// <summary>
        /// 
        /// Removes a field
        /// 
        /// </summary>
        /// <param name="field"></param>
        public void Remove(string field)
        {
            // Check
            if (this.Fields.Contains(field))
            {
                // Remove
                this.Fields.Remove(field);
            }
        }

        /// <summary>
        /// 
        /// Creates a tab rack field
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ViewFieldClass AsTabs(string name)
        {
            // Assure in ds
            DatasetFieldClass c_DFld = this.Parent.Definition[name];
            if (c_DFld.Type != DatasetFieldClass.FieldTypes.Tabs)
            {
                c_DFld.Type = DatasetFieldClass.FieldTypes.Tabs;
                c_DFld.Label = "";
                c_DFld.SaveParent();
            }

            ViewFieldClass c_Field = this[name];
            c_Field.Width = "default.tabWidth";

            return c_Field;
        }

        /// <summary>
        /// 
        /// Clear all the fields
        /// 
        /// </summary>
        public void ClearFields()
        {
            // 
            this.Fields.Clear();
        }

        /// <summary>
        /// 
        /// Generate a view form the field list in dataset
        /// 
        /// </summary>
        public void FromFields(params string[] labels)
        {
            // Clear
            this.ClearFields();

            // Get list
            List<string> c_Fields = new List<string>(this.Parent.Definition.FieldNames);

            // Starting row
            int iRow = 1;

            // Loop thru
            foreach (string sField in c_Fields)
            {
                // Get field
                DatasetFieldClass c_Field = this.Parent.Definition[sField];

                // Hidden?
                if (c_Field.Type != DatasetFieldClass.FieldTypes.Hidden)
                {
                    ViewFieldClass c_Entry = null;

                    // Loop thru labels
                    for (int i = 0; i < labels.Length; i += 2)
                    {
                        // Get
                        string sParent = labels[i];
                        string sCaption = labels[i + 1];
                        string sLabel = sCaption.AsFieldName();

                        // Match?
                        if (sField.IsSameValue(sParent))
                        {
                            c_Entry = this[sLabel];
                            if (c_Entry == null)
                            {
                                // Make in dataset
                                Definitions.DatasetFieldClass c_DSField = this.Parent.Definition[sLabel];
                                c_DSField.Type = DatasetFieldClass.FieldTypes.Label;
                                // And get
                                c_Entry = this[sLabel];
                            }
                            c_Entry.Label = sCaption;
                            c_Entry.Type = Definitions.DatasetFieldClass.FieldTypes.Label;
                            c_Entry.SetBounds(iRow.ToString());
                            // Move on
                            iRow += c_Entry.Height.ToInteger();
                        }
                    }

                    // Make field
                    c_Entry = this[sField];
                    c_Entry.Label = c_Field.Label.IfEmpty(NX.Engine.WesternNameClass.CapEachWord(sField));
                    c_Entry.Type = Definitions.DatasetFieldClass.FieldTypes.UseDataset;
                    c_Entry.SetBounds(iRow.ToString());

                    // Move on
                    iRow += c_Entry.Height.ToInteger(1);
                }
            }
        }

        /// <summary>
        /// 
        /// Maps multiple fields
        /// 
        /// </summary>
        /// <param name="fields"></param>
        public void UseFields(params string[] fields)
        {
            // Clear
            this.ClearFields();

            // Starting row
            int iRow = 1;

            // Loop thru
            foreach (string sField in fields)
            {
                // Get from dataset
                Definitions.DatasetFieldClass c_Field = this.Parent.Definition[sField];

                // Hidden?
                if (c_Field.Type != DatasetFieldClass.FieldTypes.Hidden)
                {
                    // Make field
                    ViewFieldClass c_Entry = this[sField];

                    c_Entry.Label = c_Field.Label.IfEmpty(NX.Engine.WesternNameClass.CapEachWord(sField));
                    c_Entry.Type = Definitions.DatasetFieldClass.FieldTypes.UseDataset;
                    c_Entry.SetBounds(iRow.ToString());

                    // Move on
                    iRow += c_Entry.Height.ToInteger(1);
                }
            }
        }

        /// <summary>
        /// 
        /// Assures tha a field exists in the view
        /// 
        /// </summary>
        /// <param name="field"></param>
        public void AssureFields(params string[] fields)
        {
            // Set starting row
            int iRow = 1;

            // Make list
            List<string> c_Flds = new List<string>(fields);

            // Loop thru
            foreach (string sFld in this.Object.Fields)
            {
                // Skip ourselves
                if (!c_Flds.Contains(sFld))
                {
                    // Get the def
                    ViewFieldClass c_Fld = this[sFld];

                    // Visible?
                    if (c_Fld.Type != DatasetFieldClass.FieldTypes.Hidden)
                    {
                        // Compute next row
                        int iNRow = c_Fld.Top.ToInteger(1) + c_Fld.Height.ToInteger(1) + 1;
                        // Adjust
                        if (iNRow > iRow) iRow = iNRow;
                    }
                }
            }

            // Loop
            foreach (string sFld in c_Flds)
            {
                // Must have value
                if (sFld.HasValue())
                {
                    // Make
                    ViewFieldClass c_FldN = this[sFld];
                    if (c_FldN.IsNew)
                    {
                        // Set the top
                        c_FldN.Top = iRow.ToString();
                        // Next
                        iRow++;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// Delete the view
        /// 
        /// </summary>
        public void Delete()
        {
            //
            this.Object.Delete();

            // Get the dataset name
            string sDSName = this.Parent.Name.AsDatasetName();
            // Get the view name
            string sViewName = this.Name.AsViewName();

            this.Parent.RemoveFromCache(this.Name.AsViewName());

            // Only if there is a dataset
            if (this.Parent.Parent.AllDatasets.Contains(sDSName))
            {
                // Test
                if (sViewName.IsSameValue("default") || sViewName.StartsWith("_") || sDSName.StartsWith("_"))
                {
                    // Assure exixtance
                    BuiltIn.DefaultClass.Define(this.Parent);
                }
            };

            // Signal
            this.Parent.Parent.Parent.SignalChange(this, true);
        }

        /// <summary>
        /// 
        /// Load from JSON object 
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadFrom(JObject data)
        {
            //
            this.Object.LoadFrom(data);
            this.Fields = this.Object.GetAsBObject("fields");
        }
        #endregion

        #region File list
        /// <summary>
        /// 
        /// Sets a property for multiple fields
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="fields"></param>
        public void Set(string key, string value, params string[] fields)
        {
            //
            List<string> c_Fields = new List<string>(fields);
            // If none, use all
            if(c_Fields.Count == 0)
            {
                c_Fields = new List<string>(this.Cache.Keys);
            }

            // Loop thru
            foreach(string sField in c_Fields)
            {
                this[sField].Set(key, value);
            }
        }
        #endregion
    }
}
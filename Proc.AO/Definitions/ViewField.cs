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
/// Install-Package MongoDb.Bson -Version 2.11.0
///  

using MongoDB.Bson;

using NX.Shared;

namespace Proc.AO.Definitions
{
    public class ViewFieldClass : ChildOfClass<Definitions.ViewClass>
    {
        #region Constructor
        internal ViewFieldClass(Definitions.ViewClass view, string name)
            : base(view)
        {
            // Get
            this.Document = this.Parent.Fields.GetDocument(name);

            // Assure 
            if (!this.aoField.HasValue()) this.aoField = name;
            if (!this.Document.GetField("nxtype").HasValue())
            {
                // Set defaults
                this.Type = DatasetFieldClass.FieldTypes.UseDataset;
                this.Top = "1";
                this.Left = "1";
                this.Height = "1";
                this.Width = "default.fieldWidth";

                // From dataset
                Definitions.DatasetFieldClass c_Field = this.Parent.Parent.Definition[name];
                if (c_Field != null)
                {
                    if (c_Field.Label.HasValue()) this.Label = c_Field.Label;
                    switch(c_Field.Type)
                    {
                        case DatasetFieldClass.FieldTypes.TextArea:
                            if (this.Height.IsSameValue("1")) this.Height = "default.textareaHeight";
                            break;
                    }
                }

                // Mark as new
                this.IsNew = true;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The underlying document
        /// 
        /// </summary>
        internal BsonDocument Document { get; set; }

        /// <summary>
        /// 
        /// The name of the field
        /// 
        /// </summary>
        public string aoField
        {
            get { return this.Document.GetField("aoFld"); }
            set { this.Document.SetField("aoFld", value); }
        }

        public string Top
        {
            get { return this.Document.GetField("top"); }
            set { this.Document.SetField("top", value); }
        }

        public string Left
        {
            get { return this.Document.GetField("left"); }
            set { this.Document.SetField("left", value); }
        }

        public string Height
        {
            get { return this.Document.GetField("height"); }
            set { this.Document.SetField("height", value); }
        }

        public string Width
        {
            get { return this.Document.GetField("width"); }
            set { this.Document.SetField("width", value); }
        }

        public string Label
        {
            get { return this.Document.GetField("label"); }
            set { this.Document.SetField("label", value); }
        }

        public string LabelWidth
        {
            get { return this.Document.GetField("labelWidth"); }
            set { this.Document.SetField("labelWidth", value); }
        }

        public bool ReadOnly
        {
            get { return this.Document.GetField("ro").FromDBBoolean(); }
            set { this.Document.SetField("ro", value.ToDBBoolean()); }
        }

        public string Value
        {
            get { return this.Document.GetField("value"); }
            set { this.Document.SetField("value", value); }
        }

        /// <summary>
        /// 
        /// The type 
        /// 
        /// </summary>
        public DatasetFieldClass.FieldTypes Type
        {
            get
            {
                // Assume default
                DatasetFieldClass.FieldTypes eAns = DatasetFieldClass.FieldTypes.UseDataset;

                // Convert
                try
                {
                    eAns = (DatasetFieldClass.FieldTypes)System.Enum.Parse(typeof(DatasetFieldClass.FieldTypes), this.Document.GetField("nxtype"), true);
                }
                catch { }

                return eAns;
            }
            set { this.Document.SetField("nxtype", value.ToString().ToLower()); }
        }

        public string Views
        {
            get { return this.Document.GetField("gridview"); }
            set { this.Document.SetField("gridview", value); }
        }

        /// <summary>
        /// 
        /// Is the definition new?
        /// 
        /// </summary>
        public bool IsNew { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Sets the logical bounds
        /// 
        /// </summary>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public void SetBounds(string top, string left = null, string height = null, string width = null)
        {
            //
            var eType = this.Type;
            if (eType == DatasetFieldClass.FieldTypes.UseDataset)
            {
                eType = this.Parent.Parent.Definition[this.aoField].Type;
            }

            //
            if (!left.HasValue()) left = "1";
            //
            if (!height.HasValue())
            {
                // Compute height
                switch (eType)
                {
                    case DatasetFieldClass.FieldTypes.TextArea:
                        height = "3";
                        break;

                    case DatasetFieldClass.FieldTypes.Signature:
                        height = "10";
                        break;

                    default:
                        height = "1";
                        break;
                }
                //
            }
            //
            if (!width.HasValue())
            {
                // Compute width
                switch (eType)
                {
                    case DatasetFieldClass.FieldTypes.Tabs:
                        width = "default.tabWidth";
                        break;

                    default:
                        width = "default.fieldWidth";
                        break;
                }
            }

            //
            this.Top = top;
            this.Left = left;
            this.Height = height;
            this.Width = width;
        }

        /// <summary>
        /// 
        /// Renames the field
        /// 
        /// </summary>
        /// <param name="name">The new name</param>
        public void Rename(string name)
        {
            // Set the new name
            this.aoField = name;
            // And save
            this.Parent.Save();
        }

        /// <summary>
        /// 
        /// copies the field to another one
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void CopyTo(ViewFieldClass target)
        {
            // Loop thru
            foreach (string sField in this.Document.Keys())
            {
                target.Document.Set(sField, this.Document.GetValue(sField));
            }
        }

        /// <summary>
        /// 
        /// Sets a property
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, string value)
        {
            this.Document.SetField(key, value);
        }
        #endregion
    }
}
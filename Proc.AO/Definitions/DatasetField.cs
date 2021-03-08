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

using System.Collections.Generic;

using MongoDB.Bson;

using NX.Shared;
using NX.Engine;

namespace Proc.AO.Definitions
{
    public class DatasetFieldClass : ChildOfClass<Definitions.DatasetClass>
    {
        #region Constructor
        internal DatasetFieldClass(Definitions.DatasetClass dds, string name)
            : base(dds)
        {
            // Get
            this.Document = this.Parent.Fields.GetDocument(name);
            // Assure field name
            if (!this.Name.HasValue())
            {
                this.Name = name;
                this.Label = WesternNameClass.CapEachWord(name);
                this.Type = FieldTypes.String;
            }
        }
        #endregion

        #region Enums
        public enum FieldTypes
        {
            ID,

            AccessPhone,
            Address,
            Addressee,
            Allowed,
            AutoCaps,
            Boolean,
            Button,
            CalendarEvent,
            Caps,
            City,
            ComboBox,
            CreditCard,
            CreditCardExp,
            Currency,
            Date,
            DateTime,
            DriverLicense,
            Duration,
            EMail,
            Float,
            Grid,
            Group,
            Int,
            Image,
            Keyword,
            Label,
            Link,
            ListBox,
            Lower,
            LU,
            Name,
            Password,
            Protected,
            Phone,
            PnoneEMail,
            Signature,
            SSN,
            State,
            String,
            Tabs,
            TextArea,
            Time,
            Timezone,
            Upload,
            User,
            Users,
            VIN,
            ZIP,


           // Memo,
           // Search,
           // SearchText,

           // Addressee,
           // Addresseelist,
           // State,
           // Statename,
           // Stateabbrev,
            
           // PositiveInt,
            
           // Combo,
           // Multicombo,
           // Checkbox,
           // Radiobox,

           // AccPhone,
           // WiredPhone,
           // SipPhone,
           // IntlPhone,

           // Fax,
           // AccFax,

           // Email,
           // Accemail,

           // Printer,
           // Site,
           // Link,
           // LU,
           // Upload,
           // RO,
           // Image,
           // Document,
           // Signature,

           // Billing,

            Hidden,
            XOpenMode,
            UseDataset
        }

        public enum QueryTypes
        {
            Invalid, 

            String,
            Boolean,
            Double,
            DateTime
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
        public string Name
        {
            get { return this.Document.GetField("name"); }
            set { this.Document.SetField("name", value); }
        }

        /// <summary>
        /// 
        /// The labelf for the field
        /// 
        /// </summary>
        public string Label
        {
            get { return this.Document.GetField("label"); }
            set { this.Document.SetField("label", value); }
        }

        /// <summary>
        /// 
        /// The type 
        /// 
        /// </summary>
        public FieldTypes Type
        {
            get
            {
                // Assume default
                FieldTypes eAns = FieldTypes.String;

                // Convert
                try
                {
                    eAns = (FieldTypes)System.Enum.Parse(typeof(FieldTypes), this.Document.GetField("nxtype"), true);
                }
                catch { }

                return eAns;
            }
            set { this.Document.SetField("nxtype", value.ToString().ToLower()); }
        }

        /// <summary>
        /// 
        /// Type to use in MongoDb filter
        /// 
        /// </summary>
        public QueryTypes QueryType
        {
            get
            {
                // Assume 
                QueryTypes eAns = QueryTypes.String;

                // According to type
                switch (this.Type)
                {
                    case FieldTypes.Boolean:
                        eAns = QueryTypes.Boolean;
                        break;

                    case FieldTypes.Int:
                    case FieldTypes.Float:
                    case FieldTypes.Currency:
                    case FieldTypes.Duration:
                        eAns = QueryTypes.Double;
                        break;

                    case FieldTypes.Date:
                    case FieldTypes.Time:
                    case FieldTypes.DateTime:
                        eAns = QueryTypes.String;
                        break;

                    case FieldTypes.Grid:
                    case FieldTypes.Image:
                    case FieldTypes.Tabs:
                        eAns = QueryTypes.Invalid;
                        break;
                }

                return eAns;
            }
        }

        /// <summary>
        /// 
        /// The starting value of the field
        /// 
        /// </summary>
        public string DefaultValue
        {
            get { return this.Document.GetField("defaultvalue"); }
            set { this.Document.SetField("defaultvalue", value); }
        }

        /// <summary>
        /// 
        /// The choices for the field
        /// 
        /// </summary>
        public string Choices
        {
            get { return this.Document.GetField("choices").IfEmpty(); }
            set { this.Document.SetField("choices", value); }
        }

        /// <summary>
        /// 
        /// The view to use for the grid
        /// 
        /// </summary>
        public string GridView
        {
            get { return this.Document.GetField("gridview"); }
            set { this.Document.SetField("gridview", value); }
        }

        /// <summary>
        /// 
        /// The dataset to use for the link
        /// 
        /// </summary>
        public string LinkDS
        {
            get { return this.Document.GetField("linkds"); }
            set { this.Document.SetField("linkds", value); }
        }

        /// <summary>
        /// 
        /// The map to use for the lu
        /// 
        /// </summary>
        public string LUMap
        {
            get { return this.Document.GetField("lumap"); }
            set { this.Document.SetField("lumap", value); }
        }

        /// <summary>
        /// 
        /// The field that holds the name
        /// 
        /// </summary>
        public string Reference
        {
            get { return this.Document.GetField("ref"); }
            set { this.Document.SetField("ref", value); }
        }

        /// <summary>
        /// 
        /// Is the field indexed?
        /// 
        /// </summary>
        public bool UseIndex
        {
            get { return this.Document.GetField("isindex").FromDBBoolean(); }
            set { this.Document.SetField("linkds", value.ToDBBoolean()); }
        }
        #endregion

        #region Related
        /// <summary>
        /// 
        /// Related fields
        /// 
        /// </summary>
        public string RelatedName
        {
            get { return this.Document.GetField("relname"); }
            set { this.Document.SetField("relname", value); }
        }

        public string RelatedAddress
        {
            get { return this.Document.GetField("reladdr"); }
            set { this.Document.SetField("reladdr", value); }
        }

        public string RelatedCity
        {
            get { return this.Document.GetField("relcity"); }
            set { this.Document.SetField("relcity", value); }
        }

        public string RelatedState
        {
            get { return this.Document.GetField("relstate"); }
            set { this.Document.SetField("relstate", value); }
        }

        public string RelatedZIP
        {
            get { return this.Document.GetField("relzip"); }
            set { this.Document.SetField("relzip", value); }
        }

        public string RelatedPhone
        {
            get { return this.Document.GetField("relphone"); }
            set { this.Document.SetField("relphone", value); }
        }

        public string RelatedEmail
        {
            get { return this.Document.GetField("relemail"); }
            set { this.Document.SetField("relemail", value); }
        }

        public string RelatedSubject
        {
            get { return this.Document.GetField("relsubj"); }
            set { this.Document.SetField("relsubj", value); }
        }

        public string RelatedDescription
        {
            get { return this.Document.GetField("reldesc"); }
            set { this.Document.SetField("reldesc", value); }
        }

        public string RelatedLocation
        {
            get { return this.Document.GetField("relloc"); }
            set { this.Document.SetField("relloc", value); }
        }

        public string RelatedStartDate
        {
            get { return this.Document.GetField("relstarton"); }
            set { this.Document.SetField("relstarton", value); }
        }

        public string RelatedEndDate
        {
            get { return this.Document.GetField("relendon"); }
            set { this.Document.SetField("relendon", value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Saves the definition
        /// 
        /// </summary>
        public void Save()
        {
            // Set
            this.Parent.Fields.Set(this.Name, this.Document);

            // Call parent
            this.Parent.Save();
        }

        /// <summary>
        /// 
        /// Saves the definition
        /// 
        /// </summary>
        public void SaveParent()
        {
            // Call parent
            this.Parent.Save();
        }

        /// <summary>
        /// 
        /// Renames a field
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void Rename(string name)
        {
            // Loop thru
            foreach (string sView in this.Parent.Parent.Views)
            {
                // Get the view
                ViewClass c_View = this.Parent.Parent.View(sView);
                // Get the field
                ViewFieldClass c_Field = c_View[this.Name];
                // Exists?
                if (c_Field != null)
                {
                    // Rename
                    c_Field.Rename(name);
                }
            }

            // Delete
            this.Parent.RemoveField(this.Name);
            // Set the name
            this.Name = name;
            this.Label = WesternNameClass.CapEachWord(name);
            // And save
            this.Save();
        }

        /// <summary>
        /// 
        /// Standarizes a value to the MongoDb value
        /// 
        /// </summary>
        /// <param name="value">The storeable value</param>
        /// <returns>The usable value</returns>
        public string StandarizeDBValue(string value)
        {
            // Any value?
            if(value.HasValue())
            {
                switch(this.QueryType)
                {
                    case QueryTypes.Boolean:
                        value = value.ToBoolean().ToString();
                        break;

                    case QueryTypes.Double:
                        value = value.ToDouble(0).ToString();
                        break;

                    case QueryTypes.DateTime:
                        value = value.FromDBDate().ToDBDate();
                        break;
                }
            }

            return value;
        }
        #endregion
    }
}
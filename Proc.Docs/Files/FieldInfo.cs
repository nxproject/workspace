///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
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
/// 
/// Install-Package itextSharp -Version 5.5.13.1
/// 

using System.Collections.Generic;

using iTextSharp.text.pdf;

using NX.Engine;
using NX.Shared;

namespace Proc.Docs.Files
{
    /// <summary>
    /// 
    /// The information for a merge field
    /// 
    /// </summary>
    public class FieldInfoClass
    {
        #region Constructor
        /// <summary>
        /// 
        /// Constructor (.docx field)
        /// 
        /// </summary>
        /// <param name="name">The field name</param>
        public FieldInfoClass(string name)
        {
            //
            this.Name = name;
            this.Locations = new List<LocationClass>();
            this.Type = Types.Text;
        }

        /// <summary>
        /// 
        /// Constructor (.pdf field)
        /// 
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="item">The AcroFields.Item definition</param>
        /// <param name="positions">List of croFields.FieldPosition positions</param>
        /// <param name="type">The type from AcroFields.FIELD_TYPE_xxx</param>
        public FieldInfoClass(string name,
                                AcroFields.Item item,
                                IList<AcroFields.FieldPosition> positions,
                                int type)
            : this(name)
        {
            // Locations
            int iAt = 0;
            foreach (AcroFields.FieldPosition c_Pos in positions)
            {
                string sValue = "";

                PdfDictionary c_Dict = item.GetValue(iAt) as PdfDictionary;
                PdfName c_Value = c_Dict.GetAsName(PdfName.AS);
                if (c_Value != null)
                {
                    sValue = c_Value.ToString().Substring(1);
                }

                this.Locations.Add(new LocationClass(c_Pos, sValue));
                iAt++;
            }

            // Type
            switch (type)
            {
                case AcroFields.FIELD_TYPE_CHECKBOX:
                    this.Type = Types.CheckBox;
                    break;

                case AcroFields.FIELD_TYPE_COMBO:
                    this.Type = Types.ComboBox;
                    break;

                case AcroFields.FIELD_TYPE_LIST:
                    this.Type = Types.ListBox;
                    break;

                case AcroFields.FIELD_TYPE_RADIOBUTTON:
                    this.Type = Types.RadioButton;
                    break;

                case AcroFields.FIELD_TYPE_NONE:
                case AcroFields.FIELD_TYPE_PUSHBUTTON:
                    this.Type = Types.Other;
                    break;

                case AcroFields.FIELD_TYPE_SIGNATURE:
                    this.Type = Types.Text;
                    break;

                case AcroFields.FIELD_TYPE_TEXT:
                    this.Type = Types.Text;
                    break;

            }
        }
        #endregion

        #region Enums
        public enum Types
        {
            Text,
            CheckBox,
            RadioButton,
            ListBox,
            ComboBox,
            Other
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public Types Type { get; set; }

        public List<LocationClass> Locations { get; set; }

        public string LocationValue
        {
            get
            {
                string sAns = "";

                if (this.Locations.Count > 0)
                {
                    sAns = this.Locations[0].Value;
                }

                return sAns.IfEmpty("???");
            }
        }

        public string CurrentValue { get; set; }
        #endregion

        #region Methods
        public void SetCBValues(List<string> values)
        {
            if (values.Count > 1)
            {
                for (int i = values.Count - 1; i >= 0; i--)
                {
                    if (values[i].IsSameValue("Off")) values.RemoveAt(i);
                }
            }

            for (int iLoop = 0; iLoop < this.Locations.Count; iLoop++)
            {
                this.Locations[iLoop].CBValue = values[0];

                values.Add(values[0]);
                values.RemoveAt(0);
            }
        }

        public void SetCBValue(string value)
        {
            for (int iLoop = 0; iLoop < this.Locations.Count; iLoop++)
            {
                this.Locations[iLoop].CBValue = value;
            }
        }
        #endregion

        public class LocationClass
        {
            #region Constructor
            public LocationClass(AcroFields.FieldPosition pos, string value)
            {
                //
                this.Left = pos.position.Left;
                this.Top = pos.position.Top;
                this.Width = pos.position.Right - this.Left;
                this.Height = this.Top - pos.position.Bottom;

                this.Value = value;
                this.Page = pos.page - 1;
            }
            #endregion

            #region Properties
            public string Value { get; internal set; }

            public int Page { get; internal set; }

            public float Left { get; internal set; }
            public float Top { get; internal set; }
            public float Width { get; internal set; }
            public float Height { get; internal set; }

            public string CBValue { get; set; }
            #endregion
        }
    }
}
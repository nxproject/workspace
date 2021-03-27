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
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.AO;

namespace Proc.Docs.Files
{
    /// <summary>
    /// 
    /// A document merge map.
    /// 
    /// </summary>
    public class MergeMapClass : ChildOfClass<DocumentClass>
    {
        #region Constants
        public const string MapFileExtension = "mergemap";
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// Constructor
        /// 
        /// </summary>
        /// <param name="doc">The document where thae map is kept</param>
        internal MergeMapClass(DocumentClass doc)
            : base(doc)
        {
            // Set the document
            this.Map = doc.MetadataDocument(MapFileExtension);

            // New?
            if(true || !this.Map.Exists || this.Map.WrittenOn < this.Parent.WrittenOn)
            {
                // Start with nothing
                JObject c_Fields = new JObject();

                // And temp item
                List<FieldInfoClass> c_Wkg = null;  
                    
                    // According to the type
                switch (doc.Extension)
                {
                    case "odt":
                        using (ODTClass c_Filler = new ODTClass())
                        {
                            // And merge
                            c_Wkg = c_Filler.Fields(doc.ODT());
                        }
                        break;

                    case "pdf":
                    case "fdf":
                        using (PDFClass c_Filler = new PDFClass())
                        {
                            c_Wkg = c_Filler.Fields(doc.PDF());
                        }
                        break;
                }

                // Get current
                JObject c_Current = doc.MetadataDocument(MapFileExtension).Value.ToJObject();

                // Make field list
                if (c_Wkg != null)
                {
                    // Loop thru
                    foreach (FieldInfoClass c_Field in c_Wkg)
                    {
                        c_Fields.Set(c_Field.Name, c_Current.Get(c_Field.Name).IfEmpty());
                    }
                }

                // Save
                this.Map.Value = c_Fields.ToSimpleString();
            }

            //
            this.Values = this.Map.Value.ToJObject();
        }
        #endregion

        #region Enums
        public enum PPDocTypes
        {
            PreDoc,
            PostDoc
        }
        #endregion

        #region Indexer
        /// <summary>
        /// 
        /// The merge fields
        /// 
        /// </summary>
        /// <param name="key">The field name</param>
        /// <returns>The merge expression</returns>
        public string this[string key]
        {
            get { return this.Values.Get(key); }
            set { this.Values.Set(key, value); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The document that holds the map
        /// 
        /// </summary>
        private DocumentClass Map { get; set; }

        /// <summary>
        /// The contents
        /// </summary>
        private JObject Values { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Loads a JSON object
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Load(JObject values)
        {
            this.Values = values;
        }

        /// <summary>
        /// 
        /// Returns the string representation of the map merge document
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Values.ToSimpleString();
        }

        /// <summary>
        /// 
        /// Save the map merge document
        /// 
        /// </summary>
        public void Save()
        {
            this.Map.Value = this.Values.ToSimpleString();
        }

        /// <summary>
        /// 
        /// Evaluate the map against a set of values
        /// 
        /// </summary>
        /// <param name="data">The store that holds the values</param>
        /// <returns></returns>
        public ExtendedContextClass Eval(ExtendedContextClass ctx)
        {
            // Do each entry
            foreach (string sKey in this.Values.Keys())
            {
                // Simple eval
                ctx[sKey] = ctx.Eval(this.Values.Get(sKey)).Value;
            }

            return ctx;
        }
        #endregion
    }
}
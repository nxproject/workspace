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

using System.Collections.Generic;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.Docs.Files;
using Proc.AO;

namespace Proc.Docs.Files
{
    public class PDFDocumentClass : ChildOfClass<DocumentClass>
    {
        #region Constructor
        public PDFDocumentClass(DocumentClass odoc)
            : base(odoc)
        {
            // Assume PDF
            DocumentClass c_Ans = null;

            // Handle easy case
            if (this.Parent.Extension.IsSameValue("pdf"))
            {
                c_Ans = this.Parent;
            }
            else
            {
                // Get from metadata folder
                DocumentClass c_Poss = this.Parent.MetadataDocument("pdf");
                // Is it later?
                bool bNeedProc = this.Parent.Exists && (!c_Poss.Exists || c_Poss.WrittenOn < this.Parent.WrittenOn);

                // Do we need to do?
                if (!bNeedProc)
                {
                    c_Ans = c_Poss;
                }
                else
                {
                    // Convert
                    switch (this.Parent.Extension.ToLower())
                    {
                        case "docx":
                            // 
                            c_Poss.ValueAsBytes = this.ValueAsPDF();
                            c_Ans = c_Poss;
                            break;

                        case "jpeg":
                        case "jpg":
                        case "png":
                            break;
                    }
                }
            }

            this.Document =  c_Ans;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The underlying document
        /// 
        /// </summary>
        public DocumentClass Document { get; private set; }

        /// <summary>
        /// 
        /// Make it look like a document
        /// 
        /// </summary>
        public string Path {  get { return this.Document.Path; } }

        public string Location {  get { return this.Document.Location; } }

        public byte[] ValueAsBytes {  get { return this.Document.ValueAsBytes; } }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Gets/sets the value of the file as a PDF string
        /// 
        /// </summary>
        private byte[] ValueAsPDF()
        {
            // Assume noting
            byte[] abAns = null;

            // According to type
            switch (this.Parent.Extension.ToLower())
            {
                case "pdf":
                    abAns = this.Parent.ValueAsBytes;
                    break;

                case "docx":
                    using (HTMLDocumentClass c_HTML = this.Parent.HTML())
                    {
                        abAns = ConversionClass.HTML2PDF(c_HTML.Value);
                    }
                    break;
            }

            return abAns;
        }

        /// <summary>
        /// 
        /// Merges the document with a given store of data
        /// 
        /// </summary>
        public void Merge(DocumentClass result, ExtendedContextClass ctx)
        {
            // Create support object for Adobe
            using (PDFClass c_Filler = new PDFClass())
            {
                // And merge
                result.ValueAsBytes = c_Filler.Merge(this.ValueAsBytes, ctx);
            }
        }
        #endregion
    }
}
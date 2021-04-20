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
/// Install-Package itextSharp -Version 5.5.13.1
/// Install-Package itextSharp.xmlworker -Version 5.5.13.1
/// 

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

using NX.Engine;
using NX.Shared;
using NX.Engine.Files;
using Proc.AO;

namespace Proc.Docs.Files
{
    public static class Extensions
    {
        #region Merge
        /// <summary>
        /// 
        /// The merge map
        /// 
        /// </summary>
        public static MergeMapClass MergeMap(this DocumentClass doc)
        {
            // Is it cached?
            if (doc.Storage == null)
            {
                // Load
                doc.Storage = new MergeMapClass(doc);
            }

            return doc.Storage as MergeMapClass;
        }

        /// <summary>
        /// 
        /// Merges the document with a given store of data
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data"></param>
        public static void Merge(this DocumentClass doc, DocumentClass result, ExtendedContextClass ctx, Func<string, string> preproc)
        {
            // According to type
            switch (doc.Extension)
            {
                case "odt":
                    doc.ODT().Merge(result, ctx, preproc);
                    break;

                case "pdf":
                case "fdf":
                    doc.PDF().Merge(result, ctx);
                    break;

                default:
                    // Otherwise an empty file
                    result.Value = "";
                    break;
            }
        }
        #endregion

        #region PDF
        /// <summary>
        /// 
        /// Returns the PDF version of the file
        /// 
        /// </summary>
        public static PDFDocumentClass PDF(this DocumentClass doc)
        {
            return new PDFDocumentClass(doc);
        }
        #endregion

        #region HTML
        /// <summary>
        /// 
        /// Returns the HTML version of the file
        /// 
        /// </summary>
        public static HTMLDocumentClass HTML(this DocumentClass doc, bool uniqueid = false)
        {
            return new HTMLDocumentClass(doc, uniqueid);
        }
        #endregion

        #region ODT
        /// <summary>
        /// 
        /// Returns the ODT version of the file
        /// 
        /// </summary>
        public static ODTDocumentClass ODT(this DocumentClass doc)
        {
            return new ODTDocumentClass(doc);
        }
        #endregion
    }
}

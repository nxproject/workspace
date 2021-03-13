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
    public class HTMLDocumentClass : ShadowDocumentClass
    {
        #region Constructor
        public HTMLDocumentClass(DocumentClass odoc, bool uniqueid)
            : base(odoc, "html")
        {
        // TBD hadle uniqueid
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Writes back the DOCX file
        /// 
        /// </summary>
        public void Synchronize()
        {
            // And now back
            ConversionClass.Convert(this.Document, this.Parent);
            // And flag so it does not have to recompute
            this.Location.TouchLastWriteFromPath();
        }

        /// <summary>
        /// 
        /// Merges the document with a given store of data
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data"></param>
        public void Merge(DocumentClass result, ExtendedContextClass ctx)
        {
            // Create support object for MS Word
            using (HTMLSupportClass c_Filler = new HTMLSupportClass(this))
            {
                using (HTMLDocumentClass c_Result = result.HTML())
                {
                    // And merge
                    c_Result.ValueAsBytes = c_Filler.Merge(ctx);
                    c_Result.Synchronize();
                }
            }
        }
        #endregion
    }
}
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
using System.IO;
using System.Text.RegularExpressions;
using System;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.Docs.Files;
using Proc.AO;

namespace Proc.Docs.Files
{
    public class ODTDocumentClass : ShadowDocumentClass
    {
        #region Constructor
        public ODTDocumentClass(DocumentClass odoc)
            : base(odoc, "odt")
        { }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Merges the document with a given store of data
        /// 
        /// </summary>
        public void Merge(DocumentClass result, ExtendedContextClass ctx, Func<string, string> preproc)
        {
            // Create support object for Adobe
            using (ODTClass c_Filler = new ODTClass())
            {
                // And merge
                c_Filler.Merge(this, ctx, preproc, result);
            }

            //
            using (ODTClass c_Cvt = new ODTClass())
            {
                c_Cvt.Merge(this, ctx, preproc, this.Parent);
            }
        }
        #endregion
    }
}
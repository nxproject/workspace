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
    public class ShadowDocumentClass : ChildOfClass<DocumentClass>
    {
        #region Constructor
        public ShadowDocumentClass(DocumentClass odoc, string ext)
            : base(odoc)
        {
            //
            this.Document = this.Parent.MetadataDocument(null, ext);
            // Convert
            ConversionClass.Convert(this.Parent, this.Document);
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
        public string Path { get { return this.Document.Path; } }

        public string Location { get { return this.Document.Location; } }

        public byte[] ValueAsBytes 
        { 
            get { return this.Document.ValueAsBytes; } 
            set { this.Document.ValueAsBytes = value; }
        }
        #endregion
    }
}
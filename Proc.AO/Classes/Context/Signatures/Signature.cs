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

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;

namespace Proc.AO
{
    public class SignatureClass : IDisposable
    {
        #region Constructor
        public SignatureClass(SignatureTypes type, string value)
        {
            // Save
            this.Signature = value;
            // And fingerprint
            this.Fingerprint = this.GetResource("signature_{0}.md5".FormatString(type.ToString().ToLower())).FromBytes();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Enums
        public enum SignatureTypes
        {
            User,
            Object
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The signature as dataURL
        /// 
        /// </summary>
        public string Signature { get; private set; }

        /// <summary>
        /// 
        /// As an image
        /// 
        /// </summary>
        public byte[] AsImage
        {
            get { return this.Signature.Substring(1 + this.Signature.IndexOf(",")).FromBase64Bytes(); }
        }

        /// <summary>
        /// 
        /// The fingerprint to match
        /// 
        /// </summary>
        public string Fingerprint { get; private set; }
        #endregion
    }
}

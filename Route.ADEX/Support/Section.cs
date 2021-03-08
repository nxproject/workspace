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

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;

namespace Route.ADEX
{
    public class SectionClass : ChildOfClass<InfoClass>
    {
        #region Constants
        private const string KeyUUID = "uuid";
        private const string KeyObject = "obj";
        private const string KeyURL = "url";
        private const string KeyDocs = "docs";
        #endregion

        #region Constructor
        internal SectionClass(InfoClass info, JObject values)
            : base(info)
        {
            //
            this.Values = values;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Holding area
        /// 
        /// </summary>
        private JObject Values { get; set; }

        /// <summary>
        /// 
        /// The site UUID
        /// 
        /// </summary>
        public string UUID
        {
            get { return this.Values.Get(KeyUUID); }
            set { this.Values[KeyUUID] = value; }
        }

        /// <summary>
        /// 
        /// The object UUID
        /// 
        /// </summary>
        public string Object
        {
            get { return this.Values.Get(KeyObject); }
            set { this.Values[KeyObject] = value; }
        }

        /// <summary>
        /// 
        /// The site URL
        /// 
        /// </summary>
        public string URL
        {
            get { return this.Values.Get(KeyURL); }
            set { this.Values[KeyURL] = value; }
        }

        /// <summary>
        /// 
        /// The documents
        /// 
        /// </summary>
        public List<string> documents
        {
            get { return this.Values.Get(KeyDocs).SplitSpaces(); }
            set { this.Values[KeyDocs] = value.JoinSpaces(); }
        }
        #endregion
    }
}
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
    public class InfoClass : IDisposable
    {
        #region Constants
        private const string KeyOriginator = "orig";
        private const string KeyProcessor = "proc";
        private const string KeyFn = "fn";
        #endregion

        #region Constructor
        public InfoClass()
        {
            //
            this.Payload = new JObject();
        }

        public InfoClass(string payload)
        {
            //
            this.Payload = payload.ToJObject();
            if (this.Payload == null) this.Payload = new JObject();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The payload
        /// 
        /// </summary>
        private JObject Payload { get; set; }

        public string Fn
        {
            get { return this.Payload.Get(KeyFn); }
            set { this.Payload.Set(KeyFn, value); }
        }

        /// <summary>
        /// 
        /// The information from the originator
        /// 
        /// </summary>
        public SectionClass Originator {  get { return new SectionClass(this, this.Payload.GetJObject(KeyOriginator)); } }

        /// <summary>
        /// 
        /// The information for the processor
        /// 
        /// </summary>
        public SectionClass Processor { get { return new SectionClass(this, this.Payload.GetJObject(KeyProcessor)); } }
        #endregion

        #region Methods
        public override string ToString()
        {
            return this.Payload.ToSimpleString();
        }
        #endregion
    }
}
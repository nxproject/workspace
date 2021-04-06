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
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Telemetry
{
    public class SourceClass : ChildOfClass<Proc.AO.ManagerClass>
    {
        #region Constructor
        public SourceClass(Proc.AO.ManagerClass dbmgr, string source, string sourcetype, string campaign = null)
            : base(dbmgr)
        {
            // Set
            JObject c_Values = new JObject();

            c_Values.Set("s", source.IfEmpty());
            c_Values.Set("t", sourcetype.IfEmpty());
            c_Values.Set("c", campaign.IfEmpty());
            c_Values.Set("e", DateTime.Now.ToDBDate());

            // ID
            this.ID = c_Values.ToSimpleString().MD5HashString();

            // Fill
            c_Values.Set("_id", this.ID);

            // Dataset
            AO.DatasetClass c_DS = this.Parent.DefaultDatabase[AO.DatabaseClass.DatasetTelemetry];
            // Save
            c_DS.DataCollection.AddDirect(c_Values.ToSimpleString());
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Th telemetry ID
        /// 
        /// </summary>
        private string ID { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Process the HTML/SMS
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public string FormatURL(string url, string to = null)
        {
            // UserID
            string sUser = to.IfEmpty("broadcast").ToBase64URL();

            // Replace
            return url.Replace("{{publicurl}}", "{{publicurl}}/z/{0}/{1}".FormatString(this.ID, sUser));
        }
        #endregion
    }
}
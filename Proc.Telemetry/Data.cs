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
    public class DataClass : ChildOfClass<AO.DatabaseClass>
    {
        #region Constructor
        public DataClass(AO.DatabaseClass db, string id)
            : base(db)
        {
            //
            this.ID = id;

            this.Values = this.ID.Decompress().ToJObject();
        }

        public DataClass(AO.DatabaseClass db, string template, string via, string campaign)
            : base(db)
        {
            // Set
            this.Values.Set("s", template.IfEmpty());
            this.Values.Set("t", via.IfEmpty());
            this.Values.Set("c", campaign.IfEmpty());
            this.Values.Set("e", DateTime.Now.ToDBDate());

            // ID
            this.ID = this.Values.ToSimpleString().Compress();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Th telemetry ID
        /// 
        /// </summary>
        private string ID { get; set; }

        /// <summary>
        ///  
        /// The values
        /// 
        /// </summary>
        private JObject Values { get; set; } = new JObject();

        /// <summary>
        /// 
        /// Accessors
        /// 
        /// </summary>
        public string Template { get { return this.Values.Get("s"); } }
        public string Via { get { return this.Values.Get("t"); } set { this.Values.Set("t", value); } }
        public string Campaign { get { return this.Values.Get("c"); } }

        /// <summary>
        /// 
        /// Transaction only
        /// 
        /// </summary>
        public string To { get; private set; }
        public bool IsBroadcast { get { return this.To.IsSameValue("broadcast"); } }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Adds a transaction
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="path"></param>
        /// <param name="ip"></param>
        public void AddTransaction(string user, bool decode, string path = null, string ip = null)
        {
            // Save the to
            this.To = user;
            // Try to decode
            try
            {
                if (decode)
                {
                    this.To = this.To.Decompress();
                }
            }
            catch { };

            // Clone
            JObject c_Wkg = this.Values.Clone();

            // Save
            c_Wkg.Set("x", this.To);
            if (path.HasValue()) c_Wkg.Set("r", path);
            if (ip.HasValue()) c_Wkg.Set("i", ip);
            c_Wkg.Set("d", DateTime.Now.ToDBDate());

            // Get the collection
            Proc.AO.CollectionClass c_Coll = this.Parent[Proc.AO.DatabaseClass.DatasetTelemetryData].DataCollection;
            //
            AO.ObjectClass c_Obj = this.Parent[Proc.AO.DatabaseClass.DatasetTelemetryData].New();
            // Fill
            c_Obj.CopyFrom(c_Wkg);
            // Save
            c_Obj.Save();
        }

        /// <summary>
        /// 
        /// Process the HTML/SMS
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public string AddTelemetry(string route, string url, string to = null)
        {
            // UserID
            string sTo = to.IfEmpty("broadcast").Compress();

            // Replace
            return url.Replace("{{publicurl}}", "{{publicurl}}/{2}/{0}/{1}".Replace("{0}", this.ID).Replace("{1}", sTo).Replace("{2}", route));
        }

        public string Shorten(string text, string route, string url, string to = null)
        {
            // Format
            string sTo = this.AddTelemetry(route, "{{publicurl}}", to);

            // Replace
            return text.Replace(url, sTo);
        }

        /// <summary>
        /// 
        /// Replaces all instances of an url with a telemetry version
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="patt"></param>
        /// <param name="url"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public string Replace(string text, string patt, string route, string url, string to = null)
        {
            // Format
            string sTo = this.AddTelemetry(route, url, to);

            // Replace
            return text.Replace(patt, sTo);
        }
        #endregion
    }
}
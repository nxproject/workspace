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
/// Install-Package iTextSharp -Version 5.5.13.1
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;
using Proc.AO;

namespace Proc.Docs.Files
{
    /// <summary>
    /// 
    /// A toolkit to merge into ODT fields
    /// 
    /// </summary>
    public class ODTClass : IDisposable
    {
        #region Constants
        /// <summary>
        /// 
        /// Field matching pattern
        /// 
        /// </summary>
        private const string Pattern = @"\x5B[^\x5D]*\x5D";

        /// <summary>
        /// 
        /// The entry name in th zip that holds the text
        /// 
        /// </summary>
        private const string EntryName = "content.xml";
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// Constructor
        /// 
        /// </summary>
        public ODTClass()
        { }
        #endregion

        #region IDisposable
        /// <summary>
        /// 
        /// Housekeeping
        /// 
        /// </summary>
        public void Dispose()
        { }
        #endregion

        #region Merge
        /// <summary>
        /// 
        /// Merges a store into a .pdf template
        /// 
        /// </summary>
        /// <param name="doc">The byte array of the document contents</param>
        /// <param name="values">The store</param>
        /// <returns>A byte array of the merged document</returns>
        public void Merge(ODTDocumentClass doc, ExtendedContextClass ctx, Func<string, string> preproc, NX.Engine.Files.DocumentClass result)
        {
            // 
            try
            {
                // Get the text
                string sWkg = this.GetContents(doc.Location);
                // Handlebars?
                if (preproc != null) sWkg = preproc(sWkg);

                //// Map the store
                //HandlebarDataClass c_Data = new HandlebarDataClass();
                //c_Data.Merge(ctx.Stores[Names.Passed]);

                //// Process handlebars
                //sWkg = sWkg.Handlebars(c_Data, delegate (string field, object thisvalue)
                //{
                //    // Save this
                //    c_Data.Set("this", thisvalue);

                //    // Eval
                //    return Expression.Eval(ctx, field).Value;
                //});
                // Find all the fields
                MatchCollection c_Matches = Regex.Matches(sWkg, Pattern);
                // Loop thru
                for (int i = c_Matches.Count; i > 0; i--)
                {
                    // Get
                    Match c_Match = c_Matches[i - 1];
                    // Get the pattern
                    string sPatt = c_Match.Value;
                    // Evaluate
                    using (DatumClass c_Datum = new DatumClass(ctx, sPatt))
                    {
                        // Get the value
                        string sRepl = c_Datum.Value.IfEmpty("");
                        // And replace
                        sWkg = sWkg.Replace(sPatt, sRepl);
                    }
                }

                // Write it back
                this.SetContents(result.Location, sWkg);
            }
            catch { }
        }
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// Returns a list of fields in the document
        /// 
        /// </summary>
        /// <param name="doc">The byte array of the document contents</param>
        /// <returns>The list of fields</returns>
        public List<FieldInfoClass> Fields(ODTDocumentClass doc)
        {
            // Assume no fields
            List<FieldInfoClass> c_Ans = new List<FieldInfoClass>();

            try
            {
                // Get the text
                string sWkg = this.GetContents(doc.Location);

                //
                List<string> c_Done = new List<string>();

                // Find all the fields
                MatchCollection c_Matches = Regex.Matches(sWkg, Pattern);
                for (int i = 0; i < c_Matches.Count; i++)
                {
                    // Get
                    Match c_Match = c_Matches[i];
                    // Get the pattern
                    string sPatt = c_Match.Value;
                    // Only once
                    if (c_Done.Contains(sPatt))
                    {
                        c_Done.Add(sPatt);

                        c_Ans.Add(new FieldInfoClass(sPatt.Substring(1, sPatt.Length - 2)));
                    }
                }
            }
            catch { }

            c_Ans.SortFI();

            return c_Ans;
        }
        #endregion

        #region Files
        /// <summary>
        /// 
        /// Get the text
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetContents(string path)
        {
            using (FileStream c_ZStream = new FileStream(path, FileMode.Open))
            {
                using (ZipArchive c_Pkg = new ZipArchive(c_ZStream, ZipArchiveMode.Update))
                {
                    // Find
                    ZipArchiveEntry c_Entry = c_Pkg.GetEntry(EntryName);
                    // Copy
                    using (Stream c_Source = c_Entry.Open())
                    {
                        using (MemoryStream c_Target = new MemoryStream())
                        {
                            c_Source.CopyTo(c_Target);

                            // Get body
                            return c_Target.ToArray().FromBytes();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// Sets the text
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        private void SetContents(string path, string value)
        {
            using (FileStream c_ZStream = new FileStream(path, FileMode.Open))
            {
                using (ZipArchive c_Pkg = new ZipArchive(c_ZStream, ZipArchiveMode.Update))
                {
                    // Find
                    ZipArchiveEntry c_Entry = c_Pkg.GetEntry(EntryName);
                    if (c_Entry != null) c_Entry.Delete();
                    // Copy
                    c_Entry = c_Pkg.CreateEntry(EntryName);

                    using (MemoryStream c_Source = new MemoryStream(value.ToBytes()))
                    {
                        using (Stream c_Target = c_Entry.Open())
                        {
                            c_Source.CopyTo(c_Target);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
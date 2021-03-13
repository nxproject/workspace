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
/// Install-Package docX -Version 1.7.0
/// 

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using NX.Engine;
using NX.Shared;
using Proc.AO;

namespace Proc.Docs.Files
{
    /// <summary>
    /// 
    /// A toolkit to merge .docx files.
    /// 
    /// Fields in the .docx file are in the format [xxxx]
    /// 
    /// </summary>
    public class HTMLSupportClass : ChildOfClass<HTMLDocumentClass>
    {
        #region Constants
        /// <summary>
        /// 
        /// The regular expression pattern to match a field
        /// 
        /// </summary>
        private const string PatternData = @"\x5B(.+?)\x5D";
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// Constructor
        /// 
        /// </summary>
        public HTMLSupportClass(HTMLDocumentClass doc)
            : base(doc)
        { }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The active document
        /// 
        /// </summary>
        //private byte[] Document { get; set; }

        /// <summary>
        /// 
        /// The active key/value pairs to merge
        /// 
        /// </summary>
        private ExtendedContextClass Data { get; set; }

        /// <summary>
        /// 
        /// List of fields found during a fields search
        /// 
        /// </summary>
        private List<string> Found { get; set; } = new List<string>();
        #endregion

        #region Merge
        /// <summary>
        /// 
        /// Merges a store into a .docx template
        /// 
        /// </summary>
        /// <param name="doc">The byte array of the document contents</param>
        /// <param name="values">The store</param>
        /// <returns>A byte array of the merged document</returns>
        public byte[] Merge(ExtendedContextClass ctx)
        {
            // Assume it did not go well
            byte[] c_Ans = null;

            // Save the data
            this.Data = ctx;

            try
            {
                // Get the text
                string sWkg = this.Parent.Parent.Value;
                // Find all the fields
                MatchCollection c_Matches = Regex.Matches(sWkg, @"\x5B[^\x5D]*\x5D");
                // Loop thru
                for(int i=c_Matches.Count; i> 0;i--)
                {
                    // Get
                    Match c_Match = c_Matches[i-1];
                    // Get the pattern
                    string sPatt = c_Match.Value;
                    // Get the vaue
                    string sRepl = this.Data[sPatt].IfEmpty("");
                    // And replace
                    sWkg = sWkg.Substring(0, c_Match.Index) + sRepl + sWkg.Substring(c_Match.Index + c_Match.Length);
                }

                c_Ans = sWkg.ToBytes();
            }
            catch { }

            return c_Ans;
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
        public List<FieldInfoClass> Fields(byte[] doc)
        {
            // Assume no fields
            List<FieldInfoClass> c_Ans = new List<FieldInfoClass>();

            try
            {
                // Get the text
                string sWkg = this.Parent.Parent.Value;
                // Find all the fields
                MatchCollection c_Matches = Regex.Matches(sWkg, @"\x5B[^\x5D]*\x5D");
                //
                List<string> c_Done = new List<string>();
                // Loop thru
                for (int i = 0; i<  c_Matches.Count;i++)
                {
                    // Get
                    Match c_Match = c_Matches[i];
                    // Get the pattern
                    string sPatt = c_Match.Value;
                    // Only once
                    if (c_Done.Contains(sPatt))
                    {
                        this.Parent.Parent.Parent.Parent.LogInfo("PATT: {0}".FormatString(sPatt));
                        c_Done.Add(sPatt);

                        c_Ans.Add(new FieldInfoClass(sPatt.Substring(1, sPatt.Length-2)));
                    }
                }
            }
            catch { }

            c_Ans.SortFI();

            return c_Ans;
        }
        #endregion
    }
}
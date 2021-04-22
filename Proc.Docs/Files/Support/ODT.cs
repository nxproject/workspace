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
        public void Merge(ODTDocumentClass doc,
            ExtendedContextClass ctx,
            Func<string, string> preproc,
            NX.Engine.Files.DocumentClass result,
            string signature)
        {
            // 
            try
            {
                // Open the archive
                using (ZipArchive c_Pkg = ZipFile.Open(doc.Location, ZipArchiveMode.Update))
                {
                    // Get the text
                    string sWkg = this.GetContents(c_Pkg);
                    // Handlebars?
                    if (preproc != null) sWkg = preproc(sWkg);

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
                    this.SetContents(c_Pkg, sWkg);

                    // Handle signature
                    if (signature.HasValue())
                    {
                        this.SetSignature(c_Pkg, signature);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region Archive
        /// <summary>
        /// 
        /// Gets an entry
        /// 
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private byte[] GetEntry(ZipArchive pkg, string name)
        {
            //
            byte[] abAns = null;

            try
            {
                // Find
                ZipArchiveEntry c_Entry = pkg.GetEntry(name);
                // Copy
                using (Stream c_Source = c_Entry.Open())
                {
                    using (MemoryStream c_Target = new MemoryStream())
                    {
                        c_Source.CopyTo(c_Target);

                        // Get body
                        abAns = c_Target.ToArray();
                    }
                }
            }
            catch { }

            return abAns;
        }

        private void SetEntry(ZipArchive pkg, string name, byte[] value)
        {
            // Find
            ZipArchiveEntry c_Entry = pkg.GetEntry(name);
            if (c_Entry != null) c_Entry.Delete();
            // Copy
            c_Entry = pkg.CreateEntry(name);

            using (MemoryStream c_Source = new MemoryStream(value))
            {
                using (Stream c_Target = c_Entry.Open())
                {
                    c_Source.CopyTo(c_Target);
                }
            }
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
                // Open the archive
                using (ZipArchive c_Pkg = ZipFile.Open(doc.Location, ZipArchiveMode.Update))
                {
                    // Get the text
                    string sWkg = this.GetContents(c_Pkg);

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
                        if (!c_Done.Contains(sPatt))
                        {
                            c_Done.Add(sPatt);

                            c_Ans.Add(new FieldInfoClass(sPatt.Substring(1, sPatt.Length - 2)));
                        }
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
        /// <returns></returns>
        private string GetContents(ZipArchive pkg)
        {
            //
            return this.GetEntry(pkg, EntryName).FromBytes();
        }

        /// <summary>
        /// 
        /// Fills in all the signature images
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newvalue"></param>
        private void SetSignature(ZipArchive pkg, string newvalue)
        {
            // Get the contents
            string sContents = this.GetContents(pkg);
            // Look for images
            MatchCollection c_Poss = Regex.Matches(sContents, @"xlink:href=\x22(?<ref>[^\x22]+)");
            // Any?
            if (c_Poss.Count > 0)
            {
                // Loop thru
                foreach (Match c_Match in c_Poss)
                {
                    // Get the name
                    string sName = c_Match.Groups["ref"].Value;

                    // Find
                    byte[] c_Image = this.GetEntry(pkg, sName);
                    string sMD5 = c_Image.MD5Hash().ToBase64();
                    // Is it it?
                    if (sMD5.IsSameValue("zx8/9aERR8EcdIwTNy2zYw=="))
                    {
                        // Do we have a new one?
                        if (newvalue.HasValue())
                        {
                            //
                            this.SetEntry(pkg, sName, newvalue.Substring(1 + newvalue.IndexOf(",")).FromBase64Bytes());
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
        private void SetContents(ZipArchive pkg, string value)
        {
            this.SetEntry(pkg, EntryName, value.ToBytes());
        }
        #endregion
    }
}
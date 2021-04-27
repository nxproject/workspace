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
using System.Text.RegularExpressions;
using System.Text;

using NX.Shared;
using NX.Engine;

namespace Proc.Office
{
    public class SupportClass : IDisposable
    {
        #region Constructor
        public SupportClass()
        { }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Dependency
        /// <summary>
        /// 
        /// Adds one file and its dependants to list
        /// 
        /// </summary>
        /// <param name="path">The root path</param>
        /// <param name="file">The file name</param>
        /// <param name="done">The list of files found</param>
        public void GetItem(HTTPCallClass call,
                                    string path,
                                    string file,
                                    List<string> done,
                                    List<string> nomin,
                                    StringBuilder buffer = null)
        {
            try
            {
                // Pgm?
                bool bIsPgm = file.StartsWith("*");
                if (bIsPgm) file = file.Substring(1);

                // No minify
                bool bNoMinify = file.StartsWith("!");
                if (bNoMinify) file = file.Substring(1);

                //
                if (nomin.Contains(file)) bNoMinify = true;

                // Done already?
                if (!done.Contains(file))
                {
                    // Add
                    done.Add(file);

                    // Is it a folder?
                    if (file.StartsWith("@") || file.DirectoryExists())
                    {
                        // The length to remove
                        int iPrefix = path.Length + 1;

                        // Make the path
                        string sPath = path.CombinePath(file.Substring(1).Replace(".", "/")).AdjustPathToOS();

                        // Does the path exist?
                        if (sPath.DirectoryExists())
                        {
                            // Get the files
                            foreach (string sFile in sPath.GetTreeInPath("*.js"))
                            {
                                // Remove prefix
                                string sFinal = sFile.Substring(iPrefix);
                                // Remove end
                                sFinal = sFinal.Substring(0, sFinal.Length - 3);
                                // Make into name
                                sFinal = sFinal.Replace("/", ".").Replace(@"\", ".");
                                // Add
                                this.GetItem(call, path, sFinal, done, nomin, buffer);
                            }
                        }
                    }
                    else
                    {
                        // Adjust
                        string sPath = path.CombinePath(file.Replace(".", "/") + ".js").AdjustPathToOS();

                        // Exists?
                        if (sPath.FileExists())
                        {
                            //// Get the file
                            string sContents = sPath.ReadFile();
                            // Get requires
                            MatchCollection c_Matches = Regex.Matches(sContents, @"@require\((?<item>[^\)]+)\)");
                            // Loop thru
                            foreach (Match c_Match in c_Matches)
                            {
                                // Validate piece
                                string sFile = this.ValidateName(c_Match.Groups["item"].Value);
                                // Valid?
                                if (sFile.HasValue())
                                {
                                    this.GetItem(call, path, sFile, done, nomin, buffer);
                                }
                            }

                            // Get the matches
                            c_Matches = Regex.Matches(sContents, @"(extend|include|implement)\s*\:\s*");
                            // Loop thru
                            foreach (Match c_Match in c_Matches)
                            {
                                // Get the values
                                this.GetQXItems(call, path, sContents, c_Match.Index + c_Match.Value.Length, done, nomin, buffer);
                            }

                            // If there is a stream, add
                            if (buffer != null)
                            {
                                // Force
                                bNoMinify = true;

                                // Set the file
                                //if (!bNoMinify)
                                //{
                                //    buffer.AppendLine(this.MinifyJS(sPath, sContents));
                                //}
                                //else
                                //{
                                buffer.AppendLine(sContents);
                                //}

                                // Get last written on
                                DateTime? c_On = sPath.GetLastWriteFromPath();
                                // Adjust ETag
                                if (call.RespondLastModified == null || c_On > call.RespondLastModified)
                                {
                                    // Set
                                    call.RespondLastModified = c_On;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                call.Env.LogException("GetItem: {0}".FormatString(path), e);
            }
        }

        /// <summary>
        /// 
        /// Validates a name
        /// 
        /// </summary>
        /// <param name="name">The raw name</param>
        /// <returns>Formatted name or null if not valid</returns>
        public string ValidateName(string name)
        {
            // Assume no
            string sAns = null;

            // Remove spaces
            string sPiece = name.IfEmpty().Replace(" ", "").ASCIIOnly();
            // Remove invalid
            string sDone = Regex.Replace(sPiece, @"[^a-zA-Z0-9\.]", "");
            // Must be valid
            if (sPiece.IsSameValue(sDone) && sPiece.Contains("."))
            {
                sAns = sPiece;
            }

            return sAns;
        }

        /// <summary>
        /// 
        /// Gets the qx dependency items
        /// 
        /// </summary>
        /// <param name="path">The root path</param>
        /// <param name="text">The text to search</param>
        /// <param name="start">The starting postion</param>
        private void GetQXItems(HTTPCallClass call, string path, string text, int start, List<string> done, List<string> nomin, StringBuilder buffer)
        {
            try
            {
                // Compute the length to get
                int iLen = text.Length - start;
                if (iLen > 100) iLen = 100;
                // Start from start
                string sText = text.Substring(start, iLen).Trim();
                // A list?
                if (sText.StartsWith("["))
                {
                    // Find end
                    int iEnd = text.IndexOf("]", start);
                    // Get
                    sText = text.Substring(start, iEnd - start);
                    // Split
                    string[] asPieces = sText.Replace("[", "").Replace("]", "").Split(",");
                    // Loop thru
                    foreach (string sPiece in asPieces)
                    {
                        // Validate
                        string sFile = this.ValidateName(sPiece);
                        // Add if valid
                        if (sFile.HasValue()) this.GetItem(call, path, sFile, done, nomin, buffer);
                    }
                }
                else if (sText.Contains(@"({") && sText.IndexOf(@"({") < sText.IndexOf(","))
                {
                    // Find end
                    int iEnd = text.IndexOf("}", start);
                    // Get
                    sText = text.Substring(start, iEnd - start);

                    // Do the function
                    //this.AddIfValid(list, sText.Substring(0, sText.IndexOf("(")));
                    // Get the rest
                    sText = sText.Substring(1 + sText.IndexOf("{"));
                    // Match
                    MatchCollection c_Matches = Regex.Matches(sText, @"[^\:]+\:\s*(?<item>[a-zA-Z0-9\x2E]+)");
                    // Loop thru
                    foreach (Match c_Match in c_Matches)
                    {
                        // Get the item
                        string sItem = c_Match.Groups["item"].Value;
                        // Validate
                        string sFile = this.ValidateName(sItem);
                        // Add if valid
                        if (sFile.HasValue()) this.GetItem(call, path, sFile, done, nomin, buffer);
                    }
                }
                else
                {
                    // Find end
                    int iEnd = text.IndexOf("\x0a", start);
                    // Any?
                    if (iEnd != -1)
                    {
                        // Get
                        sText = text.Substring(start, iEnd - start);
                        // Remove comma
                        sText = sText.Replace(",", "");
                    }
                    else
                    {
                        // Find comma
                        iEnd = text.IndexOf(",", start);
                        // Any?
                        if (iEnd != -1)
                        {
                            // Get
                            sText = text.Substring(start, iEnd - start);
                        }
                    }

                    // Validate
                    string sFile = this.ValidateName(sText);
                    // Add if valid
                    if (sFile.HasValue()) this.GetItem(call, path, sFile, done, nomin, buffer);
                }
            }
            catch (Exception e)
            {
                call.Env.LogException("GetQXItems: {0}".FormatString(path), e);
            }
        }

        /// <summary>
        /// 
        /// Gets the files in a folder
        /// 
        /// </summary>
        /// <param name="folder">The folder name</param>
        /// <returns></returns>
        private List<string> GetFiles(string folder)
        {
            // Assume none
            List<string> c_Ans = new List<string>();

            try
            {
                // The length to remove
                int iPrefix = folder.IndexOf("UI.qx") + 6;

                // Get the files
                foreach (string sFile in folder.AdjustPathToOS().GetTreeInPath("*.js"))
                {
                    // Remove prefix
                    string sFinal = sFile.Substring(iPrefix);
                    // Remove end
                    sFinal = sFinal.Substring(0, sFinal.Length - 3);
                    // Make into name
                    sFinal = sFinal.Replace("/", ".").Replace(@"\", ".");
                    // Add
                    c_Ans.Add(sFinal);
                }
            }
            catch { }

            return c_Ans;
        }
        #endregion

        #region Minifier
        public string MinifyJS(string path, string code)
        {
            // Assume none
            string sAns = code;

            // The target
            string sTarget = path.Substring(0, path.Length - 2) + "min";

            // Date check
            if (path.GetLastWriteFromPath() >= sTarget.GetLastWriteFromPath())
            {
                // Protect
                try
                {
                    // Remove leading comments
                    while (code.StartsWith("/*"))
                    {
                        code = Regex.Replace(code.Substring(code.IndexOf("*/") + 2), @"^[\s\x0A\x0D]*", "");
                    }
                    // Do URL
                    string sURL = "https://javascript-minifier.com/raw";
                    // And the data
                    byte[] abValue = "input={0}".FormatString(code.URLEncode()).ToBytes();
                    // Minify
                    string sMin = sURL.URLPost(abValue, "Content-Type", "application/x-www-form-urlencoded").FromBytes();
                    // Any?
                    if (!sMin.StartsWith("// Error"))
                    {
                        // Set
                        sAns = sMin;
                    }

                    // Save
                    sTarget.WriteFile(sAns);
                }
                catch { }
            }
            else
            {
                // Get from target
                sAns = sTarget.ReadFile();
            }

            return sAns;
        }
        #endregion
    }
}
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
using System.IO;

using HtmlAgilityPack;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.Docs.Files;
using Proc.AO;

namespace Proc.Docs.Files
{
    public class HTMLDocumentClass : ShadowDocumentClass
    {
        #region Constructor
        public HTMLDocumentClass(DocumentClass odoc, bool uniqueid)
            : base(odoc, "html")
        {
            // Standarize
            this.Document.Value = this.StandarizeHTML(this.Document.Value);

            // Add a unique id to each node
            if (uniqueid)
            {
                HtmlDocument c_Doc = new HtmlDocument();
                c_Doc.LoadHtml(this.Document.Value);
                this.MakeID(c_Doc.DocumentNode, "", 0);
                using (MemoryStream c_Stream = new MemoryStream())
                {
                    c_Doc.Save(c_Stream);
                    this.Document.ValueAsBytes = c_Stream.ToArray();
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Writes back the DOCX file
        /// 
        /// </summary>
        public void Synchronize()
        {
            // Make metadata
            using (DocumentClass c_Docx = this.Parent.MetadataDocument(null, this.Parent.Extension))
            {
                // And now back
                ConversionClass.Convert(this.Document, c_Docx);
                // Copy
                this.Parent.ValueAsBytes = c_Docx.ValueAsBytes;
                // And flag so it does not have to recompute
                this.Location.TouchLastWriteFromPath();
            }
        }

        /// <summary>
        /// 
        /// Add a unique id to each element
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="instance"></param>
        private void MakeID(HtmlNode node, string parent, int instance)
        {
            // Must have anode
            if (node != null && (node.NodeType == HtmlNodeType.Element || node.NodeType == HtmlNodeType.Document))
            {
                // Assure parent
                parent = parent.IfEmpty("root");

                // Assume none
                string sID = null;
                // Do we have an id?
                if (node.Attributes.Contains("id"))
                {
                    sID = node.Attributes["id"].Value;
                }
                else
                {
                    node.Attributes.Add("id", "");
                }

                // One already defined?
                if (!sID.HasValue())
                {
                    sID = parent + "_" + instance;
                    // Save
                    node.Attributes["id"].Value = sID;
                }

                // Setup
                int iCount = 0;

                // Get the children
                foreach (HtmlNode c_Child in node.ChildNodes)
                {
                    this.MakeID(c_Child, sID, iCount);
                    iCount++;
                }
            }
        }

        /// <summary>
        /// 
        /// Fixes tags in HTML
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string StandarizeHTML(string value)
        {
            // Starts properly?
            if (!value.StartsWith(@"<!DOCTYPE"))
            {
                // Something about TiyMCE
                if (value.StartsWith("<p></p>")) value = value.Substring(7);
                // Create new frame
                string sFrame = "<!DOCTYPE html ><html><body>{0}</body></html>";
                // Insert
                value = sFrame.Replace("{0}", value);
            }

            // Close tags
            var hDocument = new HtmlDocument()
            {
                OptionWriteEmptyNodes = true,
                OptionAutoCloseOnEnd = true
            };
            hDocument.LoadHtml(value);
            value = hDocument.DocumentNode.WriteTo();



            // Handle br
            value = value.Replace("<br ", "<p ");

            return value;
        }
        #endregion
    }
}
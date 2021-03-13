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
/// Install-Package itextSharp -Version 5.5.13.1
/// Install-Package itextSharp.xmlworker -Version 5.5.13.1
/// Install-Package OpenXmlPowerTools -Version 4.5.3.2
/// Install-Package DocumentFormat.OpenXML -Version 2.11.3
/// 

using System;
using System.IO;
using System.Xml.Linq;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using OpenXmlPowerTools;
using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using HtmlAgilityPack;

using NX.Engine;
using NX.Shared;
using NX.Engine.Files;

namespace Proc.Docs.Files
{
    public static class ConversionClass
    {
       #region HTML
        /// <summary>
        /// 
        /// Fixes tags in HTML
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StandarizeHTML(string value)
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
            //MatchCollection c_Matches = Regex.Matches(value, @"(?<match>\x3Cbr\s(?<id>id=\x22[^\x22]+\x22)\s\x2F\x3E)");
            //foreach (Match c_Match in c_Matches)
            //{
            //    string sCurrent = c_Match.Groups["match"].Value;
            //    string sNew = "<p " + c_Match.Groups["id"].Value + ">&nbsp;</p>";

            //    value.Replace(sCurrent, sNew);


            //}

            return value;
        }
        #endregion

        #region DOCX
        public static byte[] DOCX2HTML(byte[] value, string name = null, bool uniqueid = false)
        {
            // Assume noting
            byte[] abAns = null;

            // Make name if needed
            if (!name.HasValue())
            {
                name = "F".GUID() + ".docx";
            }

            // Protect
            try
            {
                // Parse
                abAns = ParseDOCX(name, value, uniqueid);
            }
            catch (OpenXmlPackageException e)
            {
                // Image?
                if (e.ToString().Contains("Invalid Hyperlink"))
                {
                    // Make into stream
                    using (MemoryStream c_Stream = new MemoryStream(value))
                    {
                        // Fix
                        UriFixer.FixInvalidUri(c_Stream, brokenUri => FixUri(brokenUri));
                        // Rewind
                        c_Stream.Seek(0, SeekOrigin.Begin);
                        // And again
                        abAns = ParseDOCX(name, c_Stream.ToArray());
                    }
                }
            }

            return abAns;
        }
        #endregion

        #region Support
        public static byte[] ParseDOCX(string name, byte[] value, bool uniqueid = false)
        {
            byte[] abAns = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(value, 0, value.Length);
                using (WordprocessingDocument wDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var pageTitle = name;
                    var part = wDoc.CoreFilePropertiesPart;
                    if (part != null)
                    {
                        pageTitle = (string)part.GetXDocument().Descendants(DC.title).FirstOrDefault() ?? name;
                    }

                    int imageCounter = 0;

                    // TODO: Determine max-width from size of content area.
                    WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings()
                    {
                        //AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
                        PageTitle = pageTitle,
                        FabricateCssClasses = false,
                        CssClassPrefix = "pt-",
                        RestrictToSupportedLanguages = false,
                        RestrictToSupportedNumberingFormats = false,
                        ImageHandler = imageInfo =>
                        {
                            ++imageCounter;
                            string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                            ImageFormat imageFormat = null;
                            if (extension == "png") imageFormat = ImageFormat.Png;
                            else if (extension == "gif") imageFormat = ImageFormat.Gif;
                            else if (extension == "bmp") imageFormat = ImageFormat.Bmp;
                            else if (extension == "jpeg") imageFormat = ImageFormat.Jpeg;
                            else if (extension == "tiff")
                            {
                                extension = "gif";
                                imageFormat = ImageFormat.Gif;
                            }
                            else if (extension == "x-wmf")
                            {
                                extension = "wmf";
                                imageFormat = ImageFormat.Wmf;
                            }

                            if (imageFormat == null) return null;

                            string base64 = null;
                            try
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    imageInfo.Bitmap.Save(ms, imageFormat);
                                    var ba = ms.ToArray();
                                    base64 = System.Convert.ToBase64String(ba);
                                }
                            }
                            catch (System.Runtime.InteropServices.ExternalException)
                            { return null; }

                            ImageFormat format = imageInfo.Bitmap.RawFormat;
                            ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders()
                                                        .First(c => c.FormatID == format.Guid);
                            string mimeType = codec.MimeType;

                            string imageSource =
                                    string.Format("data:{0};base64,{1}", mimeType, base64);

                            XElement img = new XElement(Xhtml.img,
                                    new XAttribute(NoNamespace.src, imageSource),
                                    imageInfo.ImgStyleAttribute,
                                    imageInfo.AltText != null ?
                                        new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                            return img;
                        }
                    };
                    XElement htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);

                    // Produce HTML document with <!DOCTYPE html > declaration to tell the browser
                    // we are using HTML5.
                    var html = new XDocument(
                        new XDocumentType("html", null, null, null),
                        htmlElement);

                    // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
                    // XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
                    // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
                    // for detailed explanation.
                    //
                    // If you further transform the XML tree returned by ConvertToHtmlTransform, you
                    // must do it correctly, or entities will not be serialized properly.

                    var htmlString = html.ToString(SaveOptions.DisableFormatting);

                    // Add a unique id to each node
                    if (uniqueid)
                    {
                        HtmlDocument c_Doc = new HtmlDocument();
                        c_Doc.LoadHtml(htmlString);
                        MakeID(c_Doc.DocumentNode, "", 0);
                        using (MemoryStream c_Stream = new MemoryStream())
                        {
                            c_Doc.Save(c_Stream);
                            abAns = c_Stream.ToArray();
                        }
                    }
                    else
                    {
                        abAns = htmlString.ToBytes();
                    }
                }
            }

            return abAns;
        }

        private static void MakeID(HtmlNode node, string parent, int instance)
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
                    MakeID(c_Child, sID, iCount);
                    iCount++;
                }
            }
        }

        private static Uri FixUri(string brokenUri)
        {
            string newURI = string.Empty;
            if (brokenUri.Contains("mailto:"))
            {
                int mailToCount = "mailto:".Length;
                brokenUri = brokenUri.Remove(0, mailToCount);
                newURI = brokenUri;
            }
            else
            {
                newURI = " ";
            }
            return new Uri(newURI);
        }
        #endregion

        #region Conversion

        /// <summary>
        /// 
        /// LibreOffice interface
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="doc"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static DocumentClass Convert(DocumentClass doc, DocumentClass to)
        {
            // Assue failure
            DocumentClass c_Ans = null;

            // Make the URL
            string sURL = doc.Parent.Parent.LoopbackURL.URLMake("libreoffice", "convert",  doc.Path.ToBase64URL(), to.Path.ToBase64URL());
            // Call
            JObject c_Resp = sURL.URLGet().FromBytes().ToJObject();
            // Any?
            if (c_Resp != null)
            {
                // Get the file
                string sPath = c_Resp.Get("path");
                // Any?
                if(sPath.HasValue())
                {
                    // Map
                    c_Ans = new DocumentClass(doc.Parent, sPath);
                }
            }

            return c_Ans;
        }
        #endregion
    }

    #region Support
    internal class HtmlToWmlReadAsXElement
    {
        public static XElement ReadAsXElement(byte[] value)
        {
            string htmlString = value.FromBytes();
            XElement html = null;
            try
            {
                html = XElement.Parse(htmlString);
            }
            catch
            {
                HtmlDocument hdoc = new HtmlDocument();
                hdoc.LoadHtml(htmlString);
                hdoc.OptionOutputAsXml = true;
                using (MemoryStream c_Wkg = new MemoryStream())

                {
                    hdoc.Save(c_Wkg);

                    string sWkg = c_Wkg.ToArray().FromBytes();
                    StringBuilder sb = new StringBuilder(sWkg);
                    sb.Replace("&amp;", "&");
                    sb.Replace("&nbsp;", "\xA0");
                    sb.Replace("&quot;", "\"");
                    sb.Replace("&lt;", "~lt;");
                    sb.Replace("&gt;", "~gt;");
                    sb.Replace("&#", "~#");
                    sb.Replace("&", "&amp;");
                    sb.Replace("~lt;", "&lt;");
                    sb.Replace("~gt;", "&gt;");
                    sb.Replace("~#", "&#");
                    html = XElement.Parse(sb.ToString());
                }
            }

            // HtmlToWmlConverter expects the HTML elements to be in no namespace, so convert all elements to no namespace.
            html = (XElement)ConvertToNoNamespace(html);
            return html;
        }

        private static object ConvertToNoNamespace(XNode node)
        {
            XElement element = node as XElement;
            if (element != null)
            {
                return new XElement(element.Name.LocalName,
                    element.Attributes().Where(a => !a.IsNamespaceDeclaration),
                    element.Nodes().Select(n => ConvertToNoNamespace(n)));
            }
            return node;
        }
    }

    public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
    {
        public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
        {
            IDictionary<string, string> attributes = tag.Attributes;
            string src;
            if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
                return new List<IElement>(1);

            if (string.IsNullOrEmpty(src))
                return new List<IElement>(1);

            if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
            {
                // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
                var base64Data = src.Substring(src.IndexOf(",") + 1);
                var imagedata = Convert.FromBase64String(base64Data);
                var image = iTextSharp.text.Image.GetInstance(imagedata);

                var list = new List<IElement>();
                var htmlPipelineContext = GetHtmlPipelineContext(ctx);
                list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext), 0, 0, true), tag, htmlPipelineContext));
                return list;
            }
            else
            {
                return base.End(ctx, tag, currentContent);
            }
        }
    }
    #endregion
}
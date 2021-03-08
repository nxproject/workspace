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
//using System.Drawing.Imaging;
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

using NX.Engine;
using NX.Shared;

namespace Proc.Docs.Files
{
    /// <summary>
    /// 
    ///  a simple parser that uses XMLWorker and XMLParser to handle converting
    ///  (most) images and hyperlinks internally
    ///  From: https://html-agility-pack.net/knowledge-base/35594030/how-can-i-use-itext-to-convert-html-with-images-and-hyperlinks-to-pdf-
    /// 
    /// </summary>
    public class ITSParser : IDisposable
    {
        #region Properties
        public virtual ILinkProvider LinkProvider { get; set; }
        public virtual IImageProvider ImageProvider { get; set; }

        public virtual HtmlPipelineContext HtmlPipelineContext { get; set; }
        public virtual ITagProcessorFactory TagProcessorFactory { get; set; }
        public virtual ICSSResolver CssResolver { get; set; }
        #endregion

        #region Constructor
        /* overloads simplfied to keep SO answer (relatively) short. if needed
         * set LinkProvider/ImageProvider after instantiating SimpleParser()
         * to override the defaults (e.g. ImageProvider.ScalePercent)
         */
        public ITSParser() : this(null) { }
        public ITSParser(string baseUri)
        {
            LinkProvider = new LinkProvider();
            ImageProvider = new ImageProvider();

            HtmlPipelineContext = new HtmlPipelineContext(null);

            // another story altogether, and not implemented for simplicity 
            TagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
            CssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Methods
        /*
         * when sending XHR via any of the popular JavaScript frameworks,
         * <img> tags are **NOT** always closed, which results in the 
         * infamous iTextSharp.tool.xml.exceptions.RuntimeWorkerException:
         * 'Invalid nested tag a found, expected closing tag img.' a simple
         * workaround.
         */
        public virtual string SimpleAjaxImgFix(string xHtml)
        {
            return Regex.Replace(
                xHtml,
                "(?<image><img[^>]+)(?<=[^/])>",
                new MatchEvaluator(match => match.Groups["image"].Value + " />"),
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );
        }

        public virtual byte[] Parse(string xHtml)
        {
            byte[] abAns = null;

            xHtml = SimpleAjaxImgFix(xHtml);

            using (MemoryStream c_Output = new MemoryStream())
            {
                using (var stringReader = new StringReader(xHtml))
            {
                using (Document document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, c_Output);
                        document.Open();

                        HtmlPipelineContext
                            .SetTagFactory(Tags.GetHtmlTagProcessorFactory())
                            .SetLinkProvider(LinkProvider)
                            .SetImageProvider(ImageProvider)
                        ;
                        var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
                        var htmlPipeline = new HtmlPipeline(HtmlPipelineContext, pdfWriterPipeline);
                        var cssResolverPipeline = new CssResolverPipeline(CssResolver, htmlPipeline);

                        XMLWorker worker = new XMLWorker(cssResolverPipeline, true);
                        XMLParser parser = new XMLParser(worker);
                        parser.Parse(stringReader);
                    }
                }

                abAns = c_Output.ToArray();
            }

            return abAns;
        }
        #endregion
    }

    // resolve URIs for LinkProvider & ImageProvider
    public class UriHelper
    {
        /* IsLocal; when running in web context:
         * [1] give LinkProvider http[s] scheme; see CreateBase(string baseUri)
         * [2] give ImageProvider relative path starting with '/' - see:
         *     Join(string relativeUri)
         */
        public bool IsLocal { get; set; }
        public Uri BaseUri { get; private set; }

        public UriHelper(string baseUri) : this(baseUri, true) { }
        public UriHelper(string baseUri, bool isLocal)
        {
            IsLocal = isLocal;
        }

        /* get URI for IImageProvider to instantiate iTextSharp.text.Image for 
         * each <img> element in the HTML.
         */
        public string Combine(string relativeUri)
        {
            /* when running in a web context, the HTML is coming from a MVC view 
             * or web form, so convert the incoming URI to a **local** path
             */
            return BaseUri.Scheme == Uri.UriSchemeFile
                ? Path.Combine(BaseUri.LocalPath, relativeUri)
                // for this example we're assuming URI.Scheme is http[s]
                : new Uri(BaseUri, relativeUri).AbsoluteUri;
        }
    }

    // make hyperlinks with relative URLs absolute
    public class LinkProvider : ILinkProvider
    {
         public LinkProvider()
        { }

        public string GetLinkRoot()
        {
            return "/";
        }
    }

    // handle <img> elements in HTML  
    public class ImageProvider : IImageProvider
    {
        // see Store(string src,  Image img)
        private Dictionary<string, iTextSharp.text.Image> _imageCache =
            new Dictionary<string, iTextSharp.text.Image>();

        public virtual float ScalePercent { get; set; }
        public virtual Regex Base64 { get; set; }

        public ImageProvider() 
            : this(67f) 
        { }
        //              hard-coded based on general past experience ^^^
        // but call the overload to supply your own
        public ImageProvider(float scalePercent)
        {
            ScalePercent = scalePercent;
            Base64 = new Regex( // rfc2045, section 6.8 (alphabet/padding)
                @"^data:image/[^;]+;base64,(?<data>[a-z0-9+/]+={0,2})$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            );
        }

        public virtual iTextSharp.text.Image ScaleImage(iTextSharp.text.Image img)
        {
            img.ScalePercent(ScalePercent);
            return img;
        }

        public virtual iTextSharp.text.Image Retrieve(string src)
        {
            if (_imageCache.ContainsKey(src)) return _imageCache[src];

            try
            {
                if (Regex.IsMatch(src, "^https?://", RegexOptions.IgnoreCase))
                {
                    return ScaleImage(iTextSharp.text.Image.GetInstance(src));
                }

                Match match;
                if ((match = Base64.Match(src)).Length > 0)
                {
                    return ScaleImage(iTextSharp.text.Image.GetInstance(
                        Convert.FromBase64String(match.Groups["data"].Value)
                    ));
                }

                var imgPath = src;
                return ScaleImage(iTextSharp.text.Image.GetInstance(imgPath));
            }
            // not implemented to keep the SO answer (relatively) short
            catch (BadElementException ex) { return null; }
            catch (IOException ex) { return null; }
            catch (Exception ex) { return null; }
        }

        /*
         * always called after Retrieve(string src):
         * [1] cache any duplicate <img> in the HTML source so the image bytes
         *     are only written to the PDF **once**, which reduces the 
         *     resulting file size.
         * [2] the cache can also **potentially** save network IO if you're
         *     running the parser in a loop, since Image.GetInstance() creates
         *     a WebRequest when an image resides on a remote server. couldn't
         *     find a CachePolicy in the source code
         */
        public virtual void Store(string src, iTextSharp.text.Image img)
        {
            if (!_imageCache.ContainsKey(src)) _imageCache.Add(src, img);
        }

        /* XMLWorker documentation for ImageProvider recommends implementing
         * GetImageRootPath():
         * 
         * http://demo.itextsupport.com/xmlworker/itextdoc/flatsite.html#itextdoc-menu-10
         * 
         * but a quick run through the debugger never hits the breakpoint, so 
         * not sure if I'm missing something, or something has changed internally 
         * with XMLWorker....
         */
        public virtual string GetImageRootPath() { return null; }
        public virtual void Reset() { }
    }
}
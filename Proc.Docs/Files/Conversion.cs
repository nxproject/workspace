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
    #region Statics
    public static class ConversionClass
    { 
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
            // Make the URL
            string sURL = doc.Parent.Parent.LoopbackURL.URLCombine("libreoffice", "convert",  doc.Path.ToBase64URL(), to.Path.ToBase64URL());

            // Call
            JObject c_Resp = sURL.URLGet().FromBytes().ToJObject();

            return to;
        }
        #endregion
    }
    #endregion
}
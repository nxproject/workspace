//
// DocxToText.cs
// Copyright (C) 2007  Eugene Pankov
//

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.IO.Compression;

using NX.Shared;
using NX.Engine;

namespace Proc.Task.iTextIF
{
    public class DocxToTextClass : IDisposable
    {
        #region Constants
        private const string ContentTypeNamespace =
            @"http://schemas.openxmlformats.org/package/2006/content-types";

        private const string WordprocessingMlNamespace =
            @"http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        private const string DocumentXmlXPath =
            "/t:Types/t:Override[@ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"]";

        private const string BodyXPath = "/w:document/w:body";
        #endregion

        #region Constructor
        public DocxToTextClass()
        { }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Methods
        public List<string> ExtractText(string path)
        {
            string sAns = null;

            ZipArchive c_Zip = ZipFile.OpenRead(path);

            string sLoc = FindDocumentXmlLocation(c_Zip);
            if (sLoc.HasValue())
            {
                sAns = ReadDocumentXml(c_Zip, sLoc);
            }

            return sAns.IfEmpty().SplitCRLF();
        }

        private string FindDocumentXmlLocation(ZipArchive archive)
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                // Find "[Content_Types].xml" zip entry

                if (string.Compare(entry.Name, "[Content_Types].xml", true) == 0)
                {
                    Stream contentTypes = entry.Open();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(contentTypes);
                    contentTypes.Close();

                    //Create an XmlNamespaceManager for resolving namespaces

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("t", ContentTypeNamespace);

                    // Find location of "document.xml"

                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(DocumentXmlXPath, nsmgr);

                    if (node != null)
                    {
                        string location = ((XmlElement)node).GetAttribute("PartName");
                        return location.TrimStart(new char[] { '/' });
                    }
                    break;
                }
            }

            return null;
        }

        private string ReadDocumentXml(ZipArchive archive, string loc)
        {
            StringBuilder c_SB = new StringBuilder();

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (string.Compare(entry.Name, loc, true) == 0)
                {
                    Stream documentXml = entry.Open();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(documentXml);
                    documentXml.Close();

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("w", WordprocessingMlNamespace);

                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(BodyXPath, nsmgr);

                    if (node == null)
                        return string.Empty;

                    c_SB.Append(ReadNode(node));

                    break;
                }
            }

            return c_SB.ToString();
        }

        private string ReadNode(XmlNode node)
        {
            if (node == null || node.NodeType != XmlNodeType.Element)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element) continue;

                switch (child.LocalName)
                {
                    case "t":                           // Text
                        sb.Append(child.InnerText.TrimEnd());

                        string space = ((XmlElement)child).GetAttribute("xml:space");
                        if (!string.IsNullOrEmpty(space) && space == "preserve")
                            sb.Append(' ');

                        break;

                    case "cr":                          // Carriage return
                    case "br":                          // Page break
                        sb.Append(Environment.NewLine);
                        break;

                    case "tab":                         // Tab
                        sb.Append("\t");
                        break;

                    case "p":                           // Paragraph
                        sb.Append(ReadNode(child));
                        sb.Append(Environment.NewLine);
                        sb.Append(Environment.NewLine);
                        break;

                    default:
                        sb.Append(ReadNode(child));
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
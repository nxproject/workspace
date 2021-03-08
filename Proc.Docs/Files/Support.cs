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

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.awt.geom;

using NX.Engine;
using NX.Shared;

namespace Proc.Docs.Files
{
    /// <summary>
    /// 
    /// Support classes for vendors code
    /// 
    /// </summary>
    public class SupportClass
    {
        #region Constants
        public const string Route = "fx";
        #endregion

        #region Statics
        public static string GetTextFromURL(string url)
        {
            return GetTextFromURL(url, null);
        }

        public static string GetTextFromURL(string url, Action<string> cb)
        {
            string sAns = "";

            byte[] abValue = url.URLGet();
            if (abValue != null && abValue.Length > 0) sAns = GetText(abValue, cb);

            return sAns;
        }

        public static string GetText(byte[] value)
        {
            return GetText(value, null);
        }

        public static string GetText(byte[] value, Action<string> cb)
        {
            string sAns = "";

            PdfReader.unethicalreading = true;

            using (System.IO.MemoryStream c_Stream = new System.IO.MemoryStream(value))
            {
                PdfReader c_Reader = new PdfReader(c_Stream);
                for (int iPage = 1; iPage <= c_Reader.NumberOfPages; iPage++)
                {
                    string sPage = PdfTextExtractor.GetTextFromPage(c_Reader, iPage, new LocationTextExtractionStrategy());

                    if (cb == null)
                    {
                        sAns += sPage;
                    }
                    else
                    {
                        cb(sPage);
                    }
                }
            }

            return sAns;
        }

        public static void Merge(System.IO.Stream result, System.IO.Stream[] streams)
        {
            PdfReader.unethicalreading = true;

            Document c_Doc = new Document(PageSize.LETTER);
            PdfWriter c_Writer = PdfWriter.GetInstance(c_Doc, result);
            c_Doc.Open();

            foreach (System.IO.Stream c_Input in streams)
            {
                PdfReader c_Reader = new PdfReader(c_Input);

                for (int index = 1; index <= c_Reader.NumberOfPages; index++)
                {
                    CopyPage(c_Reader, index, c_Doc, c_Writer);
                }
            }

            c_Doc.Close();
        }

        private static float getScale(float width, float height)
        {
            float scaleX = PageSize.LETTER.Width / width;
            float scaleY = PageSize.LETTER.Height / height;
            return width > height ? Math.Min(scaleX, scaleY) : Math.Max(scaleX,
                scaleY);
        }

        public static void Concantenate(System.IO.Stream result, System.IO.Stream[] streams, string[] names)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            PdfReader.unethicalreading = true;

            int iAt = 0;
            iTextSharp.text.Font bookmarkFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLUE);

            foreach (System.IO.Stream c_Stream in streams)
            {
                try
                {
                    reader = new PdfReader(c_Stream);
                    if (sourceDocument == null)
                    {
                        sourceDocument = new Document(); //reader.GetPageSizeWithRotation(0));
                        pdfCopyProvider = new PdfCopy(sourceDocument, result);
                        sourceDocument.Open();
                    }

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        try
                        {
                            if (i == 1 && names != null)
                            {
                                //First create a paragraph using the filename as the heading
                                iTextSharp.text.Paragraph para = new iTextSharp.text.Paragraph(names[iAt], bookmarkFont);
                                //Then create a chapter from the above paragraph
                                iTextSharp.text.Chapter chpter = new iTextSharp.text.Chapter(para, iAt + 1);
                                //Finally add the chapter to the document
                                pdfCopyProvider.Add(chpter);
                            }

                            importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                            pdfCopyProvider.AddPage(importedPage);
                        }
                        catch { }
                    }
                    reader.Close();
                }
                catch { }

                iAt++;
            }

            if (sourceDocument != null)
            {
                sourceDocument.Close();
            }
        }

        public static void Split(System.IO.Stream input, string pages, Func<int, System.IO.Stream> fn)
        {
            PdfReader.unethicalreading = true;

            PdfReader c_Reader = new PdfReader(input);

            List<string> c_SPages = pages.SplitSpaces();

            int iUsed = 0;
            List<int> c_Pages = new List<int>();
            List<int> c_Stars = new List<int>();

            for (int i = 0; i < c_SPages.Count; i++)
            {
                c_Pages.Add(0);

                string sPages = c_SPages[i];

                if (sPages.IsSameValue("*"))
                {
                    c_Stars.Add(i);
                }
                else
                {
                    int iPages = sPages.ToInteger(0);
                    c_Pages[i] = iPages;

                    if (iPages < 0)
                    {
                        iUsed += -iPages;
                    }
                    else
                    {
                        iUsed += iPages;
                    }
                }
            }

            if (c_Stars.Count > 0)
            {
                int iAvl = c_Reader.NumberOfPages - iUsed;
                if (iAvl < 0) iAvl = 0;

                int iBlk = (int)Math.Round(iAvl / (float)c_Stars.Count);
                for (int j = 0; j < c_Stars.Count - 1; j++)
                {
                    c_Pages[c_Stars[j]] = iBlk;
                    iAvl -= iBlk;
                }

                c_Pages[c_Stars[c_Stars.Count - 1]] = iAvl;
            }

            int iAt = 1;
            int iFile = 0;

            foreach (int iCount in c_Pages)
            {
                if (iCount <= 0)
                {
                    iAt -= iCount;
                }
                else
                {
                    if (iAt <= c_Reader.NumberOfPages)
                    {
                        iFile++;
                        System.IO.Stream c_Out = fn(iFile);

                        Document c_Doc = new Document(PageSize.LETTER);
                        PdfWriter c_Writer = PdfWriter.GetInstance(c_Doc, c_Out);
                        c_Doc.Open();

                        for (int i = 0; i < iCount; i++)
                        {
                            CopyPage(c_Reader, iAt, c_Doc, c_Writer);
                            iAt++;

                        }

                        c_Doc.Close();
                    }
                }
            }
        }

        private static void CopyPage(PdfReader reader, int page, Document outdoc, PdfWriter writer)
        {
            if (page <= reader.NumberOfPages)
            {
                outdoc.NewPage();
                PdfContentByte c_CB = writer.DirectContent;

                PdfImportedPage c_Page = writer.GetImportedPage(reader, page);
                Rectangle c_PageSize = reader.GetPageSizeWithRotation(page);
                float oWidth = c_PageSize.Width;
                float oHeight = c_PageSize.Height;

                float scale = getScale(oWidth, oHeight);
                float scaledWidth = oWidth * scale;
                float scaledHeight = oHeight * scale;
                int rotation = reader.GetPageRotation(page);

                AffineTransform c_Trans = new AffineTransform(scale, 0, 0, scale, 0, 0);

                switch (rotation)
                {
                    case 0:
                        c_CB.AddTemplate(c_Page, c_Trans);
                        break;

                    case 90:
                        AffineTransform c_T90 = new AffineTransform(0, -1f, 1f, 0, 0, scaledHeight);
                        c_T90.Concatenate(c_Trans);
                        c_CB.AddTemplate(c_Page, c_T90);
                        break;

                    case 180:
                        AffineTransform c_T180 = new AffineTransform(-1f, 0, 0, -1f, scaledWidth,
                            scaledHeight);
                        c_T180.Concatenate(c_Trans);
                        c_CB.AddTemplate(c_Page, c_T180);
                        break;

                    case 270:
                        AffineTransform c_T270 = new AffineTransform(0, 1f, -1f, 0, scaledWidth, 0);
                        c_T270.Concatenate(c_Trans);
                        c_CB.AddTemplate(c_Page, c_T270);
                        break;

                    default:
                        c_CB.AddTemplate(c_Page, scale, 0, 0, scale, 0, 0);
                        break;
                }
            }

        }
        #endregion
    }

    public class RectAndText
    {
        #region Constructor
        public RectAndText(iTextSharp.text.Rectangle rect, int page, String text)
        {
            //this.Rect = rect;
            this.Left = rect.Left;
            this.Top = rect.Top;
            this.Height = rect.Height;
            this.Width = rect.Width;

            this.Text = text;
            this.Page = page;
            this.SetMode();
        }

        public RectAndText(float top, float left, float width, float height, int page, String text)
        {
            //this.Rect = rect;
            this.Left = left;
            this.Top = top;
            this.Height = height;
            this.Width = width;

            this.Text = text;
            this.Page = page;
            this.SetMode();
        }
        #endregion

        #region Enums
        public enum Modes
        {
            Signature,
            SignatureDated,
            Initials,
            InitialsDated,
            NoUnderline,
            NoUnderlineDated,
            Restrict,
            RestrictDated,
        }
        #endregion

        #region Properties
        //public iTextSharp.text.Rectangle Rect { get; internal set; }
        public float Left { get; internal set; }
        public float Top { get; internal set; }
        public float Width { get; internal set; }
        public float Height { get; internal set; }

        public String Text { get; internal set; }
        public int Page { get; internal set; }
        public Modes Mode { get; internal set; }
        #endregion

        #region Methods
        private void SetMode()
        {
            this.Mode = Modes.Signature;

            if (this.Text.HasValue())
            {
                char cMode = this.Text[0];

                switch (cMode)
                {
                    case 'x':
                        this.Mode = Modes.Signature;
                        break;

                    case 'X':
                        this.Mode = Modes.SignatureDated;
                        break;

                    case 'i':
                        this.Mode = Modes.Initials;
                        break;

                    case 'I':
                        this.Mode = Modes.InitialsDated;
                        break;

                    case 'n':
                        this.Mode = Modes.NoUnderline;
                        break;

                    case 'N':
                        this.Mode = Modes.NoUnderlineDated;
                        break;

                    case 'r':
                        this.Mode = Modes.Restrict;
                        break;

                    case 'R':
                        this.Mode = Modes.RestrictDated;
                        break;
                }
            }
        }
        #endregion
    }

    public class SignatureTextExtractionStrategy : LocationTextExtractionStrategy
    {
        #region Properties
        public List<RectAndText> myPoints = new List<RectAndText>();
        public int Page { get; set; }
        public string Pattern { get; set; }
        #endregion

        #region Methods
        //Automatically called for each chunk of text in the PDF
        public override void RenderText(TextRenderInfo renderInfo)
        {
            base.RenderText(renderInfo);

            string sText = renderInfo.GetText();

            if (!this.Pattern.HasValue() || sText.Matches(this.Pattern))
            {
                //Get the bounding box for the chunk of text
                var bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
                var topRight = renderInfo.GetAscentLine().GetEndPoint();

                //Create a rectangle from it
                var rect = new iTextSharp.text.Rectangle(
                                                        bottomLeft[Vector.I1],
                                                        bottomLeft[Vector.I2],
                                                        topRight[Vector.I1],
                                                        topRight[Vector.I2]
                                                        );

                //Add this to our main collection
                this.myPoints.Add(new RectAndText(rect, this.Page, sText));
            }
        }
        #endregion
    }

    class ImageRenderListener : IRenderListener
    {
        #region Properties
        private string name;
        //private int counter = 100000;

        public List<RectAndText> myPoints = new List<RectAndText>();
        #endregion

        #region Constructor
        public ImageRenderListener(String name)
        {
            this.name = name;
        }
        #endregion

        #region IRenderListener
        public void BeginTextBlock() { }

        public void EndTextBlock() { }

        public void RenderText(TextRenderInfo renderInfo) { }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            try
            {
                PdfImageObject image = renderInfo.GetImage();
                if (image == null) return;

                Matrix matrix = renderInfo.GetImageCTM();
                float x = matrix[Matrix.I31];
                float y = matrix[Matrix.I32];
                float w = matrix[Matrix.I11];
                float h = matrix[Matrix.I22];

                this.myPoints.Add(new RectAndText(y, x, w, h, 1, ""));
                //int number = renderInfo.GetRef() != null ? renderInfo.GetRef().Number : counter++;
                //String filename = String.Format("%s-%s.%s", name, number, image.GetFileType());
                //FileOutputStream os = new FileOutputStream(filename);
                //os.write(image.GetImageAsBytes());
                //os.flush();
                //os.close();

                PdfDictionary imageDictionary = image.GetDictionary();
                PRStream maskStream = (PRStream)imageDictionary.GetAsStream(PdfName.SMASK);
                //if (maskStream != null)
                //{
                //    PdfImageObject maskImage = new PdfImageObject(maskStream);
                //    filename = String.Format("%s-%s-mask.%s", name, number, maskImage.GetFileType());
                //    os = new FileOutputStream(filename);
                //    os.write(maskImage.GetImageAsBytes());
                //    os.flush();
                //    os.close();
                //}
            }
            catch { }
        }
        #endregion
    }


    public class DocumentInfoClass
    {
        #region Properties
        public List<PageInfoClass> Pages { get; set; } = new List<PageInfoClass>();
        public List<FieldInfoClass> Fields { get; set; } = new List<FieldInfoClass>();
        #endregion
    }

    public class PageInfoClass
    {
        #region Properties
        public float Height { get; set; }
        public float Width { get; set; }
        #endregion
    }
}
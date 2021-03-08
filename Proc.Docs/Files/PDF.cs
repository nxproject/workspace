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
using System.Text;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.awt.geom;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;
using Proc.AO;

namespace Proc.Docs.Files
{
    /// <summary>
    /// 
    /// A toolkit to merge into PDF fields
    /// 
    /// </summary>
    public class PDFClass : IDisposable
    {
        #region Constructor
        /// <summary>
        /// 
        /// Constructor
        /// 
        /// </summary>
        public PDFClass()
        {
            //
            PdfReader.unethicalreading = true;
        }
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
        public byte[] Merge(byte[] doc, ExtendedContextClass ctx)
        {
            //
            byte[] abAns = null;

            // Get the fields
            List<FieldInfoClass> c_Fields = this.Fields(doc);

            // Create the output
            using (System.IO.MemoryStream c_Stream = new System.IO.MemoryStream())
            {
                // Set the input
                PdfReader c_Reader = new PdfReader(doc);

                // And the output
                PdfStamper c_Stamper = new PdfStamper(c_Reader, c_Stream);

                // Do each field
                foreach (FieldInfoClass sKey in c_Fields)
                {
                    try
                    {
                        switch (sKey.Name)
                        {
                            case "Signature":
                            case "Image":
                                break;

                            default:
                                c_Stamper.AcroFields.SetField(sKey.Name, ctx[sKey.Name]);
                                break;
                        }
                    }
                    catch { }
                }

                //if (flatten) c_Stamper.FormFlattening = true;

                // All done
                c_Stamper.Close();
                c_Reader.Close();

                // Get the result
                abAns = c_Stream.ToArray();
            }

            return abAns;
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
            List<FieldInfoClass> c_Ans = new List<FieldInfoClass>();

            try
            {
                PdfReader pdfReader = new PdfReader(doc);
                AcroFields pdfFormFields = pdfReader.AcroFields;

                foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFields.Fields)
                {
                    string sKey = kvp.Key.ToString();

                    FieldInfoClass c_Entry = new FieldInfoClass(sKey,
                                                                    kvp.Value,
                                                                    pdfFormFields.GetFieldPositions(sKey),
                                                                    pdfFormFields.GetFieldType(sKey));

                    c_Entry.CurrentValue = pdfFormFields.GetField(sKey);

                    try
                    {
                        if (c_Entry.Type == FieldInfoClass.Types.CheckBox)
                        {
                            List<string> c_Values = new List<string>(pdfFormFields.GetAppearanceStates(sKey));
                            if (c_Values.Count > 0)
                            {
                                c_Entry.SetCBValues(new List<string>(pdfFormFields.GetAppearanceStates(sKey)));
                            }
                            else
                            {
                                c_Entry.SetCBValue(GetCheckBoxExportValue(pdfFormFields, sKey));
                            }
                        }
                        else if (c_Entry.Type == FieldInfoClass.Types.RadioButton)
                        {
                            c_Entry.SetCBValues(new List<string>(pdfFormFields.GetAppearanceStates(sKey)));
                        }
                    }
                    catch { }

                    c_Ans.Add(c_Entry);
                }

                pdfReader.Close();
            }
            catch { }

            c_Ans.SortFI();

            return c_Ans;
        }
        #endregion

        #region Signatures
        public byte[] SignX(byte[] doc,
                                System.Drawing.Image signature,
                                bool center,
                                string instance = null,
                                string date = null)
        {
            //
            byte[] abAns = null;

            BaseFont c_BF = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            // Create the output
            using (System.IO.MemoryStream c_Stream = new System.IO.MemoryStream())
            {
                // Set the input
                PdfReader c_Reader = new PdfReader(doc);

                // And the output
                PdfStamper c_Stamper = new PdfStamper(c_Reader, c_Stream);

                // Make the image
                System.Drawing.Color c_B = signature.BackgroundColor();
                BaseColor c_BC = new BaseColor(c_B.ToArgb());

                signature = signature.Crop(c_B);

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(signature, c_BC, false);
                int iHi = signature.Height;
                int iWi = signature.Width;

                // Get the location of the signatures
                List<RectAndText> c_Locs = this.GetSignatureLocations(doc);

                // Make sure instance has value
                instance = instance.IfEmpty();

                // Do each field
                foreach (RectAndText c_Loc in c_Locs)
                {
                    //
                    PdfContentByte c_CB = c_Stamper.GetOverContent(c_Loc.Page);

                    // Get text
                    string sEnding = this.GetSignatureInstance(c_Loc);
                    if (sEnding.IsSameValue(instance))
                    {
                        // Extra space for initials type
                        float fSep = 0f;
                        switch (c_Loc.Mode)
                        {
                            case RectAndText.Modes.Initials:
                            case RectAndText.Modes.InitialsDated:
                                fSep += 1f;
                                break;
                        }

                        // Clear the space
                        c_CB.MoveTo(c_Loc.Left, c_Loc.Top - c_Loc.Height);
                        c_CB.SetColorFill(new BaseColor(0xFF, 0xFF, 0xFF));
                        c_CB.Rectangle(c_Loc.Left,
                                        c_Loc.Top - c_Loc.Height,
                                        c_Loc.Width,
                                        c_Loc.Height + fSep);
                        c_CB.Fill();
                        c_CB.Stroke();

                        // If the box is small enough, assume initials
                        float iFinalH = 0;
                        float iFinalW = 0;

                        float fOffsetfromLine = 3f;
                        float fOffsetfromX = 0f;

                        switch (c_Loc.Mode)
                        {
                            case RectAndText.Modes.Initials:
                            case RectAndText.Modes.InitialsDated:
                                fOffsetfromX = 0f; // 0.45f * c_Loc.Height;
                                iFinalW = c_Loc.Width - fOffsetfromX;
                                iFinalH = (iHi * iFinalW) / iWi;
                                break;

                            case RectAndText.Modes.Restrict:
                            case RectAndText.Modes.RestrictDated:
                                fOffsetfromX = 0f; // 0.45f * c_Loc.Height;
                                iFinalW = c_Loc.Width - fOffsetfromX;
                                iFinalH = c_Loc.Height * 2.5f;
                                break;

                            default:
                                //case RectAndText.Modes.Signature:
                                //case RectAndText.Modes.SignatureDated:
                                //case RectAndText.Modes.NoUnderline:
                                //case RectAndText.Modes.NoUnderlineDated:
                                fOffsetfromX = 0f; // 0.65f * c_Loc.Height;
                                iFinalH = 3.0f * c_Loc.Height;
                                iFinalW = (iWi * iFinalH) / iHi;
                                break;
                        }

                        //
                        float fAdjWidth = c_Loc.Width - fOffsetfromX;

                        // Never wider than the area
                        if (iFinalW > fAdjWidth)
                        {
                            float dRatio = (fAdjWidth - c_Loc.Height) / iFinalW;
                            iFinalW *= dRatio;
                            if (c_Loc.Mode == RectAndText.Modes.Restrict || c_Loc.Mode == RectAndText.Modes.Restrict)
                            { }
                            else
                            {
                                iFinalH *= dRatio;
                            }
                        }
                        //Scale it, change the signature size
                        img.ScaleAbsolute(iFinalW, iFinalH); // c_Loc.Rect.Height);
                                                             //Position it set the position it should appear on the PDF
                        if (center)
                        {
                            img.SetAbsolutePosition(fOffsetfromX + c_Loc.Left + (c_Loc.Width - iFinalW) / 2, c_Loc.Top + fOffsetfromLine - c_Loc.Height);
                        }
                        else
                        {
                            img.SetAbsolutePosition(fOffsetfromX + c_Loc.Left, c_Loc.Top + fOffsetfromLine - c_Loc.Height);
                        }

                        //Add it to page of the document
                        c_CB.AddImage(img);

                        // Dated?
                        if (date.HasValue())
                        {
                            float fFactor = 0;

                            switch (c_Loc.Mode)
                            {
                                case RectAndText.Modes.InitialsDated:
                                case RectAndText.Modes.RestrictDated:
                                    fFactor = 0.33f;
                                    break;

                                case RectAndText.Modes.SignatureDated:
                                case RectAndText.Modes.NoUnderlineDated:
                                    fFactor = 0.5f;
                                    break;
                            }

                            if (fFactor > 0)
                            {
                                float fHi = c_Loc.Height * fFactor;
                                Font c_Font = FontFactory.GetFont("Arial", fHi, Font.NORMAL, BaseColor.BLUE);

                                ColumnText.ShowTextAligned(c_CB,
                                                                Element.ALIGN_RIGHT,
                                                                new Phrase(date, c_Font),
                                                                c_Loc.Left + c_Loc.Width,
                                                                c_Loc.Top - (fHi + c_Loc.Height), 0);
                            }
                        }

                        // Underline
                        if (c_Loc.Mode != RectAndText.Modes.NoUnderline &&
                            c_Loc.Mode != RectAndText.Modes.NoUnderlineDated)
                        {

                            c_CB.SetColorFill(new BaseColor(0x00, 0x00, 0x00));
                            c_CB.MoveTo(c_Loc.Left, c_Loc.Top - c_Loc.Height);
                            c_CB.LineTo(c_Loc.Left + c_Loc.Width, c_Loc.Top - c_Loc.Height);
                            c_CB.Stroke();
                        }
                    }
                }

                // All done
                c_Stamper.Close();
                c_Reader.Close();

                // Get the result
                abAns = c_Stream.ToArray();
            }

            return abAns;
        }

        public byte[] SignX(byte[] doc,
                                byte[] signature,
                                bool center,
                                string instance = null,
                                string date = null)
        {
            return this.SignX(doc, signature.ToImage(), center, instance, date);
        }

        #endregion

        #region Tools
        /// <summary>
        /// 
        /// Returns the number of pages in a document
        /// 
        /// </summary>
        /// <param name="doc">The byte array of the document contents</param>
        /// <returns>The number of pages</returns>
        public int PageCount(byte[] doc)
        {
            int iAns = 0;

            try
            {
                PdfReader pdfReader = new PdfReader(doc);
                iAns = pdfReader.NumberOfPages;
            }
            catch { }

            return iAns;
        }

        /// <summary>
        /// 
        /// Checks to see if the document is password protected
        /// 
        /// </summary>
        /// <param name="doc">The byte array of the document contents</param>
        /// <returns>True is the document is password protected</returns>
        public bool IsPasswordProtected(byte[] doc)
        {
            bool bAns = false;
            try
            {
                PdfReader pdfReader = new PdfReader(doc);
            }
            catch //(iTextSharp.text.exceptions.BadPasswordException e)
            {
                bAns = true;
            }

            return bAns;
        }

        /// <summary>
        /// 
        /// Flatten the document.  Fields afe converted into text
        /// 
        /// </summary>
        /// <param name="doc">The byte array of the document contents</param>
        /// <returns>The flattened document</returns>
        public byte[] Flatten(byte[] doc)
        {
            //
            byte[] abAns = null;

            // Create the output
            using (System.IO.MemoryStream c_Stream = new System.IO.MemoryStream())
            {
                // Set the input
                PdfReader c_Reader = new PdfReader(doc);

                // And the output
                PdfStamper c_Stamper = new PdfStamper(c_Reader, c_Stream);

                c_Stamper.FormFlattening = true;

                // All done
                c_Stamper.Close();
                c_Reader.Close();

                // Get the result
                abAns = c_Stream.ToArray();
            }

            return abAns;
        }

        public List<RectAndText> GetImageLocations(byte[] doc)
        {
            PdfReader reader = new PdfReader(doc);
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            ImageRenderListener listener = new ImageRenderListener("testpdf");

            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                parser.ProcessContent(i, listener);
            }

            return listener.myPoints;
        }

        public List<RectAndText> GetTextLocations(byte[] doc, string patt)
        {
            var c_Strategy = new SignatureTextExtractionStrategy();
            c_Strategy.Pattern = patt;

            using (var c_Reader = new PdfReader(doc))
            {
                for (int page = 1; page <= c_Reader.NumberOfPages; page++)
                {
                    c_Strategy.Page = page;

                    var ex = PdfTextExtractor.GetTextFromPage(c_Reader, page, c_Strategy);
                }
            }

            return c_Strategy.myPoints;
        }

        public List<RectAndText> GetSignatureLocations(byte[] doc)
        {
            return this.GetTextLocations(doc, @"[xXiInNrR]\s?\x5F+.*");
        }

        public string GetSignatureInstance(RectAndText area)
        {
            string sInstance = "";

            int iPos = area.Text.LastIndexOf("_");
            if (iPos != -1)
            {
                sInstance = area.Text.Substring(iPos).Trim().AlphaNumOnly();
            }

            return sInstance;
        }

        public List<string> GetSignatureInstances(byte[] doc)
        {
            List<string> c_Ans = new List<string>();

            List<RectAndText> c_Poss = this.GetSignatureLocations(doc);
            foreach (RectAndText c_Area in c_Poss)
            {
                string sInstance = this.GetSignatureInstance(c_Area);
                if (c_Ans.IndexOf(sInstance) == -1) c_Ans.Add(sInstance);
            }

            return c_Ans;
        }

        public byte[] ReplaceImage(byte[] doc, System.Drawing.Image image)
        {
            //
            byte[] abAns = null;

            // Create the output
            using (System.IO.MemoryStream c_Stream = new System.IO.MemoryStream())
            {
                // Set the input
                PdfReader c_Reader = new PdfReader(doc);

                // And the output
                PdfStamper c_Stamper = new PdfStamper(c_Reader, c_Stream);

                PdfWriter c_Writer = c_Stamper.Writer;
                Image c_Image = Image.GetInstance(image, BaseColor.WHITE, true);

                for (int iPage = 1; iPage <= c_Reader.NumberOfPages; iPage++)
                {
                    PdfDictionary c_Page = c_Reader.GetPageN(iPage);
                    PdfDictionary c_Resources = (PdfDictionary)PdfReader.GetPdfObject(c_Page.Get(PdfName.RESOURCES));
                    PdfDictionary c_XObject = (PdfDictionary)PdfReader.GetPdfObject(c_Resources.Get(PdfName.XOBJECT));

                    if (c_XObject != null)
                    {
                        foreach (PdfName name in c_XObject.Keys)
                        {
                            PdfObject c_Obj = c_XObject.Get(name);
                            if (c_Obj.IsIndirect())
                            {
                                try
                                {
                                    PdfDictionary c_Dict = (PdfDictionary)PdfReader.GetPdfObject(c_Obj);
                                    PdfName c_SubType = (PdfName)PdfReader.GetPdfObject(c_Dict.Get(PdfName.SUBTYPE));

                                    if (PdfName.IMAGE.Equals(c_SubType))
                                    {
                                        PdfReader.KillIndirect(c_Obj);
                                        Image maskImage = c_Image.ImageMask;
                                        if (maskImage != null)
                                            c_Writer.AddDirectImageSimple(maskImage);
                                        c_Writer.AddDirectImageSimple(c_Image, c_Obj as PRIndirectReference);
                                        //break;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }

                // All done
                c_Stamper.Close();
                c_Reader.Close();

                // Get the result
                abAns = c_Stream.ToArray();
            }

            return abAns;
        }

        //public byte[] ReplaceImageX(byte[] doc, System.Drawing.Image image)
        //{
        //    //
        //    byte[] abAns = null;

        //    // Create the output
        //    using (System.IO.MemoryStream c_Stream = new System.IO.MemoryStream())
        //    {
        //        // Set the input
        //        PdfReader c_Reader = new PdfReader(doc);

        //        // And the output
        //        PdfStamper c_Stamper = new PdfStamper(c_Reader, c_Stream);

        //        // Make the image
        //        byte[] imageBytes = image.ToBytes();
        //        MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
        //        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms);
        //        int iHi = image.Height;
        //        int iWi = image.Width;

        //        // Get the location of the signatures
        //        List<RectAndText> c_Locs = this.GetImageLocations(doc);

        //        // Do each field
        //        foreach (RectAndText c_Loc in c_Locs)
        //        {
        //            // Scale rectangle
        //            float iFinalH = c_Loc.Height;
        //            float iFinalW = c_Loc.Height;
        //            //Scale it, change the signature size
        //            img.ScaleAbsolute(iFinalW, iFinalH); // c_Loc.Rect.Height);
        //                                                 //Position it set the position it should appear on the PDF
        //            img.SetAbsolutePosition(c_Loc.Left, c_Loc.Top);
        //            //Add it to page of the document
        //            c_Stamper.GetOverContent(c_Loc.Page).AddImage(img);
        //        }

        //        // All done
        //        c_Stamper.Close();
        //        c_Reader.Close();

        //        // Get the result
        //        abAns = c_Stream.ToArray();
        //    }

        //    return abAns;
        //}

        public byte[] ReplaceImage(byte[] doc, byte[] image)
        {
            return this.ReplaceImage(doc, image.ToImage());
        }

        public DocumentInfoClass Document(byte[] doc)
        {
            //
            DocumentInfoClass c_Ans = new DocumentInfoClass();

            // Open
            PdfReader pdfReader = new PdfReader(doc);
            // And create each page
            for (int iPage = 0; iPage < pdfReader.NumberOfPages; iPage++)
            {
                Rectangle c_Page = pdfReader.GetPageSizeWithRotation(iPage + 1);

                PageInfoClass c_Info = new PageInfoClass();
                c_Info.Height = c_Page.Height;
                c_Info.Width = c_Page.Width;

                c_Ans.Pages.Add(c_Info);
            }

            // Now the fields
            AcroFields pdfFormFields = pdfReader.AcroFields;

            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFields.Fields)
            {
                string sKey = kvp.Key.ToString();

                FieldInfoClass c_Entry = new FieldInfoClass(sKey,
                                                                kvp.Value,
                                                                pdfFormFields.GetFieldPositions(sKey),
                                                                pdfFormFields.GetFieldType(sKey));

                c_Entry.CurrentValue = pdfFormFields.GetField(sKey);

                try
                {
                    if (c_Entry.Type == FieldInfoClass.Types.CheckBox)
                    {
                        List<string> c_Values = new List<string>(pdfFormFields.GetAppearanceStates(sKey));
                        if (c_Values.Count > 0)
                        {
                            c_Entry.SetCBValues(new List<string>(pdfFormFields.GetAppearanceStates(sKey)));
                        }
                        else
                        {
                            c_Entry.SetCBValue(GetCheckBoxExportValue(pdfFormFields, sKey));
                        }
                    }
                    else if (c_Entry.Type == FieldInfoClass.Types.RadioButton)
                    {
                        c_Entry.SetCBValues(new List<string>(pdfFormFields.GetAppearanceStates(sKey)));
                    }
                }
                catch { }

                c_Ans.Fields.Add(c_Entry);
            }

            //Adjust each field for proper position
            foreach (FieldInfoClass c_Field in c_Ans.Fields)
            {
                foreach (FieldInfoClass.LocationClass c_Pos in c_Field.Locations)
                {
                    PageInfoClass c_Page = c_Ans.Pages[c_Pos.Page];

                    c_Pos.Top = c_Page.Height - c_Pos.Top;
                    //c_Pos.Left = c_Page.Width - c_Pos.Left;
                }
            }

            // We are done
            pdfReader.Close();

            return c_Ans;
        }

        private string GetCheckBoxExportValue(AcroFields fields, string cbFieldName)
        {
            string sAns = null;

            AcroFields.Item item = fields.GetFieldItem(cbFieldName);
            try
            {
                PdfDictionary valueDict = item.GetValue(0) as PdfDictionary;
                PdfDictionary appDict = valueDict.GetAsDict(PdfName.AP);

                if (appDict != null)
                {
                    PdfDictionary normalApp = appDict.GetAsDict(PdfName.N);

                    foreach (PdfName curKey in normalApp.Keys)
                    {
                        if (!PdfName.OFF.Equals(curKey))
                        {
                            // string will have a leading '/' character
                            sAns = curKey.ToString();
                        }
                    }
                }

                if (!sAns.HasValue())
                {
                    PdfName curVal = valueDict.GetAsName(PdfName.AS);
                    if (curVal != null)
                    {
                        sAns = curVal.ToString();
                    }
                }
            }
            catch { }

            if (sAns.HasValue() && sAns.StartsWith("/")) sAns = sAns.Substring(1);

            return sAns;
        }

        public string Text(byte[] doc)
        {
            PdfReader reader = new PdfReader(doc);

            StringBuilder c_Buffer = new StringBuilder();

            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                c_Buffer.AppendLine(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));
            }

            return c_Buffer.ToString();
        }

        public List<string> TextByPage(byte[] doc)
        {
            PdfReader reader = new PdfReader(doc);

            List<string> c_Buffer = new List<string>();

            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                c_Buffer.Add(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));
            }

            return c_Buffer;
        }

        public List<PdfObject> Images(byte[] doc)
        {
            List<PdfObject> c_Ans = new List<PdfObject>();

            PdfReader pdf = new PdfReader(doc);

            try
            {
                for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNumber);
                    c_Ans.AddRange(FindImageInPDFDictionary(pg));
                }
            }
            catch { }
            finally
            {
                pdf.Close();
            }

            return c_Ans;
        }

        private List<PdfObject> FindImageInPDFDictionary(PdfDictionary pg)
        {
            List<PdfObject> c_Ans = new List<PdfObject>();

            PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {
                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));

                        //image at the root of the pdf
                        if (PdfName.IMAGE.Equals(type))
                        {
                            c_Ans.Add(obj);
                        }// image inside a form
                        else if (PdfName.FORM.Equals(type))
                        {
                            c_Ans.AddRange(FindImageInPDFDictionary(tg));
                        } //image inside a group
                        else if (PdfName.GROUP.Equals(type))
                        {
                            c_Ans.AddRange(FindImageInPDFDictionary(tg));
                        }
                    }
                }
            }

            return c_Ans;
        }

        public byte[] ExtractPages(byte[] doc, int start, int end)
        {
            // get input document
            PdfReader inputPdf = new PdfReader(doc);

            // retrieve the total number of pages
            int pageCount = inputPdf.NumberOfPages;

            if (end < start)
            {
                int iTemp = start;
                start = end;
                end = iTemp;
            }
            if (start < 1)
            {
                start = 1;
            }
            if (end > pageCount)
            {
                end = pageCount;
            }

            // load the input document
            Document inputDoc = new Document(inputPdf.GetPageSizeWithRotation(1));

            // create the filestream
            using (MemoryStream fs = new MemoryStream())
            {
                // create the output writer
                PdfWriter outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                inputDoc.Open();
                PdfContentByte cb1 = outputWriter.DirectContent;

                // copy pages from input to output document
                for (int i = start; i <= end; i++)
                {
                    inputDoc.SetPageSize(inputPdf.GetPageSizeWithRotation(i));
                    inputDoc.NewPage();

                    PdfImportedPage page =
                        outputWriter.GetImportedPage(inputPdf, i);
                    int rotation = inputPdf.GetPageRotation(i);

                    if (rotation == 90 || rotation == 270)
                    {
                        cb1.AddTemplate(page, 0, -1f, 1f, 0, 0,
                            inputPdf.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                    }
                }


                inputDoc.Close();

                return fs.ToArray();
            }
        }

        public byte[] MergePDF(params byte[][] pdf)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            using (MemoryStream output = new MemoryStream())
            {
                try
                {
                    // Initialize pdf writer
                    PdfWriter writer = PdfWriter.GetInstance(document, output);
                    //writer.PageEvent = new PdfPageEvents();

                    // Open document to write
                    document.Open();
                    PdfContentByte content = writer.DirectContent;

                    // Iterate through all pdf documents
                    for (int fileCounter = 0; fileCounter < pdf.Length; fileCounter++)
                    {
                        if (pdf[fileCounter] != null)
                        {
                            // Create PDF reader
                            PdfReader reader = new PdfReader(pdf[fileCounter]);

                            int numberOfPages = reader.NumberOfPages;

                            // Iterate through all pages
                            for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                            {
                                // Determine page size for the current page
                                document.SetPageSize(reader.GetPageSizeWithRotation(currentPageIndex));

                                // Create page
                                document.NewPage();
                                PdfImportedPage importedPage = writer.GetImportedPage(reader, currentPageIndex);
                                content.AddTemplate(importedPage, 1f, 0, 0, 1f, 0, 0);
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    document.Close();
                }
                return output.GetBuffer();
            }
        }

        public static BaseFont WMBaseFont =
                BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        private byte[] WaterMark(byte[] bytes, string wm, BaseFont bf = null)
        {


            using (var ms = new MemoryStream(10 * 1024))
            {
                using (var reader = new PdfReader(bytes))
                using (var stamper = new PdfStamper(reader, ms))
                {
                    int times = reader.NumberOfPages;
                    for (int i = 1; i <= times; i++)
                    {
                        var dc = stamper.GetOverContent(i);
                        PDFClass.DoWM(dc, wm, bf, 48, 35, new BaseColor(70, 70, 255), reader.GetPageSizeWithRotation(i));
                    }
                    stamper.Close();
                }
                return ms.ToArray();
            }
        }

        public static void DoWM(PdfContentByte dc, string text, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize, Rectangle rect = null)
        {
            if (font == null)
            {
                font = PDFClass.WMBaseFont;
            }

            var gstate = new PdfGState { FillOpacity = 0.1f, StrokeOpacity = 0.3f };
            dc.SaveState();
            dc.SetGState(gstate);
            dc.SetColorFill(color);
            dc.BeginText();
            dc.SetFontAndSize(font, fontSize);
            var ps = rect ?? realPageSize; /*dc.PdfDocument.PageSize is not always correct*/
            var x = (ps.Right + ps.Left) / 2;
            var y = (ps.Bottom + ps.Top) / 2;
            dc.ShowTextAligned(Element.ALIGN_CENTER, text, x, y, angle);
            dc.EndText();
            dc.RestoreState();
        }

        public static void DoRightMargin(PdfContentByte dc, string text, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize, Rectangle rect = null)
        {
            if (font == null)
            {
                font = PDFClass.WMBaseFont;
            }

            var gstate = new PdfGState { FillOpacity = 0.5f, StrokeOpacity = 0.5f };
            dc.SaveState();
            dc.SetGState(gstate);
            dc.SetColorFill(color);
            dc.BeginText();
            dc.SetFontAndSize(font, fontSize);
            var ps = rect ?? realPageSize; /*dc.PdfDocument.PageSize is not always correct*/
            var x = ps.Right + ps.Left;
            var y = (ps.Bottom + ps.Top) / 2;
            dc.ShowTextAligned(Element.ALIGN_CENTER, text, x, y, angle);
            dc.EndText();
            dc.RestoreState();
        }

        public static void DoLeftMargin(PdfContentByte dc, string text, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize, Rectangle rect = null)
        {
            if (font == null)
            {
                font = PDFClass.WMBaseFont;
            }

            var gstate = new PdfGState { FillOpacity = 0.5f, StrokeOpacity = 0.5f };
            dc.SaveState();
            dc.SetGState(gstate);
            dc.SetColorFill(color);
            dc.BeginText();
            dc.SetFontAndSize(font, fontSize);
            var ps = rect ?? realPageSize; /*dc.PdfDocument.PageSize is not always correct*/
            var x = fontSize;
            var y = (ps.Bottom + ps.Top) / 2;
            dc.ShowTextAligned(Element.ALIGN_CENTER, text, x, y, angle);
            dc.EndText();
            dc.RestoreState();
        }

        public static void DoBates(PdfContentByte dc, string text, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize, Rectangle rect = null)
        {
            if (font == null)
            {
                font = PDFClass.WMBaseFont;
            }

            var gstate = new PdfGState { FillOpacity = 1f, StrokeOpacity = 1f };
            dc.SaveState();
            dc.SetGState(gstate);
            dc.SetColorFill(color);
            dc.BeginText();
            dc.SetFontAndSize(font, fontSize);
            var ps = rect ?? realPageSize; /*dc.PdfDocument.PageSize is not always correct*/
            var x = ps.Right + ps.Left;
            var y = 0;
            dc.ShowTextAligned(Element.ALIGN_RIGHT, text, x, y, angle);
            dc.EndText();
            dc.RestoreState();
        }

        public byte[] BatesStamp(byte[] doc, string text = null)
        {
            byte[] abAns = null;

            float fY = 15f;
            if (text.HasValue()) fY *= 2;

            Font c_Font = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.RED);
            using (MemoryStream c_Stream = new MemoryStream())
            {
                PdfReader c_Reader = new PdfReader(doc);
                using (PdfStamper stamper = new PdfStamper(c_Reader, c_Stream))
                {
                    int iCount = c_Reader.NumberOfPages;
                    string sFormat = @"{0:" + "".RPad(iCount.ToString().Length, "0") + "}/" + iCount.ToString();

                    for (int iPage = 1; iPage <= iCount; iPage++)
                    {
                        string sPage = sFormat.FormatString(iPage);

                        if (text.HasValue()) sPage = text;

                        ColumnText.ShowTextAligned(stamper.GetUnderContent(iPage), Element.ALIGN_RIGHT, new Phrase(sPage, c_Font), 568f, fY, 0);
                    }
                }
                abAns = c_Stream.ToArray();
            }

            return abAns;
        }

        public byte[] MarkX(byte[] doc, JArray locs)
        {
            byte[] abAns = null;

            try
            {
                // open the reader
                PdfReader c_Reader = new PdfReader(doc);
                Rectangle c_Size = c_Reader.GetPageSizeWithRotation(1);
                //Document c_Doc = new Document(c_Size);



                // open the writer
                using (MemoryStream c_Stream = new MemoryStream())
                {
                    //
                    PdfStamper c_Stamper = new PdfStamper(c_Reader, c_Stream);

                    //PdfWriter c_Writer = PdfWriter.GetInstance(c_Doc, c_Stream);
                    //c_Doc.Open();

                    // 
                    int iCount = c_Reader.NumberOfPages;

                    for (int iPage = 1; iPage <= iCount; iPage++)
                    {
                        // the pdf content
                        PdfContentByte c_CB = c_Stamper.GetOverContent(iPage);
                        BaseFont c_Font = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                        // Determine page size for the current page
                        Rectangle c_PSize = c_Reader.GetPageSizeWithRotation(iPage);
                        //c_Doc.SetPageSize(c_PSize);

                        //
                        float fPH = c_PSize.Height;
                        float fPW = c_PSize.Width;

                        // Create page
                        //c_Doc.NewPage();
                        //PdfImportedPage c_Page = c_Writer.GetImportedPage(c_Reader, iPage);
                        //c_CB.AddTemplate(c_Page, 1f, 0, 0, 1f, 0, 0);

                        // Thru the list
                        for (int iEntry = 0; iEntry < locs.Count; iEntry++)
                        {
                            // Get the entry
                            JObject c_Entry = locs.GetJObject(iEntry);
                            // Is it in this page?
                            if (iPage == c_Entry.Get("page").ToInteger(0))
                            {
                                // Always black
                                c_CB.SetColorFill(BaseColor.BLACK);

                                // Compute
                                float fIX = fPW * c_Entry.Get("left").ToFloat();
                                float fIW = fPW * c_Entry.Get("width").ToFloat();
                                float fIY = fPH * (1 - (c_Entry.Get("top").ToFloat() + c_Entry.Get("height").ToFloat()));
                                float fIH = fPH * c_Entry.Get("height").ToFloat();
                                string sText = c_Entry.Get("text");

                                // Compute font
                                c_CB.SetFontAndSize(c_Font, fIH * 0.89f); // * 0.75f);

                                // Write the text
                                c_CB.BeginText();
                                c_CB.SetTextMatrix(fIX, fIY + 15f);
                                c_CB.ShowText(sText);
                                //c_CB.ShowTextAligned(1, sText, fIX, fIY, 0);
                                c_CB.EndText();

                            }
                        }
                    }

                    //
                    //c_Doc.Close();
                    //c_Writer.Close();
                    c_Stamper.Close();
                    c_Reader.Close();

                    abAns = c_Stream.ToArray();
                }
            }
            catch { }

            return abAns;
        }

        public byte[] FourUp(byte[] doc)
        {
            //
            byte[] abAns = null;

            // Create the output
            using (System.IO.MemoryStream c_Stream = new System.IO.MemoryStream())
            {
                try
                {
                    float fRed = 0.48f;
                    float fAdj = 1.04f;

                    // Set the input
                    PdfReader c_Reader = new PdfReader(doc);

                    //Create our combined file
                    var c_Document = new Document();
                    var c_Writer = PdfWriter.GetInstance(c_Document, c_Stream);
                    c_Writer.CloseStream = false;

                    // Open
                    c_Document.Open();
                    PdfContentByte cb1 = c_Writer.DirectContent;

                    // amke the transforms
                    AffineTransform[] c_Trans = new AffineTransform[4];

                    AffineTransform c_Transform = new AffineTransform();
                    c_Transform.Scale(fRed, fRed);
                    c_Transform.Translate(0, c_Document.PageSize.Height);
                    c_Trans[0] = c_Transform;

                    c_Transform = new AffineTransform();
                    c_Transform.Scale(fRed, fRed);
                    c_Transform.Translate(c_Document.PageSize.Width * fAdj, c_Document.PageSize.Height);
                    c_Trans[1] = c_Transform;

                    c_Transform = new AffineTransform();
                    c_Transform.Scale(fRed, fRed);
                    c_Trans[2] = c_Transform;

                    c_Transform = new AffineTransform();
                    c_Transform.Scale(fRed, fRed);
                    c_Transform.Translate(c_Document.PageSize.Width * fAdj, 0);
                    c_Trans[3] = c_Transform;

                    //Get the number of pages in the original file
                    int iPages = c_Reader.NumberOfPages;

                    //Loop through each page
                    for (int i = 0; i < iPages; i++)
                    {
                        try
                        {
                            //We're putting four original pages on one new page so add a new page every four pages
                            if (i % 4 == 0)
                            {
                                c_Document.NewPage();
                            }

                            //Get a page from the reader (remember that PdfReader pages are one-based)
                            var c_IPage = c_Writer.GetImportedPage(c_Reader, (i + 1));

                            //Add our imported page using the matrix that we set above
                            cb1.AddTemplate(c_IPage, c_Trans[i % 4]);
                        }
                        catch (Exception e1)
                        {
                            var a = e1;
                        }
                    }

                    // All done
                    c_Writer.Close();
                    c_Reader.Close();
                }
                catch (Exception e2)
                {
                    var a = e2;
                }

                // Get the result
                abAns = c_Stream.ToArray();
            }

            return abAns;
        }
        #endregion
    }
}
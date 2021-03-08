///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
/// 
/// This work is covered by GPL v3 as defined in https://www.gnu.org/licenses/gpl-3.0.en.html
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
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.awt.geom;

using NX.Engine;
using NX.Shared;
using NX.Engine.Files;
using Proc.Docs;
using Proc.Docs.Files;

namespace Proc.Docs
{
    public class PDFDocumentClass : BasedObjectClass
    {
        #region Constructor
        public PDFDocumentClass(AO.ExtendedContextClass env,
                                    DocumentClass output,
                                    string title,
                                    string creator = null,
                                    AO.UUIDClass baseuuid = null,
                                    string ts = null,
                                    string url = null)
            : base(env)
        {
            // Save document
            this.ODocument = output;

            // Crete stream
            this.ODocument.Location.GetDirectoryFromPath().AssurePath();
            //this.ODocument.Path.AssurePath();
            this.WStream = new FileStream(this.ODocument.Location, FileMode.OpenOrCreate);

            // Create document
            this.WDoc = new Document(iTextSharp.text.PageSize.LETTER);

            // Create writer
            this.WWriter = PdfWriter.GetInstance(this.WDoc, this.WStream);

            // Handler for labeling
            // Handler for labeling
            this.WHandler = new MainTextEventsHandler();
            //this.WHandler.Size = PageSize.LETTER;
            this.WWriter.PageEvent = this.WHandler;
            this.WHandler.DocumentLabel = title;
            this.WHandler.DateStamp = ts;

            // Open the document
            this.WDoc.Open();

            // And set the header
            this.WDoc.AddTitle(title);
            this.WDoc.AddCreationDate();
            if (creator.HasValue()) this.WDoc.AddCreator(creator);

            this.URL = url;
            this.BaseUUID = baseuuid;
        }
        #endregion

        #region Properties
        public AO.ExtendedContextClass Env { get { return this.Root as AO.ExtendedContextClass; } }
        public DocumentClass ODocument { get; set; }

        private Document WDoc { get; set; }
        private PdfWriter WWriter { get; set; }
        private FileStream WStream { get; set; }
        public MainTextEventsHandler WHandler { get; set; }

        //public string DocumentLabel { get; set; }
        //public string ChapterLabel { get; set; }

        public BaseColor ObjectHeaderColor = new BaseColor(173, 216, 230);
        public BaseColor ObjectLabelColor = new BaseColor(211, 211, 211);
        public BaseColor LinkColor = new BaseColor(255, 228, 181);
        public BaseColor LocalLinkColor = new BaseColor(238, 232, 170);
        public BaseColor LabelColor = new BaseColor(245, 255, 250);
        public BaseColor BaseUUIDColor = new BaseColor(255, 250, 205);

        public PdfOutline RootOutline { get { return this.WWriter.RootOutline; } }

        public string URL { get; set; }
        public AO.UUIDClass BaseUUID { get; set; }

        public Rectangle PageSize { get { return this.WDoc.PageSize; } }
        #endregion

        #region Methods
        public void Save()
        {
            // Close the document
            this.WDoc.Close();
        }

        public void PageBreak()
        {
            this.WDoc.NewPage();
        }

        public void AddParagraph(string text)
        {
            this.WDoc.Add(new Paragraph(text));
        }

        public void SkipLines(int lines = 1)
        {
            while (lines > 0)
            {
                this.WDoc.Add(new Paragraph(" "));
                lines--;
            }
        }

        public PdfOutline FormatObject(OrgObjectClass obj, bool breakby, bool noentry, PdfOutline chapter = null)
        {
            //
            bool bShowCaption = false;

            //
            if (chapter == null)
            {
                chapter = this.AddChapter(obj.Description);
            }
            else
            {
                bShowCaption = true;
            }

            //this.SkipLines();

            // Build HTML
            AO.Definitions.DatasetClass c_DS = obj.Object.Dataset.Definition;
            List<AO.Definitions.DatasetFieldClass> c_Fields = new List<AO.Definitions.DatasetFieldClass>();
            foreach (string sFld in c_DS.Fields.Names)
            {
                c_Fields.Add(c_DS[sFld]);
            }

            // Now for the base fields
            PDFTableX c_Body = this.MakeView(chapter,
                                                obj.Object.Dataset,
                                                obj.Object.Dataset.View("default"), // TBD
                                                obj,
                                                obj.Object.ObjectDescription);

            // Documents
            if (bShowCaption && obj.Documents.Count > 0)
            {
                this.AddSplit(c_Body, "Documents", true);

                List<OrgDocumentClass> c_Docs = obj.Documents.Sorted;
                foreach (OrgDocumentClass c_Doc in c_Docs)
                {
                    this.AddDocument(c_Body, c_Doc);
                }
            }

            // Add
            this.WDoc.Add(c_Body);
            this.SkipLines();

            return c_Body.Node;
        }

        public void AddTable(PDFTableX table)
        {
            this.WDoc.Add(table);
        }

        public void AddPDF(IElement ele)
        {
            this.WDoc.Add(ele);
        }

        private PdfOutline AddLabel(PDFTableX table,
                                        PdfOutline sect,
                                        string label,
                                        string id,
                                        int depth,
                                        bool noentry)
        {
            if (!label.IsSameValue(table.LastLabel))
            {
                table.LastLabel = label;

                PdfPCell c_Cell = null;

                if (sect != null)
                {
                    // The ID
                    string sID = id;
                    if (!sID.HasValue()) sID = "L".GUID();

                    // 
                    Chunk c_A = new Chunk(label);
                    c_A.SetLocalDestination(sID);
                    Paragraph c_P = new Paragraph();
                    c_P.Add(c_A);

                    c_Cell = new PdfPCell(c_P);

                    if (!noentry)
                    {
                        PdfAction c_Action = PdfAction.GotoLocalPage(sID, false);
                        sect = new PdfOutline(sect, c_Action, label);
                    }
                }
                else
                {
                    c_Cell = new PdfPCell(new Phrase(label));
                }

                c_Cell.Colspan = table.NumberOfColumns;
                c_Cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

                c_Cell.BackgroundColor = depth == -1 ? ObjectHeaderColor : ObjectLabelColor;
                table.AddCell(c_Cell);

                if (table.LabelStart == 0)
                {
                    table.LabelStart = table.Rows.Count;
                }
            }

            return sect;
        }

        private void AddDocument(PDFTableX table, OrgDocumentClass doc)
        {
            PdfPCell c_Cell = null;

            // The ID
            string sID = doc.ID;

            // 
            Chunk c_A = new Chunk(doc.Description);
            c_A.SetAction(PdfAction.GotoLocalPage(sID, false));
            Paragraph c_P = new Paragraph();
            c_P.Add(c_A);

            c_Cell = new PdfPCell(c_P);

            c_Cell.Colspan = table.NumberOfColumns;
            c_Cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

            c_Cell.BackgroundColor = this.LocalLinkColor;
            table.AddCell(c_Cell);
        }

        private void AddObject(PDFTableX table, OrgObjectClass obj)
        {
            PdfPCell c_Cell = null;

            // The ID
            string sID = obj.ID;

            // 
            Chunk c_A = new Chunk(obj.Description);
            c_A.SetAction(PdfAction.GotoLocalPage(sID, false));
            Paragraph c_P = new Paragraph();
            c_P.Add(c_A);

            c_Cell = new PdfPCell(c_P);

            c_Cell.Colspan = table.NumberOfColumns;
            c_Cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

            c_Cell.BackgroundColor = this.LocalLinkColor;
            table.AddCell(c_Cell);
        }

        private void AddSplit(PDFTableX table, string label, bool fill)
        {
            PdfPCell c_Cell = null;
            c_Cell = new PdfPCell(new Phrase(label));

            c_Cell.Colspan = table.NumberOfColumns;
            c_Cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

            c_Cell.BackgroundColor = fill ? BaseColor.LIGHT_GRAY : BaseColor.WHITE;
            table.AddCell(c_Cell);
        }

        private void AddRow(PDFTableX table, params object[] values)
        {
            table.LastLabel = null;

            for (int iCol = 0; iCol < table.NumberOfColumns; iCol++)
            {
                if (iCol >= values.Length)
                {
                    table.AddCell("");
                }
                else
                {
                    object c_Val = values[iCol];
                    if (c_Val is Paragraph)
                    {
                        table.AddCell((Paragraph)c_Val);
                    }
                    else
                    {
                        table.AddCell(c_Val.ToStringSafe());
                    }
                }
            }

            if (table.LabelStart != 0)
            {
                List<int> c_Rows = new List<int>();
                for (int iRow = table.LabelStart; iRow <= table.Rows.Count; iRow++)
                {
                    c_Rows.Add(iRow - 1);
                }

                table.KeepRowsTogether(c_Rows.ToArray());

                table.LabelStart = 0;
            }
        }

        private Paragraph MakeLink(string value, string uri)
        {
            Chunk c_CE = new Chunk(value);
            c_CE.SetAction(new PdfAction(new Uri(uri)));
            Paragraph c_PE = new Paragraph();
            c_PE.Add(c_CE);

            return c_PE;
        }

        private PDFTableX MakeView(
                                    PdfOutline section,
                                    AO.DatasetClass ds,
                                    AO.Definitions.ViewClass viewdef,
                                    OrgObjectClass obj,
                                    string label,
                                    PDFTableX table = null,
                                    int depth = 0,
                                    JObject values = null)
        {
            // Get the definition
            AO.Definitions.DatasetClass c_DSDef = ds.Definition;

            // Create the table
            if (table == null)
            {
                table = new PDFTableX(2);
                table.SetWidths(new float[] { 1f, 2f });
            }

            // Add the section
            section = this.AddLabel(table, section, label, null, depth, false);

            // Add the fields
            foreach (string sFld in viewdef.Fields.Names)
            {
                // Get the definition
                AO.Definitions.DatasetFieldClass c_Fld = c_DSDef[sFld];

                // Only the valid ones
                if (c_Fld != null && c_Fld.Type != AO.Definitions.DatasetFieldClass.FieldTypes.Hidden)
                {
                    // Obtained
                    string sName = c_Fld.Name;

                    string sValue = null;
                    if (values != null)
                    {
                        sValue = values.Get(sName);
                    }
                    else
                    {
                        sValue = obj.Get(sName);
                    }

                    object c_Label = c_Fld.Label;
                    BaseColor c_LabelColor = this.LabelColor;
                    object c_Value = new Phrase(sValue);
                    BaseColor c_ValueColor = BaseColor.WHITE;

                    switch (c_Fld.Type)
                    {

                        case AO.Definitions.DatasetFieldClass.FieldTypes.Tabs:
                            // Add the label
                            this.AddLabel(table, null, c_Fld.Label, null, depth, true);
                            // Get the views
                            List<string> c_Views = c_Fld.GridView.SplitSpaces();
                            // Loop thru
                            foreach (string sView in c_Views)
                            {
                                // Make the section

                                // Get the view
                                AO.Definitions.ViewClass c_ViewT = ds.View(sView);
                                // Draw
                                this.MakeView(section, ds, c_ViewT, obj, c_ViewT.Caption, table, depth + 1);
                            }
                            break;

                        //case AO.Definitions.DatasetFieldClass.FieldTypes.Group:
                        //    // Get the view
                        //    AO.Definitions.ViewClass c_View = ds.View(c_Fld.GridView);

                        //    this.MakeView(
                        //                            section,
                        //                            ds,
                        //                            c_View,
                        //                            obj,
                        //                            c_Fld.Label,
                        //                            table,
                        //                            depth + 1);

                        //    c_Value = null;
                        //    break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.Grid:
                            // Add the label
                            this.AddLabel(table, null, c_Fld.Label, null, depth, true);
                            // Convert to array
                            JArray c_Values = sValue.ToJArray();
                            if (c_Values != null)
                            {
                                // Get the view
                                AO.Definitions.ViewClass c_ViewG = ds.View(c_Fld.GridView);
                                // Loop thru
                                for (int j = 0; j < c_Values.Count; j++)
                                {
                                    this.MakeView(
                                                    section,
                                                    ds,
                                                    c_ViewG,
                                                    obj,
                                                    "Row #{0}".FormatString(j + 1),
                                                    table,
                                                    depth + 1,
                                                    c_Values.GetJObject(j));
                                }
                            }
                            c_Value = null;
                            break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.Boolean:
                            c_Value = new Phrase(sValue.HasValue() ? (  sValue.FromDBBoolean() ? "YES" : "NO") : "");
                            break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.Date:
                            if (sValue.HasValue()) c_Value = new Paragraph(sValue.FromDBDate().AdjustTimezone().ToShortDateString());
                            break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.DateTime:
                            if (sValue.HasValue()) c_Value = new Paragraph(sValue.FromDBDate().AdjustTimezone().ToString());
                            break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.Label:
                            this.AddLabel(table, null, c_Fld.Label, null, depth, true);
                            break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.Link:
                            if (sValue.HasValue())
                            {
                                if (AO.UUIDClass.IsValid(sValue))
                                {
                                    AO.UUIDClass c_UUID = new AO.UUIDClass(this.Env.Database, sValue);
                                    using (AO.ObjectClass c_Linked = c_UUID.Dataset[c_UUID.ID])
                                    {
                                        sValue = c_Linked.ObjectDescription;
                                    }

                                    c_Value = new Paragraph(sValue);
                                }
                                else
                                {
                                    c_Value = new Paragraph("");
                                }
                            }
                            break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.Upload:
                        case AO.Definitions.DatasetFieldClass.FieldTypes.CreditCard:
                        case AO.Definitions.DatasetFieldClass.FieldTypes.CreditCardExp:
                        case AO.Definitions.DatasetFieldClass.FieldTypes.Image:
                        case AO.Definitions.DatasetFieldClass.FieldTypes.Signature:
                        case AO.Definitions.DatasetFieldClass.FieldTypes.Duration:
                        case AO.Definitions.DatasetFieldClass.FieldTypes.Button:
                            c_Value = null;
                            break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.EMail:
                            if (sValue.HasValue())
                            {
                                c_Value = this.MakeLink(sValue, "mailto:{0}".FormatString(sValue));
                                c_ValueColor = this.LinkColor;
                            }
                            break;

                        case AO.Definitions.DatasetFieldClass.FieldTypes.Phone:
                            if (sValue.HasValue())
                            {
                                c_Value = this.MakeLink(sValue, "tel:{0}".FormatString(sValue.NumOnly()));
                                c_ValueColor = this.LinkColor;
                            }
                            break;

                        default:
                            break;
                    }

                    if (c_Value != null)
                    {
                        table.LastLabel = null;

                        if (c_Label == null) c_Label = "";
                        PdfPCell c_Cell = new PdfPCell(new Phrase(c_Label.ToStringSafe()));
                        c_Cell.BackgroundColor = c_LabelColor;
                        table.AddCell(c_Cell);

                        if (c_Value is Paragraph)
                        {
                            c_Cell = new PdfPCell((Paragraph)c_Value);
                        }
                        else if (c_Value is Phrase)
                        {
                            c_Cell = new PdfPCell((Phrase)c_Value);
                        }
                        else
                        {
                            c_Cell = new PdfPCell(new Phrase(c_Value.ToStringSafe()));
                        }
                        c_Cell.BackgroundColor = c_ValueColor;
                        table.AddCell(c_Cell);

                        if (table.LabelStart != 0)
                        {
                            List<int> c_Rows = new List<int>();

                            for (int iRow = table.LabelStart; iRow <= table.Rows.Count; iRow++)
                            {
                                c_Rows.Add(iRow - 1);
                            }

                            table.KeepRowsTogether(c_Rows.ToArray());
                            table.LabelStart = 0;
                        }

                    }
                }
            }

            table.Node = section;

            return table;
        }

        public void FormatDocument(OrgDocumentClass doc, PdfOutline node, string caption)
        {
            try
            {
                //
                using (PDFClass c_PDF = new PDFClass())
                {
                    Proc.Docs.Files.PDFDocumentClass c_PDFD = doc.Document.PDF();
                    if (c_PDFD != null)
                    {
                        byte[] abAns = c_PDF.Flatten(c_PDFD.ValueAsBytes);

                        //
                        PdfReader c_Reader = new PdfReader(abAns);
                        PdfContentByte c_CB = this.WWriter.DirectContent;
                        int iPages = c_Reader.NumberOfPages;

                        for (int iPage = 1; iPage < iPages + 1; iPage++)
                        {
                            this.WHandler.PageLabel = doc.Description + " ({0}/{1})".FormatString(iPage, iPages);

                            this.PageBreak();

                            if (iPage == 1)
                            {
                                Chunk c_A = new Chunk(" ");
                                c_A.SetLocalDestination(doc.ID);
                                Paragraph c_P = new Paragraph();
                                c_P.Add(c_A);

                                this.WDoc.Add(c_P);

                                PdfAction c_Action = PdfAction.GotoLocalPage(doc.ID, false);
                                PdfOutline c_New = new PdfOutline(node, c_Action, caption);
                            }

                            AffineTransform c_Trans = this.getAffineTransform(c_Reader, this.WWriter, iPage); ;

                            PdfImportedPage c_Page = this.WWriter.GetImportedPage(c_Reader, iPage);

                            c_CB.AddTemplate(c_Page, c_Trans);
                        }

                        List<OrgObjectClass> c_Uses = doc.ObjectUses;
                        if (c_Uses.Count > 0)
                        {
                            this.WHandler.PageLabel = doc.Description;

                            PDFTableX table = new PDFTableX(1);
                            this.AddLabel(table, null, "Uses", null, 1, true);

                            foreach (OrgObjectClass c_Obj in c_Uses)
                            {
                                this.AddObject(table, c_Obj);
                            }

                            this.PageBreak();
                            this.WDoc.Add(table);
                        }
                    }
                }
            }
            catch { }
        }

        public PdfOutline AddChapter(string label, bool open = true, PdfOutline at = null)
        {
            // Phony pointer
            string sID = "M{0}".FormatString(label);

            bool bNoPage = false;
            if (at == null && this.WWriter.RootOutline.Kids.Count == 0) bNoPage = true;

            if (!bNoPage) this.PageBreak();
            //this.WDoc.Add(new Chunk(title, new Font(MM.Vendors.iTextIF.PDFClass.WMBaseFont, 14f, Font.NORMAL, BaseColor.BLUE)));

            Chunk c_A = new Chunk(" ");
            c_A.SetLocalDestination(sID);
            Paragraph c_P = new Paragraph();
            c_P.Add(c_A);
            this.WDoc.Add(c_P);

            if (at == null) at = this.WWriter.RootOutline;

            // Display
            PdfAction c_Action = PdfAction.GotoLocalPage(sID, false);
            PdfOutline c_C = new PdfOutline(at, c_Action, label, open);

            return c_C; ;
        }

        public PdfOutline AddStub(string label, bool open = true, PdfOutline at = null)
        {
            if (at == null) at = this.WWriter.RootOutline;

            // Display
            PdfAction c_Action = null;
            PdfOutline c_C = new PdfOutline(at, c_Action, this.FormatLabel(label), open);

            return c_C; ;
        }

        private AffineTransform getAffineTransform(PdfReader reader, PdfWriter writer, int pageNum)
        {
            Rectangle readerPageSize = reader.GetPageSize(pageNum);
            Rectangle writerPageSize = writer.PageSize;

            float rPageHeight = readerPageSize.Height;
            float rPageWidth = readerPageSize.Width;

            float wPageHeight = writerPageSize.Height;
            float wPageWidth = writerPageSize.Width;

            int pageRotation = reader.GetPageRotation(pageNum);

            bool rotate = (rPageWidth > rPageHeight) && (pageRotation == 0 || pageRotation == 180);
            if (!rotate)
                rotate = ((rPageHeight > rPageWidth) && (pageRotation == 90 || pageRotation == 270));
            //if changing rotation gives us better space rotate an extra 90 degrees.
            if (rotate) pageRotation += 90;
            double randrotate = (double)pageRotation * Math.PI / (double)180;

            AffineTransform transform = new AffineTransform();
            float margin = 0;
            float scale = 1.0f;
            if (pageRotation == 90 || pageRotation == 270)
            {
                scale = Math.Min((wPageHeight - 2 * margin) / rPageWidth, (wPageWidth - 2 * margin) / rPageHeight);
            }
            else
            {
                scale = Math.Min(wPageHeight / rPageHeight, wPageWidth / rPageWidth);
            }

            transform.Translate((wPageWidth / 2) + margin, wPageHeight / 2 + margin);
            transform.Rotate(-randrotate);
            transform.Scale(scale, scale);
            transform.Translate(-rPageWidth / 2, -rPageHeight / 2);
            return transform;
        }

        private string FormatLabel(string value)
        {
            int iPos = value.IndexOf("\t");
            if (iPos != -1) value = value.Substring(iPos + 1);

            return value;
        }
        #endregion
    }

    public class MainTextEventsHandler : PdfPageEventHelper
    {
        #region Properties
        public PDFDocumentClass Document { get; set; }
        public string DocumentLabel { get; set; }
        public string PageLabel { get; set; }

        public float DocumentLabelSize { get; set; } = 12f;
        public float PageLabelSize { get; set; } = 18f;
        public float BatesSize { get; set; } = 12f;
        public float Rotation { get; set; } = 90f;
        public float BatesRotation { get; set; } = 0f;
        public BaseColor LabelColor { get; set; } = new BaseColor(70, 70, 255);
        public BaseColor BatesColor { get; set; } = new BaseColor(178, 34, 34);
        //public Rectangle Size { get; set; }

        private int Page { get; set; }

        private string LastDocumentLabel { get; set; }
        private string LastPageLabel { get; set; }
        private string LastPageNumber { get; set; }

        public bool DoAfter { get; set; }
        public string DateStamp { get; set; }
        #endregion

        #region Methods
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            //
            this.Page++;

            // Save
            this.LastDocumentLabel = this.DocumentLabel;
            this.LastPageLabel = this.PageLabel;
            this.LastPageNumber = this.Page.ToString();

            if (!this.DoAfter)
            {
                this.WriteIt(writer, document);
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            if (this.DoAfter)
            {
                this.WriteIt(writer, document);
            }

            base.OnEndPage(writer, document);
        }

        private void WriteIt(PdfWriter writer, Document document)
        {
            PdfContentByte c_CB = writer.DirectContent;
            Rectangle c_Size = writer.PageSize;

            string sPage = this.LastPageNumber;
            if (this.DateStamp.HasValue())
            {
                sPage = this.DateStamp + " - " + sPage;
            }

            if (this.LastDocumentLabel.HasValue())
            {
                Proc.Docs.Files.PDFClass.DoLeftMargin(c_CB,
                                                            this.LastDocumentLabel,
                                                            null,
                                                            this.DocumentLabelSize,
                                                            this.Rotation,
                                                            this.LabelColor,
                                                            c_Size);
            }

            if (this.LastPageLabel.HasValue())
            {
                Proc.Docs.Files.PDFClass.DoRightMargin(c_CB,
                                                            this.LastPageLabel, null,
                                                            this.PageLabelSize,
                                                            this.Rotation,
                                                            this.LabelColor,
                                                            c_Size);
            }

            Proc.Docs.Files.PDFClass.DoBates(c_CB,
                                                            sPage, null,
                                                            this.BatesSize,
                                                            this.BatesRotation,
                                                            this.BatesColor,
                                                            c_Size);
        }
        #endregion
    }

    public class PDFTableX : PdfPTable
    {
        #region Constructor
        public PDFTableX(int cols)
            : base(cols)
        { }
        #endregion

        #region Properties
        public int LabelStart { get; set; }
        public string LastLabel { get; set; }

        public PdfOutline Node { get; set; }
        #endregion
    }

    public class PDFOutlineNode
    {
        #region Properties
        public PdfOutline Root { get; set; }
        public PdfOutline At { get; set; }
        #endregion
    }
}
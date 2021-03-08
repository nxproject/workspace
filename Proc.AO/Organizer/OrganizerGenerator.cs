///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.AO
{
    public class OrganizerGeneratorClass : BasedObjectClass
    {
        #region Constants
        //internal const string OrganizerFileName = "Organizer.pdf";
        public const string Creator = "NXOrganizer";
        #endregion

        #region Constructor
        public OrganizerGeneratorClass(OrgContextClass env,
                                        AO.ObjectClass obj,
                                        JObject values,
                                        string rules,
                                        string doc,
                                        string folder,
                                        string ts,
                                        string url)
            : this(env, obj, values, OrganizerRulesClass.Make(rules), doc, folder, ts, url)
        { }

        public OrganizerGeneratorClass(OrgContextClass env,
                                        AO.ObjectClass obj,
                                        JObject values,
                                        OrganizerRulesClass rules,
                                        string doc,
                                        string folder,
                                        string ts,
                                        string url)
            : base(obj, true)
        {
            //
            this.Env = env;
            string sID = obj.UUID.ToString();
            this.Objects = new OrgObjectsClass(env);
            this.Rules = rules;
            if (this.Rules == null) this.Rules = new OrganizerRulesClass();
            this.DateStamp = ts.IfEmpty();
            this.URL = url;

            // Info
            string sDSR = obj.UUID.Dataset.Name;
            AO.DatasetClass c_DS = env.Associate.Datasets[sDSR];

            // Map ourselves
            this.RootObject = this.Objects.Add(obj);

            // If no values, use object
            if (values == null || values.Keys().Count == 0)
            {
                values = this.RootObject.Object.Values;
            }
            else
            {
                this.RootObject.Values = values;
            }

            // Now for the name
            string sDoc = doc.IfEmpty("Organizer{0}.pdf");
            if (c_DS != null && c_DS.OrganizerName.HasValue())
            {
                string sExtra = this.RootObject.ApplyFormat(c_DS.OrganizerName);
                if (sExtra.HasValue()) sExtra = " " + sExtra;
                sDoc = sDoc.FormatString(sExtra);
            }
            else
            {
                sDoc = sDoc.FormatString("");
            }

            // Create the result
            if (folder.HasValue())
            {
                this.OutputDocument = new DocReferenceClass(this.Env, folder, sDoc, this.RootObject.Object);
            }
            else
            {
                this.OutputDocument = new DocReferenceClass(this.Env, sDoc, this.RootObject.Object);
            }

            this.OutputDocument.FolderPath.AssurePath();
            this.OutputDocument.Delete();

            // Make the base map
            if (this.Rules.IncludeDocuments(sDSR))
            {
                this.RootObject.Documents = new OrgDocumentsClass(env, this.RootObject);

                List<string> c_Names = new List<string>();
                foreach (OrgDocumentClass c_Doc in this.RootObject.Documents.Sorted)
                {
                    c_Names.Add(c_Doc.Description);
                }
                //env.Env.LogInfo("ORG: Root docs - {0} - {1}".FormatString(c_Names.Count, c_Names.Join(", ")));
            }
            else
            {
                //env.Env.LogInfo("ORG: No root docs");
                this.RootObject.Documents = new OrgDocumentsClass();
            }

            // Get the dataset            
            if (c_DS != null)
            {
                // Make the query
                AO.XQueryClass c_Qry = new AO.XQueryClass();
                c_Qry.AddFilter("_parent", sID);

                // Get the sub datasets to search
                List<string> c_Sub = new List<string>();

                AO.LayoutClass c_Lay = c_DS.Layout;
                c_Sub.AddRange(c_Lay.Children);

                foreach (AO.LinkDefinitionClass c_Link in c_Lay.Links)
                {
                    if (c_Sub.IndexOf(c_Lay.Dataset.Name) == -1)
                    {
                        c_Sub.Add(c_Lay.Dataset.Name);
                    }
                }

                // Do the links
                foreach (string sFld in c_DS.Links)
                {
                    // Get the value
                    string sValue = values.Get(sFld).IfEmpty();
                    // UUID?
                    if (sValue.HasValue() && AO.UUIDClass.IsValid(sValue))
                    {
                        // Make it
                        AO.UUIDClass c_UUID = new AO.UUIDClass(sValue);
                        string sDS = c_UUID.Dataset;

                        if (!c_DS.ExcludeFromOrganizer)
                        {
                            // Flags
                            bool bInclOrg = this.Rules.IncludeInOrganizer(sDS);
                            bool bInclDocs = this.Rules.IncludeDocuments(sDS);

                            // Do we do?
                            if (bInclOrg || bInclDocs)
                            {
                                if (c_Sub.IndexOf(sDS) == -1)
                                {
                                    c_Sub.Add(sDS);
                                }

                                // And add
                                OrgObjectClass c_Entry = this.Objects.Add(c_UUID);

                                // Build the map
                                if (bInclDocs)
                                {
                                    // Map
                                    c_Entry.Documents = new OrgDocumentsClass(this.Env, c_Entry);
                                    // And add to overall map
                                    this.RootObject.Documents.Merge(c_Entry.Documents);
                                }
                            }
                        }
                    }
                }

                // Now process each
                foreach (string sDS in c_Sub)
                {
                    // Flags
                    bool bInclOrg = this.Rules.IncludeInOrganizer(sDS);
                    bool bInclDocs = this.Rules.IncludeDocuments(sDS);

                    // Do we do?
                    if (bInclOrg || bInclDocs)
                    {
                        // Get related
                        List<AO.UUIDClass> c_Related = env.Associate.Query(sDS, c_Qry);

                        // Process each
                        foreach (AO.UUIDClass c_SubID in c_Related)
                        {
                            // Make the entry
                            OrgObjectClass c_Entry = this.Objects.Add(c_SubID);

                            // Build the map
                            if (bInclDocs)
                            {
                                // Map
                                c_Entry.Documents = new OrgDocumentsClass(this.Env, c_Entry);
                                // And add to overall map
                                this.RootObject.Documents.Merge(c_Entry.Documents);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Properties
        private OrgContextClass Env { get; set; }
        internal AO.ObjectClass Object { get { return this.Root as AO.ObjectClass; } }
        public DocumentClass OutputDocument { get; set; }

        private OrgObjectsClass Objects { get; set; }
        private OrgObjectClass RootObject { get; set; }

        internal OrganizerRulesClass Rules { get; set; }
        internal string DateStamp { get; set; }
        internal string URL { get; set; }
        #endregion

        #region Methods
        public TreeNodeClass GenerateTree()
        {
            // Result
            TreeNodeClass c_Doc = new TreeNodeClass(null, null);

            try
            {
                // Setup
                string sDSRoot = this.RootObject.Object.UUID.Dataset;

                // Do we do the root?
                if (this.Rules.IncludeInOrganizer(sDSRoot))
                {
                    //
                    c_Doc.FormatObject(this.RootObject);
                }

                //List<OrgObjectClass> c_AllObjects = new List<OrgObjectClass>();
                //c_AllObjects.Add(this.RootObject);

                // All of the datasets!
                foreach (string sDS in this.Objects.Datasets)
                {
                    if (!sDS.IsSameValue(sDSRoot) && this.Rules.IncludeInOrganizer(sDS))
                    {
                        OrgDSClass c_DSM = this.Objects[sDS];
                        List<OrgObjectClass> c_List = c_DSM.Sorted;

                        if (c_List.Count > 0)
                        {
                            TreeNodeClass c_C = c_Doc.AddChapter(this.Env.Associate.Datasets[sDS].Description);

                            foreach (OrgObjectClass c_Entry in c_List)
                            {
                                if (!c_DSM.ByOnly)
                                {
                                    // Add
                                    TreeNodeClass c_At = c_Doc.FormatObject(c_Entry, c_C);

                                    // Point to the documents
                                    if (c_At != null && !c_DSM.ByOnly)
                                    {
                                        List<OrgDocumentClass> c_Docs = this.RootObject.Documents.OwnedBy(c_Entry);
                                        if (c_Docs.Count > 0)
                                        {
                                            string sCaption = "Document" + (c_Docs.Count == 1 ? "" : "s");
                                            TreeNodeClass c_AtX = c_Doc.AddStub(sCaption, c_At);

                                            foreach (OrgDocumentClass c_DocC in c_Docs)
                                            {
                                                c_Doc.FormatDocument(c_DocC, c_AtX, c_DocC.Description);
                                            }
                                        }
                                    }

                                    //c_AllObjects.Add(c_Entry);
                                }
                            }

                            // Do the By
                            if (c_DSM.ByFields.Count > 0)
                            {
                                Dictionary<string, Dictionary<string, List<OrgObjectClass>>> c_Entries = c_DSM.By(true);

                                List<string> c_Bys = new List<string>(c_Entries.Keys);
                                c_Bys.Sort();

                                foreach (string sBy in c_Bys)
                                {
                                    TreeNodeClass c_At = c_Doc.AddStub(sBy, c_C);

                                    Dictionary<string, List<OrgObjectClass>> c_Types = c_Entries[sBy];
                                    List<string> c_TypesS = new List<string>(c_Types.Keys);
                                    c_TypesS.Sort();

                                    foreach (string sType in c_TypesS)
                                    {
                                        TreeNodeClass c_At2 = c_Doc.AddStub(sType, c_At);

                                        List<OrgObjectClass> c_Objs = c_Types[sType];
                                        for (int iObj = 0; iObj < c_Objs.Count; iObj++)
                                        {
                                            OrgObjectClass c_Obj = c_Objs[iObj];

                                            TreeNodeClass c_Node = new TreeNodeClass(c_Obj.Description, c_Obj.Object.UUID.ToString(), c_At2);
                                            c_At2.Add(c_Node);

                                            // Point to the documents
                                            List<OrgDocumentClass> c_DocsX = this.RootObject.Documents.OwnedBy(c_Obj);
                                            if (c_DocsX.Count > 0)
                                            {
                                                string sCaption = "Document" + (c_DocsX.Count == 1 ? "" : "s");
                                                TreeNodeClass c_At3 = c_Doc.AddStub(sCaption, c_At2);

                                                foreach (OrgDocumentClass c_DocC in c_DocsX)
                                                {
                                                    c_Doc.FormatDocument(c_DocC, c_At3, c_DocC.Description);
                                                    //TreeNodeClass c_NewZ = new TreeNodeClass(c_DocC.Description, c_DocC.ID, c_At3);
                                                    //c_At3.Add(c_NewZ);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Any Documents?            
                if (this.Rules.IncludeDocuments(sDSRoot))
                {
                    bool bDocsAlpha = this.Env.Associate.Datasets[sDSRoot].OrganizeByAlpha;

                    List<OrgDocumentClass> c_Docs = this.RootObject.Documents.Sorted;

                    if (c_Docs.Count > 0)
                    {
                        string sCaption = "Document" + (c_Docs.Count == 1 ? "" : "s");
                        TreeNodeClass c_C = c_Doc.AddStub(sCaption);

                        // To alphabetize
                        string sFirstC = null;
                        TreeNodeClass c_First = null;

                        for (int iDoc = 0; iDoc < c_Docs.Count; iDoc++)
                        {
                            OrgDocumentClass c_File = c_Docs[iDoc];

                            TreeNodeClass c_At0 = c_C;
                            if (bDocsAlpha)
                            {
                                string sNextC = c_File.Group; // c_File.Description.Substring(0, 1);
                                if (c_First == null || !sFirstC.IsSameValue(sNextC))
                                {
                                    sFirstC = sNextC;
                                    c_First = c_Doc.AddStub(sFirstC, c_At0);
                                }
                                c_At0 = c_First;
                            }

                            List<OrgDocumentClass> c_SameName = new List<OrgDocumentClass>();
                            c_SameName.Add(c_File);

                            for (int iDocN = iDoc + 1; iDocN < c_Docs.Count; iDocN++)
                            {
                                if (!c_File.Description.IsSameValue(c_Docs[iDocN].Description))
                                {
                                    break;
                                }
                                else
                                {
                                    iDoc++;
                                    c_SameName.Add(c_Docs[iDocN]);
                                }
                            }

                            TreeNodeClass c_At = c_At0;
                            if (c_SameName.Count != 1)
                            {
                                c_At = c_Doc.AddStub(c_File.Description, c_At);
                            }

                            foreach (OrgDocumentClass c_FileX in c_SameName)
                            {
                                string sCaptionX = c_FileX.Description;
                                if (c_SameName.Count != 1) sCaptionX = c_FileX.Object.Description;

                                c_Doc.FormatDocument(c_FileX, c_At, sCaptionX);
                            }
                        }
                    }
                }

                //c_Doc.AllObjects = c_AllObjects;
                c_Doc.AllObjects = this.Objects;
            }
            catch (Exception e)
            {
                this.Env.Env.LogException("GenTree", e);
            }

            return c_Doc;
        }

        public DocReferenceClass GeneratePDF()
        {
            try
            {
                // Setup
                string sDSRoot = this.RootObject.Object.UUID.Dataset;

                // Document
                PDFDocumentClass c_Doc = new PDFDocumentClass(this.Env,
                                                                this.OutputDocument,
                                                                this.RootObject.Description,
                                                                OrganizerGeneratorClass.Creator,
                                                                this.RootObject.Object.UUID,
                                                                this.DateStamp,
                                                                this.URL);

                // Do we do the root?
                if (this.Rules.IncludeInOrganizer(sDSRoot))
                {
                    //
                    c_Doc.FormatObject(this.RootObject, false, false);

                    //OrgDSClass c_DSM = this.Objects[sDSRoot];
                    //PdfOutline c_C = c_Doc.AddChapter(this.Env.Associate.Datasets[sDSRoot].Description, false);
                    //PdfOutline c_At = c_Doc.FormatObject(this.RootObject, true, c_DSM.ByOnly, c_C);

                    //List<OrgDocumentClass> c_Docs = this.RootObject.Documents.OwnedBy(this.RootObject);
                    //if (c_Docs.Count > 0)
                    //{
                    //    string sCaption = "Document" + (c_Docs.Count == 1 ? "" : "s");
                    //    c_At = c_Doc.AddStub(sCaption, false, c_At);

                    //    foreach (OrgDocumentClass c_DocC in c_Docs)
                    //    {
                    //        PdfAction c_Action = PdfAction.GotoLocalPage(c_DocC.ID, false);
                    //        PdfOutline c_New = new PdfOutline(c_At, c_Action, c_DocC.Description);
                    //    }
                    //}
                }

                // All of the datasets!
                foreach (string sDS in this.Objects.Datasets)
                {
                    if (!sDS.IsSameValue(sDSRoot) && this.Rules.IncludeInOrganizer(sDS))
                    {
                        OrgDSClass c_DSM = this.Objects[sDS];

                        List<OrgObjectClass> c_List = c_DSM.Sorted;

                        if (c_List.Count > 0)
                        {
                            c_Doc.WHandler.PageLabel = this.Env.Associate.Datasets[sDS].Description;
                            PdfOutline c_C = c_Doc.AddChapter(this.Env.Associate.Datasets[sDS].Description, false);

                            foreach (OrgObjectClass c_Entry in c_List)
                            {
                                // Add
                                if (!c_DSM.ByOnly || c_Entry.HasBy(c_DSM.ByFields))
                                {
                                    PdfOutline c_At = c_Doc.FormatObject(c_Entry, true, c_DSM.ByOnly, c_C);

                                    // Point to the documents
                                    if (c_At != null && !c_DSM.ByOnly)
                                    {
                                        List<OrgDocumentClass> c_Docs = this.RootObject.Documents.OwnedBy(c_Entry);
                                        if (c_Docs.Count > 0)
                                        {
                                            string sCaption = "Document" + (c_Docs.Count == 1 ? "" : "s");
                                            c_At = c_Doc.AddStub(sCaption, false, c_At);

                                            foreach (OrgDocumentClass c_DocC in c_Docs)
                                            {
                                                PdfAction c_Action = PdfAction.GotoLocalPage(c_DocC.ID, false);
                                                PdfOutline c_New = new PdfOutline(c_At, c_Action, c_DocC.Description);
                                            }
                                        }
                                    }
                                }
                            }

                            // Do the By
                            if (c_DSM.ByFields.Count > 0)
                            {
                                Dictionary<string, Dictionary<string, List<OrgObjectClass>>> c_Entries = c_DSM.By(false);
                                List<string> c_Bys = new List<string>(c_Entries.Keys);
                                c_Bys.Sort();

                                foreach (string sBy in c_Bys)
                                {
                                    PdfOutline c_At = c_Doc.AddStub(sBy, false, c_C);

                                    Dictionary<string, List<OrgObjectClass>> c_Types = c_Entries[sBy];
                                    List<string> c_TypesS = new List<string>(c_Types.Keys);
                                    c_TypesS.Sort();

                                    foreach (string sType in c_TypesS)
                                    {
                                        PdfOutline c_At2 = c_Doc.AddStub(sType, false, c_At);

                                        List<OrgObjectClass> c_Objs = c_Types[sType];
                                        for (int iObj = 0; iObj < c_Objs.Count; iObj++)
                                        {
                                            OrgObjectClass c_Obj = c_Objs[iObj];

                                            PdfAction c_Action = PdfAction.GotoLocalPage(c_Obj.ID, false);
                                            PdfOutline c_New = new PdfOutline(c_At2, c_Action, c_Obj.Description);

                                            // Point to the documents
                                            List<OrgDocumentClass> c_DocsX = this.RootObject.Documents.OwnedBy(c_Obj);
                                            if (c_DocsX.Count > 0)
                                            {
                                                string sCaption = "Document" + (c_DocsX.Count == 1 ? "" : "s");
                                                PdfOutline c_At3 = c_Doc.AddStub(sCaption, false, c_At2);

                                                foreach (OrgDocumentClass c_DocC in c_DocsX)
                                                {
                                                    PdfAction c_ActionZ = PdfAction.GotoLocalPage(c_DocC.ID, false);
                                                    PdfOutline c_NewZ = new PdfOutline(c_At3, c_ActionZ, c_DocC.Description);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Reverse
                c_Doc.WHandler.DoAfter = true;

                // Any Documents?            
                if (this.Rules.IncludeDocuments(sDSRoot))
                {
                    bool bDocsAlpha = this.Env.Associate.Datasets[sDSRoot].OrganizeByAlpha;

                    List<OrgDocumentClass> c_Docs = this.RootObject.Documents.Sorted;

                    if (c_Docs.Count > 0)
                    {
                        string sCaption = "Document" + (c_Docs.Count == 1 ? "" : "s");
                        PdfOutline c_C = c_Doc.AddStub(sCaption, true);

                        // To alphabetize
                        string sFirstC = null;
                        PdfOutline c_First = null;

                        for (int iDoc = 0; iDoc < c_Docs.Count; iDoc++)
                        {
                            OrgDocumentClass c_File = c_Docs[iDoc];

                            PdfOutline c_At0 = c_C;
                            if (bDocsAlpha)
                            {
                                string sNextC = c_File.Group; // c_File.Description.Substring(0, 1);
                                if (c_First == null || !sFirstC.IsSameValue(sNextC))
                                {
                                    sFirstC = sNextC;
                                    c_First = c_Doc.AddStub(sFirstC, false, c_At0);
                                }
                                c_At0 = c_First;
                            }

                            List<OrgDocumentClass> c_SameName = new List<OrgDocumentClass>();
                            c_SameName.Add(c_File);

                            for (int iDocN = iDoc + 1; iDocN < c_Docs.Count; iDocN++)
                            {
                                if (!c_File.Description.IsSameValue(c_Docs[iDocN].Description))
                                {
                                    break;
                                }
                                else
                                {
                                    iDoc++;
                                    c_SameName.Add(c_Docs[iDocN]);
                                }
                            }

                            PdfOutline c_At = c_At0;
                            if (c_SameName.Count != 1)
                            {
                                c_At = c_Doc.AddStub(c_File.Description, false, c_At);
                            }

                            foreach (OrgDocumentClass c_FileX in c_SameName)
                            {
                                string sCaptionX = c_FileX.Description;
                                if (c_SameName.Count != 1) sCaptionX = c_FileX.Object.Description;

                                c_Doc.FormatDocument(c_FileX, c_At, sCaptionX);
                            }
                        }
                    }
                }

                // End page
                //c_Doc.WHandler.PageLabel = "";
                //c_Doc.NewPage();

                // Store
                c_Doc.Save();
            }
            catch (Exception e)
            {
                this.Env.Env.LogException("GenPDF", e);
            }

            return this.OutputDocument;
        }

        #endregion
    }
}
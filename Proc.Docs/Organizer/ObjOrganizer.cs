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

using NX.Engine;
using NX.Shared;
using NX.Engine.Files;
using Proc.Docs;
using Proc.Docs.Files;
using Proc.AO;

namespace Proc.Docs
{
    public class OrganizerGeneratorClass : BasedObjectClass
    {
        #region Constants
        public const string Creator = "Organizer";
        #endregion

        #region Constructor
        public OrganizerGeneratorClass(AO.ExtendedContextClass ctx,
                                        AO.ObjectClass obj,
                                        JObject values,
                                        string rules,
                                        string doc,
                                        string folder,
                                        string ts,
                                        string url)
            : this(ctx, obj, values, OrganizerRulesClass.Make(rules), doc, folder, ts, url)
        { }

        public OrganizerGeneratorClass(AO.ExtendedContextClass env,
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
            AO.DatasetClass c_DS = obj.UUID.Dataset;

            // Map ourselves
            this.RootObject = this.Objects.Add(obj);

            // If no values, use object
            if (values == null || values.Keys().Count == 0)
            {
                values = this.RootObject.Object.AsJObject;
            }
            else
            {
                this.RootObject.Values = values;
            }

            // Now for the name
            string sDoc = doc.IfEmpty("Organizer{0}.pdf");
            sDoc = sDoc.FormatString("");

            // Create the result
            FolderClass c_Folder = this.RootObject.Object.Folder;
            if (folder.HasValue())
            {
                c_Folder = c_Folder.SubFolder(folder);
            }
            c_Folder.AssurePath();

            this.OutputDocument = new DocumentClass(this.Env.DocumentManager, c_Folder, sDoc);
            this.OutputDocument.Delete();

            // Make the base map
            this.RootObject.Documents = new OrgDocumentsClass(env, this.RootObject);

            List<string> c_Names = new List<string>();
            foreach (OrgDocumentClass c_Doc in this.RootObject.Documents.Sorted)
            {
                c_Names.Add(c_Doc.Description);
            }

            // Get the dataset            
            if (c_DS != null)
            {
                // Make the query
                AO.QueryClass c_Qry = new AO.QueryClass(c_DS.DataCollection);
                c_Qry.Add(AO.ObjectClass.FieldParent, sID, AO.QueryElementClass.QueryOps.Eq);

                // Get the sub datasets to search
                List<string> c_Sub = new List<string>();

                c_Sub.AddRange(c_DS.Definition.ChildDSs.SplitSpaces());

                // Now process each
                foreach (string sDS in c_Sub)
                {
                    // Flags
                    bool bInclOrg = this.Rules.IncludeInOrganizer(sDS);
                    bool bInclDocs = this.Rules.IncludeDocuments(sDS);

                    // Do we do?
                    if (bInclOrg || bInclDocs)
                    {
                        // Make query
                        AO.QueryClass c_QryX = new AO.QueryClass(env.Database[sDS].DataCollection);
                        c_QryX.Add(AO.ObjectClass.FieldParent, obj.UUID.ToString(), AO.QueryElementClass.QueryOps.Eq);

                        // Process each
                        foreach (AO.ObjectClass c_Obj in c_QryX.FindObjects())
                        {
                            // Make the entry
                            OrgObjectClass c_Entry = this.Objects.Add(c_Obj);

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
        private AO.ExtendedContextClass Env { get; set; }
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
                string sDSRoot = this.RootObject.Object.UUID.Dataset.Name;

                // Do we do the root?
                if (this.Rules.IncludeInOrganizer(sDSRoot))
                {
                    //
                    c_Doc.FormatObject(this.RootObject);
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
                            TreeNodeClass c_C = c_Doc.AddChapter(this.Env.Database[sDS].Definition.Caption);

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
                    bool bDocsAlpha = this.Env.Database[sDSRoot].Definition.OrganizeByAlpha;

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
            catch { }

            return c_Doc;
        }

        public DocumentClass GeneratePDF()
        {
            try
            {
                // Setup
                string sDSRoot = this.RootObject.Object.UUID.Dataset.Name;

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
                            string sCaption = this.Env.Database[sDS].Definition.Caption;
                            c_Doc.WHandler.PageLabel = sCaption;
                            PdfOutline c_C = c_Doc.AddChapter(sCaption, false);

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
                                            c_At = c_Doc.AddStub("Document" + (c_Docs.Count == 1 ? "" : "s"), false, c_At);

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
                                                PdfOutline c_At3 = c_Doc.AddStub("Document" + (c_DocsX.Count == 1 ? "" : "s"), false, c_At2);

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
                    bool bDocsAlpha = this.Env.Database[sDSRoot].Definition.OrganizeByAlpha;

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
            catch { }

            return this.OutputDocument;
        }

        #endregion
    }

    public class OrgPointClass : BasedObjectClass
    {
        #region Constructor
        public OrgPointClass(AO.ExtendedContextClass env, object item, string label, string id)
            : base(item, true)
        {
            //
            this.Env = env;
            this.ID = id;
            this.Description = label.IfEmpty().Trim().IfEmpty("Undefined");
        }
        #endregion

        #region Properties
        public AO.ExtendedContextClass Env { get; internal set; }
        public string ID { get; set; }

        public virtual string Description { get; }
        #endregion
    }

    public class OrgPointTableClass : Dictionary<string, OrgPointClass>
    {
        #region Indexer
        public new OrgPointClass this[string id]
        {
            get
            {
                OrgPointClass c_Ans = null;

                if (this.ContainsKey(id)) c_Ans = base[id];

                return c_Ans;
            }
        }
        #endregion

        #region Properties
        public bool ReverseSort { get; set; }
        public List<OrgPointClass> Sorted
        {
            get
            {
                List<OrgPointClass> c_Ans = new List<OrgPointClass>(this.Values);

                c_Ans.Sort();
                if (this.ReverseSort)
                {
                    c_Ans.Reverse();
                }

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public OrgPointClass Add(OrgPointClass pt)
        {
            if (!this.ContainsKey(pt.ID))
            {
                this.Add(pt.ID, pt);
            }
            else
            {
                pt = this[pt.ID];
            }

            return pt;
        }
        #endregion
    }

    public class OrgObjectClass : OrgPointClass, IComparable
    {
        #region Constructor
        public OrgObjectClass(AO.ExtendedContextClass env, AO.ObjectClass obj)
            : base(env, obj, obj.ObjectDescription, OrgObjectClass.MakeID(obj.UUID))
        { }
        #endregion

        #region Properties
        public AO.ObjectClass Object { get { return this.Root as AO.ObjectClass; } }

        public bool IsLocal { get; set; }
        public OrgDocumentsClass Documents { get; set; }
        public JObject Values { get; set; }
        #endregion

        #region Methods
        public string Get(string fld)
        {
            string sAns = null;

            if (fld.IsSameValue("desc"))
            {
                sAns = this.Object.ObjectDescription;
            }
            else if (this.Values != null)
            {
                sAns = this.Values.Get(fld);
            }
            else
            {
                sAns = this.Object[fld];
            }

            return sAns.IfEmpty();
        }

        public bool HasBy(List<string> flds)
        {
            bool bAns = false;

            foreach (string sFld in flds)
            {//        if (this.Get(sFld).HasValue())
                {
                    bAns = true;
                    break;
                }
            }

            return bAns;
        }

        public string ApplyFormat(string fmt)
        {
            string sAns = "";

            List<string> c_Fmts = fmt.SplitSpaces();
            foreach (string sFmt in c_Fmts)
            {
                string sValue = null;

                if ((sFmt.StartsWith("'") && sFmt.EndsWith("'")) ||
                    (sFmt.StartsWith("\"") && sFmt.EndsWith("\"")))
                {
                    sValue = sFmt.Substring(1, sFmt.Length - 2);
                }
                else
                {
                    sValue = this.Get(sFmt);
                }

                if (sValue.HasValue())
                {
                    if (sAns.Length > 0) sAns += " ";
                    sAns += sValue;
                }
            }

            return sAns;
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            int iAns = 0;

            OrgObjectClass c_Other = obj as OrgObjectClass;
            if (c_Other != null) iAns = this.Object.ObjectDescription.CompareTo(c_Other.Object.ObjectDescription);

            return iAns;
        }
        #endregion

        #region Statics
        public static string MakeID(AO.UUIDClass uuid)
        {
            return "O" + uuid.ToString().MD5HashString();
        }
        #endregion
    }

    public class OrgDSClass : BasedObjectClass, IComparable
    {
        #region Constructor
        internal OrgDSClass(AO.ExtendedContextClass env, AO.DatasetClass ds)
            : base(env, true)
        {
            //
            this.Dataset = ds.Name;
            this.Description = ds.Definition.Caption;
            this.Objects.ReverseSort = false;
            this.ByFields = ds.Definition.OrganizeBy.SplitSpaces();
            this.ByOnly = ds.Definition.OrganizeByOnly;

            this.ByLabels = new List<string>();
            this.ByFormat = new List<string>();
            for (int iFld = 0; iFld < this.ByFields.Count; iFld++)
            {
                string sFld = this.ByFields[iFld];

                string sFmt = "d";
                int iPos = sFld.IndexOf(":");
                if (iPos != -1)
                {
                    sFmt = sFld.Substring(iPos + 1);
                    sFld = sFld.Substring(0, iPos);
                }

                string sLabel = sFld;

                AO.Definitions.DatasetFieldClass c_Def = ds.Definition[sFld];
                if (c_Def != null) sLabel = c_Def.Label;

                this.ByLabels.Add(sLabel.IfEmpty(sFld));
                this.ByFormat.Add(sFmt);
                this.ByFields[iFld] = sFld;
            }
        }
        #endregion

        #region IDisposable
        public override void Dispose()
        { }
        #endregion

        #region Indexer
        public OrgObjectClass this[AO.UUIDClass uuid]
        {
            get { return this.Objects[OrgObjectClass.MakeID(uuid)] as OrgObjectClass; }
        }
        #endregion

        #region Properties
        public AO.ExtendedContextClass Env { get { return this.Root as AO.ExtendedContextClass; } }
        public string Dataset { get; internal set; }
        public string Description { get; internal set; }

        private OrgPointTableClass Objects { get; set; } = new OrgPointTableClass();

        public List<OrgObjectClass> Sorted
        {
            get
            {
                List<OrgObjectClass> c_Ans = new List<OrgObjectClass>();

                foreach (OrgPointClass c_Pt in this.Objects.Sorted) c_Ans.Add(c_Pt as OrgObjectClass);

                return c_Ans;
            }
        }
        public List<string> ByFields { get; internal set; }
        public List<string> ByLabels { get; internal set; }
        public List<string> ByFormat { get; internal set; }
        public bool ByOnly { get; internal set; }

        public Dictionary<string, Dictionary<string, List<OrgObjectClass>>> By(bool uuid)
        {
            Dictionary<string, Dictionary<string, List<OrgObjectClass>>> c_Ans = new Dictionary<string, Dictionary<string, List<OrgObjectClass>>>();

            foreach (OrgObjectClass c_Obj in this.Sorted)
            {
                for (int iFld = 0; iFld < this.ByFields.Count; iFld++)
                {
                    string sFld = this.ByFields[iFld];

                    string sValue = c_Obj.Get(sFld);
                    if (sValue.HasValue())
                    {
                        if (sValue.IsISODate())
                        {
                            DateTime c_On = sValue.FromDBDate().AdjustTimezone();

                            switch (this.ByFormat[iFld])
                            {
                                case "w":
                                    int iDays = c_On.DayOfWeek - DayOfWeek.Monday;
                                    DateTime c_Start = c_On.AddDays(-iDays);
                                    DateTime c_End = c_Start.AddDays(6);
                                    sValue = c_Start.ToString("yyyyMMdd") + "\t" + c_Start.ToString("MM/dd/yyyy") + " - " + c_End.ToString("MM/dd/yyyy");
                                    break;

                                case "m":
                                    sValue = c_On.ToString("yyyyMM") + "\t" + c_On.ToString("MMMM yyyy");
                                    break;

                                case "y":
                                    sValue = c_On.ToString("yyyy") + "\t" + c_On.ToString("yyyy");
                                    break;

                                default:
                                    sValue += "\t" + c_On.ToString("MM/dd/yyyy");
                                    break;
                            }
                        }

                        if (!c_Ans.ContainsKey(this.ByLabels[iFld])) c_Ans.Add(this.ByLabels[iFld], new Dictionary<string, List<OrgObjectClass>>());
                        Dictionary<string, List<OrgObjectClass>> c_At = c_Ans[this.ByLabels[iFld]];

                        if (!c_At.ContainsKey(sValue)) c_At.Add(sValue, new List<OrgObjectClass>());

                        c_At[sValue].Add(c_Obj);
                    }
                }
            }

            return c_Ans;
        }
        #endregion

        #region Methods
        public OrgObjectClass Add(AO.ObjectClass obj, bool islocal)
        {
            OrgObjectClass c_Ans = this.Objects.Add(new OrgObjectClass(this.Env, obj)) as OrgObjectClass;

            c_Ans.IsLocal = islocal;

            return c_Ans;
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            int iAns = 0;

            OrgDSClass c_Other = obj as OrgDSClass;
            if (c_Other != null) iAns = this.Description.CompareTo(c_Other.Description);

            return iAns;
        }
        #endregion
    }

    public class OrgObjectsClass : BasedObjectClass
    {
        #region Constructor
        public OrgObjectsClass(AO.ExtendedContextClass env)
            : base(env, true)
        { }
        #endregion

        #region Indexer
        public OrgDSClass this[string ds]
        {
            get
            {
                OrgDSClass c_Ans = null;

                if (this.Objects.ContainsKey(ds)) c_Ans = this.Objects[ds];

                return c_Ans;
            }
        }
        #endregion

        #region Properties
        public AO.ExtendedContextClass Env { get { return this.Root as AO.ExtendedContextClass; } }

        private Dictionary<string, OrgDSClass> Objects { get; set; } = new Dictionary<string, OrgDSClass>();
        public OrgObjectClass RootObject { get; internal set; }

        public List<string> Datasets
        {
            get
            {
                List<string> c_Ans = new List<string>();

                List<OrgDSClass> c_DSS = new List<OrgDSClass>(this.Objects.Values);
                c_DSS.Sort();
                foreach (OrgDSClass c_DS in c_DSS)
                {
                    c_Ans.Add(c_DS.Dataset);
                }

                return c_Ans;
            }
        }

        public List<OrgObjectClass> ObjectList
        {
            get
            {
                List<OrgObjectClass> c_Ans = new List<OrgObjectClass>();

                foreach (string sDS in this.Objects.Keys)
                {
                    OrgDSClass c_DS = this.Objects[sDS];
                    c_Ans.AddRange(c_DS.Sorted);
                }

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public OrgObjectClass Add(AO.ObjectClass obj)
        {
            //
            string sDS = obj.UUID.Dataset.Name;

            // Do we have a dataset?
            if (!this.Objects.ContainsKey(sDS))
            {
                this.Objects.Add(sDS, new OrgDSClass(this.Env, obj.UUID.Dataset));
            }

            // Is this the first?
            bool bFirst = this.RootObject == null;

            // Add
            OrgObjectClass c_Ans = this.Objects[sDS].Add(obj, !bFirst);

            // If first, save
            if (bFirst) this.RootObject = c_Ans;

            //
            return c_Ans;
        }
        #endregion
    }

    public class OrgDocumentClass : OrgPointClass, IComparable
    {
        #region Constructor
        public OrgDocumentClass(AO.ExtendedContextClass env, DocumentClass doc, OrgObjectClass obj)
            : base(env, doc, doc.NameOnly, OrgDocumentClass.MakeID(doc))
        {
            //
            this.Object = obj;
        }
        #endregion

        #region Properties
        public DocumentClass Document { get { return this.Root as DocumentClass; } }
        public OrgObjectClass Object { get; internal set; }
        public string Group { get { return this.Document.NameOnly.Substring(0, 1).ToUpper(); } }

        public List<OrgDocumentClass> Uses { get; internal set; } = new List<OrgDocumentClass>();
        public List<OrgObjectClass> ObjectUses
        {
            get
            {
                Dictionary<string, OrgObjectClass> c_Uses = new Dictionary<string, OrgObjectClass>();
                List<string> c_Done = new List<string>();

                this.GetObjectUses(c_Uses, c_Done);

                return new List<OrgObjectClass>(c_Uses.Values);
            }
        }
        public void GetObjectUses(Dictionary<string, OrgObjectClass> uses, List<string> done)
        {
            if (done.IndexOf(this.ID) == -1)
            {
                done.Add(this.ID);

                if (!uses.ContainsKey(this.Object.ID))
                {
                    uses.Add(this.Object.ID, this.Object);
                }

                foreach (OrgDocumentClass c_Doc in this.Uses)
                {
                    c_Doc.GetObjectUses(uses, done);
                }
            }
        }
        #endregion

        #region Methods
        public void AddUse(OrgDocumentClass doc)
        {
            this.Uses.Add(doc);
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            int iAns = 0;

            OrgDocumentClass c_Other = obj as OrgDocumentClass;
            if (c_Other != null)
            {
                //iAns = this.Object.ID.CompareTo(c_Other.Object.ID);
                //if (iAns == 0)
                //{
                iAns = this.Document.NameOnly.CompareTo(c_Other.Document.NameOnly);
                //}
            }

            return iAns;
        }
        #endregion

        #region Statics
        public static string MakeID(DocumentClass doc)
        {
            return "D" + doc.Name.MD5HashString();
        }
        #endregion
    }

    public class OrgDocumentsClass : OrgPointTableClass, IDisposable
    {
        #region Constructor
        internal OrgDocumentsClass()
        { }

        internal OrgDocumentsClass(AO.ExtendedContextClass env, OrgObjectClass obj)
            : this(env, obj.Object.Folder, obj)
        { }

        internal OrgDocumentsClass(AO.ExtendedContextClass env, FolderClass folder, OrgObjectClass obj)
        {
            //
            this.Env = env;

            // Do sub-folders
            List<FolderClass> folders = folder.Folders;
            foreach (FolderClass sDir in folders)
            {
                if (!sDir.Name.StartsWith("_"))
                {
                    using (OrgDocumentsClass c_Sub = new OrgDocumentsClass(this.Env, sDir, obj))
                    {
                        this.Merge(c_Sub);
                    }
                }
            }

            // Do files
            List<DocumentClass> c_Files = folder.Files;
            foreach (DocumentClass c_Doc in c_Files)
            {
                //env.Env.LogInfo("FILE: {0}".FormatString(sFile));

                // Add the file
                try
                {
                    this.Add(c_Doc, obj);
                }
                catch { }
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public AO.ExtendedContextClass Env { get; internal set; }
        public new List<OrgDocumentClass> Sorted
        {
            get
            {
                List<OrgDocumentClass> c_Ans = new List<OrgDocumentClass>();

                foreach (OrgPointClass c_Pt in base.Sorted) c_Ans.Add(c_Pt as OrgDocumentClass);

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public OrgDocumentClass Add(DocumentClass doc, OrgObjectClass obj)
        {
            OrgDocumentClass c_Ans = null;

            if (doc.Exists)
            {
                // Use the PDF!
                Proc.Docs.Files.PDFDocumentClass c_PDF = doc.PDF();
                bool bAdd = true;

                // Open it and check creator
                try
                {
                    PdfReader c_Reader = new PdfReader(c_PDF.Document.Value);
                    if (c_Reader.Info.ContainsKey("Creator"))
                    {
                        bAdd = !OrganizerGeneratorClass.Creator.IsSameValue(c_Reader.Info["Creator"]);
                    }
                }
                catch { }

                if (bAdd)
                {
                    OrgDocumentClass c_Doc = new OrgDocumentClass(this.Env, c_PDF.Document, obj);
                    c_Ans = this.Add(c_Doc);
                }
            }

            return c_Ans;
        }

        private OrgDocumentClass Add(OrgDocumentClass entry)
        {
            OrgDocumentClass c_Ans = entry;

            if (base.ContainsKey(entry.ID))
            {
                c_Ans = base[entry.ID] as OrgDocumentClass;
            }
            else
            {
                base.Add(entry);
            }

            // Add use
            c_Ans.AddUse(entry);

            return c_Ans;
        }

        public bool Merge(OrgDocumentsClass map)
        {
            bool bAns = false;

            foreach (string sKey in map.Keys)
            {
                this.Add(map[sKey] as OrgDocumentClass);
                bAns = true;
            }

            return bAns;
        }

        public List<OrgDocumentClass> OwnedBy(OrgObjectClass obj)
        {
            List<OrgDocumentClass> c_Ans = this.Sorted;

            for (int iDoc = c_Ans.Count - 1; iDoc >= 0; iDoc--)
            {
                if (obj.ID.CompareTo(c_Ans[iDoc].Object.ID) != 0) c_Ans.RemoveAt(iDoc);
            }

            return c_Ans;
        }
        #endregion
    }

    public class OrganizerRulesClass : IDisposable
    {
        #region Constructor
        public OrganizerRulesClass()
        {
            this.IncludeAllInOrganizer = true;
            this.IncludeAllDocuments = true;
        }

        public OrganizerRulesClass(params string[] ds)
        {
            foreach (string sEntry in ds)
            {
                string sDS = sEntry;
                string sOptions = "";

                int iPos = sEntry.IndexOf(":");
                if (iPos != -1)
                {
                    sOptions = sEntry.Substring(iPos);
                    sDS = sEntry.Substring(0, iPos);
                }

                OrganizerEntryClass c_Entry = new OrganizerEntryClass(sDS);

                if (!sOptions.HasValue())
                {
                    c_Entry.IncludeInOrganizer = true;
                    c_Entry.IncludeDocuments = true;
                }
                else
                {
                    c_Entry.IncludeInOrganizer = sOptions.IndexOf("o") != -1;
                    c_Entry.IncludeDocuments = sOptions.IndexOf("d") != -1;
                }

                this.Add(sDS, c_Entry);
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Indexer
        public OrganizerEntryClass this[string ds]
        {
            get
            {
                OrganizerEntryClass c_Ans = null;

                if (this.Entries.ContainsKey(ds)) c_Ans = this.Entries[ds];

                return c_Ans;
            }
        }
        #endregion

        #region Properties
        internal Dictionary<string, OrganizerEntryClass> Entries { get; set; } = new Dictionary<string, OrganizerEntryClass>();

        public bool IncludeAllInOrganizer { get; set; }
        public bool IncludeAllDocuments { get; set; }
        #endregion

        #region Methods
        public void Add(string ds, OrganizerEntryClass rules)
        {
            if (this.Entries.ContainsKey(ds))
            {
                this.Entries[ds] = rules;
            }
            else
            {
                this.Entries.Add(ds, rules);
            }
        }

        public bool IncludeInOrganizer(string ds)
        {
            bool bAns = this.IncludeAllInOrganizer;

            if (!bAns)
            {
                OrganizerEntryClass c_Entry = this[ds];
                if (c_Entry != null) bAns = c_Entry.IncludeInOrganizer;
            }

            return bAns;
        }

        public bool IncludeDocuments(string ds)
        {
            bool bAns = this.IncludeAllDocuments;

            if (!bAns)
            {
                OrganizerEntryClass c_Entry = this[ds];
                if (c_Entry != null) bAns = c_Entry.IncludeDocuments;
            }

            return bAns;
        }
        #endregion

        #region Statics
        public static OrganizerRulesClass Make(string options)
        {
            OrganizerRulesClass c_Ans = null;

            if (options.HasValue())
            {
                c_Ans = new OrganizerRulesClass(options.SplitSpaces(true).ToArray());
            }

            return c_Ans;
        }
        #endregion
    }

    public class OrganizerEntryClass : IDisposable
    {
        #region Constructor
        public OrganizerEntryClass(string ds)
        { }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Enums
        #endregion

        #region Properties
        public bool IncludeInOrganizer { get; set; }
        public bool IncludeDocuments { get; set; }
        #endregion
    }

    public class TreeNodeClass : BasedObjectClass
    {
        #region Constructor
        public TreeNodeClass(string label,
                                string id,
                                TreeNodeClass parent = null)
            : base(parent)
        {
            //
            this.ID = id.IfEmpty();
            this.Label = label;
        }
        #endregion

        #region Properties
        public TreeNodeClass Parent { get { return this.Root as TreeNodeClass; } }
        public TreeNodeClass RootNode
        {
            get
            {
                TreeNodeClass c_Ans = this;

                if (this.Parent != null) c_Ans = this.Parent.RootNode;

                return c_Ans;
            }
        }

        public string Label { get; internal set; }
        public string ID { get; internal set; }

        private List<TreeNodeClass> Children { get; set; } = new List<TreeNodeClass>();

        public OrgObjectsClass AllObjects { get; set; }
        #endregion

        #region Methods
        public void Add(TreeNodeClass node)
        {
            this.Children.Add(node);
        }

        public JObject GetNode()
        {
            JObject c_Node = new JObject();

            c_Node.Set("text", this.Label);
            c_Node.Set("id", this.ID);

            if (!this.ID.HasValue() || this.Children.Count > 0)
            {
                c_Node.Set("leaf", false);
                c_Node.Set("children", this.GetChildren());
            }
            else
            {
                c_Node.Set("leaf", true);
                c_Node.Set("cls", "file");
            }

            return c_Node;
        }

        public JArray GetChildren()
        {
            JArray c_Ans = new JArray();

            foreach (TreeNodeClass c_Node in this.Children)
            {
                c_Ans.Add(c_Node.GetNode());
            }

            return c_Ans;
        }
        #endregion

        #region Builder
        public TreeNodeClass FormatObject(OrgObjectClass obj, TreeNodeClass at = null)
        {
            if (at == null) at = this.RootNode;

            at.Add(new TreeNodeClass(obj.Description, obj.Object.UUID.ToString(), at));

            return at;
        }

        public TreeNodeClass AddChapter(string label)
        {
            TreeNodeClass at = this.RootNode;

            TreeNodeClass c_At = new TreeNodeClass(label, null, at);
            at.Add(c_At);

            return c_At;
        }

        public TreeNodeClass AddStub(string label, TreeNodeClass at = null)
        {
            if (at == null) at = this.RootNode;

            TreeNodeClass c_At = new TreeNodeClass(this.FormatLabel(label), null, at);
            at.Add(c_At);

            return c_At;
        }

        public void FormatDocument(OrgDocumentClass doc, TreeNodeClass node, string caption)
        {
            node.Add(new TreeNodeClass(caption, doc.Document.Path, node));
        }

        private string FormatLabel(string value)
        {
            int iPos = value.IndexOf("\t");
            if (iPos != -1) value = value.Substring(iPos + 1);

            return value;
        }
        #endregion
    }
}
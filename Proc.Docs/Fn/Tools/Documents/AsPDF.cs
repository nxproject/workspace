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

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.Docs.Files;
using Proc.AO;

namespace Proc.Docs
{
    /// <summary>
    /// 
    /// Retrieves multiple files as PDF
    /// 
    /// </summary>
    public class TDAsPDF : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Make return
            StoreClass c_Ans = new StoreClass();
            
            // Get the object infos
            string sObjDS = store["ds"].AsDatasetName();
            string sObjID = store["id"];
            string sName = store["name"];

            // Valid?
            if (sObjDS.HasValue() && sObjID.HasValue())
            {
                // Get the manager
                Proc.AO.ManagerClass c_ObjMgr = call.Env.Globals.Get<Proc.AO.ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_ObjMgr.DefaultDatabase[sObjDS];

                // Get the object
                using (Proc.AO.ObjectClass c_Obj = c_DS[sObjID])
                {
                    // Get the list
                    List<string> c_List = store.GetAsJArray("list").ToList();
                    
                    // Get the manager
                    NX.Engine.Files.ManagerClass c_Mgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

                    // Expand
                    c_List = c_List.ExpandEntries(c_Mgr);

                    // How many?
                    int iCount = c_List.Count;
                    // Any?
                    if (iCount == 0)
                    { }
                    else
                    {
                        // One and PDF?
                        if (iCount == 1 && !sName.HasValue())
                        {
                            using (DocumentClass c_Doc = new DocumentClass(c_Mgr, c_List[0]))
                            {
                                // Path
                                c_Ans["path"] = c_Doc.PDF().Path;
                            }
                        }
                        else
                        {
                            // Make the output
                            DocumentClass c_Output = null;
                            // Did we get a name?
                            if(sName.HasValue())
                            {
                                c_Output = new DocumentClass(c_Mgr, c_Obj.Folder, sName + ".pdf");
                            }
                            else 
                            {
                                // Create the ID
                                string sID = "C" + c_List.Join(",").MD5HashString() + ".pdf";

                                c_Output = new DocumentClass(c_Mgr, c_Obj.Folder.SubFolder("_temp"), sID);
                            }

                            // The PDF tools
                            using (PDFClass c_Eng = new PDFClass())
                            {
                                // Holding area
                                byte[][] aabBuffer = new byte[c_List.Count][];
                                iCount = 0;

                                // Loop thru
                                foreach (string sPath in c_List)
                                {
                                    using (DocumentClass c_Doc = new DocumentClass(c_Mgr, sPath))
                                    {
                                        // Get PDF
                                        using (Files.PDFDocumentClass c_PDF = c_Doc.PDF())
                                        {
                                            // Any?
                                            if (c_PDF != null)
                                            {
                                                // Save
                                                aabBuffer[iCount++] = c_PDF.ValueAsBytes;
                                            }
                                        }
                                    }
                                }

                                // Any output?
                                if (c_Output != null)
                                {
                                    // Merge
                                    c_Output.ValueAsBytes = c_Eng.MergePDF(aabBuffer);

                                    // Path
                                    c_Ans["path"] = c_Output.Path;

                                    // Cleanuo
                                    c_Output.Dispose();
                                }
                            }
                        }
                    }
                }
            }

            return c_Ans;
        }
    }
}
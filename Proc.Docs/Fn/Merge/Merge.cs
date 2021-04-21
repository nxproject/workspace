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
using Proc.AO;
using Proc.Docs.Files;

namespace Proc.Docs
{
    /// <summary>
    /// 
    /// merges a document
    /// 
    /// </summary>
    public class Merge : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Make return
            StoreClass c_Ans = new StoreClass();

            // Get the parameters
            string sObjDS = store["ds"].AsDatasetName();
            string sObjID = store["id"];
            string sPath = store["path"];
            StoreClass c_Passed = store.GetAsStore("data");

            // Result
            List<string> c_Done = new List<string>();

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
                    // Float account
                    c_Obj.FloatAccount();

                    // Get the list
                    List<string> c_List = new List<string>() { sPath };

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
                        // Loop thru
                        foreach (string sTemplate in c_List)
                        {
                            // Get source
                            using (DocumentClass c_Source = new DocumentClass(c_Mgr, sTemplate))
                            {
                                // Get the map
                                MergeMapClass c_Map = c_Source.MergeMap();

                                // Make the context
                                using (ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, c_Passed, null, call.UserInfo.Name))
                                {
                                    // Make target
                                    using (DocumentClass c_Target = new DocumentClass(c_Mgr, c_Obj.Folder, c_Source.Name))
                                    {
                                        // Merge
                                        c_Source.Merge(c_Target, c_Map.Eval(c_Ctx), delegate(string text)
                                        {
                                            // Do handlebars
                                            HandlebarDataClass c_HData = new HandlebarDataClass(call.Env);
                                            // Add the object
                                            c_HData.Merge(c_Obj.Explode(c_Ctx));
                                            // Merge
                                            return text.Handlebars(c_HData);
                                        });

                                        // Add
                                        c_Done.Add(c_Target.Path);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            c_Ans.Set("path", c_Done.ToJArray());

            return c_Ans;
        }
    }
}
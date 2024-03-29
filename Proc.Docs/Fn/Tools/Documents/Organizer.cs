﻿///--------------------------------------------------------------------------------
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
    /// Creates an organizer for an object
    /// 
    /// </summary>
    public class Organizer : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Make return
            StoreClass c_Ans = new StoreClass();

            // Get the object infos
            string sObjDS = store["ds"].AsDatasetName();
            string sObjID = store["id"];
            string sDesc = store["desc"];
            StoreClass c_Data = new StoreClass( store.GetAsJObject("data"));

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
                    using (ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, c_Data, c_Obj, call.UserInfo.Name))
                    {
                        string sTS = DateTime.Now.AdjustTimezone().ToTimeStamp();

                        using (OrganizerGeneratorClass c_Eng = new OrganizerGeneratorClass(c_Ctx,
                                                                                       c_Obj,
                                                                                       null,
                                                                                       "",
                                                                                       "Organizer.pdf",
                                                                                       "From tools",
                                                                                       sTS,
                                                                                       null))
                        {
                            NX.Engine.Files.DocumentClass c_Doc = c_Eng.GeneratePDF();
                            c_Ans["path"] = c_Doc.Path;
                        }
                    }
                }
            }

            return c_Ans;
        }
    }
}
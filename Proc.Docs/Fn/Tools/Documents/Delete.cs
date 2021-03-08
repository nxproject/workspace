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
    /// Deletes multiple files 
    /// 
    /// </summary>
    public class TDDelete : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Make return
            StoreClass c_Ans = new StoreClass();

            // Get the manager
            NX.Engine.Files.ManagerClass c_Mgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

            // Get the list
            List<string> c_List = store.GetAsJArray("list").ToList();

            // How many?
            int iCount = c_List.Count;
            // Loop thru
            foreach (string sPath in c_List)
            {
                if (sPath.EndsWith("/"))
                {
                    using (FolderClass c_Folder = new FolderClass(c_Mgr, sPath))
                    {
                        c_Folder.Delete();

                        // Parse
                        string[] asPieces = sPath.Split("/", System.StringSplitOptions.RemoveEmptyEntries);
                        if(asPieces.Length == 4)
                        {
                            c_Folder.AssurePath();
                        }
                    }
                }
                else
                {
                    using (DocumentClass c_Doc = new DocumentClass(c_Mgr, sPath))
                    {
                        // Delete
                        c_Doc.Delete();
                    }
                }
            }

            return c_Ans;
        }
    }
}
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
    public class TDMoveTo : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Make return
            StoreClass c_Ans = new StoreClass();

            // Get the object infos
            string sPath = store["path"];
            string sFolder = store["folder"];

            // 
            if (sPath.HasValue() && sFolder.HasValue())
            {
                // Get the manager
                NX.Engine.Files.ManagerClass c_Mgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

                // Source
                DocumentClass c_Source = new DocumentClass(c_Mgr, sPath);
                // And the target forlder
                FolderClass c_Folder = new FolderClass(c_Mgr, sFolder);
                // Move 
                c_Source.MoveTo(c_Folder[c_Source.Name]);
            }

            return c_Ans;
        }

        #region Methods
        private void MoveFile(NX.Engine.Files.ManagerClass mgr, DocumentClass source, FolderClass target)
        {
            using (DocumentClass c_Target = new DocumentClass(mgr, target, source.Name))
            {
                // Copy
                source.MoveTo(c_Target);
            }
        }

        private void MoveFolder(NX.Engine.Files.ManagerClass mgr, FolderClass source, FolderClass target)
        {
            // Get the files
            List<DocumentClass> c_Docs = source.Files;
            // Loop thru
            foreach(DocumentClass c_Doc in c_Docs)
            {
                this.MoveFile(mgr, c_Doc, target);
            }

            // Get the folders
            List<FolderClass> c_Folders = source.Folders;
            // Loop thru
            foreach(FolderClass c_Folder in c_Folders)
            {
                // Move
                this.MoveFolder(mgr, c_Folder, target);
            }

            // Delete self
            source.Delete();
        }
        #endregion
    }
}
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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class FTPDownloadList : CommandClass
    {
        #region Constants
        private const string ArgHTTP = "conn";
        private const string ArgList = "list";
        private const string ArgObj = "obj";
        private const string ArgFolder = "folder";
        private const string ArgDel = "del";
        #endregion

        #region Constructor
        public FTPDownloadList()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgHTTP, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The connection to use"));
                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The list to store the files"));
                c_P.Add(ArgObj, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The object to store the files under"));
                c_P.Add(ArgFolder, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The folder"));
                c_P.Add(ArgDel, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "If true, delete when downloaded"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.FTP, "Downloads a set of files from the server", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "ftp.download.list"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sHTTP = args.Get(ArgHTTP);
            string sList = args.Get(ArgList);
            string sObj = args.Get(ArgObj);
            string sFolder = args.Get(ArgFolder);
            bool bDel = args.GetAsBool(ArgDel, false);

            FolderClass c_Folder = ctx.Objects[sObj].Folder;
            if (sFolder.HasValue()) c_Folder = c_Folder.SubFolder(sFolder);

            // Do
            if (ctx.FTP.Has(sHTTP))
            {
                FTPClientClass c_FTP = ctx.FTP[sHTTP];

                List<string> c_Files = c_FTP.Files(null);
                foreach (string sFile in c_Files)
                {
                    string sDoc = sFile.GetFileNameFromPath();

                    DocumentClass c_Doc = new DocumentClass(ctx.DocumentManager, c_Folder, sFile);

                    if (c_FTP.Download(sFile, c_Doc.Location))
                    {
                        ctx.Documents[sDoc] = c_Doc;

                        if (sList.HasValue())
                        {
                            ctx.DocumentLists[sList].Add(sDoc);
                        }

                        if (bDel)
                        {
                            c_FTP.Delete(sFile);
                        }
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
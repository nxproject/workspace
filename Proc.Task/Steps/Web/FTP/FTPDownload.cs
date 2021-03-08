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
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class FTPDownload : CommandClass
    {
        #region Constants
        private const string ArgHTTP = "conn";
        private const string ArgDoc = "doc";
        private const string ArgFile = "file";
        #endregion

        #region Constructor
        public FTPDownload()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgHTTP, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The connection to use"));
                c_P.Add(ArgFile, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The file name at the server"));
                c_P.Add(ArgDoc, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The document name"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.FTP, "Downloads a file from the server", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "ftp.download"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sHTTP = args.Get(ArgHTTP);
            string sDoc = args.Get(ArgDoc);
            string sFile = args.Get(ArgFile);

            // Do
            if (ctx.FTP.Has(sHTTP))
            {
                NX.Engine.Files.DocumentClass c_Doc = ctx.Documents[sDoc];
                if (c_Doc.Exists) c_Doc.Delete();

                ctx.FTP[sHTTP].Download(sFile, c_Doc.Location);
            }

            return eAns;
        }
        #endregion
    }
}
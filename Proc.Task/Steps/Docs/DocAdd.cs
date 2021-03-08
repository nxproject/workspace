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

using System.Collections.Generic;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;

namespace Proc.Task
{
    public class DocAdd : CommandClass
    {
        #region Constants
        private const string ArgName = "name";
        private const string ArgObj = "obj";
        private const string ArgPath = "path";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DocAdd()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The name to be used"));
                c_P.Add(ArgObj, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the object which is the parent"));
                c_P.Add(ArgPath, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The path of the document"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Docs, "Adds a document to the working set", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "doc.add"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sName = args.Get(ArgName);
            string sObj = args.Get(ArgObj);
            string sPath = args.Get(ArgPath);

            //
            AO.ObjectClass c_Obj = ctx.Objects[sObj];
            NX.Engine.Files.FolderClass c_Folder = c_Obj.Folder;

            // 
            NX.Engine.Files.DocumentClass c_Doc = new NX.Engine.Files.DocumentClass(ctx.DocumentManager, c_Folder, sPath);
            ctx.Documents[sName] = c_Doc;

            return eAns;
        }
        #endregion
    }
}
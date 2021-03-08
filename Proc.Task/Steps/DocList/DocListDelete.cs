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
    public class DocListDelete : CommandClass
    {
        #region Constants
        private const string ArgList = "list";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DocListDelete()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The list"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.DocList, "Deletes all the documents in list", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "doc.list.delete"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sList = args.Get(ArgList);

            // Get the list
            List<string> c_Docs = ctx.DocumentLists[sList];

            // Call
            for (int iLoop = 0; iLoop < c_Docs.Count; iLoop++)
            {
                NX.Engine.Files.DocumentClass c_Source = ctx.Documents[c_Docs[iLoop]];

                if (c_Source != null)
                {
                    c_Source.Delete();
                    ctx.Documents[c_Docs[iLoop]] = null;

                }
            }
            //
            ctx.DocumentLists[sList].Clear();

            return eAns;
        }
        #endregion
    }
}
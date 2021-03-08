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
    public class DocCopy : CommandClass
    {
        #region Constants
        private const string ArgFrom = "from";
        private const string ArgTo = "to";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DocCopy()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgFrom, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The source document name"));
                c_P.Add(ArgTo, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The target document name"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Docs, "Copies a document", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "doc.copy"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sFrom = args.Get(ArgFrom);
            string sTo = args.Get(ArgTo);

            // 
            NX.Engine.Files.DocumentClass c_Source = ctx.Documents[sFrom];
            NX.Engine.Files.DocumentClass c_Target = ctx.Documents[sTo];

            if (c_Source != null && c_Target != null)
            {
                c_Target.Value = c_Source.Value;
            }

            return eAns;
        }
        #endregion
    }
}
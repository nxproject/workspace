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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;
using Proc.Docs.Files;
using Common.TaskWF;

namespace Proc.Task
{
    public class MergeDoc : CommandClass
    {
        #region Constants
        private const string ArgDoc = "doc";
        private const string ArgTo = "to";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public MergeDoc()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgDoc, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The document to be merged"));
                c_P.Add(ArgTo, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The merged PDF document"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Docs, "Merges a .DOCX or .PDF file with values from the context", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "merge.doc"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sDoc = args.Get(ArgDoc);
            string sTo = args.Get(ArgTo);
            
            // Get source
            DocumentClass c_Source = ctx.Documents[sDoc];
            // Get the map
            MergeMapClass c_Map = c_Source.MergeMap();
            // Make target
            DocumentClass c_Target = ctx.Documents[sTo];

            // Merge
            c_Source.Merge(c_Target, c_Map.Eval(ctx));

            return eAns;
        }
        #endregion
    }
}
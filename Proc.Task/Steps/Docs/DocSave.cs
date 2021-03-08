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
    public class DocSaveText : CommandClass
    {
        #region Constants
        private const string ArgDoc = "doc";
        private const string ArgText = "text";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DocSaveText()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgDoc, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The name of the document to be saved"));
                c_P.Add(ArgText, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The text"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Docs, "Saves a document", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "doc.save"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sDoc = args.Get(ArgDoc);
            string sText = args.Get(ArgText);

            // 
            string sValue = ctx.Texts[sText].Text.ToString();
            ctx.Documents[sDoc].Value = sValue;

            return eAns;
        }
        #endregion
    }
}
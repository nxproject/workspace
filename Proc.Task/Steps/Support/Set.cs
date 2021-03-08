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

using NX.Shared;
using NX.Engine;
using Common.TaskWF;

namespace Proc.Task
{
    public class Set : CommandClass
    {
        #region Constants
        private const string ArgFld = "field";
        private const string ArgValue = "value";
        #endregion

        #region Constructor
        public Set()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgFld, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The field"));
                c_P.Add(ArgValue, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The value"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Ops, "Sets a value", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "set"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sFld = args.GetRaw(ArgFld);
            string sValue = args.Get(ArgValue);

            // Allow for eval as target
            using (DatumClass c_Datum = new DatumClass(ctx, sFld))
            {
                if (c_Datum.Type == DatumClass.Types.Expression)
                {
                    sFld = c_Datum.Value;
                }
            }

            // Make the datum
            using (DatumClass c_Datum = new DatumClass(ctx, sFld))
            {
                // And set the value
                c_Datum.Value = sValue;
            }

            return eAns;
        }
        #endregion
    }
}
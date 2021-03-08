///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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

using Newtonsoft.Json.Linq;

namespace Proc.Task
{
    public class BillingApply : CommandClass
    {
        #region Constants
        private const string ArgCharges = "charges";
        private const string ArgSource = "source";
        #endregion

        #region Constructor
        public BillingApply(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParameterDefinitionClass> c_P = new NamedListClass<ParameterDefinitionClass>();

                c_P.Add(ArgSource, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The source field"));

                return new DescriptionClass(CategoriesClass.Array, "Sets Biiled On date to source", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "billing.apply"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sSource = args.GetDefinedRaw(ArgSource);

            // Do
            using (GridWrapper c_Grid = new GridWrapper(args.GetDefined(ArgSource)))
            {
                string sNow = ctx.Today().ToShortDateString();

                for (int i = 0; i < c_Grid.Count; i++)
                {
                    c_Grid.Row = i;

                    if (c_Grid[GridWrapper.Columns.BilledOn].IsSameValue("-"))
                    {
                        c_Grid[GridWrapper.Columns.BilledOn] = sNow;
                    }
                }

                using (DatumClass c_Datum = new DatumClass(ctx, sSource))
                {
                    c_Datum.Value = c_Grid.ToString();
                }
            }

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
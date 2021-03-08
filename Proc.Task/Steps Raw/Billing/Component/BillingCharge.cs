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
    public class BillingCharge : CommandClass
    {
        #region Constants
        private const string ArgCode = "code";
        private const string ArgDesc = "desc";
        private const string ArgSource = "source";
        #endregion

        #region Constructor
        public BillingCharge(EnvironmentClass env)
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
                c_P.Add(ArgCode, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The billing code"));
                c_P.Add(ArgDesc, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The description"));

                return new DescriptionClass(CategoriesClass.Array, "Adds a charge to working environment based on component", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "billing.charge"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sSource = args.GetDefinedRaw(ArgSource);
            string sDesc = args.GetDefined(ArgDesc);
            string sCode = args.GetDefined(ArgCode);

            // Do
            using (GridWrapper c_Grid = new GridWrapper(args.GetDefined(ArgSource)))
            {
                for (int i = 0; i < c_Grid.Count; i++)
                {
                    c_Grid.Row = i;

                    if ((!c_Grid[GridWrapper.Columns.BilledOn].HasValue() ||
                            c_Grid[GridWrapper.Columns.BilledOn] == "-") &&
                            c_Grid[GridWrapper.Columns.Amount].ToDouble(0) > 0)
                    {
                        c_Grid[GridWrapper.Columns.BilledOn] = "-";

                        ChargeClass c_Charge = ctx.Charges[sSource].New(sCode, sDesc,
                                                                c_Grid[GridWrapper.Columns.Detail],
                                                                c_Grid[GridWrapper.Columns.Units].ToDouble(0),
                                                                c_Grid[GridWrapper.Columns.Rate].ToDouble(0),
                                                                true);

                        string sOn = c_Grid[GridWrapper.Columns.On];
                        if (sOn.HasValue()) c_Charge.On = sOn.FromDBDate();
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
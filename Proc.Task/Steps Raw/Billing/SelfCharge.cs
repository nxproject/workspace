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
    public class BillCharge : CommandClass
    {
        #region Constants
        private const string ArgCode = "code";
        private const string ArgDesc = "desc";
        private const string ArgDetails = "details";
        private const string ArgUnits = "units";
        private const string ArgRate = "rate";
        #endregion

        #region Constructor
        public BillCharge(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParameterDefinitionClass> c_P = new NamedListClass<ParameterDefinitionClass>();

                c_P.Add(ArgCode, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The billing code"));
                c_P.Add(ArgDesc, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The description"));
                c_P.Add(ArgDetails, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The details"));
                c_P.Add(ArgUnits, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The number of units"));
                c_P.Add(ArgRate, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The rate per unit"));

                return new DescriptionClass(CategoriesClass.Array, "Bills a charge to the local site", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "charges.bill"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sCode = args.GetDefined(ArgCode);
            string sDesc = args.GetDefined(ArgDesc);
            string sDetails = args.GetDefined(ArgDetails);
            double dUnits = args.GetDefined(ArgUnits).ToDouble();
            double dRate = args.GetDefined(ArgRate).ToDouble();

            // Do
            using (ChargeClass c_Charge = new ChargeClass(dUnits, sCode, sDesc, sDetails, dRate))
            {
                c_Charge.Save(ctx.User.Name);
            }

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
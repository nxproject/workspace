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
    public class ReceivedEach : CommandClass
    {
        #region Constants
        private const string ArgBlock = "block";
        private const string ArgCode = "task";
        private const string ArgAtEnd = "goto";

        private const string ArgCCode = "code";
        private const string ArgCDesc = "desc";
        private const string ArgCDetails = "details";
        private const string ArgCUnits = "units";
        private const string ArgCRate = "rate";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ReceivedEach(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParameterDefinitionClass> c_P = new NamedListClass<ParameterDefinitionClass>();

                c_P.Add(ArgBlock, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The block of charges"));
                c_P.Add(ArgCode, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The chore to be called"));
                c_P.Add(ArgAtEnd, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The label to goto when done"));

                c_P.Add(ArgCCode, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The billing code"));
                c_P.Add(ArgCDesc, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The description"));
                c_P.Add(ArgCDetails, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The details"));
                c_P.Add(ArgCUnits, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The number of units"));
                c_P.Add(ArgCRate, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The rate per unit"));

                return new DescriptionClass(CategoriesClass.Array, "Calls a chore for each charge.  The count is at [*l:listcount]", c_P);
            }
        }
        #endregion

        #region Code Line

        public override string Command
        {
            get { return "charges.received.each"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sProc = args.GetDefined(ArgCode);
            string sGoto = args.GetDefined(ArgAtEnd);

            string sCode = args.GetDefinedRaw(ArgCCode);
            string sDesc = args.GetDefinedRaw(ArgCDesc);
            string sDetails = args.GetDefinedRaw(ArgCDetails);
            string dUnits = args.GetDefinedRaw(ArgCUnits);
            string dRate = args.GetDefinedRaw(ArgCRate);

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgAtEnd, sGoto))
            {
                // Get the parameters
                SvcInfoClass c_Info = ctx.Info.SvcInfo;
                if (c_Info == null)
                {
                    ctx.Parent.LogError("Missing service info");
                }
                else
                {
                    // Get the list
                    ChargesClass c_Charges = c_Info.ExternalCharges;

                    // Setup
                    SubManagerClass c_Sub = ctx.GetSub(sProc, sGoto);

                    // Debug
                    ctx["[*l:listcount]"] = c_Charges.Count.ToString();

                    // Call
                    for (int iLoop = 0; iLoop < c_Charges.Count; iLoop++)
                    {
                        // Map the values
                        ChargeClass c_Charge = c_Charges[iLoop];
                        if (sCode.HasValue()) ctx[sCode] = c_Charge.Code;
                        if (sDesc.HasValue()) ctx[sDesc] = c_Charge.Desc;
                        if (sDetails.HasValue()) ctx[sDetails] = c_Charge.Details;
                        if (dUnits.HasValue()) ctx[dUnits] = c_Charge.Units.ToString();
                        if (dRate.HasValue()) ctx[dRate] = c_Charge.VRate.ToString();

                        // 
                        eAns = c_Sub.Do(args, eAns);
                    }
                }

                eAns = ReturnClass.GoTo(sGoto);
            }

            return eAns;
        }
        #endregion
    }
}
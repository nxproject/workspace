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

using System.Collections.Generic;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class ChargesEach : CommandClass
    {
        #region Constants
        private const string ArgBlock = "block";

        private const string ArgCCode = "code";
        private const string ArgCDesc = "desc";
        private const string ArgCDetails = "details";
        private const string ArgCUnits = "units";
        private const string ArgCRate = "rate";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ChargesEach(EnvironmentClass env)
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
                
                c_P.Add(ArgCCode, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required,  "The billing code"));
                c_P.Add(ArgCDesc, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required,  "The description"));
                c_P.Add(ArgCDetails, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The details"));
                c_P.Add(ArgCUnits, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required,  "The number of units"));
                c_P.Add(ArgCRate, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required,  "The rate per unit"));

                return new DescriptionClass(CategoriesClass.Array, "Call code for each charge", c_P, "Call");
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "charges.each"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the call step
            string sCall = ctx.Outcomes["Call"];
            if (sCall.HasValue())
            {
                // Get the parameters
                string sBlock = args.GetDefined(ArgBlock);

                // Get the list
                ChargesClass c_Docs = ctx.Charges[sBlock];
                // Any?
                if (c_Docs != null)
                {
                    //
                    string sCode = args.GetDefinedRaw(ArgCCode);
                    string sDesc = args.GetDefinedRaw(ArgCDesc);
                    string sDetails = args.GetDefinedRaw(ArgCDetails);
                    string dUnits = args.GetDefinedRaw(ArgCUnits);
                    string dRate = args.GetDefinedRaw(ArgCRate);

                    // Loop thru
                    for (int iLoop = 0; iLoop < c_Docs.Count; iLoop++)
                    {
                        // Debug
                        ctx["[*l:listat]"] = iLoop.ToString();
                        ctx["[*l:listcount]"] = c_Docs.Count.ToString();

                        // Map the values
                        ChargeClass c_Charge = c_Docs[iLoop];

                        if (sCode.HasValue()) ctx[sCode] = c_Charge.Code;
                        if (sDesc.HasValue()) ctx[sDesc] = c_Charge.Desc;
                        if (sDetails.HasValue()) ctx[sDetails] = c_Charge.Details;
                        if (dUnits.HasValue()) ctx[dUnits] = c_Charge.Units.ToString();
                        if (dRate.HasValue()) ctx[dRate] = c_Charge.VRate.ToString();

                        // Call
                        eAns = ctx.Instance.Exec(ctx, args, sCall, args.Depth+1);
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
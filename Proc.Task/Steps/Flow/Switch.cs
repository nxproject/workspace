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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;
using Proc.AO;

namespace Proc.Task
{
    public class Switch : CommandClass
    {
        #region Constants
        private const string ArgsExpr = "expression";
        private const string ArgsCases = "cases";
        #endregion

        #region Constructor
        public Switch()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgsExpr, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The expression to evaluate. The evaluated value will be used to switch on"));
                c_P.Add(ArgsCases, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "A comma-separated list of possible outcomes of the expression", "list"));

                DescriptionClass c_Ans = new DescriptionClass(CategoriesClass.Flow, 
                    "Switch execution based on a given expression", 
                    c_P,
                    OutcomesClass.Dynamic(@"x => x.state.cases"));
                c_Ans.RuntimeDescription = @"x => !!x.state.expression ? `Switch execution based on <strong>${ x.state.expression.expression }</strong>.` : x.definition.description";

                return c_Ans;
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "switch"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            // 
            string sExpr = args.Get(ArgsExpr);

            return new ReturnClass(args.Step.Outcomes[sExpr]);
        }
        #endregion
    }
}
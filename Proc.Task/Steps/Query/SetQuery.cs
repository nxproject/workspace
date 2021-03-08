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
using Proc.Docs;

namespace Proc.Task
{
    public class SetQuery : CommandClass
    {
        #region Constants
        private const string ArgQuery = "query";
        private const string ArgFld = "field";
        private const string ArgOp = "op";
        private const string ArgValue = "value";
        #endregion

        #region Constructor
        public SetQuery()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgQuery, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The query"));
                c_P.Add(ArgFld, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The field"));
                c_P.Add(ArgOp, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The operation"));
                c_P.Add(ArgValue, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The value"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Query, "Sets a query expression", c_P);
            }
        }
        #endregion

        #region Code Line
        //StringBuilder c_Help = new StringBuilder();
        //c_Help.AppendLine("The following operations are supported:");
        //c_Help.AppendLine("");
        //c_Help.AppendLine("=   - Field equal to value");
        //c_Help.AppendLine("<>  - Field not equal to value");
        //c_Help.AppendLine("<   - Field less than value");
        //c_Help.AppendLine("<=  - Field less than or equal to value");
        //c_Help.AppendLine(">   - Field greater than value");
        //c_Help.AppendLine(">=  - Field greater than or equal to value");
        //c_Help.AppendLine("??  - Field in set of values (space delimited)");
        //c_Help.AppendLine("!?? - Field not in set of values (space delimited)");
        //c_Help.AppendLine("?   - Field like value");
        //c_Help.AppendLine("!?  - Field not like value");
        //c_Help.AppendLine("@   - Field exists (value is not used)");
        //c_Help.AppendLine("!@  - Field does not exists (value is not used)");
        //c_Help.AppendLine("><  - Field between values (space delimited)");

        public override string Command
        {
            get { return "query.add"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sQry = args.Get(ArgQuery);
            string sFld = args.Get(ArgFld);
            string sValue = args.Get(ArgValue);
            string sOp = args.Get(ArgOp);

            // 
            TaskQueryClass c_Qry = ctx.Queries[sQry];

            // Add
            c_Qry.Add(ctx, sFld, sValue, QueryElementClass.StringToOp(sOp));

            return eAns;
        }
        #endregion
    }
}
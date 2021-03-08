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

namespace Proc.Workflow
{
    public class Fork : ActivityClass
    {
        #region Constants
        private const string ArgsBranches = "branches";
        #endregion

        #region Constructor
        public Fork()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgsBranches, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "A comma-separated list of names representing branches", "list"));

                this.AddSystem(c_P, false, true);

                DescriptionClass c_Ans = new DescriptionClass(CategoriesClass.Flow, 
                    "Fork workflow execution into multiple branches", 
                    c_P,
                    OutcomesClass.Dynamic(@"x => x.state.branches"));

                return c_Ans;
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "fork"; }
        }

        public override ReturnClass ExecStep(WorkflowContextClass ctx, ArgsClass args)
        {
            // Get the manager
            ManagerClass c_Mgr = ctx.Parent.Globals.Get<ManagerClass>();


            // Get the outcomes
            foreach (string sOutcoome in args.Step.Outcomes.Values)
            {
                // Anu?
                if (sOutcoome.HasValue())
                {
                    c_Mgr.Exec(ctx.Group.Object, ctx.Group, ctx.Stores[AO.Names.Passed], ctx.User.Name, sOutcoome, 0);
                }
            }

            return ReturnClass.None;
        }
        #endregion
    }
}
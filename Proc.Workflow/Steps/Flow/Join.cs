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
    public class Join : ActivityClass
    {
        #region Constants
        private const string ArgsJMode = "joinMode";
        #endregion

        #region Constructor
        public Join()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgsJMode, new ParamDefinitionClass(ParamDefinitionClass.Types.Required,
                    @"Either wait for all or any",
                    "select",
                    new List<string>() { "WaitAll", "WaitAny" }));

                this.AddSystem(c_P, false, true);

                DescriptionClass c_Ans = new DescriptionClass(CategoriesClass.Flow, 
                    "Merge workflow execution back into a single branch", 
                    c_P,
                    OutcomesClass.WorkflowDoneOnly());
                c_Ans.RuntimeDescription = @"x => !!x.state.joinMode ? `Merge workflow execution back into a single branch using mode <strong>${ x.state.joinMode }</strong>` : x.definition.description";

                return c_Ans;
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "join"; }
        }

        public override ReturnClass ExecStep(WorkflowContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Did we do already?
            if (ctx.Group.IsDone(args.Step.ID))
            {
                eAns = ReturnClass.None;
            }
            else
            {
                // Get inputs
                List<string> c_Inputs = new List<string>(args.Step.Inputs.Values);
                // And set the count
                int iCount = 0;

                // Loop thru
                foreach (string sStep in c_Inputs)
                {
                    if (ctx.Group.IsDone(sStep))
                    {
                        iCount++;
                    }
                }

                // Fail
                bool bOK = false;
                // Accorging to type
                switch (args.Step[ArgsJMode])
                {
                    case "WaitAll":
                        bOK = iCount == c_Inputs.Count;
                        break;
                    case "WaitAny":
                        bOK = iCount > 0;
                        break;
                }

                // Are we done?
                if (!bOK)
                {
                    eAns = ReturnClass.None;
                }
            }

            return eAns;

            return eAns;
        }
        #endregion
    }
}
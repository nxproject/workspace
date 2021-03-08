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
    public class Do : ActivityClass
    {
        #region Constants
        private const string ArgsSubj = "subject";
        private const string ArgsMsg = "message";
        private const string ArgsAssgTo = "assignedTo";
        private const string ArgsDur = "duration";
        #endregion

        #region Constructor
        public Do()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgsSubj, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The subject line"));
                c_P.Add(ArgsMsg, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The message line"));
                c_P.Add(ArgsAssgTo, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The group to whom the activity is assigned to"));
                c_P.Add(ArgsDur, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The duration length"));

                this.AddSystem(c_P);

                DescriptionClass c_Ans =  new DescriptionClass(CategoriesClass.Action, "Creates activity", c_P, 
                    OutcomesClass.WorkflowDefaultPlus("Fail", "OnOverdue", "OnError"));
                c_Ans.RuntimeDescription = @"x => !!x.state.subject ? \`<strong>\${ x.state.subject }</strong>\` : x.definition.description";

                return c_Ans;
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "do"; }
        }

        public override ReturnClass ExecStep(WorkflowContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the current object
            ObjectClass c_Obj = ctx.Objects[""];
            // Any?
            if(c_Obj != null)
            {
                // Make activity
                AO.ObjectClass c_Activity = c_Obj.MakeWorkflowActivity(ctx.Group, args.Step);
                // And exit
                eAns = ReturnClass.End;
            }
            else
            {
                eAns = ReturnClass.Failure("Missing object");
            }

            return eAns;
        }
        #endregion
    }
}
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
    public class Delay : ActivityClass
    {
        #region Constants
       private const string ArgsDur = "duration";
        #endregion

        #region Constructor
        public Delay()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgsDur, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The duration length"));

                this.AddSystem(c_P);

                DescriptionClass c_Ans = new DescriptionClass(CategoriesClass.Action, 
                    "Delays for a specified duration", 
                    c_P,
                    OutcomesClass.WorkflowDoneOnly());
                c_Ans.RuntimeDescription = @"x => !!x.state.duration ? `Delay for <strong>${ x.state.duration.expression }</strong>` : x.definition.description";

                return c_Ans;
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "delay"; }
        }

        public override ReturnClass ExecStep(WorkflowContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.End;

            // Compute end
            DateTime c_Till = DateTime.Now.AsUTC().AddSeconds(args.Get(ArgsDur).ToDBDuration().ToSeconds());

            // Create data
            StoreClass c_Data = ctx.Group.ToGroupData(
                AO.Extended.WorkflowClass.FieldWFIfDone, args.Step.Outcomes["Done"]
            );

            // Make entry
            AO.Extended.WorkflowClass c_Shadow = ctx.Group.New("Workflow.Continue", c_Data, c_Till) as AO.Extended.WorkflowClass;

            // And save
            c_Shadow.Save();

            return eAns;
        }
        #endregion
    }
}
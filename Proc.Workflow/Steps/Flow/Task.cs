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
    public class Task : ActivityClass
    {
        #region Constants
        private const string ArgTask = "task";
        private const string ArgStore = "store";
        private const string ArgReturn = "return";
        #endregion

        #region Constructor
        public Task()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgTask, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The task to call"));
                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The store to pass"));
                c_P.Add(ArgReturn, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The store to use for the return"));

                this.AddSystem(c_P);

                DescriptionClass c_Ans = new DescriptionClass(CategoriesClass.Action,
                    "Calls a task",
                    c_P,
                    OutcomesClass.Only("Done"));
                c_Ans.RuntimeDescription = @"x => !!x.state.task ? `Calls <strong>${ x.state.task.expression }</strong>` : x.definition.description";

                return c_Ans;
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "task"; }
        }

        public override ReturnClass ExecStep(WorkflowContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            //
            string sTask = args.Get(ArgTask);
            string sStore = args.Get(ArgStore);
            string sRet = args.Get(ArgReturn);

            // Get the store
            StoreClass c_Store = ctx.Stores[sStore];
            // If none, make one
            if (c_Store == null) c_Store = new StoreClass();

            //
            using (TaskParamsClass c_Params = new TaskParamsClass(ctx.Env))
            {
                c_Params.Task = sTask;
                c_Params.AddStore(Names.Passed, c_Store);
                c_Params["return"] = sRet;

                if (ctx.Objects.Has("changes")) c_Params.AddObject("changes", ctx.Objects["changes"]);
                if (ctx.Objects.Has("current")) c_Params.AddObject("current", ctx.Objects["current"]);

                StoreClass c_Ret = c_Params.Call();

                // Do we have a return?
                if (sRet.HasValue())
                {
                    // Save
                    ctx.Stores[sRet] = c_Ret;
                }

                return eAns;
            }
        }
        #endregion
    }
}
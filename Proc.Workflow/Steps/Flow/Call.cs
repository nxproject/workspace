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
    public class Call : ActivityClass
    {
        #region Constants
        private const string ArgWF = "workflow";
        private const string ArgName = "name";
        private const string ArgStore = "store";
        #endregion

        #region Constructor
        public Call()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgWF, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The workflow to start"));
                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The user defined name", "text"));
                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The store to pass"));

                this.AddSystem(c_P);

                DescriptionClass c_Ans = new DescriptionClass(CategoriesClass.Action, 
                    "Starts a workflow", 
                    c_P,
                    OutcomesClass.Only("Done", "OnError"));
                c_Ans.RuntimeDescription = @"x => !!x.state.workflow ? `Starts <strong>${ x.state.workflow.expression }</strong>` : x.definition.description";

                return c_Ans;
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "call"; }
        }

        public override ReturnClass ExecStep(WorkflowContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // 
            string sWF = args.Get(ArgWF);
            string sInstance = args.Get(ArgName);
            string sStore = args.Get(ArgStore);

            // Get the passed object
            AO.ObjectClass c_Obj = ctx.Objects[AO.Names.Passed];
            
            // Get the params
            StoreClass c_Params = ctx.Stores[sStore];
            if (c_Params == null) c_Params = new StoreClass();

            // Make store
            StoreClass c_Args = new StoreClass();
            c_Args["ds"] = c_Obj.UUID.Dataset.Name;
            c_Args["id"] = c_Obj.UUID.ID;
            c_Args["wf"] = sWF;
            c_Args["instance"] = sInstance;
            c_Args.Set("params", c_Params);

            if(!ctx.CallFN("Workflow.Start", c_Args)["ok"].FromDBBoolean())
            {
                eAns = ReturnClass.Failure("Unable to call task {0}".FormatString(sWF));
            }

            return eAns;
        }
        #endregion
    }
}
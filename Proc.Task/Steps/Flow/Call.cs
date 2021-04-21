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
    public class Call : CommandClass
    {
        #region Constants
        private const string ArgCode = "task";
        private const string ArgStore = "store";

        //private const string ArgIXX = "if";
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

                c_P.Add(ArgCode, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The task to be called"));
                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Store to pass as {0}".FormatString(Names.Passed)));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Flow, "Calls a task, returning to calling task on exit", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "call"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sProc = args.Get(ArgCode);

            // Save
            StoreClass c_Pushed = null;

            //
            using (TaskParamsClass c_Params = new TaskParamsClass(ctx.Env))
            {
                c_Params.Task = sProc;

                // Load from current context
                ctx.AsParams(c_Params);

                // he store
                string sStore = args.Get(ArgStore);
                if (sStore.HasValue())
                {
                    // Save
                    c_Pushed = ctx.Stores[Names.Passed];
                    // Make new
                    ctx.Stores[Names.Passed] = ctx.Stores[sStore];
                }

                c_Params.Call();
            }

            // Do we have a passed store?
            if (c_Pushed != null)
            {
                ctx.Stores[Names.Passed] = c_Pushed;
            }

            return eAns;
        }
        #endregion
    }
}
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
    public class ArrayEach : CommandClass
    {
        #region Constants
        private const string ArgField = "field";
        private const string ArgStore = "store";
        private const string ArgArray = "array";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ArrayEach()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgArray, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the array"));
                c_P.Add(ArgField, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The field to use for the current value"));
                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The store to use for the current value"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Array,
                    "Calls a task for each object in the list.  The working object is found at [*l:listobj] and the count is at [*l:listcount]",
                    c_P,
                    OutcomesClass.WorkflowDefaultPlus("Call"));
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "array.each"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sArray = args.Get(ArgArray);
            string sField = args.GetRaw(ArgField);
            string sStore = args.Get(ArgStore);

            // Save the curent store
            string sCurrObj = ctx.Stores.Default;

            // Get the list
            ArrayClass c_Items = ctx.Arrays[sArray];
            if (c_Items != null)
            {
                // Get the call
                string sCall = args.Step.Outcomes["Call"];
                //
                if (sCall.HasValue())
                {
                    // Loop thru
                    for (int iLoop = 0; iLoop < c_Items.Count; iLoop++)
                    {
                        // Save the value
                        ctx[sField] = c_Items.GetString(iLoop);
                        ctx["[*l:next]"] = c_Items.GetString(iLoop + 1);
                        ctx["[*l:at]"] = iLoop.ToString();

                        // Call
                        ctx.Instance.Exec(ctx, args, sCall, args.Depth + 1);

                        // Update the loop
                        iLoop = ctx["[*l:at]"].ToInteger(iLoop);
                    }

                    // restore state
                    ctx.Stores.Use(sCurrObj);
                }
            }

            return eAns;
        }
        #endregion
    }
}
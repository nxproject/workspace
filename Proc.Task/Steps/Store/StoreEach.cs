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
    public class StoreEach : CommandClass
    {
        #region Constants
        private const string ArgStore = "store";
        private const string ArgField = "field";
        private const string ArgValue = "value";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public StoreEach()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Store to use"));
                c_P.Add(ArgField, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "Field in to use to pass name"));
                c_P.Add(ArgValue, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "Field in to use to pass value"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Store, 
                    "Calls code, passing an element of the array", 
                    c_P,
                    OutcomesClass.TaskDefaultPlus("Call"));
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "store.each"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sField = args.GetRaw(ArgField);
            string sValue = args.GetRaw(ArgValue);
            string sStore = args.Get(ArgStore);

            // And Get the source store
            StoreClass c_Source = ctx.Stores[sStore];
            if (c_Source != null)
            {
                // Get the call
                string sCall = args.Step.Outcomes["Call"];
                //
                if (sCall.HasValue())
                {
                    // Loop for each value
                    foreach (string sKey in c_Source.Keys)
                    {
                        // Move
                        ctx[sField] = sKey;
                        ctx[sValue] = c_Source[sKey];

                        // Call
                        ctx.Instance.Exec(ctx, args, sCall, args.Depth + 1);
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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

using Proc.AO;
using Proc.Docs;

namespace Proc.Chore
{
    public class UIFButton : UITarget
    {
        #region Constants
        private const string ArgLabel = "label";
        private const string ArgChore = "chore";
        private const string ArgStore = "store";
        #endregion

        #region Constructor
        public UIFButton(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a button"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required, ArgLabel, "The label",
            CommandClass.Required, ArgChore, "The chore",
            CommandClass.Optional, ArgStore, "The store to pass as values");

            }
        }

        public override string BaseCommand
        {
            get { return "button"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sLabel = args.GetDefined(ArgLabel);
            string sChore = args.GetDefined(ArgChore);
            string sTo = args.GetDefined(ArgTo);
            string sStore = args.GetDefined(ArgStore);

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgTo, sTo,
                                                ArgLabel, sLabel,
                                                ArgChore, sChore))
            {
                // Treat as button
                arguments.Set(ArgPos, "button");

                // Get the form
                UIFormClass c_Form = env.HTML[sTo] as UIFormClass;
                if (c_Form != null)
                {
                    // The store.  Note that if no store nothing is passed
                    JObject c_Values = null;
                    if (sStore.HasValue()) c_Values = env.Stores[sStore];

                    // Create
                    HTMLFragmentClass c_Ele = c_Form.AppendButton("", sLabel, "", env.UI.State.User, sChore, c_Values);

                    // Save
                    this.BaseExecStep(env, arguments, c_Ele);

                    eAns = ReturnClass.OK;
                }
            }

            return eAns;
        }
        #endregion
    }
}
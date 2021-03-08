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
    public class UIForm : UITarget
    {
        #region Constants
        private const string ArgName = "name";
        private const string ArgSubmit = "submit";
        private const string ArgReset = "reset";
        //private const string ArgButtons = "buttons";
        private const string ArgChore = "chore";
        private const string ArgStore = "store";
        #endregion

        #region Constructor
        public UIForm(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a submission form"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required, ArgName, "The form name",
            CommandClass.Required, ArgChore, "The chore to run when submitted",
            CommandClass.Optional, ArgSubmit, "The label for the submit button",
            CommandClass.Optional, ArgReset, "The label for the reset button",
            CommandClass.Optional, ArgStore, "The store to pass as values");
            //CommandClass.Optional, ArgButtons, "The label/chore space delimited pairs for other buttons",

            }
        }

        public override string BaseCommand
        {
            get { return "form"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sID = args.GetDefined(ArgID).IfEmpty("F".UUID());
            string sName = args.GetDefined(ArgName);
            string sChore = args.GetDefined(ArgChore);
            string sStore = args.GetDefined(ArgStore);
            string sSubmit = args.GetDefined(ArgSubmit);
            string sReset = args.GetDefined(ArgReset);
            //List<string> c_Buttons = args.GetDefined(ArgButtons).SplitSpaces(true);

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgChore, sChore,
                                                ArgName, sName))
            {
                // The store.  Note that if no store nothing is passed
                JObject c_Values = null;
                if (sStore.HasValue()) c_Values = env.Stores[sStore];

                // Set the container
                HTMLFragmentClass c_Ele = sID.UIForm(sName, sSubmit, sChore, c_Values, sReset, env.UI.State.User);
                // Save
                this.BaseExecStep(env, arguments, c_Ele);

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
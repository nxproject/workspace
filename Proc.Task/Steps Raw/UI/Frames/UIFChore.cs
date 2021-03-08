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
    public class UIFChore : CommandClass
    {
        #region Constants
        private const string ArgChore = "chore";
        private const string ArgTitle = "title";
        private const string ArgStore = "store";
        #endregion

        #region Constructor
        public UIFChore(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a chore callframe"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Optional, ArgChore, "The chore to run",
            CommandClass.Optional, ArgTitle, "The title in the menu",
            CommandClass.Optional, ArgStore, "The store to pass");

            }
        }

        public override string Command
        {
            get { return "ui.frame.chore"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get
            string sChore = args.GetDefined(ArgChore);
            string sTitle = args.GetDefined(ArgTitle);
            string sStore = args.GetDefined(ArgStore);

            // Do
            if (sChore.HasValue())
            {
                JObject c_Values = null;
                if (sStore.HasValue()) c_Values = env.Stores[sStore];

                env.UI.Add(new UICtxChoreClass("", sTitle.IfEmpty(sChore), sChore, c_Values, env.UI.State.User));
            }

            return ReturnClass.OK;
        }
        #endregion
    }
}
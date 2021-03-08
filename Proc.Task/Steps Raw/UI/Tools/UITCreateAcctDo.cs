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
    public class UITCreateAcctDo : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgStore = "store";
        private const string ArgUser = "user";
        private const string ArgPwd = "pwd";
        //private const string ArgFlag = "flag";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public UITCreateAcctDo(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Code Line
        public override string Description { get { return("Creates account"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Optional, ArgStore, "Form data store");
            //CommandClass.Optional, ArgUser, "User ID field in store",
            //CommandClass.Optional, ArgPwd, "Password field in store",

            //CommandClass.Required,ArgFlag, "Field where pass/fail (true/false) flag will be put");

            }
        }

        public override string Command
        {
            get { return "ui.tool.createacctdo"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            string sStore = args.GetDefined(ArgStore);
            string sDS = env.UI.State.User.DS;
            string sUser = env.UI.State.User.MapEMail; // args.GetDefined(ArgUser).IfEmpty("user");
            string sPwd = env.UI.State.User.MapPwd;  //args.GetDefined(ArgPwd).IfEmpty("pwd");            string sFlag = args.GetDefinedRaw(ArgFlag);

            // Get the store
            JObject c_Info = env.Stores[sStore];

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgDS, sDS,
                                                ArgUser, sUser,
                                                ArgPwd, sPwd))
            {
                // Get the actuals
                string sActualUser = c_Info.Get(sUser).ToLower();
                c_Info.Set(sUser, "");
                string sActualPwd = c_Info.Get(sPwd);
                c_Info.Set(sPwd, "");

                //env.Env.LogInfo("NAME: " + sActualUser);
                //env.Env.LogInfo("PWD: " + sActualPwd);

                // Validate
                if (this.CheckValidity(ctx,
                                                    ArgUser, sActualUser,
                                                    ArgPwd, sActualPwd))
                {
                    if (!UIUserClass.Exists(env, sActualUser))
                    {
                        env.UI.State.User.Create(env, sActualUser, sActualPwd);

                        // Tell user
                        arguments.Set("msg", "Account created!");
                        arguments.Set("msgclass", "isa_info");
                    }
                    else
                    {
                        arguments.Set("msg", "Account already exists");
                    }
                }
            }

            // Call the security layer
            using (UITSecurity c_Call = new UITSecurity(env.Env))
            {
                eAns = c_Call.ExecStep(env, arguments);
            }

            return eAns;
        }
        #endregion
    }
}
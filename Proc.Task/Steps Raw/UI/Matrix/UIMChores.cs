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
    public class UIMChores : CommandClass
    {
        #region Constants
        private const string ArgChoreLogin = "home";
        private const string ArgChoreCreate = "usercreate";
        private const string ArgChorePwdRequest = "passwordreset";
        private const string ArgChoreCreateDo = "usercreatedo";
        private const string ArgChorePwdRequestDo = "passwordresetdo";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public UIMChores(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Code Line
        
        {
            public override string Description { get { return

            public override string Description { get { return("Maps the chores");

            CommandClass.Required,ArgChoreLogin, "The login chore");
            CommandClass.Required,ArgChoreCreate, "The chore that displays user create screen");
            CommandClass.Required,ArgChoreCreateDo, "The chore that does the user creation");
            CommandClass.Required,ArgChorePwdRequest, "The chore that display the password change screen");
            CommandClass.Required,ArgChorePwdRequestDo, "The chore that does the password change");

            c_Def.Save();
        }

        public override string Command
        {
            get { return "ui.map.chores"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            string sLogin = args.GetDefined(ArgChoreLogin);
            string sCreate = args.GetDefined(ArgChoreCreate);
            string sCreateDo = args.GetDefined(ArgChoreCreateDo);
            string sPwd = args.GetDefined(ArgChorePwdRequest);
            string sPwdDo = args.GetDefined(ArgChorePwdRequestDo);

            // Map
            env.UI.State.Chores.UseMatrix(sLogin,
                                                sCreate,
                                                 sCreateDo,
                                                 sPwd,
                                                 sPwdDo);

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
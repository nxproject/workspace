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
    public class UITSecurity : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgStore = "store";
        private const string ArgUser = "user";
        private const string ArgPwd = "pwd";
        private const string ArgCreate = "create";
        private const string ArgCaptcha = "captcha";
        private const string ArgFlag = "flag";

        private const string ArgMsg = "msg";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public UITSecurity(EnvironmentClass env)
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

            public override string Description { get { return("Assures that portal user is logged in");

            CommandClass.Required,ArgFlag, "Field to store the valid flag into");
            CommandClass.Optional, ArgStore, "Form data store",
            //CommandClass.Optional, ArgUser, "User ID field in store",
            //CommandClass.Optional, ArgPwd, "Password field in store",

            CommandClass.Optional, ArgCreate, "Create login screen if check fails", "true");

            CommandClass.Optional, ArgCaptcha, "Use reCaptcha for validation", "true");

            }
        }

        public override string Command
        {
            get { return "ui.tool.security"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            string sStore = args.GetDefined(ArgStore);
            string sDS = env.UI.State.User.DS;
            string sUser = env.UI.State.User.MapEMail; // args.GetDefined(ArgUser).IfEmpty("user");
            string sPwd = env.UI.State.User.MapPwd;  //args.GetDefined(ArgPwd).IfEmpty("pwd");
            bool bCreate = args.GetDefinedAsBool(ArgCreate, true);
            bool bUseCaptcha = args.GetDefinedAsBool(ArgCaptcha, true);
            string sMsg = args.GetDefined(ArgMsg);
            string sMsgClass = args.GetDefined(ArgMsg + "class");
            string sFlag = args.GetDefinedRaw(ArgFlag);

            // Assume failure
            bool bFlag = false;

            // Get the store
            JObject c_Info = env.Stores[sStore];

            // Check for user
            if (env.UI.State.User.IsValid)
            {
                bFlag = true;
            }
            else
            {
                // Check for captcha
                bool bCaptcha = c_Info.UICaptcha(bUseCaptcha);
                if (bCaptcha)
                {
                    // First see if there is a signature
                    if (env.UI.State.User.EMail.HasValue())
                    {
                        // Done
                        bFlag = true;
                    }
                    else
                    {
                        // Validate
                        if (this.CheckValidity(ctx,
                                                            ArgDS, sDS,
                                                            ArgUser, sUser,
                                                            ArgPwd, sPwd))
                        {
                            // Get the actuals
                            string sActualUser = c_Info.Get(sUser).ToLower();
                            string sActualPwd = c_Info.Get(sPwd);

                            // Validate
                            if (this.CheckValidity(ctx,
                                                                ArgUser, sActualUser,
                                                                ArgPwd, sActualPwd))
                            {
                                // Set
                                bFlag = env.UI.State.User.Use(env, sActualUser, sActualPwd);
                            }
                        }
                    }
                }
                //else
                //{
                //    sMsg ="reCAPTCHA failed";
                //    sMsgClass = "isa_error";
                //}
            }

            // Do we create a screen?
            if (bCreate && !bFlag)
            {
                GeneratePage(env, sUser, sPwd, c_Info, bUseCaptcha, env.UI.State.Chores.Home, env.UI.State.Chores.ChangePwd, env.UI.State.Chores.CreateUser, sMsg, sMsgClass);
            }

            if(sFlag.HasValue()) env[sFlag] = bFlag ? "1" : "0";

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion

        #region Statics
        public static void GeneratePage(SandboxClass env,
                                            string user,
                                            string pwd,
                                            JObject info,
                                            bool useCaptcha,
                                            string choreLogin,
                                            string choreReset,
                                            string choreCreate,
                                            string msg = null,
                                            string msgclass = null)
        {
            // Map the UI
            UIWorkspaceClass c_WS = env.UI;
            // Defaults
            c_WS.Logo = null;
            c_WS.Info = null;
            // The form  container
            UICtxClass c_Ctx = new UICtxClass("login", "Welcome", UICtxClass.TitleTypes.Main);
            c_WS.Add(c_Ctx);

            // Make the fields
            List<HTMLFragmentClass> c_Body = new List<HTMLFragmentClass>();

            if (msg.HasValue())
            {
                c_Body.Add(msg.UIText(msgclass));
            }

            c_Body.Add(user.UIStringField("", user, "", false, "Enter your account name", (JSFragmentClass)null));
            c_Body.Add(pwd.UIPasswordField("", pwd, "", false, "Enter your password", (JSFragmentClass)null));

            // Security
            if (useCaptcha)
            {
                c_Body.Add("".UICaptcha(info, c_WS));
            }

            // Make  the form
            UIFormClass c_Form = "".UIForm("login", "Login", choreLogin, null, "Reset", env.UI.State.User, c_Body);
            // Extra buttons
            if (choreCreate.HasValue() || choreReset.HasValue())
            {
                if (choreCreate.HasValue())
                {
                    c_Form.AppendButton("", "Create", "", c_WS.State.User, choreCreate, null);
                }

                if (choreReset.HasValue())
                {
                    c_Form.AppendButton("", "Reset Pwd", "", c_WS.State.User, choreReset, null);
                }
            }

            c_Ctx.Contents.Add(c_Form);

            // The help
            UICtxClass c_CtxH = new UICtxClass("help", "Help", UICtxClass.TitleTypes.H2);
            c_WS.Add(c_CtxH);

            HTMLFragmentClass c_List = UICmpClass.UIListV(UICmpClass.ListDecorations.Circle);
            c_CtxH.Contents.Add(c_List);
            c_List.Append("Enter your current email or if creating a new account, the email address that you want to use".UIText());
            c_List.Append("Enter your current password, or if you have forgotten or want to change your password, enter the new password".UIText());
            if (useCaptcha)
            {
                c_List.Append("Click on the reCAPTCHA check box and answer the questions".UIText());
            }

            c_CtxH.Contents.Add("Options".UIText());
            c_List = UICmpClass.UIListV(UICmpClass.ListDecorations.Square);
            c_CtxH.Contents.Add(c_List);
            c_List.Append("Login - Logs you in".UIText());
            c_List.Append("Reset - Clears all the fields".UIText());
            if (choreCreate.HasValue())
            {
                c_List.Append("Create - Creates new account.  EMail address given must not be one already in use".UIText());
            }
            if (choreReset.HasValue())
            {
                c_List.Append("Reset Pwd - Changes the password of the email address given".UIText());
            }
        }
        #endregion
    }
}
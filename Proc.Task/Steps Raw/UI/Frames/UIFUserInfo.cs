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
    public class UIFUserInfo : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgStore = "store";
        private const string ArgUser = "user";
        private const string ArgPwd = "pwd";
        private const string ArgCaptcha = "captcha";

        //private const string ArgIXX = "if";

        private const string FormName = "userinfo";
        #endregion

        #region Constructor
        public UIFUserInfo(EnvironmentClass env)
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

            public override string Description { get { return("Sets the user information");

            CommandClass.Optional, ArgStore, "Form data store",
            CommandClass.Optional, ArgUser, "User ID field in store",
            CommandClass.Optional, ArgPwd, "Password field in store",

            CommandClass.Optional, ArgCaptcha, "Use reCaptcha for validation", "true");

            }
        }

        public override string Command
        {
            get { return "ui.frame.userinfo"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            string sStore = args.GetDefined(ArgStore);
            string sDS = env.UI.State.User.DS;
            string sUser = env.UI.State.User.MapEMail; // args.GetDefined(ArgUser).IfEmpty("user");
            string sPwd = env.UI.State.User.MapPwd;  //args.GetDefined(ArgPwd).IfEmpty("pwd");            string sFlag = args.GetDefinedRaw(ArgFlag);
            bool bUseCaptcha = args.GetDefinedAsBool(ArgCaptcha, true);

            // Get the store
            JObject c_Info = env.Stores[sStore];

            //env.Env.LogInfo("Using {0}".FormatString(c_Info.ToSimpleString()));

            // Is it a return?
            if (c_Info.UIIsReturn(FormName))
            {
                // Load
                env.UI.State.User.Load(c_Info, false);
                // Save
                env.UI.State.User.Save();
                // Go to starting point
                using (ChoreNameClass c_ChoreRef = new ChoreNameClass(env.UI.State.Chores.Home))
                {
                    env.ExecStep(c_ChoreRef, arguments);
                }

                eAns = ReturnClass.OK;
            }
            else
            {
                // Validate
                if (this.CheckValidity(ctx,
                                                    ArgDS, sDS,
                                                    ArgUser, sUser,
                                                    ArgPwd, sPwd))
                {
                    // Map the UI
                    UIWorkspaceClass c_WS = env.UI;
                    UIUserClass c_Assoc = c_WS.State.User;

                    // Defaults
                    c_WS.Logo = null;
                    c_WS.Info = null;
                    // The one  container
                    UICtxClass c_Ctx = new UICtxClass("info", "Account Info", UICtxClass.TitleTypes.Main);
                    // Make  the form
                    var c_Form = "".UIForm(FormName, "Update", "", null, "Reset", env.UI.State.User);
                    // Add the fields
                    if (c_Assoc.MapName.HasValue())
                    {
                        c_Form.Body.Append("".UIAutoCapsField("Name", c_Assoc.MapName, c_Assoc.Name, false, "Your name", (JSFragmentClass)null));
                    }

                    if (c_Assoc.MapCompany.HasValue())
                    {
                        c_Form.Body.Append("".UIAutoCapsField("Company", c_Assoc.MapCompany, c_Assoc.Company, false, "Company name you work for", (JSFragmentClass)null));
                    }

                    if (c_Assoc.MapAddress.HasValue())
                    {
                        c_Form.Body.Append("".UIAutoCapsField("Address", c_Assoc.MapAddress, c_Assoc.Address, false, "Street Address", (JSFragmentClass)null));
                    }

                    if (c_Assoc.MapCity.HasValue())
                    {
                        c_Form.Body.Append("".UIAutoCapsField("City", c_Assoc.MapCity, c_Assoc.City, false, "City", (JSFragmentClass)null));
                    }

                    if (c_Assoc.MapState.HasValue())
                    {
                        c_Form.Body.Append("".UIStateField("State", c_Assoc.MapState, c_Assoc.State, false, "State", (JSFragmentClass)null));
                    }

                    if (c_Assoc.MapZIP.HasValue())
                    {
                        c_Form.Body.Append("".UIZIPField("ZIP", c_Assoc.MapZIP, c_Assoc.ZIP, false, "ZIP Code", (JSFragmentClass)null));
                    }

                    if (c_Assoc.MapPhone.HasValue())
                    {
                        c_Form.Body.Append("".UIPhoneField("Phone", c_Assoc.MapPhone, c_Assoc.Phone, false, "Phone", (JSFragmentClass)null));
                    }

                    // Security
                    if (bUseCaptcha)
                    {
                        c_Form.Body.Append("".UICaptcha(c_Info, c_WS));
                    }
                    // Add
                    c_Ctx.Contents.Add(c_Form);
                    c_WS.Add(c_Ctx);

                    eAns = ReturnClass.OK;
                }
            }

            return eAns;
        }
        #endregion
    }
}
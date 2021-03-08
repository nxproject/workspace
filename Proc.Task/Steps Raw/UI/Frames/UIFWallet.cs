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
    public class UIFWallet : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgStore = "store";
        //private const string ArgUser = "user";
        //private const string ArgPwd = "pwd";
        private const string ArgCaptcha = "captcha";
        private const string ArgCC = "cc";

        //private const string ArgIXX = "if";

        private const string FormName = "cc";
        #endregion

        #region Constructor
        public UIFWallet(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Code Line
        public override string Description { get { return("Updates credit card info"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Optional, ArgStore, "Form data store",
            CommandClass.Optional, ArgCC, "Credit Card", "1",
            //CommandClass.Optional, ArgUser, "User ID field in store",
            //CommandClass.Optional, ArgPwd, "Password field in store",

            CommandClass.Optional, ArgCaptcha, "Use reCaptcha for validation", "true");

            }
        }

        public override string Command
        {
            get { return "ui.frame.cc"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            string sStore = args.GetDefined(ArgStore);
            int iWallet = args.GetDefined(ArgCC).ToInteger(0);
            string sDS = env.UI.State.User.DS;
            //string sUser = env.UI.State.User.MapEMail; // args.GetDefined(ArgUser).IfEmpty("user");
            //string sPwd = env.UI.State.User.MapPwd;  //args.GetDefined(ArgPwd).IfEmpty("pwd");            string sFlag = args.GetDefinedRaw(ArgFlag);
            bool bUseCaptcha = args.GetDefinedAsBool(ArgCaptcha, true);

            // Get the store
            JObject c_Info = env.Stores[sStore];

            //env.Env.LogInfo("Using {0}".FormatString(c_Info.ToSimpleString()));

            // Is it a return?
            if (c_Info.UIIsReturn(FormName))
            {
                // Load
                AO.CreditCardClass c_CC = env.UI.State.User.Wallet[iWallet];
                c_CC.Number = c_Info.Get(AO.CreditCardClass.KeyNumber);
                c_CC.Expiration = c_Info.Get(AO.CreditCardClass.KeyExp);
                c_CC.Name = c_Info.Get(AO.CreditCardClass.public override string Description { get { return);
                c_CC.Address = c_Info.Get(AO.CreditCardClass.KeyAddress);
                c_CC.City = c_Info.Get(AO.CreditCardClass.KeyCity);
                c_CC.State = c_Info.Get(AO.CreditCardClass.KeyState);
                c_CC.ZIP = c_Info.Get(AO.CreditCardClass.KeyZIP);
                c_CC.Phone = c_Info.Get(AO.CreditCardClass.KeyPhone);
                c_CC.CVC = c_Info.Get(AO.CreditCardClass.KeyCVC);

                // Save
                c_CC.Save();
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
                                                    ArgDS, sDS))
                {
                    // Map the UI
                    UIWorkspaceClass c_WS = env.UI;
                    AO.CreditCardClass c_CC = c_WS.State.User.Wallet[iWallet];

                    // Defaults
                    c_WS.Logo = null;
                    c_WS.Info = null;
                    // The one  container
                    UICtxClass c_Ctx = new UICtxClass("winfo", "Credit Card", UICtxClass.TitleTypes.Main);
                    // Make  the form
                    var c_Form = "".UIForm(FormName, "Update", "", null, "Reset", env.UI.State.User);
                    // Add the fields
                    c_Form.Body.Append("".UICCField("Number", AO.CreditCardClass.KeyNumber, c_CC.Number, false, "Your name", (JSFragmentClass)null));
                    c_Form.Body.Append("".UICCExpField("Exp", AO.CreditCardClass.KeyExp, c_CC.Expiration, false, "Your name", (JSFragmentClass)null));
                    c_Form.Body.Append("".UINumberField("CVC", AO.CreditCardClass.KeyCVC, c_CC.CVC, false, "CVC", (JSFragmentClass)null));

                    c_Form.Body.Append("".UICapsField("Name", AO.CreditCardClass.public override string Description { get { return, c_CC.Name, false, "Name on credit card", (JSFragmentClass)null));
                    c_Form.Body.Append("".UICapsField("Address", AO.CreditCardClass.KeyAddress, c_CC.Address, false, "Address", (JSFragmentClass)null));
                    c_Form.Body.Append("".UICapsField("City", AO.CreditCardClass.KeyCity, c_CC.City, false, "City", (JSFragmentClass)null));
                    c_Form.Body.Append("".UIStateField("State", AO.CreditCardClass.KeyState, c_CC.State, false, "State", (JSFragmentClass)null));
                    c_Form.Body.Append("".UIZIPField("ZIP", AO.CreditCardClass.KeyZIP, c_CC.ZIP, false, "ZIP", (JSFragmentClass)null));
                    c_Form.Body.Append("".UIPhoneField("Phone", AO.CreditCardClass.KeyPhone, c_CC.Phone, false, "Phone", (JSFragmentClass)null));

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
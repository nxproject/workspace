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
    public class UIFTarget : UITarget
    {
        #region Constants
        private new const string ArgID = "id";
        private const string ArgLabel = "label";
        private const string ArgField = "field";
        private const string ArgValue = "value";
        private const string ArgHint = "hint";
        private const string ArgRO = "ro";
        private const string ArgJS = "js";
        #endregion

        #region Constructor
        public UIFTarget(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override DefinitionClass BaseDocumentation(DefinitionClass def)
        {
            def = this.BaseDescribe(def);

            CommandClass.Optional, ArgID, "The field dom ID",
            CommandClass.Required, ArgField, "The field name");
            CommandClass.Optional, ArgLabel, "The label",
            CommandClass.Optional, ArgValue, "The default value",
            CommandClass.Optional, ArgHint, "The hint to display when field is empty",
            CommandClass.Optional, ArgJS, "Script to run at blur",

            return this.BaseExtraDocumentation(def);
        }

        public virtual DefinitionClass BaseExtraDocumentation(DefinitionClass def)
        {
            }
        }

        public virtual DefinitionClass BaseDescribe(DefinitionClass def)
        {
            }
        }

        public virtual HTMLFragmentClass BaseElement(SandboxClass env,
                                                        ArgsClass arguments,
                                                        string id,
                                                        string label,
                                                        string field,
                                                        string value,
                                                        bool ro,
                                                        string hint,
                                                        JSFragmentClass js)
        {
            return null;
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sID = args.GetDefined(ArgID).IfEmpty("f".UUID());
            string sLabel = args.GetDefined(ArgLabel);
            string sField = args.GetDefined(ArgField);
            string sValue = args.GetDefined(ArgValue);
            string sHint = args.GetDefined(ArgHint);
            bool bRO = args.GetDefinedAsBool(ArgRO, false);
            string sJS = args.GetDefined(ArgJS);

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgField, sField))
            {
                JSFragmentClass c_JS = null;
                if (sJS.HasValue()) c_JS = new JSFragmentClass(sJS, true);

                HTMLFragmentClass c_Ele = this.BaseElement(env, arguments, sID, sLabel, sField, sValue, bRO, sHint, c_JS);

                // Save
                if (c_Ele != null)
                {
                    this.BaseExecStep(env, arguments, c_Ele);

                    eAns = ReturnClass.OK;
                }
            }

            return eAns;
        }
        #endregion
    }
}
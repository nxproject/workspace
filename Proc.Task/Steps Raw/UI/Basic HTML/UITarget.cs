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
    public class UITarget : CommandClass
    {
        #region Constants
        public const string ArgID = "name";
        public const string ArgTo = "to";
        public const string ArgPos = "pos";
        #endregion

        #region Constructor
        public UITarget(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return ""; } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            // Add targets
            CommandClass.Optional, ArgID, "The name of the element",
            CommandClass.Optional, ArgTo, "The name of the element to append to",
            CommandClass.Optional, ArgPos, "Position to append to (t/b/#)");

            }
        }

        public override string Command
        {
            get
            {
                string sAns = this.BaseCommand;
                if (sAns.HasValue()) sAns = "ui." + sAns;

                return sAns;
            }
        }

        public virtual string BaseCommand
        {
            get { return null; }
        }

        public void BaseExecStep(SandboxClass env, ArgsClass arguments, HTMLFragmentClass value)
        {
            // Get the parameters
            string sID = args.GetDefined(ArgID);
            string sTo = args.GetDefined(ArgTo);
            string sPos = args.GetDefined(ArgPos);

            // Use defualt
            if(!sTo.HasValue() && !sID.HasValue())
            {
                sTo = env.HTML.Default;
            }

            // Append
            if (sTo.HasValue())
            {
                // Get the target
                object c_Target = env.HTML[sTo];
                if (c_Target is UICtxClass)
                {
                    UICtxClass c_Ctx = c_Target as UICtxClass;
                    c_Ctx.Contents.Add(value);
                }
                else if (c_Target is UIFormClass)
                {
                    UIFormClass c_Form = c_Target as UIFormClass;

                    if(sPos.IsSameValue("button"))
                    {}
                    else
                    {
                        c_Form.Body.Append(value, sPos);
                    }
                }
                else if (c_Target is HTMLFragmentClass)
                {
                    HTMLFragmentClass c_Frag = c_Target as HTMLFragmentClass;
                    c_Frag.Append(value, sPos);
                }
            }

            // Set
            if(sID.HasValue())
            {
                // Save
                env.HTML[sID] = value;
            }
        }
        #endregion
    }
}
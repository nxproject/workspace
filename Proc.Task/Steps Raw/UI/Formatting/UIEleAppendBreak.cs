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
    public class UIEleAppendBreak : CommandClass
    {
        #region Constants
        private const string ArgAppendTo = "appendto";
        #endregion

        #region Constructor
        public UIEleAppendBreak(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Appends an element to a container or other element"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Optional, ArgAppendTo, "The name of the element/container to append to");

            }
        }

        public override string Command
        {
            get { return "ui.append.break"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sAppend = args.GetDefined(ArgAppendTo);

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgAppendTo, sAppend))
            {
                     // Get the target
                    object c_Target = env.HTML[sAppend];
                    if (c_Target is UICtxClass)
                    {
                        UICtxClass c_Ctx = c_Target as UICtxClass;
                        c_Ctx.Contents.Add(HTMLFragmentClass.Parse("<br/>"));

                        eAns = ReturnClass.OK;
                    }
                    else if (c_Target is HTMLFragmentClass)
                    {
                        HTMLFragmentClass c_Frag = c_Target as HTMLFragmentClass;
                        c_Frag.AppendBreak("");

                        eAns = ReturnClass.OK;
                    }
            }

            return eAns;
        }
        #endregion
    }
}
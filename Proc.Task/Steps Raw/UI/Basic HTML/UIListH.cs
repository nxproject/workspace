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
    public class UIListH : UITarget
    {
        #region Constants
        private const string ArgStyle = "style";
        #endregion

        #region Constructor
        public UIListH(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a horizontal list"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Optional, ArgStyle, "The style of the bullet (n/c/s/r/a)");

            }
        }

        public override string BaseCommand
        {
            get { return "list.horizontal"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            //
            string sStyle = args.GetDefined(ArgStyle);
            UICmpClass.ListDecorations eDeco = UICmpClass.ListDecorations.None;
            switch (sStyle.ToLower())
            {
                case "c":
                    eDeco = UICmpClass.ListDecorations.Circle;
                    break;
                case "s":
                    eDeco = UICmpClass.ListDecorations.Square;
                    break;
                case "r":
                    eDeco = UICmpClass.ListDecorations.Roman;
                    break;
                case "a":
                    eDeco = UICmpClass.ListDecorations.Alpha;
                    break;
                default:
                    eDeco = UICmpClass.ListDecorations.None;
                    break;
            }

            // Create
            HTMLFragmentClass c_Ele = UICmpClass.UIListH(eDeco);

            // Save
            this.BaseExecStep(env, arguments, c_Ele);

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
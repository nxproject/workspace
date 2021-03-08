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
    public class UIElement : UITarget
    {
        #region Constants
        private const string ArgVerb = "tag";
        private const string ArgValue = "value";
        private const string ArgAttr = "attr";
        #endregion

        #region Constructor
        public UIElement(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates an element"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required, ArgVerb, "The tag for the element",
            CommandClass.Optional, ArgValue, "The value for the element",
            CommandClass.Optional, ArgAttr, "Space delimited list of attributes key/value pairs");

            }
        }

        public override string BaseCommand
        {
            get { return "ele"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sVerb = args.GetDefined(ArgVerb);
            string sValue = args.GetDefined(ArgValue);
            List<string> c_Attr = args.GetDefined(ArgAttr).SplitSpaces(true);

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgVerb, sVerb))
            {
                // Set the container
                HTMLFragmentClass c_Ele = new HTMLFragmentClass(sVerb, sValue, (JSFragmentClass)null, c_Attr.ToArray());
                // Save
                this.BaseExecStep(env, arguments, c_Ele);

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
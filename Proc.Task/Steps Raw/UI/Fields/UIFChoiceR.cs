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
    public class UIFChoiceR : UIFTarget
    {
        #region Constants
        private const string ArgChoices = "choices";
        private const string ArgHorz = "horz";
        #endregion

        #region Constructor
        public UIFChoiceR(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a form choice field"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(
            CommandClass.Optional, ArgChoices, "A list of choices delimited by spaces",
            CommandClass.Optional, ArgHorz, "True if list is to be shown horizontally");
            }
        }

        public override string BaseCommand
        {
            get { return "form.choice.radio"; }
        }

        public override HTMLFragmentClass BaseElement(SandboxClass env,
                                                        ArgsClass arguments,
                                                        string id,
                                                        string label,
                                                        string field,
                                                        string value,
                                                        bool ro,
                                                        string hint,
                                                        JSFragmentClass js)
        {
            //
            bool bH = args.GetDefinedAsBool(ArgHorz, false);
            List<string> c_Choices = args.GetDefined(ArgChoices).SplitSpaces(true);

            return id.UIChoiceRField(label, field, value, ro, hint, bH, js, c_Choices.ToArray());
        }
        #endregion
    }
}
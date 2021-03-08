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
    public class UIFURL : CommandClass
    {
        #region Constants
        private const string ArgURL = "url";
        private const string ArgTitle = "title";
        #endregion

        #region Constructor
        public UIFURL(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        
        {
            public override string Description { get { return

            public override string Description { get { return("Creates a URL redirect frame");

            CommandClass.Optional, ArgURL, "The url to redirect to",
            CommandClass.Optional, ArgTitle, "The title in the menu",

            }
        }

        public override string Command
        {
            get { return "ui.frame.url"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get
            string sURL = args.GetDefined(ArgURL);
            string sTitle = args.GetDefined(ArgTitle);

            // Do
            if (sURL.HasValue())
            {
                env.UI.Add(new UICtxRefClass("", sTitle.IfEmpty(sURL), sURL));
            }

            return ReturnClass.OK;
        }
        #endregion
    }
}
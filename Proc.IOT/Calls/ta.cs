///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
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
using Common.TaskWF;
using Proc.AO;

namespace Proc.IOTIF
{
    public class ta : IOTRequestClass
    {
        #region Constructor
        public ta()
        { }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "ta"; }
        }

        public override StoreClass ExecStep(IOTContextClass ctx)
        {
            StoreClass c_Ans = null;

            // Do we have an object id?
            if (ctx.Option.HasValue())
            {
                //
                using (TaskParamsClass c_Params = new TaskParamsClass(ctx.Env))
                {
                    c_Params.Task = ctx.Verb;
                    c_Params.AddStore("passed", ctx.Params);
                    c_Params["return"] = AO.Names.Passed;

                    if (ctx.Stores.Has("changes")) c_Params.AddStore("changes", ctx.Stores["changes"]);
                    if (ctx.Stores.Has("current")) c_Params.AddStore("current", ctx.Stores["current"]);

                    c_Params.Call();
                }
            }

            if (c_Ans == null) c_Ans = new StoreClass();

            return c_Ans;
        }
        #endregion
    }
}
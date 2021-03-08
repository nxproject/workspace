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
using Proc.Docs;

namespace Proc.Task
{
    public class Inc : CommandClass
    {
        #region Constants
        private const string ArgFld = "field";
        private const string ArgValue = "value";
        #endregion

        #region Constructor
        public Inc()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgFld, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The field"));
                c_P.Add(ArgValue, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The value to increment by"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Ops, "Increments a field", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "incr"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sFld = args.GetRaw(ArgFld);
            string sValue = args.Get(ArgFld);
            string sDelta = args.Get(ArgValue).IfEmpty("1");

            //
            ctx[sFld] = (sValue.ToInteger(0) + sDelta.ToInteger(1)).ToString();

            return eAns;
        }
        #endregion
    }
}
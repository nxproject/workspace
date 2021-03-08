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
    public class ObjTouch : CommandClass
    {
        #region Constants
        private const string ArgName = "obj";
        private const string ArgFld = "field";
        #endregion

        #region Constructor
        public ObjTouch()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The object to use"));
                c_P.Add(ArgFld, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The field"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, "Flags the field as changed", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.touch"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sObj = args.Get(ArgName);
            string sFld = args.GetRaw(ArgFld);

            // Allow for eval as target
            if (sFld.HasValue())
            {
                using (DatumClass c_Datum = new DatumClass(ctx, sFld))
                {
                    if (c_Datum.Type == DatumClass.Types.Expression)
                    {
                        sFld = c_Datum.Value;
                    }
                }
            }

            // 
            AO.ObjectClass c_Obj = ctx.Objects[sObj];
            if (c_Obj != null)
            {
                c_Obj.MarkChanged(sFld);
            }

            return eAns;
        }
        #endregion
    }
}
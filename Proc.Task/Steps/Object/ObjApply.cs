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
    public class ObjApply : CommandClass
    {
        #region Constants
        private const string ArgName = "obj";
        private const string ArgSource = "store";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjApply()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The name of the object"));
                c_P.Add(ArgSource, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The source object"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, "Applies a store to an object", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.apply"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            //string sSource = args.Get(ArgSource);
            string sName = args.Get(ArgName);
            string sSource = args.Get(ArgSource).IfEmpty("_changes");

            // Get the object
            AO.ObjectClass c_Obj = ctx.Objects[sName];
            if (c_Obj != null)
            {
                // Get the store
                StoreClass c_Store = ctx.Stores[sSource];
                if (c_Store != null)
                {
                    // Do each
                    foreach (string sField in c_Store.Keys)
                    {
                        c_Obj[sField] = c_Store[sField];
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
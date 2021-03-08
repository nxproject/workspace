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
    public class ObjListDelete : CommandClass
    {
        #region Constants
        private const string ArgList = "list";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjListDelete()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The list"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.ObjList, "Deletes all objects in the list", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.list.delete"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sList = args.Get(ArgList);

            // Get the list
            ObjectListClass c_Docs = ctx.ObjectLists[sList];

            // Call
            for (int iLoop = 0; iLoop < c_Docs.Count; iLoop++)
            {
                // 
                string sName = c_Docs[iLoop];

                // Get the object
                AO.ObjectClass c_Obj = ctx.Objects[sName];
                if (c_Obj != null)
                {
                    c_Obj.Delete();
                    ctx.Objects.Remove(sName);
                }
            }

            // Say bye to list
            ctx.ObjectLists[sList].Clear();

            return eAns;
        }
        #endregion
    }
}
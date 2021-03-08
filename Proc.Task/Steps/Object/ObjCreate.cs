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
    public class ObjCreate : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgStore = "store";
        private const string ArgName = "obj";
        private const string ArgParent = "parent";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjCreate()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgDS, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The dataset of the object"));
                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The store holding the values"));
                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the object"));
                c_P.Add(ArgParent, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the parent object"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, "Creates a new working object", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.create"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the args
            string sDS = args.Get(ArgDS);
            string sName = args.Get(ArgName);
            string sParent = args.Get(ArgParent);

            // Assume none
            StoreClass c_Info = ctx.Stores[args.Get(ArgStore)];

            // Do we have a parent?
            if (sParent.HasValue())
            {
                // Get the object
                AO.ObjectClass c_Parent = ctx.Objects[sParent];
                // Any?
                if (c_Parent != null)
                {
                    // Add
                    c_Info[AO.ObjectClass.FieldParent] = c_Parent.UUID.ToString();
                }
            }

            // Create object
            AO.ObjectClass c_Obj = ctx.Database[sDS].New(null, ctx);

            // Process
            foreach (string sKey in c_Info.Keys)
            {
                // Save
                c_Obj[sKey] = c_Info[sKey];
            }

            // Make into working object
            ctx.Objects[sName] = c_Obj;

            return eAns;
        }
        #endregion
    }
}
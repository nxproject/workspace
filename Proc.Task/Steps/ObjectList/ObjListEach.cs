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
    public class ObjListEach : CommandClass
    {
        #region Constants
       private const string ArgStore = "store";
        private const string ArgList = "list";
        private const string ArgObj = "obj";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjListEach()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The list"));
                c_P.Add(ArgObj, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The object name to use inside the chore"));
                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Store to pass as {0}".FormatString(Names.Passed)));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.ObjList, 
                    "Calls a chore for each object in the list.  The working object is found at [*l:listobj] and the count is at [*l:listcount]", 
                    c_P, 
                    OutcomesClass.TaskDefaultPlus("Call"));
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.list.each"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sList = args.Get(ArgList);
            string sObj = args.Get(ArgObj);

            /// Get the call
            string sCall = args.Step.Outcomes["Call"];
            //
            if (sCall.HasValue())
            {
                // Setup as no store passed
                StoreClass c_Passed = null;

                // Do we have a atore?
                string sStore = args.Get(ArgStore);
                if (sStore.HasValue())
                {
                    c_Passed = ctx.Stores[Names.Passed];
                    ctx.Stores[Names.Passed] = ctx.Stores[sStore];
                }

                // Save the curent object
                string sCurrObj = ctx.Objects.Default;

                // Get the list
                List<string> c_Docs = ctx.ObjectLists[sList];

                // Debug
                ctx["[*l:listcount]"] = c_Docs.Count.ToString();

                // Loop thru
                for (int iLoop = 0; iLoop < c_Docs.Count; iLoop++)
                {
                    // Get the object
                    AO.ObjectClass c_Obj = ctx.Objects[c_Docs[iLoop]];

                    // Add to system
                    ctx.Objects[sObj] = c_Obj;
                    // Set in sys store
                    ctx["[*l:at]"] = iLoop.ToString();

                    // Set the use object
                    ctx.Objects.Use(sObj);

                    // Call
                    ctx.Instance.Exec(ctx, args, sCall, args.Depth + 1);

                    // Update the loop
                    iLoop = ctx["[*l:at]"].ToInteger(iLoop);

                    // Remove
                    ctx.Objects.Remove(sObj);
                }

                // Do we have a passed store?
                if (c_Passed != null)
                {
                    ctx.Stores[Names.Passed] = c_Passed;
                }

                // restore state
                ctx.Objects.Use(sCurrObj);
            }

            return eAns;
        }
        #endregion
    }
}
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
    public class ObjEach : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgQuery = "query";
        private const string ArgList = "list";
        private const string ArgDir = "dir";
        private const string ArgLimit = "limit";
        private const string ArgSkip = "skip";
        private const string ArgSort = "sortby";
        private const string ArgObj = "as";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjEach()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The list"));
                c_P.Add(ArgDS, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The dataset of the object"));
                c_P.Add(ArgQuery, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The store that is the query"));
                c_P.Add(ArgSort, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Sort by"));
                c_P.Add(ArgLimit, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Max number of objects to return"));
                c_P.Add(ArgSkip, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Number of objects to skip"));
                c_P.Add(ArgDir, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Direction of sort (asc/desc)"));
                c_P.Add(ArgObj, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The name of the passed object"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, 
                    "Call code for each object in a query", 
                    c_P, 
                    OutcomesClass.TaskDefaultPlus("Call"));
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.each"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the call step
            string sCall = ctx.Outcomes["Call"];
            if (sCall.HasValue())
            {
                // Get the parameters
                string sDS = args.Get(ArgDS);
                string sList = args.Get(ArgList);
                string sQuery = args.Get(ArgQuery);
                string sSort = args.Get(ArgSort).IfEmpty("_desc");
                int iLimit = args.Get(ArgLimit).ToInteger(-1);
                int iSkip = args.Get(ArgSkip).ToInteger(0);
                bool bDesc = !args.Get(ArgDir).IsSameValue("desc");
                string sObj = args.Get(ArgObj).IfEmpty(Names.Passed);

                // Get the query
                TaskQueryClass c_TQ = ctx.Queries[sQuery];
                if (c_TQ != null)
                {
                    // Make the query
                    using (AO.QueryClass c_Qry = new QueryClass(ctx, ctx.Database[sDS].DataCollection, c_TQ.Expressions))
                    {
                        // Query
                        List<AO.ObjectClass> c_Objs = c_Qry.FindObjects( iLimit, iSkip, sSort, bDesc);
                        // Any?
                        if (c_Objs != null)
                        {
                            // Save the curent object
                            string sCurrObj = ctx.Objects.Default;

                            // Set 
                            for (int iLoop = 0; iLoop < c_Objs.Count; iLoop++)
                            {
                                // Debug
                                ctx["[*l:at]"] = iLoop.ToString();
                                ctx["[*l:count]"] = c_Objs.Count.ToString();

                                // Set
                                ctx.Objects[sObj] = c_Objs[iLoop];
                                ctx.Objects.Use(sObj);

                                // Call
                                eAns = ctx.Instance.Exec(ctx, args, sCall, args.Depth + 1);
                            }

                            // Reset
                            ctx.Objects.Use(sCurrObj);
                        }
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
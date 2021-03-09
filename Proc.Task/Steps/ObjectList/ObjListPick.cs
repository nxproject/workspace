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
    public class ObjListPick : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgQuery = "query";
        private const string ArgList = "list";
        private const string ArgDir = "dir";
        private const string ArgLimit = "limit";
        private const string ArgSort = "sortby";
        private const string ArgMerge = "merge";
        private const string ArgSkip = "skip";
        #endregion

        #region Constructor
        public ObjListPick()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The name of the list"));
                c_P.Add(ArgDS, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The dataset of the object"));
                c_P.Add(ArgQuery, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The store that is the query"));
                c_P.Add(ArgSort, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Sort by"));
                c_P.Add(ArgLimit, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Max number of objects to return"));
                c_P.Add(ArgSkip, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Number of objects to skip"));
                c_P.Add(ArgDir, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Direction of sort (asc/desc)"));
                c_P.Add(ArgMerge, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "True if lists are merged"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.ObjList, "Creates an object list using a query", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.list.pick"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sDS = args.Get(ArgDS);
            string sList = args.Get(ArgList);
            string sQuery = args.Get(ArgQuery);
            string sSort = args.Get(ArgSort).IfEmpty(AO.ObjectClass.FieldDescription);
            int iLimit = args.Get(ArgLimit).ToInteger(-1);
            int iSkip = args.Get(ArgSkip).ToInteger(0);
            bool bDir = !args.Get(ArgDir).IsSameValue("desc");
            bool bMerge = args.GetAsBool(ArgMerge, false);

            // Get list
            ObjectListClass c_List = ctx.ObjectLists[sList];
            if (!bMerge) c_List.Clear();
            // Get the count
            int iAt = c_List.Count;
            
            // Parse fields to set
            QueryClass c_Query = new QueryClass(ctx, ctx.Database[sDS].DataCollection, ctx.Queries[sQuery].Expressions);
            //
            List<AO.ObjectClass> c_Objs = c_Query.FindObjects(iLimit, iSkip, sSort, bDir);
            // Loop thru
            foreach (AO.ObjectClass c_Obj in c_Objs)
            {
                // Add
                string sName = sList + "_" + iAt;

                ctx.Objects[sName] = c_Obj;
                c_List.Add(sName);
            }

            return eAns;
        }
        #endregion
    }
}
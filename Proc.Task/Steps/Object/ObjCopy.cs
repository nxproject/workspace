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
    public class ObjCopy : CommandClass
    {
        #region Constants
        private const string ArgSource = "obj";
        private const string ArgTarget = "to";
        private const string ArgFlds = "flds";
        private const string ArgExcl = "excl";
        private const string ArgExt = "ext";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjCopy()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgSource, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the source object"));
                c_P.Add(ArgTarget, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The name of the target object"));
                c_P.Add(ArgFlds, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Fields to include"));
                c_P.Add(ArgExcl, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Fields to exclude"));
                c_P.Add(ArgExt, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Copy extensions"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, "Copies one object to another", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.copy"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the args
            string sSource = args.Get(ArgSource);
            string sTarget = args.Get(ArgTarget);
            string sFlds = args.Get(ArgFlds);
            string sExcl = args.Get(ArgExcl);
            bool bExt = args.GetAsBool(ArgExt, true);

            // Map the objects
            AO.ObjectClass c_Source = ctx.Objects[sSource];
            AO.ObjectClass c_Target = ctx.Objects[sTarget];

            // Get the data
            JObject c_Data = c_Source.AsJObject.ToSimpleString().ToJObject();
            // Remove parent
            c_Data.Remove(AO.ObjectClass.FieldParent);

            // Do we remove extensions?
            if (!bExt)
            {
                foreach (string sKey in c_Data.Keys())
                {
                    if (sKey.Substring(1).IndexOf("_") != -1)
                    {
                        c_Data.Remove(sKey);
                    }
                }
            }

            // Do we have exclude?
            if (sExcl.HasValue())
            {
                // Make list
                List<string> c_Flds = sExcl.SplitSpaces(true);
                // And do
                foreach (string sFld in c_Flds)
                {
                    c_Data.Remove(sFld);
                }
            }

            // Do we have include?
            if (sFlds.HasValue())
            {
                // Make list
                List<string> c_Flds = sFlds.SplitSpaces(true);
                // Make result
                JObject c_New = new JObject();
                // And do
                foreach (string sFld in c_Data.Keys())
                {
                    string sPrefix = "";
                    int iPos = -1;

                    if (sFld.StartsWith("_"))
                    {
                        iPos = sFld.Substring(1).IndexOf("_");
                        if (iPos != -1) iPos++;
                    }
                    else
                    {
                        iPos = sFld.IndexOf("_");
                    }

                    if (iPos != -1)
                    {
                        sPrefix = sFld.Substring(0, iPos);
                    }
                    else
                    {
                        sPrefix = sFld;
                    }

                    if (c_Flds.IndexOf(sPrefix) != -1)
                    {
                        c_New.Set(sFld, c_Data.Get(sFld));
                    }
                }
                // Swap
                c_Data = c_New;
            }

            // Process
            foreach (string sKey in c_Data.Keys())
            {
                // Save
                c_Target[sKey] = c_Data.Get(sKey);
            }

            return eAns;
        }
        #endregion
    }
}
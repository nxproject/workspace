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
    public class ObjLU : CommandClass
    {
        #region Constants
        private const string ArgName = "obj";
        private const string ArgFld = "field";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjLU()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the object"));
                c_P.Add(ArgFld, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The field"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, "Carries out lookup on a field", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.lu"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            //string sSource = args.Get(ArgSource);
            string sName = args.Get(ArgName);
            string sFld = args.GetRaw(ArgFld);

            // Get the object
            AO.ObjectClass c_Obj = ctx.Objects[sName];
            if (c_Obj != null)
            {
                // Get the field definition
                AO.DatasetClass c_DS = ctx.Database[c_Obj.UUID.Dataset.Name];
                if (c_DS != null)
                {
                    AO.Definitions.DatasetFieldClass c_Def = c_DS.Definition[sFld];
                    if (c_Def != null && c_Def.Type == AO.Definitions.DatasetFieldClass.FieldTypes.LU)
                    {
                        string sValue = c_Obj[sFld];

                        try
                        {
                            if (sValue.HasValue())
                            {
                                // The dataset that is te source
                                AO.DatasetClass c_LUDS = ctx.Database[c_Def.LinkDS];
                                // Make the query
                                using (AO.QueryClass c_Qry = new QueryClass(c_LUDS.DataCollection))
                                {
                                    // Add
                                    c_Qry.Add(AO.ObjectClass.FieldDescription, QueryElementClass.QueryOps.Any, sValue);

                                    // Get
                                    List<AO.ObjectClass> c_Source = c_Qry.FindObjects(1);
                                    if (c_Source.Count == 1)
                                    {
                                        // Get map
                                        List<string> c_Map = c_Def.LUMap.SplitSpaces();

                                        if (c_Map == null || c_Map.Count < 2)
                                        {
                                            string sMap = AO.ObjectClass.FieldDescription;
                                            if (c_Map != null) sMap = c_Map[0];

                                            c_Map = new List<string>();
                                            c_Map.Add(sFld);
                                            c_Map.Add(sMap);
                                        }

                                        if (c_Map != null)
                                        {
                                            for (int i = 0; i < c_Map.Count; i += 2)
                                            {
                                                string sTarget = c_Map[i];
                                                string sSource = c_Map[i + 1];

                                                string sOldVal = c_Obj[sTarget];
                                                string sNewVal = c_Source[0][sSource];

                                                if (!sOldVal.IsExactSameValue(sNewVal))
                                                {
                                                    c_Obj[sTarget] = sNewVal;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        catch { }
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
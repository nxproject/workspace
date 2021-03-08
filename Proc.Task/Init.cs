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

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;
using Common.TaskWF;

namespace Proc.Task
{
    /// <summary>
    /// 
    /// Default setup
    /// 
    /// </summary>
    public class Init : FNClass
    {
        public override void Initialize(EnvironmentClass env)
        {
            base.Initialize(env);

            // Load manager
            ManagerClass c_Mgr = env.Globals.Get<ManagerClass>();

            // Extend eval
            FunctionsDefinitions c_Defs = Context.FunctionsTable;

            //
            c_Defs.AddFn(new StaticFunction("arraycount", delegate (Context ctx, object[] ps)
            {
                TaskContextClass c_Ctx = ctx as TaskContextClass;

                int iAns = -1;

                ArrayClass c_Arr = c_Ctx.Arrays[XCVT.ToString(ps[0])];
                if (c_Arr != null)
                {
                    iAns = c_Arr.Count;
                }

                return iAns;
            }, 1, 1, 
            "Returns the count of values in an array",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "The name of the array")));

            c_Defs.AddFn(new StaticFunction("objfield", delegate (Context ctx, object[] ps)
            {
                if (ps.Length == 2)
                {
                    return MakeObjField(XCVT.ToString(ps[0]), XCVT.ToString(ps[1]));
                }
                else
                {
                    return MakeObjField(null, XCVT.ToString(ps[0]));
                }
            }, 1, 2,
            "Returns the eval field reference of a field in an object",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "Either the field name for the default object object name or the object name"),
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Optional, "The field name if the previous parameter is the object name")));

            c_Defs.AddFn(new StaticFunction("objisnew", delegate (Context ctx, object[] ps)
            {
                bool bAns = true;

                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;
                if (c_Ctx != null)
                {
                    var c_Obj = c_Ctx.Objects[XCVT.ToString(ps[0])];
                    if (c_Obj != null)
                    {
                        bAns = c_Obj.IsNew;
                    }
                }
                return bAns;
            }, 1, 1,
            "Returns true if the object is new",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "The object name")));

            c_Defs.AddFn(new StaticFunction("objid", delegate (Context ctx, object[] ps)
            {
                string sAns = "";

                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;
                if (c_Ctx != null)
                {
                    AO.ObjectClass c_Obj = c_Ctx.Objects[XCVT.ToString(ps[0])];
                    if (c_Obj != null) sAns = c_Obj.UUID.ToString();
                }

                return sAns;
            }, 1, 1,
            "Returns the UUID of the object",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "The object name")));

            c_Defs.AddFn(new StaticFunction("objidonly", delegate (Context ctx, object[] ps)
            {
                string sAns = "";

                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;
                if (c_Ctx != null)
                {
                    var c_Obj = c_Ctx.Objects[XCVT.ToString(ps[0])];
                    if (c_Obj != null) sAns = c_Obj.UUID.ID;
                }

                return sAns;
            }, 1, 1,
            "Returns the ID portion of the UUID of the object",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "The object name")));

            c_Defs.AddFn(new StaticFunction("objds", delegate (Context ctx, object[] ps)
            {
                string sAns = "";

                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;
                if (c_Ctx != null)
                {
                    var c_Obj = c_Ctx.Objects[XCVT.ToString(ps[0])];
                    if (c_Obj != null) sAns = c_Obj.UUID.Dataset.Name;
                }

                return sAns;
            }, 1, 1,
            "Returns true if the object is new",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "The object name")));

            c_Defs.AddFn(new StaticFunction("placeholder", delegate (Context ctx, object[] ps)
            {
                string sAns = "";


                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;
                if (c_Ctx != null)
                {
                    string sID = XCVT.ToString(ps[0]);
                    if (AO.UUIDClass.IsValid(sID))
                    {
                        using (AO.UUIDClass c_UUID = new AO.UUIDClass(c_Ctx.Database, sID))
                        {
                            using (AO.ObjectClass c_Obj = c_UUID.Dataset[c_UUID.ID])
                            {
                                sAns = c_Obj.ObjectDescription;
                            }
                        }
                    }
                }

                return sAns;
            }, 1, 1,
            "Returns the placeholder of the object",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "The object name")));

            c_Defs.AddFn(new StaticFunction("linkdesc", delegate (Context ctx, object[] ps)
            {
                string sAns = "";


                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;
                if (c_Ctx != null)
                {
                    string sID = XCVT.ToString(ps[0]);
                    if (AO.UUIDClass.IsValid(sID))
                    {
                        using (AO.UUIDClass c_UUID = new AO.UUIDClass(c_Ctx.Database, sID))
                        {
                            using (AO.ObjectClass c_Obj = c_UUID.Dataset[c_UUID.ID])
                            {
                                sAns = c_Obj.ObjectDescription;
                            }
                        }
                    }
                }

                return sAns;
            }, 1, 1,
            "Returns the placeholder of the object",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "The object name")));

            c_Defs.AddFn(new StaticFunction("objlistcount", delegate (Context ctx, object[] ps)
            {
                TaskContextClass c_Ctx = ctx as TaskContextClass;

                int iAns = -1;

                ObjectListClass c_Arr = c_Ctx.ObjectLists[XCVT.ToString(ps[0])];
                if (c_Arr != null)
                {
                    iAns = c_Arr.Count;
                }

                return iAns;
            }, 1, 1,
            "Returns the count of values in a list",
            new NX.Engine.ParameterDefinitionClass(NX.Engine.ParameterDefinitionClass.Types.Required, "The name of the object list")));

            // Load comamnds
            c_Mgr.SelfLoad(env);

            if (env["document"].FromDBBoolean())
            {
                // Write MD
                c_Mgr.GenerateMD(@"C:\Candid Concepts\NX\Office\UI.QX\help\info.\README_D_TASKS.md");
                // Write Elsa file
                c_Mgr.GenerateElsa(@"C:\Candid Concepts\NX\Others\Elsa\src\plugins\task-activities.ts");
            }
        }

        #region Statics
        public static string MakeObjField(string obj, string value)
        {
            value = value.IfEmpty();
            if (obj.HasValue()) value = obj + ":" + value;

            if (!value.StartsWith("[") && !value.EndsWith("]")) value = "[" + value + "]";
            return value;
        }

        public static string MakeStoreField(string store, string value)
        {
            value = value.IfEmpty();
            if (store.HasValue()) value = store + ":" + value;

            if (!value.StartsWith("[*") && !value.EndsWith("]")) value = "[*" + value + "]";
            return value;
        }
        #endregion
    }
}
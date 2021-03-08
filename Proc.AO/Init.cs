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

using NX.Engine;
using NX.Shared;

namespace Proc.AO
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
            c_Defs.AddFn(new StaticFunction("objvalue", delegate (Context ctx, object[] ps)
            {
                if (ps.Length == 2)
                {
                    return Expression.Eval(ctx, MakeObjField(XCVT.ToString(ps[0]), XCVT.ToString(ps[1]))).Value;
                }
                else
                {
                    return Expression.Eval(ctx, MakeObjField(null, XCVT.ToString(ps[0]))).Value;
                }
            }, 1, 2,
            "Returns the field value for an optional object and field",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The object name if a second value is give, otherwise the field name"),
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The field name")));
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
            "Returns the field representation for an optional object and field",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The object name if a second value is give, otherwise the field name"),
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The field name")));
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
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The object name")));
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
            "Returns the object UUID",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The object name")));
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
            "Returns the ID portion object UUID",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The object name")));

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
            "Returns the dataset portion object UUID",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The object name")));

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
            "Returns the object placeholder",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The object name")));

            c_Defs.AddFn(new StaticFunction("user", delegate (Context ctx, object[] ps)
            {
                string sAns = "";

                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;
                if (c_Ctx != null && c_Ctx.User != null)
                {
                    sAns = c_Ctx.User.Name;
                }

                return sAns;
            }, 0, 0,
            "Returns the current user name"));
        }

        #region Statics
        /// <summary>
        /// 
        /// Returns the eval object string for a given object and field
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string MakeObjField(string obj, string value)
        {
            value = value.IfEmpty();
            if (obj.HasValue()) value = obj + ":" + value;

            if (!value.StartsWith("[") && !value.EndsWith("]")) value = "[" + value + "]";
            return value;
        }

        /// <summary>
        /// 
        /// Returns the eval store string for a given store and field
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
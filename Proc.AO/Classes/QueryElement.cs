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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// A filter element
    /// 
    /// </summary>
    public class QueryElementClass : IDisposable
    {
        #region Constructor
        public QueryElementClass(ExtendedContextClass ctx, JObject value, QueryOps op = QueryOps.Eq)
        {
            this.Field = value.Get("field");
            this.Operator = op;
            try
            {
                this.Operator = (QueryOps)Enum.Parse(typeof(QueryOps), value.Get("op"));
            }
            catch { }
            this.SubOperator = SubqueryOps.All;
            try
            {
                this.SubOperator = (SubqueryOps)Enum.Parse(typeof(SubqueryOps), value.Get("sop"));
            }
            catch { }

            JArray c_List = value.GetJArray("queries");
            if (c_List != null)
            {
                for (int i = 0; i < c_List.Count; i++)
                {
                    this.Queries.Add(new QueryElementClass(ctx, c_List.GetJObject(i)));
                }
            }

            Tuple<QueryOps, string> c_Value = Parse(value.Get("value").IfEmpty(), this.Operator);
            this.Operator = c_Value.Item1;

            this.SetValue(ctx, c_Value.Item2);

            this.Initialize();
        }

        public QueryElementClass(ExtendedContextClass ctx, JArray values, QueryOps op = QueryOps.Eq)
        {
            this.SubOperator = SubqueryOps.All;

            if (values != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    //
                    QueryElementClass c_Sub = null;

                    // Get entry
                    JObject c_Entry = values.GetJObject(i);
                    // Any?
                    if (c_Entry != null)
                    {
                        // Parse
                        c_Sub = new QueryElementClass(ctx, c_Entry, op);
                    }
                    else
                    {
                        // Do we have a list?
                        JArray c_Qrys = values.GetJArray(i);
                        // If so, make a subquery
                        if (c_Qrys != null)
                        {
                            // Parse
                            c_Sub = new QueryElementClass(ctx, c_Qrys, op);
                        }
                    }

                    if (c_Sub != null)
                    {
                        this.Queries.Add(c_Sub);
                    }
                }
            }

            this.Initialize();
        }

        public QueryElementClass(ExtendedContextClass ctx, string field, string value, QueryOps op = QueryOps.Eq)
        {
            //
            this.Field = field;
            this.Operator = op;

            this.SetValue(ctx, value);

            this.Initialize();
        }

        private void Initialize()
        {
            //
            if (this.Field.IsSameValue("_desc"))
            {
                this.Field = "_search";
                this.Value = this.Value.IfEmpty().ToUpper();
            }
        }
        #endregion

        #region Enums
        /// <summary>
        /// 
        /// Operators allowed
        /// 
        /// </summary>
        public enum QueryOps
        {
            Eq,
            Ne,
            Gt,
            Gte,
            Lt,
            Lte,
            In,
            Nin,
            Like,
            Notlike,
            Exists,
            Notexists,
            Any
        }

        public enum SubqueryOps
        {
            All,
            Any
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// 
        /// Houekeeping
        /// 
        /// </summary>
        public void Dispose()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The field name
        /// 
        /// </summary>
        public string Field { get; internal set; }

        /// <summary>
        /// 
        ///  The value
        ///  
        /// </summary>
        public string Value { get; internal set; }

        /// <summary>
        /// 
        /// Operation
        /// 
        /// </summary>
        public QueryOps Operator { get; internal set; }

        public SubqueryOps SubOperator { get; set; } = SubqueryOps.All;

        /// <summary>
        /// 
        /// List of child queries
        /// 
        /// </summary>
        public List<QueryElementClass> Queries { get; private set; } = new List<QueryElementClass>();
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Converts query element to JSON object
        /// </summary>
        /// <returns></returns>
        public JObject ToJObject()
        {
            //
            JObject c_Wkg = new JObject();

            c_Wkg.Set("field", this.Field.IfEmpty());
            c_Wkg.Set("op", this.Operator.ToString());
            c_Wkg.Set("value", this.Value.IfEmpty());
            c_Wkg.Set("sop", this.SubOperator.ToString());

            JArray c_List = new JArray();
            foreach (QueryElementClass c_SQ in this.Queries)
            {
                c_List.Add(c_SQ.ToJObject());
            }
            c_Wkg.Set("queries", c_List);

            return c_Wkg;
        }

        /// <summary>
        /// 
        /// Sets the value which could be computed
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="value"></param>
        public void SetValue(ExtendedContextClass ctx, string value)
        {
            if (ctx != null)
            {
                using (DatumClass c_Datum = new DatumClass(ctx, value))
                {
                    switch (c_Datum.Type)
                    {
                        case DatumClass.Types.Data:
                        case DatumClass.Types.Store:
                        case DatumClass.Types.Expression:
                            value = c_Datum.Value;
                            break;
                    }
                }
            }

            this.Value = value;
        }
        #endregion

        #region Statics
        /// <summary>
        /// 
        /// Converts an ops to its string representation
        /// 
        /// </summary>
        /// <param name="op">The op</param>
        /// <returns>The string representation</returns>
        public static string OpToString(QueryOps op)
        {
            // Assume none
            string sAns = "";

            // According to op
            switch (op)
            {
                case QueryOps.Eq:
                    sAns = "=";
                    break;

                case QueryOps.Ne:
                    sAns = "!=";
                    break;

                case QueryOps.Gt:
                    sAns = ">";
                    break;

                case QueryOps.Gte:
                    sAns = ">=";
                    break;

                case QueryOps.Lt:
                    sAns = "<";
                    break;

                case QueryOps.Lte:
                    sAns = "<=";
                    break;

                case QueryOps.In:
                    sAns = "??";
                    break;

                case QueryOps.Nin:
                    sAns = "!??";
                    break;

                case QueryOps.Like:
                    sAns = "?";
                    break;

                case QueryOps.Notlike:
                    sAns = "!?";
                    break;

                case QueryOps.Exists:
                    sAns = "@";
                    break;

                case QueryOps.Notexists:
                    sAns = "!@";
                    break;

                case QueryOps.Any:
                    sAns = "%";
                    break;
            }

            return sAns.IfEmpty("=");
        }

        /// <summary>
        /// 
        /// Converts a string to an op
        /// 
        /// </summary>
        /// <param name="op">The string</param>
        /// <returns>The op</returns>
        public static QueryOps StringToOp(string op)
        {
            // Assume
            QueryOps eAns = QueryOps.Eq;

            // And parse
            switch (op)
            {
                case "<>":
                case "!=":
                case "!==":
                case "Ne":
                    eAns = QueryOps.Ne;
                    break;

                case ">":
                case "Gt":
                    eAns = QueryOps.Gt;
                    break;

                case ">=":
                case "Gte":
                    eAns = QueryOps.Gte;
                    break;

                case "<":
                case "Lt":
                    eAns = QueryOps.Lt;
                    break;

                case "<=":
                case "Lte":
                    eAns = QueryOps.Lte;
                    break;

                case "!??":
                case "Nin":
                    eAns = QueryOps.Nin;
                    break;

                case "??":
                case "In":
                    eAns = QueryOps.In;
                    break;

                case "?":
                case "Like":
                    eAns = QueryOps.Like;
                    break;

                case "!?":
                case "Notlike":
                    eAns = QueryOps.Notlike;
                    break;

                case "@":
                case "Exists":
                    eAns = QueryOps.Exists;
                    break;

                case "!@":
                case "Notexists":
                    eAns = QueryOps.Notexists;
                    break;

                case "%":
                case "Any":
                    eAns = QueryOps.Any;
                    break;

                default:
                    eAns = QueryOps.Eq;
                    break;
            }

            return eAns;
        }

        /// <summary>
        /// 
        /// Parses a value into op/value tuple
        /// 
        /// </summary>
        /// <param name="value">The string value with a possible leading operator</param>
        /// <returns></returns>
        public static Tuple<QueryOps, string> Parse(string value, QueryElementClass.QueryOps ops = QueryElementClass.QueryOps.Eq)
        {
            // Assume no operators
            Tuple<QueryOps, string> c_Ans = new Tuple<QueryOps, string>(ops, value);

            // Get the leading part
            Match c_Leading = Regex.Match(value.IfEmpty(), @"(?<op>^[=<>?!@%]+)(?<value>.+)");

            // Did we get any?
            if (c_Leading.Success)
            {
                // Get the operator
                c_Ans = new Tuple<QueryOps, string>(StringToOp(c_Leading.Groups["op"].Value), c_Leading.Groups["value"].Value);
            }

            return c_Ans;
        }
        #endregion
    }
}
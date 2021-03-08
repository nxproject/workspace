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
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
/// 

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MongoDB.Driver;
using MongoDB.Bson;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Gets an object
    /// 
    /// </summary>
    public class QueryGet : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sDS = store["ds"].AsDatasetName();

            string sCols = store["cols"];
            List<string> c_Cols = null;
            if (sCols.HasValue()) c_Cols = sCols.SplitSpaces();

            bool bTransform = store["transform"].IsSameValue("y") && c_Cols != null;

            string sPrefixField = store["prefixfield"].IfEmpty("_id");
            string sPrefixValue = store["prefixvalue"];
            bool bExtra = store["extra"].IsSameValue("y");

            string sPrefix = store["idprefix"];
            if(sPrefix.HasValue())
            {
                sPrefixValue = sPrefix;
                sPrefixField = "_id";
                bExtra = sPrefix.StartsWith("#");
            }

            // Valid?
            if (sDS.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                // 
                NamedListClass<TransformClass> c_Transforms = new NamedListClass<TransformClass>();
                JArray c_Derived = new JArray();
                if (bTransform)
                {
                    // Loop thru
                    foreach (string sFld in c_Cols)
                    {
                        // Make
                        TransformClass c_Ctx = new TransformClass(sFld, c_DS);
                        // Build the derivers
                        c_Ctx.MakeDerivers(c_Derived);
                        // Save
                        c_Transforms[sFld] = c_Ctx;
                    }
                }

                using (ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, store, null, call.UserInfo.Name))
                {
                    CollectionClass c_Coll = c_DS.DataCollection;
                    if (bExtra) c_Coll = c_DS.SettingsCollection;

                    // Make the query
                    using (QueryClass c_Query = new QueryClass(c_Ctx, c_Coll, store.GetAsJArray("query"), QueryElementClass.QueryOps.Any))
                    {
                        // Handle prefix
                        if(sPrefixValue.HasValue())
                        {
                            c_Query.Add(sPrefixField, QueryElementClass.QueryOps.Like, "^" + sPrefixValue);
                        }

                        // Compute skip
                        int iSkip = 0;
                        if (store["firstRow"].HasValue()) iSkip = store["firstRow"].ToInteger(0);
                        // Compute max
                        int iCount = -1;
                        if (store["lastRow"].HasValue()) iCount = 1 + store["lastRow"].ToInteger(0) - iSkip;
                        // Sort
                        string sSort = store["sortCol"];
                        // make desc sort case insensitive
                        if (sSort.IsSameValue(ObjectClass.FieldDescription)) sSort = ObjectClass.FieldSearch;

                        // Get list
                        List<BsonDocument> c_Docs = c_Query.Find(iCount, iSkip, sSort, !store["sortOrder"].IsSameValue("desc"), store["fields"]);

                        // Make output array
                        JArray c_Data = new JArray();

                        // Loop thru
                        foreach (BsonDocument c_Doc in c_Docs)
                        {
                            // Get
                            JObject c_Obj = c_Doc.ToJObject();

                            // Limited?
                            if (c_Cols != null)
                            {
                                // Make temp
                                JObject c_Wkg = new JObject();
                                // Loop thru
                                foreach (string sFld in c_Cols)
                                {
                                    // Get the value
                                    string sValue = c_Obj.Get(sFld);

                                    // Transform
                                    if (bTransform)
                                    {
                                        // Get
                                        TransformClass c_Trx = c_Transforms[sFld];
                                        if (c_Trx != null)
                                        {
                                            c_Trx.Format(c_Wkg, sValue);
                                        }
                                        else
                                        {
                                            // Save
                                            c_Wkg.Set(sFld, sValue);
                                        }
                                    }
                                    else
                                    {
                                        // Save
                                        c_Wkg.Set(sFld, sValue);
                                    }
                                }

                                // Swap
                                c_Obj = c_Wkg;
                            }

                            // Add
                            c_Data.Add(c_Obj);
                        }

                        // Save
                        c_Ans.Set("data", c_Data);
                        c_Ans.Set("derived", c_Derived);

                    }
                }
            }

            return c_Ans;
        }
    }

    public class TransformClass
    {
        #region Constructor
        public TransformClass(string fld, DatasetClass ds)
        {
            // 
            this.Field = fld;

            // Get the field definition
            Definitions.DatasetFieldClass c_Fld = ds.Definition[this.Field];
            if (c_Fld != null)
            {
                //
                this.Label = c_Fld.Label.IfEmpty(this.Field).Replace(".", "");
                this.Type = c_Fld.Type;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The original field name
        /// 
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// 
        /// Teh final name
        /// 
        /// </summary>
        private string Label { get; set; }

        /// <summary>
        /// 
        /// The field type
        /// 
        /// </summary>
        private Definitions.DatasetFieldClass.FieldTypes Type { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Make the extra fields
        /// 
        /// </summary>
        /// <param name="defs"></param>
        public void MakeDerivers(JArray defs)
        {            
            //
            switch(this.Type)
            {
                case Definitions.DatasetFieldClass.FieldTypes.Date:
                case Definitions.DatasetFieldClass.FieldTypes.DateTime:
                    defs.Add(this.Make("(month)", "substr", "7"));
                    defs.Add(this.Make("(year)", "substr", "4"));
                    break;
            }
        }

        /// <summary>
        /// 
        /// Make a singl deriver
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="fn"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private JObject Make(string label, string fn, string option)
        {
            //
            JObject c_Def = new JObject();

            c_Def.Set("name", this.Label + label);
            c_Def.Set("fn", fn); 
            c_Def.Set("field", this.Label);
            c_Def.Set("option", option);

            return c_Def;
        }

        /// <summary>
        /// 
        /// Formats a value
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="value"></param>
        public void Format(JObject values, string value)
        {
            // Do we have a value
            if (value.HasValue())
            {
                //
                switch (this.Type)
                {
                    case Definitions.DatasetFieldClass.FieldTypes.Date:
                        value = value.FromDBDate().FormattedAs("yyyy-MM-dd");
                        break;

                    case Definitions.DatasetFieldClass.FieldTypes.DateTime:
                        value = value.FromDBDate().FormattedAs("yyyy-MM-dd HH:mm");
                        break;
                }
            }

            // Save
            values.Set(this.Label, value);
        }
        #endregion
    }
}
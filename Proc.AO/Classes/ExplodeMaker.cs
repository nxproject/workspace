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
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public static class ExplodeMakerClass
    {
        #region Enums
        public enum ExplodeModes
        {
            None,
            UpDown,
            Up,
            Down
        }
        #endregion

        #region Methods
        public static JObject Explode(this ObjectClass obj, StoreClass passed = null, ExplodeModes mode = ExplodeModes.UpDown)
        {
            JObject c_Ans = new JObject();

            // Must be valid
            if (obj != null)
            {
                //
                c_Ans = obj.AsJObject;
                if (passed != null)
                {
                    c_Ans.Merge(passed.SynchObject);
                }

                string sUUID = obj.UUID.ToString();

                if (mode != ExplodeModes.None)
                {
                    // Get billing
                    if (mode == ExplodeModes.UpDown)
                    {
                        // Get the bill items
                        string sBillTo = c_Ans.Get("_billto");
                        string sBillAt = c_Ans.Get("_billat");
                        // Is it a billable object
                        if (sBillAt.HasValue() && sBillTo.HasValue())
                        {
                            c_Ans.Set("_billing", ExplodeMakerClass.Billing(obj.Dataset.Parent, sBillTo, sBillAt));
                        }
                    }
                }

                // Loop thru
                foreach (string sField in obj.Dataset.Definition.FieldNames)
                {
                    // Get the thield definition
                    Definitions.DatasetFieldClass c_Field = obj.Dataset.Definition[sField];
                    // Get the value
                    string sValue = c_Ans.Get(sField);

                    // Any?
                    if (sValue.HasValue())
                    {
                        // According to type
                        switch (c_Field.Type)
                        {
                            case Definitions.DatasetFieldClass.FieldTypes.Date:
                                c_Ans.Set(sField, sValue.FromDBDate().FormattedAs("MM/dd/yyyy"));
                                break;

                            case Definitions.DatasetFieldClass.FieldTypes.DateTime:
                                c_Ans.Set(sField, sValue.FromDBDate().FormattedAs("MM/dd/yyyy hh:mm tt"));
                                break;

                            case Definitions.DatasetFieldClass.FieldTypes.Currency:
                                c_Ans.Set(sField, "{0:c}".FormatString(sValue.ToDouble(0)));
                                break;

                            case Definitions.DatasetFieldClass.FieldTypes.Grid:
                                // TBD
                                break;
                        }
                    }
                    else
                    {
                        c_Ans.Set(sField, "");
                    }
                }

                //
                if (mode == ExplodeModes.Down || mode == ExplodeModes.UpDown)
                {
                    // Child datasets
                    string sDSS = obj.Dataset.Definition.ChildDSs.IfEmpty();
                    if (sDSS.HasValue())
                    {
                        // Make holder
                        JObject c_Holder = new JObject();
                        // Split
                        List<string> c_DSS = sDSS.SplitSpaces();
                        // Any?
                        // Loop thru
                        foreach (string sDS in c_DSS)
                        {
                            c_Holder.Set(sDS, ExplodeMakerClass.Query(obj.Dataset.Parent, sDS, "_parent", sUUID));
                        }

                        // Save 
                        c_Ans.Add("_child", c_Holder);
                    }

                    // Linked datasets
                    sDSS = obj.Dataset.Definition.RelatedDSs.IfEmpty();
                    if (sDSS.HasValue())
                    {
                        // Make holder
                        JObject c_Holder = new JObject();
                        // Split
                        List<string> c_DSS = sDSS.SplitSpaces();
                        // Any?
                        // Loop thru
                        for (int i = 0; i < c_DSS.Count; i += 2)
                        {
                            // Parse
                            string sDS = c_DSS[i];
                            string sField = c_DSS[i + 1];

                            c_Holder.Set(sDS + "_" + sField, ExplodeMakerClass.Query(obj.Dataset.Parent, sDS, sField, sUUID));
                        }

                        // Save
                        c_Ans.Add("_links", c_Holder);
                    }
                }
            }

            return c_Ans;
        }

        private static JArray Query(DatabaseClass db, string ds, string field, string value)
        {
            //
            JArray c_Ans = new JArray();

            // Make a query
            using (QueryClass c_Qry = new QueryClass(db[ds].DataCollection))
            {
                // 
                c_Qry.Add(field, QueryElementClass.QueryOps.Eq, value);

                // Loop thru
                foreach (ObjectClass c_Obj in c_Qry.FindObjects())
                {
                    // Add
                    c_Ans.Add(c_Obj.UUID.ToString());
                }
            }

            return c_Ans;
        }

        private static JObject Billing(DatabaseClass db, string to, string at)
        {
            //
            JObject c_Ans = new JObject();

            // 
            c_Ans.Set("_charges", ExplodeMakerClass.BillingItems(db, DatabaseClass.DatasetBiilCharge, to, at));
            c_Ans.Set("_subscriptions", ExplodeMakerClass.BillingItems(db, DatabaseClass.DatasetBiilSubscription, to, at));
            c_Ans.Set("_invoices", ExplodeMakerClass.BillingItems(db, DatabaseClass.DatasetBiilInvoice, to, at));

          
            return c_Ans;
        }

        private static JArray BillingItems(DatabaseClass db, string ds, string to, string at)
        {
            //
            JArray c_Ans = new JArray();

            // Make a query
            using (QueryClass c_Qry = new QueryClass(db[ds].DataCollection))
            {
                // 
                c_Qry.Add("_billto", QueryElementClass.QueryOps.Eq, to);
                c_Qry.Add("_billat", QueryElementClass.QueryOps.Eq, at);

                // Loop thru
                foreach (ObjectClass c_Obj in c_Qry.FindObjects())
                {
                    // Add
                    c_Ans.Add(c_Obj.UUID.ToString());
                }
            }

            return c_Ans;
        }
        #endregion
    }
}

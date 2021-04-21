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
        public enum DirectionModes
        {
            None,
            UpDown,
            Up,
            Down,
            Data
        }

        public enum ExplodeModes
        {
            Yes,
            No
        }
        #endregion

        #region Methods
        public static JObject Explode(this ObjectClass obj, ExplodeModes explode, AO.ExtendedContextClass ctx = null, StoreClass passed = null, DirectionModes mode = DirectionModes.UpDown)
        {
            JObject c_Ans = new JObject();

            // Must be valid
            if (obj != null)
            {
                // Start with base
                c_Ans = obj.AsJObject;

                // Merge passed
                if (passed != null)
                {
                    c_Ans.Merge(passed.SynchObject);
                }

                // Clean it
                c_Ans = c_Ans.CleanObject().Clear("").SetMongoInfo();

                // Handle context
                if (ctx != null)
                {
                    // Get store list
                    List<string> c_Stores = ctx.Stores.Keys;
                    // Loop thru
                    foreach (string sStore in c_Stores)
                    {
                        // handle accordingly
                        switch (sStore)
                        {
                            case Names.Passed:
                                // Merge
                                c_Ans.Merge(ctx.Stores[sStore]);
                                break;

                            case Names.Sys:
                                JObject c_Sys = ctx.Stores[sStore].SynchObject.Use(
                                    "name", "addr1", "city", "state", "zip", "phone",
                                    "officeopen", "officeclose", "timezone").SetMongoInfo();

                                string sWkg = c_Sys.Get("officeopen");
                                if (sWkg.HasValue())
                                {
                                    c_Sys.Set("officeopen", sWkg.DateFromDB(obj.Dataset.Parent).FormattedAs("hh:mm tt"));
                                }

                                sWkg = c_Sys.Get("officeclose");
                                if (sWkg.HasValue())
                                {
                                    c_Sys.Set("officeclose", sWkg.DateFromDB(obj.Dataset.Parent).FormattedAs("hh:mm tt"));
                                }

                                c_Ans.Set("_" + sStore, c_Sys);
                                break;

                            default:
                                // Make child
                                c_Ans.Set("_" + sStore, ctx.Stores[sStore].SynchObject.CleanObject().Clear("pwd").SetMongoInfo().ExpandLinks(obj.Dataset.Parent));
                                break;
                        }
                    }
                }

                string sUUID = obj.UUID.ToString();

                //
                bool bBillingShown = false;
                if (mode != DirectionModes.None)
                {
                    // Get billing
                    if (mode == DirectionModes.UpDown)
                    {
                        // Get the bill items
                        string sBillTo = c_Ans.Get("_billto");
                        string sBillAt = c_Ans.Get("_billat");
                        // Is it a billable object
                        if (sBillAt.HasValue() && sBillTo.HasValue())
                        {
                            c_Ans.Set("_billing", ExplodeMakerClass.Billing(explode, obj.Dataset.Parent, sBillTo, sBillAt));

                            bBillingShown = true;
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
                                c_Ans.Set(sField, sValue.DateFromDB(obj.Dataset.Parent).FormattedAs("MM/dd/yyyy"));
                                break;

                            case Definitions.DatasetFieldClass.FieldTypes.DateTime:
                                c_Ans.Set(sField, sValue.DateFromDB(obj.Dataset.Parent).FormattedAs("MM/dd/yyyy hh:mm tt"));
                                break;

                            case Definitions.DatasetFieldClass.FieldTypes.Time:
                                c_Ans.Set(sField, sValue.DateFromDB(obj.Dataset.Parent).FormattedAs("hh:mm tt"));
                                break;

                            case Definitions.DatasetFieldClass.FieldTypes.Currency:
                                c_Ans.Set(sField, "{0:#0.00}".FormatString(sValue.ToDouble(0)));
                                break;

                            case Definitions.DatasetFieldClass.FieldTypes.Grid:
                                // TBD
                                break;

                            case Definitions.DatasetFieldClass.FieldTypes.Link:
                                if (mode == DirectionModes.None)
                                {
                                    c_Ans.Remove(sField);
                                }
                                else if (explode == ExplodeModes.Yes)
                                {
                                    using (UUIDClass c_UUID = new UUIDClass(obj.Dataset.Parent, sValue))
                                    {
                                        c_Ans.Set(sField, c_UUID.AsObject.Explode(explode,
                                            mode: (mode == DirectionModes.Data ? DirectionModes.None : DirectionModes.Data)).ExpandLinks(obj.Dataset.Parent));
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        c_Ans.Set(sField, "");
                    }
                }

                //
                if (mode == DirectionModes.Down || mode == DirectionModes.UpDown)
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
                            c_Holder.Set(sDS, ExplodeMakerClass.Query(explode, obj.Dataset.Parent, sDS, "_parent", sUUID));
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

                            c_Holder.Set(sDS + "_" + sField, ExplodeMakerClass.Query(explode, obj.Dataset.Parent, sDS, sField, sUUID));
                        }

                        // Save
                        c_Ans.Add("_links", c_Holder);
                    }
                }

                c_Ans = c_Ans.ProcessLink(obj.Dataset.Parent, "_parent");

                string sDocs = c_Ans.Get("_documents");
                if (sDocs.HasValue())
                {
                    NX.Engine.Files.ManagerClass c_Mgr = obj.Parent.Parent.Parent.Parent.Globals.Get<NX.Engine.Files.ManagerClass>();

                    using (NX.Engine.Files.FolderClass c_Folder = new NX.Engine.Files.FolderClass(c_Mgr, sDocs.Substring(2)))
                    {
                        c_Ans.Set("_documents", c_Folder.Tree());
                    }
                }

                c_Ans = c_Ans.ProcessLink(obj.Dataset.Parent, "_billto");
                c_Ans = c_Ans.ProcessLink(obj.Dataset.Parent, "_billat");
            }

            return c_Ans;
        }

        private static JArray Query(ExplodeModes explode, DatabaseClass db, string ds, string field, string value)
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
                    if (explode == ExplodeModes.Yes)
                    {
                        c_Ans.Add(c_Obj.Explode(explode, mode: DirectionModes.None));
                    }
                    else
                    {
                        c_Ans.Add(c_Obj.UUID.ToString());
                    }
                }
            }

            return c_Ans;
        }

        private static JObject Billing(ExplodeModes explode, DatabaseClass db, string to, string at)
        {
            //
            JObject c_Ans = new JObject();

            // 
            c_Ans.Set("_charges", ExplodeMakerClass.BillingItems(explode, db, DatabaseClass.DatasetBiilCharge, to, at));
            c_Ans.Set("_subscriptions", ExplodeMakerClass.BillingItems(explode, db, DatabaseClass.DatasetBiilSubscription, to, at));
            c_Ans.Set("_invoices", ExplodeMakerClass.BillingItems(explode, db, DatabaseClass.DatasetBiilInvoice, to, at));


            return c_Ans;
        }

        private static JArray BillingItems(ExplodeModes explode, DatabaseClass db, string ds, string to, string at)
        {
            //
            JArray c_Ans = new JArray();

            // Make a query
            using (QueryClass c_Qry = new QueryClass(db[ds].DataCollection))
            {
                // 
                c_Qry.Add("acct", QueryElementClass.QueryOps.Eq, to);
                c_Qry.Add("at", QueryElementClass.QueryOps.Eq, at);

                // Loop thru
                foreach (ObjectClass c_Obj in c_Qry.FindObjects())
                {
                    // Add
                    if (explode == ExplodeModes.Yes)
                    {
                        c_Ans.Add(c_Obj.Explode(explode, mode: DirectionModes.None));
                    }
                    else
                    {
                        c_Ans.Add(c_Obj.UUID.ToString());
                    }
                }
            }

            return c_Ans;
        }

        private static JObject CleanObject(this JObject value)
        {
            return value.Keep("_id", "_ds", "_desc", "_ver", "_documents", "_billto", "_billat", "_parent");
        }

        private static JObject Keep(this JObject value, params string[] keys)
        {
            // Make list
            List<string> c_Keep = new List<string>(keys);

            // Loop thru
            foreach (string sKey in value.Keys())
            {
                // System?
                if (sKey.StartsWith("_") && !c_Keep.Contains(sKey))
                {
                    value.Remove(sKey);
                }
            }

            return value;
        }

        private static JObject Use(this JObject value, params string[] keys)
        { // Make list
            List<string> c_Keep = new List<string>(keys);

            // Loop thru
            foreach (string sKey in value.Keys())
            {
                // System?
                if (!c_Keep.Contains(sKey))
                {
                    value.Remove(sKey);
                }
            }

            return value;
        }

        private static JObject Clear(this JObject value, params string[] keys)
        { // Make list
            List<string> c_Keep = new List<string>(keys);

            // Loop thru
            foreach (string sKey in value.Keys())
            {
                // System?
                if (c_Keep.Contains(sKey))
                {
                    value.Remove(sKey);
                }
            }

            return value;
        }

        private static JObject SetMongoInfo(this JObject value)
        {
            string sWkg = value.Get("_cre");
            if (sWkg.HasValue()) value.Set("_cre", sWkg.MongoIDtoDate().FormattedAs("MM/dd/yyyy hh:mm tt"));

            sWkg = value.Get("_ver");
            if (sWkg.HasValue()) value.Set("_ver", sWkg.MongoIDtoDate().FormattedAs("MM/dd/yyyy hh:mm tt"));

            return value;
        }

        private static JObject ExpandLinks(this JObject value, DatabaseClass db)
        {
            // Loop thru
            foreach (string sField in value.Keys())
            {
                // Get the value
                string sValue = value.Get(sField);
                // Any?
                if (sValue.HasValue())
                {
                    if (UUIDClass.IsValid(sValue))
                    {
                        using (UUIDClass c_UUID = new UUIDClass(db, sValue))
                        {
                            value.Set(sField, c_UUID.AsObject.Explode(ExplodeModes.No, mode: DirectionModes.None));
                        }
                    }
                }
            }

            return value;
        }

        private static JObject ProcessLink(this JObject value, DatabaseClass db, string field)
        {
            string sParent = value.Get(field);
            if (sParent.HasValue() && UUIDClass.IsValid(sParent))
            {
                using (UUIDClass c_UUID = new UUIDClass(db, sParent))
                {
                    value.Set(field, c_UUID.AsObject.Explode(ExplodeModes.No, mode: DirectionModes.Data));
                }
            }

            return value;
        }

        private static DateTime DateFromDB(this string value, DatabaseClass db)
        {
            return value.FromDBDate().AdjustTimezone(db.SiteInfo.Timezone);
        }
        #endregion
    }
}

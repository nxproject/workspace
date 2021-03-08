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
using Proc.AO;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Gets objects as calendar
    /// 
    /// </summary>
    public class CalendarGet : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sDS = store["ds"].AsDatasetName();
            string sFrom = store["from"];
            string sTo = store["to"];
            bool bMgr = store["mgr"].FromDBBoolean();
            JArray c_Qry = store.GetAsJArray("query");

            // Valid?
            if (sDS.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                using (ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, store, null, store["_user"]))
                {
                    // Make the query
                    using (QueryClass c_Query = new QueryClass(c_Ctx, c_DS.DataCollection, c_Qry, QueryElementClass.QueryOps.Any))
                    {
                        // Add date range
                        c_Query.Add("_calstart", QueryElementClass.QueryOps.Gte, "d" + sFrom);
                        c_Query.Add("_calstart", QueryElementClass.QueryOps.Lte, "d" + sTo);

                        // Privacy
                        if (c_DS.Definition.PrivacyAllow && !bMgr)
                        {
                            c_Query.Add(c_DS.Definition.PrivateField, QueryElementClass.QueryOps.Eq, call.UserInfo.Name);
                        }

                        // Get list
                        List<BsonDocument> c_Docs = c_Query.Find();

                        // Make output array
                        JArray c_Data = new JArray();

                        // Loop thru
                        foreach (BsonDocument c_Doc in c_Docs)
                        {
                            // Add
                            c_Data.Add(c_Doc.ToJObject());
                        }

                        // Save
                        c_Ans.Set("data", c_Data);

                    }
                }
            }

            return c_Ans;
        }
    }
}
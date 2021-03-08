﻿///--------------------------------------------------------------------------------
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
    public class QueryCount : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sDS = store["ds"].AsDatasetName();

            string sPrefixField = store["prefixfield"].IfEmpty("_id");
            string sPrefixValue = store["prefixvalue"];
            bool bExtra = store["extra"].IsSameValue("y");

            string sPrefix = store["idprefix"];
            if (sPrefix.HasValue())
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

                using (ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, store, null, call.UserInfo.Name))
                {
                    CollectionClass c_Coll = c_DS.DataCollection;
                    if (bExtra) c_Coll = c_DS.SettingsCollection;

                    // Make the query
                    using (QueryClass c_Query = new QueryClass(c_Ctx, c_Coll, store.GetAsJArray("query"), QueryElementClass.QueryOps.Any))
                    {
                        // Handle prefix
                        if (sPrefixValue.HasValue())
                        {
                            c_Query.Add(sPrefixField, QueryElementClass.QueryOps.Like, "^" + sPrefixValue);
                        }

                        // Count
                        c_Ans["count"] = c_Query.ComputeCount(999).ToString();
                    }
                }
            }

            return c_Ans;
        }
    }
}
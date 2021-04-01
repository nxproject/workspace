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

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Gets an object
    /// 
    /// </summary>
    public class ObjectGeMatch : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sDS = store["ds"].AsDatasetName();
            string sFld = store["fld"];
            string sValue = store["value"];
            string sMatches = store["matches"];
            bool bCreate = store["create"].FromDBBoolean();
            JObject c_Data = store["data"].ToJObject();
            bool bForce = store["force"].FromDBBoolean();

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            // Do we have multiple possibles?
            if (sMatches.HasValue())
            {
                // Break apart
                List<string> c_List = sMatches.SplitSpaces();
                // Lopp thru
                for (int i = 0; i < c_List.Count; i += 2)
                {
                    // Get
                    sDS = c_List[i].AsDatasetName();
                    sFld = c_List[i + 1].AsFieldName();
                    // Map
                    DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];
                    // Do the query
                    using (QueryClass c_Qry = new QueryClass(c_DS.DataCollection))
                    {
                        // Add statement
                        c_Qry.Add(sFld, QueryElementClass.QueryOps.Eq, sValue);
                        // Get the first object
                        List<ObjectClass> c_Poss = c_Qry.FindObjects(1);
                        // Any?
                        if (c_Poss.Count == 1)
                        {
                            //
                            if(bForce)
                            {
                                // The extra data
                                if (c_Data != null)
                                {
                                    // Get the object
                                    ObjectClass c_Obj = c_Poss[0];

                                    // Loop thru
                                    foreach (string sKey in c_Data.Keys())
                                    {
                                        // Map
                                        c_Obj[sKey] = c_Data.Get(sKey);
                                    }

                                    // Save
                                    c_Obj.Save();
                                }
                            }
                            // Save the id
                            c_Ans["id"] = c_Poss[0].UUID.ToString();
                            break;
                        }
                    }
                }

                // Do we need to create?
                if (bCreate && !c_Ans["id"].HasValue())
                {
                    // Get
                    sDS = c_List[0].AsDatasetName();
                    sFld = c_List[1].AsFieldName();
                    // Map
                    DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                    using (ObjectClass c_Obj = c_DS.New())
                    {
                        // Fill in
                        c_Obj[sFld] = sValue;
                        // The extra data
                        if (c_Data != null)
                        {
                            // Loop thru
                            foreach (string sKey in c_Data.Keys())
                            {
                                // Map
                                c_Obj[sKey] = c_Data.Get(sKey);
                            }
                        }
                        // Save
                        c_Obj.Save();
                        // And pass back
                        c_Ans["id"] = c_Obj.UUID.ToString();
                    }
                }
            }
            else if (sDS.HasValue() && sFld.HasValue() && sValue.HasValue())
            {
                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                // Make query
                using (QueryClass c_Qry = new QueryClass(c_DS.DataCollection))
                {
                    // Add statement
                    c_Qry.Add(sFld, QueryElementClass.QueryOps.Eq, sValue);

                    // Get the first object
                    List<ObjectClass> c_Poss = c_Qry.FindObjects(1);
                    // Any?
                    if (c_Poss.Count == 1)
                    {
                        // Do we need to update?
                        if(bForce)
                        {
                            // The extra data
                            if (c_Data != null)
                            {
                                // Get the object
                                ObjectClass c_Obj = c_Poss[0];

                                // Loop thru
                                foreach (string sKey in c_Data.Keys())
                                {
                                    // Map
                                    c_Obj[sKey] = c_Data.Get(sKey);
                                }

                                // Save
                                c_Obj.Save();
                            }
                        }
                        // Save the id
                        c_Ans["id"] = c_Poss[0].UUID.ToString();
                    }
                    else if (bCreate)
                    {
                        using (ObjectClass c_Obj = c_DS.New())
                        {
                            // Fill in
                            c_Obj[sFld] = sValue;
                            // The extra data
                            if (c_Data != null)
                            {
                                // Loop thru
                                foreach (string sKey in c_Data.Keys())
                                {
                                    // Map
                                    c_Obj[sKey] = c_Data.Get(sKey);
                                }
                            }
                            // Save
                            c_Obj.Save();
                            // And pass back
                            c_Ans["id"] = c_Obj.UUID.ToString();
                        }
                    }
                }
            }

            return c_Ans;
        }
    }
}
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

using Newtonsoft.Json.Linq;

using System.Collections.Generic;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Sets an object
    /// 
    /// </summary>
    public class AccessSet : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Get the params
            string sDS = DatabaseClass.DatasetBillAccess;
            string sID = store["id"];
            JObject c_Data = store.GetAsJObject("data");

            // Valid?
            if (sDS.HasValue() && sID.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                // Query
                using (QueryClass c_Qry = new QueryClass(c_Mgr.DefaultDatabase[sDS].DataCollection))
                {
                    c_Qry.Add("name", QueryElementClass.QueryOps.Eq, sID);

                    // Loop thru
                    foreach (ObjectClass c_Obj in c_Qry.FindObjects())
                    {
                        // Must have been there
                        if (c_Obj["name"].IsSameValue(sID))
                        {
                            // Reset 
                            foreach (string sFld in c_Data.Keys())
                            {
                                c_Obj[sFld] = c_Data.Get(sFld);
                            }
                            // Save
                            c_Obj.Save();
                        }
                    }
                }
            }

            return store;
        }
    }
}
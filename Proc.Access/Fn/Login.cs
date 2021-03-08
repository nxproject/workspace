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
using System;
using System.Linq;
using NX.Engine;
using NX.Shared;

using Proc.AO;

namespace Proc.Access
{
    /// <summary>
    /// 
    /// Logins user
    /// 
    /// </summary>
    public class Login : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass values)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sUser = values["user"];
            string sPwd = values["pwd"];

            // Valid?
            if (sUser.HasValue() && sPwd.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[DatabaseClass.DatasetUser];
                // Get the user
                Proc.AO.Definitions.UserClass c_User = Proc.AO.Definitions.UserClass.Get(call.Env, sUser);
                // Is it real?
                bool bOk = c_User.IsValid;
                // Is it valid?
                if (bOk) bOk = c_User.ValidatePassword(sPwd);
                // Passed tests?
                if (!bOk)
                {
                    // Make the filter
                    QueryClass c_Filter = new QueryClass(c_DS.DataCollection);
                    c_Filter.Add("allowed", QueryElementClass.QueryOps.Eq, "*");
                    // Get count
                    if (c_Filter.Find(1).Count == 0)
                    {
                        // Fill
                        c_User.SetPassword(sPwd);
                        c_User.Allowed = "*";
                        // Save
                        c_User.Save();

                        // Flag
                        bOk = true;
                    }
                }

                // Make return
                if (bOk)
                {
                    // Fill
                    c_User.Parent.ToStore(c_Ans);

                    // Call
                    StoreClass c_Partial = call.FN("Office.GetStartMenu", 
                                                    new StoreClass("name", c_User.Name, 
                                                                    "allowed", c_User.Allowed));

                    // And save passed
                    c_Ans.Set("commands", c_Partial["commands"]);
                    c_Ans.Set("menu", c_Partial["menu"]);
                    c_Ans.Set("datasets", c_Partial["datasets"]); 
                    c_Ans.Set("icons", c_Partial["icons"]);
                    c_Ans.Set("docs", c_Partial["docs"]);
                    c_Ans.Set("groups", c_Partial["groups"]);
                    c_Ans.Set("sio", c_Partial["sio"]);
                    c_Ans.Set("selectors", c_Partial["selectors"]);

                    // Add list
                    List<string> c_List = Enum.GetValues(typeof(AO.Definitions.DatasetFieldClass.FieldTypes))
                        .Cast<AO.Definitions.DatasetFieldClass.FieldTypes>()
                        .Select(v => v.ToString().ToLower())
                        .ToList();
                    // Remove
                    c_List.Remove("usedataset");
                    c_List.Remove("xopenmode");
                    c_List.Remove("allowed");
                    // Save
                    c_Ans.Set("fieldtypes", c_List.ToJArray());
                }

                // Cleanup
                c_User.Dispose();
            }

            return c_Ans;
        }
    }
}
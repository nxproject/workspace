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
    public class Connect : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass values)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sID = values["item"];

            // Valid?
            if (sID.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                IndexItemClass c_Item = c_Mgr.DefaultDatabase.IndexStore[sID];
                // Valid?
                if (c_Item != null)
                {
                    // Set the prefix
                    string sPrefix = null;
                    switch(c_Item.Type)
                    {
                        case "access":
                            sPrefix = "a:";
                            break;

                        case "chat":
                            sPrefix = "*";
                            break;
                    }
                    // Must be valid
                    if (sPrefix.HasValue())
                    {
                        // Grab
                        StoreClass c_Stored = c_Item.Values;

                        // Fill
                        c_Ans["_id"] = sPrefix + c_Stored["name"];
                        c_Ans["pwd"] = c_Stored["pwd"];
                        c_Ans["openmode"] = "stack";
                        c_Ans["openmodechild"] = "right";

                        c_Ans["tool"] = c_Stored["tool"];
                        c_Ans["toolparams"] = c_Stored["toolparams"];
                        
                        if(c_Stored["allowed"].HasValue())
                        {
                            // Call
                            StoreClass c_Partial = call.FN("Office.GetStartMenu",
                                                            new StoreClass("name", c_Stored["name"],
                                                                            "allowed", c_Stored["allowed"]));

                            // And save passed
                            c_Ans.Set("commands", c_Partial["commands"]);
                            c_Ans.Set("menu", c_Partial["menu"]);
                            c_Ans.Set("datasets", c_Partial["datasets"]);
                            c_Ans.Set("icons", c_Partial["icons"]);
                            c_Ans.Set("docs", c_Partial["docs"]);
                            c_Ans.Set("groups", c_Partial["groups"]);
                        }

                        // Add list
                        List<string> c_List = Enum.GetValues(typeof(AO.Definitions.DatasetFieldClass.FieldTypes))
                            .Cast<AO.Definitions.DatasetFieldClass.FieldTypes>()
                            .Select(v => v.ToString().ToLower())
                            .ToList();
                        // Remove
                        c_List.Remove("usedataset");
                        c_List.Remove("xopenmode");
                        c_List.Remove("xaccount");
                        c_List.Remove("xmdeditor");
                        c_List.Remove("xhtmleditor");
                        c_List.Remove("xemaileditor");
                        // Save
                        c_Ans.Set("fieldtypes", c_List.ToJArray());
                    }
                }
            }

            return c_Ans;
        }
    }
}
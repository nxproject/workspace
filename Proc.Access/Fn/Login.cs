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
            bool bRM = values["rm"].FromDBBoolean();

            // Is it a remembeer me token?
            if (bRM)
            {
                // Valid?
                Tuple<string, string>c_RM = call.Env.RMDecode(sUser);
                // Valid?
                if (c_RM.Item1.HasValue() && c_RM.Item2.HasValue())
                {
                    sUser = c_RM.Item1;
                    sPwd = c_RM.Item2;
                }
            }

            // Valid?
            if (sUser.HasValue() && sPwd.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[DatabaseClass.DatasetUser];
                // Get the user
                Proc.AO.Definitions.UserClass c_User = Proc.AO.Definitions.UserClass.Get(call.Env, sUser);

                // Get what we need
                string sName = c_User.Name;
                string sAllowed = c_User.Allowed;

                // Is it real?
                bool bOk = c_User.IsValid;
                // Is it valid?
                if (bOk) bOk = AO.Definitions.UserClass.ValidatePassword(c_User.Password, sPwd);

                // 
                if (!bOk)
                {
                    // Query access
                    using (QueryClass c_Qry = new QueryClass(c_Mgr.DefaultDatabase[DatabaseClass.DatasetBillAccess].DataCollection))
                    {
                        // As email
                        c_Qry.Add("name", QueryElementClass.QueryOps.Eq, sUser);
                        // Any?
                        List<AO.ObjectClass> c_Poss = c_Qry.FindObjects();
                        // Loop thru
                        foreach (AO.ObjectClass c_PO in c_Poss)
                        {
                            // Validate
                            bOk = AO.Definitions.UserClass.ValidatePassword(c_PO["pwd"], sPwd);
                            if (bOk)
                            {
                                sName = sUser.ToLower();
                                sAllowed = c_PO["allowed"];

                                // Update last login
                                c_PO["lastin"] = DateTime.Now.ToDBDate();
                                c_PO.Save();
                                break;
                            }
                        }

                        // If not as phone
                        if (!bOk)
                        {
                            // Clear
                            c_Qry.Reset();

                            // Format as phone
                            string sPhone = sUser.ToPhone();

                            // Any?
                            if (sPhone.HasValue())
                            {
                                // Add
                                ;
                                c_Qry.Add("name", QueryElementClass.QueryOps.Eq, sPhone);
                                // Any?
                                c_Poss = c_Qry.FindObjects();
                                // Loop thru
                                foreach (AO.ObjectClass c_PO in c_Poss)
                                {
                                    // Validate
                                    bOk = AO.Definitions.UserClass.ValidatePassword(c_PO["pwd"], sPwd);
                                    if (bOk)
                                    {
                                        sName = sPhone;
                                        sAllowed = c_PO["allowed"];
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    c_User["name"] = sName;

                    // Defualt?
                    if (bOk && !sAllowed.HasValue())
                    {
                        // Get from site
                        sAllowed = c_Mgr.DefaultDatabase.SiteInfo.AccountDefaultAllowed.IfEmpty("?ACCT *:-");
                    }
                }

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
                    // Delete temp files
                    NX.Engine.Files.ManagerClass c_DocMgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();
                    using(NX.Engine.Files.FolderClass c_Folder = new NX.Engine.Files.FolderClass(c_DocMgr, AO.ExtendedUserClass.UserFolder.CombinePath(sName)))
                    {
                        c_Folder.Delete();
                    }

                    // Fill
                    c_User.Parent.ToStore(c_Ans);

                    // Call
                    StoreClass c_Partial = call.FN("Office.GetStartMenu",
                                                    new StoreClass("name", sName,
                                                                    "allowed", sAllowed));

                    // And save passed
                    c_Ans.Set("commands", c_Partial["commands"]);
                    c_Ans.Set("menu", c_Partial["menu"]);
                    c_Ans.Set("datasets", c_Partial["datasets"]);
                    c_Ans.Set("icons", c_Partial["icons"]);
                    c_Ans.Set("docs", c_Partial["docs"]);
                    c_Ans.Set("groups", c_Partial["groups"]);
                    c_Ans.Set("sio", c_Partial["sio"]);
                    c_Ans.Set("selectors", c_Partial["selectors"]);

                    c_Ans.Set("sioid", sUser);

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
                    c_List.Remove("allowed");
                    // Save
                    c_Ans.Set("fieldtypes", c_List.ToJArray());

                    // Remember me?
                    if (bRM)
                    {
                        c_Ans.Set("_rm", call.Env.RMEncode(sUser, sPwd));
                    }

                }

                // Cleanup
                c_User.Dispose();
            }

            return c_Ans;
        }
    }
}
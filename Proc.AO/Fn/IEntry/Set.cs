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

using System;
using System.Collections.Generic;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Sets an index entry
    /// 
    /// </summary>
    public class IEntrySet : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            string sOwner = "".GUID();
            if(call.UserInfo != null)
            {
                if(call.UserInfo.Name.HasValue())
                {
                    sOwner = call.UserInfo.Name;
                }
            }

            // Get the name
            string sID = store["name"].AsFieldName();

            // Get the user dataset
            IndexItemClass c_Item = null;
            if (sID.HasValue())
            {
                c_Item = c_Mgr.DefaultDatabase.IndexStore[sID];
            }
            else
            {
                c_Item = c_Mgr.DefaultDatabase.IndexStore.Next(sOwner);
            }

            // Valid?
            if (c_Item != null)
            {
                // Save
                c_Item.Type = store["type"];
                c_Item.Expiration = DateTime.Now.AddMinutes(store["limit"].ToDouble(10));
                c_Item.Uses = store["uses"].ToInteger(1);
                c_Item.Values = new StoreClass(store.GetAsJObject("value"));

                c_Item.Save();

                c_Ans["id"] = c_Item.ID;
            }

            return c_Ans;
        }
    }
}
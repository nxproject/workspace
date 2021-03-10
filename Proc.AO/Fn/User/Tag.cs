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

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Sets/clear tag
    /// 
    /// </summary>
    public class Tag : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sUser = store["user"];
            string sType = store["type"];
            string sDS = store["ds"];
            string sID = store["id"];
            string sAction = store["action"];
            string sLoc = store["geo"];

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            // Special actions
            switch (sAction)
            {
                case "eod":
                    // Get active tags
                    List<TagClass> c_Active = c_Mgr.DefaultDatabase.Tagged.ActiveTags(sUser, "pin");
                    // Loop thru
                    foreach(TagClass c_TagX in c_Active)
                    {
                        c_TagX.Freeze("Freeze all", sLoc);
                    }
                    break;

                default:
                    // And the object
                    UUIDClass c_UUID = new UUIDClass(c_Mgr.DefaultDatabase, sDS, sID);
                    // Get the tag
                    TagClass c_Tag = c_Mgr.DefaultDatabase.Tagged[sUser, sType, c_UUID];

                    // If new, start
                    if (c_Tag.IsNew) sAction = "new";

                    // By the action
                    switch (sAction)
                    {
                        case "new":
                            c_Ans["value"] = c_Tag.Start(sLoc, true);
                            break;

                        case "start":
                            c_Ans["value"] = c_Tag.Start(sLoc);
                            break;

                        case "startf":
                            c_Ans["value"] = c_Tag.Start(sLoc, false, true);
                            break;

                        case "show":
                            c_Ans["value"] = c_Tag.Explain;
                            break;

                        case "stop":
                            c_Ans["value"] = c_Tag.End(sLoc);
                            break;

                        case "freeze":
                            if (c_Tag.IsEnded)
                            {
                                c_Ans["value"] = "has ended";
                            }
                            else if (c_Tag.IsFrozen)
                            {
                                c_Ans["value"] = "already frozen";
                            }
                            else
                            {
                                c_Ans["value"] = c_Tag.Freeze(store["reason"], sLoc);
                            }
                            break;

                        case "continue":
                            if (!c_Tag.IsFrozen)
                            {
                                if (c_Tag.Status == TagClass.Statuses.Active)
                                {
                                    c_Ans["value"] = "already active";
                                }
                                else
                                {
                                    c_Ans["value"] = c_Tag.Start(sLoc);
                                }
                            }
                            else
                            {
                                c_Ans["value"] = c_Tag.Unfreeze(sLoc);
                            }
                            break;
                    }

                    //
                    c_Ans["list"] = c_Tag.SegmentsAsJArray.ToSimpleString();
                    break;
            }

            return c_Ans;
        }
    }
}
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

using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Sets an object
    /// 
    /// </summary>
    public class ObjectSet : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Get the params
            string sDS = store["ds"].AsDatasetName();
            string sID = store["id"];
            JObject c_Data = store.GetAsJObject("data");
            bool IsEmailTemplate = store["cleanEmail"].IfEmpty().FromDBBoolean();

            // Valid?
            if (sDS.HasValue() && sID.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                // Get the object
                using (Proc.AO.ObjectClass c_Obj = c_DS[sID])
                {
                    // Any?
                    if (c_Data != null)
                    {
                        // Get the original
                        JObject c_Orig = c_Obj.AsJObject;

                        // Loop thru
                        foreach (string sField in c_Data.Keys())
                        {
                            string sValue = c_Data.Get(sField);

                            //
                            if (IsEmailTemplate)
                            {
                                // Get the wrappers
                                MatchCollection c_Matches = Regex.Matches(sValue, @"\x7B\x7B[^\x3C\x7B\x7D]*\x3C[^\x7D]*\x3E\x7D\x7D");
                                // Loop thru
                                foreach(Match c_Match in c_Matches)
                                {
                                    // Get the value
                                    string sWPatt = c_Match.Value;
                                    // Get the field
                                    Match c_Field = Regex.Match(sWPatt, @"\x3E[^\x3C]+\x3C");
                                    if (c_Field.Success)
                                    {
                                        // Get the field
                                        string sFPatt = c_Field.Value;
                                        // Make replacement
                                        string sRepl = sWPatt.Substring(2, sWPatt.Length - 4);
                                        // And field
                                        string sFRepl = ">{{" + sFPatt.Substring(1, sFPatt.Length - 2) + "}}<";
                                        // Replace field
                                        sRepl = sRepl.Replace(sFPatt, sFRepl);
                                        // Replace
                                        sValue = sValue.Replace(sWPatt, sRepl);
                                    }
                                }
                            }

                            c_Obj[sField] = sValue;
                        }

                        // Save
                        store["ok"] = c_Obj.Save(force: !c_Obj.Collection.IsData, user: store["_user"], runtask: true, orig: c_Orig).ToDBBoolean();
                    }
                }
            }

            return store;
        }
    }
}
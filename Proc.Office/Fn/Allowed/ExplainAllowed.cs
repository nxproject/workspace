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
using System.Text;

using NX.Engine;
using NX.Shared;

using Proc.AO;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Explains an allowed definition
    /// 
    /// </summary>
    public class ExplainAllowed : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            // Make the processor
            using (AO.Definitions.UserPrivilegesClass c_Proc = new AO.Definitions.UserPrivilegesClass(store["allowed"],
                                                                            c_Mgr.DefaultDatabase[DatabaseClass.DatasetAllowed],
                                                                            c_Mgr.DefaultDatabase.Datasets))
            {
                // Get
                string sExpanded = c_Proc.Process(c_Mgr.DefaultDatabase.Datasets);
                // Parse
                ItemsClass c_Pieces = (AO.Definitions.UserPrivilegesClass.Parse(sExpanded));
                // Holding area
                StringBuilder c_Buffer = new StringBuilder();


                List<string> c_GTools = new List<string>();
                List<string> c_GSelectors = new List<string>();

                // Loop thru
                for (int i = 0; i < c_Pieces.Count; i++)
                {
                    // Get tthe piece
                    ItemClass c_Piece = c_Pieces[i];

                    string sView = null;
                    List<string> c_Groups = new List<string>();
                    List<string> c_Tools = new List<string>();

                    foreach (ItemOptionClass c_Option in c_Piece.Options)
                    {
                        // According to option
                        switch (c_Option.Option)
                        {
                            case "@":
                                sView = c_Option.Value;
                                break;

                            case "%":
                                c_Groups.Add(c_Option.Value);
                                break;

                            case "$":
                                if (c_Piece.Key.HasValue())
                                {
                                    c_Tools.Add(c_Option.Value);
                                }
                                else
                                {
                                    c_GTools.Add(c_Option.Value);
                                }
                                break;

                            case "?":
                                if (!c_Piece.Key.HasValue())
                                {
                                    c_GSelectors.Add(c_Option.Value);
                                }
                                break;
                        }
                    }

                    // Only datasets
                    if (c_Piece.Key.HasValue())
                    {
                        // Add
                        c_Buffer.AppendLine("Dataset: {0}".FormatString(c_Piece.Key));

                        List<string> c_Privs = new List<string>();
                        if (c_Piece.Value.Contains("a")) c_Privs.Add("Add");
                        if (c_Piece.Value.Contains("v")) c_Privs.Add("View");
                        if (c_Piece.Value.Contains("x")) c_Privs.Add("Delete");
                        if (c_Piece.Value.Contains("d")) c_Privs.Add("Documents/Merge");
                        if (c_Piece.Value.Contains("c")) c_Privs.Add("Calendar");
                        if (c_Piece.Value.Contains("t")) c_Privs.Add("Tasks");
                        if (c_Piece.Value.Contains("r")) c_Privs.Add("Reports");
                        if (c_Piece.Value.Contains("w")) c_Privs.Add("Workflows");

                        if (c_Piece.Value.Contains("y")) c_Privs.Add("Disallow dataset");
                        if (c_Piece.Value.Contains("z")) c_Privs.Add("Force dataset in Start menu");
                        if (c_Privs.Count > 0)
                        {
                            c_Buffer.AppendLine("  Privileges: {0}".FormatString(c_Privs.Join(", ")));
                        }

                        //
                        c_Buffer.AppendLine("  View: {0}".FormatString(sView));
                        if (c_Groups.Count > 0)
                        {
                            c_Buffer.AppendLine("  Groups: {0}".FormatString(c_Groups.Join(", ")));
                        }
                        if (c_Tools.Count > 0)
                        {
                            c_Buffer.AppendLine("  Tools: {0}".FormatString(c_Tools.Join(", ")));
                        }

                        c_Buffer.AppendLine("");
                    }
                    else
                    {
                        
                    }
                }

                if (c_GSelectors.Count > 0)
                {
                    c_Buffer.AppendLine("  Selectors: {0}".FormatString(c_GSelectors.Join(", ")));

                    foreach(string sSel in c_GSelectors)
                    {
                        List<string> c_SDS = new List<string>();
                        foreach(string sWDS in c_Mgr.DefaultDatabase.Datasets)
                        {
                            AO.Definitions.DatasetClass c_Def = c_Mgr.DefaultDatabase[sWDS].Definition;
                            if(c_Def.Selector.IsSameValue(sSel))
                            {
                                c_SDS.Add(sWDS + ":" + c_Def.Caption);
                            }
                        }
                        c_Buffer.Append("     For {0}: {1}".FormatString(sSel, c_SDS.Join(", ")));
                    }
                }

                if (c_GTools.Count > 0)
                {
                    c_Buffer.AppendLine("  Global Tools: {0}".FormatString(c_GTools.Join(", ")));
                }

                c_Ans["explanation"] = c_Buffer.ToString();
            }

            return c_Ans;
        }
    }
}
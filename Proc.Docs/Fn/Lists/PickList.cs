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
using NX.Engine.Files;
using NX.Shared;
using Proc.AO;

namespace Proc.Docs
{
    /// <summary>
    /// 
    /// Gets an object
    /// 
    /// </summary>
    public class DocumentPickList : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sDS = store["ds"].AsDatasetName();
            string sID = store["id"];

            // Valid?
            if (sDS.HasValue() && sID.HasValue())
            {
                // Get the manager
                Proc.AO.ManagerClass c_Mgr = call.Env.Globals.Get<Proc.AO.ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                // Get the object
                using (Proc.AO.ObjectClass c_Obj = c_DS[sID])
                {
                    // Make the list
                    JArray c_List = new JArray();

                    // Folders
                    foreach(FolderClass c_Folder in c_Obj.Folder.Folders )
                    {
                        if (!c_Folder.Name.StartsWith("_") && !c_Folder.Name.IsSameValue("Backups"))
                        {
                            JObject c_Entry = new JObject();

                            c_Entry.Set("label", c_Folder.Name);
                            c_Entry.Set("path", c_Folder.Path + "/");
                            c_Entry.Set("icon", "folder");

                            c_List.Add(c_Entry);
                        }
                    }

                    // Files
                    foreach(DocumentClass c_Doc in c_Obj.Folder.Files)
                    {
                        JObject c_Entry = new JObject();

                        c_Entry.Set("label", c_Doc.NameOnly);
                        c_Entry.Set("path", c_Doc.Path);
                        c_Entry.Set("icon", "page");

                        c_List.Add(c_Entry);
                    }

                    // Save the list
                    c_Ans.Set("list", c_List);
                }
            }

            return c_Ans;
        }
    }
}
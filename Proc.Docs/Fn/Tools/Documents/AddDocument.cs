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

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.Docs.Files;
using Proc.AO;

namespace Proc.Docs
{
    /// <summary>
    /// 
    /// Adds a document
    /// 
    /// </summary>
    public class TDAddDocument : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass values)
        {
            // Make response
            StoreClass c_Ans = new StoreClass();

            // Get params
            string sDS = values["ds"].AsDatasetName();
            string sID = values["id"];
            string sName = values["name"];
            string sTemplate = values["template"];
            string sFolder = values["folder"];
            StoreClass c_Passed = values.GetAsStore("data");
            bool bNoMerge = sID.IsSameValue("templates");

            // Valid?
            if (sName.HasValue())
            {
                // Get the manager
                NX.Engine.Files.ManagerClass c_Mgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

                // Process template
                if (sTemplate.HasValue())
                {
                    // Get the folder
                    using (FolderClass c_Folder = new FolderClass(c_Mgr, sFolder))
                    {
                        //
                        DocumentClass c_Template = null;
                        bool bIsTemp = false;

                        // System?
                        if (sTemplate.StartsWith("!"))
                        {
                            // The system document
                            string sQXDir = "".WorkingDirectory().CombinePath("ui.qx").CombinePath("docs").CombinePath(sTemplate.Substring(1) + ".odt");
                            // A temp
                            c_Template = new DocumentClass(c_Mgr, c_Folder.SubFolder("_merge"), "F" + sQXDir.MD5HashString() + ".odt");
                            // Copy
                            c_Template.ValueAsBytes = sQXDir.ReadFileAsBytes();
                            // Remember to delete
                            bIsTemp = true;
                        }
                        else
                        {
                            c_Template = new DocumentClass(c_Mgr, "/" + sTemplate);
                        }

                        // Make target
                        using (DocumentClass c_Target = new DocumentClass(c_Mgr, c_Folder, sName + ".odt"))
                        {
                            // Merge?
                            if (bNoMerge)
                            {
                                // Copy
                                c_Template.CopyTo(c_Target, true);
                            }
                            else
                            {
                                // Make the context
                                using (ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, c_Passed,  null, call.UserInfo.Name))
                                {
                                    // Merge
                                    c_Template.Merge(c_Target, c_Template.MergeMap().Eval(c_Ctx));
                                }
                            }

                            // Pass back
                            c_Ans["path"] = c_Target.Path;
                        }

                        // Delete
                        if (bIsTemp) c_Template.Delete();
                    }
                }
            }

            return c_Ans;
        }
    }
}
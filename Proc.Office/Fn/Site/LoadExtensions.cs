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

using NX.Engine;
using NX.Shared;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Load extension files
    /// 
    /// </summary>
    public class LoadExtensions : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass values)
        {
            // Setup the return
            StoreClass c_Ans = new StoreClass();

            // Get the manager
            Proc.AO.ManagerClass c_Mgr = call.Env.Globals.Get<Proc.AO.ManagerClass>();

            // Get the site info
            //Proc.AO.SiteInfoClass c_Info = c_Mgr.DefaultDatabase.SystemDataset.SiteInfo;

            // Setup
            string sSource = call.Env.DynamicFolder;
            string sTarget = "".WorkingDirectory();
            // Any?
            if (sSource.HasValue())
            {
                call.Env.LogInfo("Loading extensionsfrom {0} to {1}".FormatString(sSource, sTarget));

                List<string> c_Procs = new List<string>();

                // Load
                this.LoadFolder(sSource, sTarget, c_Procs, true);

                // Now use the procs
                foreach(string sProc in c_Procs)
                {
                    call.Env.Use(sProc);
                }
            }

            // And return
            return c_Ans;
        }

        #region Methods
        private void LoadFolder(string source, string target, List<string> procs, bool atroot)
        {
            // Get the file list
            List<string> c_Files = source.GetFilesInPath();
            // Loop thru
            foreach(string sFile in c_Files)
            {
                // Get the name
                string sName = sFile.GetFileNameFromPath();
                // Copy
                sFile.CopyFile(target.CombinePath(sName));
                // Proc and root?
                if(atroot && sName.StartsWith("Proc."))
                {
                    // Add to list
                    procs.Add(sName.GetFileNameOnlyFromPath());
                }
            }

            // Get subfolders
            List<string> c_Folders = source.GetDirectoriesInPath();
            // Loop thru
            foreach(string sFolder in c_Folders)
            {
                // Get the name
                string sName = sFolder.GetDirectoryNameFromPath();
                // Copy
                this.LoadFolder(sFolder, target.CombinePath(sName), procs, false);
            }
        }
        #endregion
    }
}
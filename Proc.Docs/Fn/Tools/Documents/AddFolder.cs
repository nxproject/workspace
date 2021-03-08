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
using Proc.AO;

namespace Proc.Docs
{
    /// <summary>
    /// 
    /// Adds a document
    /// 
    /// </summary>
    public class TDAddFolder : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass values)
        {
            // Make response
            StoreClass c_Ans = new StoreClass();

            // Get params
            string sName = values["name"];
            string sFolder = values["folder"];

            // Valid?
            if (sName.HasValue())
            {
                // Get the manager
                NX.Engine.Files.ManagerClass c_Mgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

                // Map
                using(FolderClass c_Folder = new FolderClass(c_Mgr, sFolder))
                {
                    // Make new
                    c_Folder.SubFolder(sName).AssurePath();
                }
            }

            return c_Ans;
        }

    }
}
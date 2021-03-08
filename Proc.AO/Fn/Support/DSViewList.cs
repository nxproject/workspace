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

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Gets a map of the datasets and views
    /// 
    /// </summary>
    public class DSViewList : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            // Make array
            JObject c_Menu = new JObject();

            // Loop thru
            foreach(string sDS in c_Mgr.DefaultDatabase.Datasets)
            {
                // Get dataset
                Definitions.DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS].Definition;

                // Entry
                JObject c_Entry = new JObject();

                // Icon
                c_Entry.Set("icon", c_DS.Icon);
                c_Entry.Set("caption", c_DS.Caption);
                c_Entry.Set("views", c_Mgr.DefaultDatabase[sDS].Views.ToJArray());

                // Make entry
                c_Menu.Set(sDS, c_Entry);
            }

            // Save the list
            c_Ans.Set("list", c_Menu);

            return c_Ans;
        }
    }
}
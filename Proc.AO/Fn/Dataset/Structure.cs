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
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Gets a dataset structure
    /// 
    /// </summary>
    public class DatasetStructure : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sDS = store["ds"].AsDatasetName();

            // Valid?
            if (sDS.HasValue())
            {
                // Return
                JObject c_Tree = new JObject();

                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                // Add the dataset
                c_Ans.Set("ds", c_DS.Definition.Object.AsJObject);

                // Get the views
                List<string> c_Views = c_DS.Views;
                // Loop thru
                foreach (string sView in c_Views)
                {
                    // Get
                    Definitions.ViewClass c_View = c_DS.View(sView);
                    // Save
                    c_Tree.Set(sView, c_View.Object.AsJObject);
                }
                // Save
                c_Ans.Set("views", c_Tree);
            }

            return c_Ans;
        }
    }
}
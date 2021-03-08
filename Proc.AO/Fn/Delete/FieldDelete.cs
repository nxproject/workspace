﻿///--------------------------------------------------------------------------------
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

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Deletes a view
    /// 
    /// </summary>
    public class FieldDelete : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Get the params
            string sDS = store["ds"].AsDatasetName();
            string sView = store["view"].AsViewName();
            string sField = store["field"].AsFieldName();
            bool bViewOnly = store["viewonly"].FromDBBoolean();

            // Valid?
            if (sDS.HasValue() && sView.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                // Get the view
                Definitions.ViewClass c_View = c_DS.View(sView);
                // Delete
                c_View.Remove(sField);
                // Save
                c_View.Save();

                // View only?
                if(!bViewOnly)
                {
                    // Map
                    Definitions.DatasetClass c_DSX = c_DS.Definition;
                    // Delete
                    c_DSX.RemoveField(sField);
                    // Save
                    c_DSX.Save();
                }
            }

            return store;
        }
    }
}
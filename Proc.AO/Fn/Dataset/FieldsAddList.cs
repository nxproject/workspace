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

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Gets an object
    /// 
    /// </summary>
    public class FieldAddList : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sList = store["list"];
            string sDS = store["ds"].AsDatasetName();
            string sView = store["view"].AsViewName();

            // Valid?
            if (sList.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the dataset
                Definitions.DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS].Definition;

                Definitions.ViewClass c_View = null;
                // Do we have a view?
                if (sView.HasValue())
                {
                    c_View = c_DS.Parent.View(sView);
                }

                // Split
                List<string> c_List = sList.SplitSpaces();

                // Return
                c_Ans.Set("fields", c_List.ToJArray());

                // Loop thru
                foreach (string sField in c_List)
                {
                    // Convert to proper name
                    string sProper = sField.AsFieldName();

                    //  
                    Definitions.DatasetFieldClass c_Field = c_DS[sProper];
                    c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;

                    //// Do we have a view?
                    //if (c_View != null)
                    //{
                    //    // Create the field
                    //    Definitions.ViewFieldClass c_VField = c_View[sProper];
                    //    // Really new?
                    //    if(c_VField.IsNew)
                    //    {
                    //        c_VField.Label = WesternNameClass.CapEachWord(sProper);
                    //        c_VField.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                    //    }
                    //}
                }

                // Save
                c_DS.Save();
                if (c_View != null) c_View.Save();
            }

            return c_Ans;
        }
    }
}
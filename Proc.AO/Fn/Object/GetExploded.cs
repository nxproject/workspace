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
    public class ObjectGetExploded : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sDS = store["ds"].AsDatasetName();
            string sID = store["id"];
            bool bFloatAccount = store["floataccount"].FromDBBoolean();

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
                    // Hadle the account bit
                    if (bFloatAccount) c_Obj.FloatAccount();

                    // Make the stack
                    ExplodeStackClass c_Stack = new ExplodeStackClass(c_Mgr.DefaultDatabase);
                    // Explode the object
                    c_Stack.Add(ExplodeStackClass.ExplodeModes.UpDown, c_Obj);
                    // Copy all fields
                    c_Ans = new StoreClass(c_Stack.Result);
                }
            }

            return c_Ans;
        }
    }
}
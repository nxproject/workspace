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

namespace Proc.IOTIF
{
    public class ManagerClass : ExtManagerClass<IOTRequestClass>
    {
        #region Constructor
        /// <summary>
        /// 
        /// Constructor
        /// 
        /// </summary>
        /// <param name="env">The current environment</param>
        public ManagerClass(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Returns the process code for a given name
        /// 
        /// </summary>
        /// <param name="name">The function name</param>
        /// <returns>The function code (if any)</returns>
        public IOTRequestClass GetCommand(string name)
        {
            return (IOTRequestClass)this.Get(name);
        }

        /// <summary>
        /// 
        /// Lookup processor
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public StoreClass Lookup(IOTContextClass ctx, string ds)
        {
            StoreClass c_Ans = new StoreClass();

            if (ctx.Option.HasValue())
            {
                // Get the entry
                AO.ObjectClass c_Entry = ctx.DBManager.DefaultDatabase[ds][ctx.Option];
                // Return the code
                c_Ans[ctx.Code] = c_Entry["code"];
            }
            
            // 
            return c_Ans;
        }
        #endregion
    }
}
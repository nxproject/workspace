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

using System.Text;

using NX.Engine;
using NX.Shared;

namespace Proc.IOTIF
{
    /// <summary>
    /// 
    /// Default setup
    /// 
    /// </summary>
    public class Init : FNClass
    {
        public override void Initialize(EnvironmentClass env)
        {

            // Load manager
            ManagerClass c_Mgr = env.Globals.Get<ManagerClass>();

            c_Mgr.SelfLoad(env);

            // Load datasets
            AO.ManagerClass c_DBMgr = env.Globals.Get<AO.ManagerClass>();

            if (c_DBMgr.DefaultDatabase.SiteInfo.IOTEnabled)
            {
                var x = c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetIOTAgent];
                x = c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetIOTClient];
                x = c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetIOTUnit];
                x = c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetIOTKeyboard];
                x = c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetIOTVerb];
                x = c_DBMgr.DefaultDatabase[AO.DatabaseClass.DatasetIOTMacro];
            }

            base.Initialize(env);
        }
    }
}
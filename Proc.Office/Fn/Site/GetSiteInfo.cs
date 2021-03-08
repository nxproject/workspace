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
using NX.Shared;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Returns the site information
    /// 
    /// </summary>
    public class GetSiteInfo : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass values)
        {
            // Get the manager
            Proc.AO.ManagerClass c_Mgr = call.Env.Globals.Get<Proc.AO.ManagerClass>();

            // Get the site info
            Proc.AO.SiteInfoClass c_Info = c_Mgr.DefaultDatabase.SiteInfo;
            // And return
            return new StoreClass(c_Info.AsJObject);
        }
    }
}
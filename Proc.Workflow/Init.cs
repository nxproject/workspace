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

namespace Proc.Workflow
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
            base.Initialize(env);

            // Load manager
            ManagerClass c_Mgr = env.Globals.Get<ManagerClass>();

            // Load comamnds
            c_Mgr.SelfLoad(env);

            // Elsa
            if (env["document"].FromDBBoolean())
            {
                // Write MD
                c_Mgr.GenerateMD(@"C:\Candid Concepts\NX\Office\UI.QX\help\info.\README_D_WF.md");
                // Write Elsa file
                c_Mgr.GenerateElsa(@"C:\Candid Concepts\NX\Others\Elsa\src\plugins\wf-activities.ts");
            }
        }

        #region Statics
        public static string MakeObjField(string obj, string value)
        {
            value = value.IfEmpty();
            if (obj.HasValue()) value = obj + ":" + value;

            if (!value.StartsWith("[") && !value.EndsWith("]")) value = "[" + value + "]";
            return value;
        }

        public static string MakeStoreField(string store, string value)
        {
            value = value.IfEmpty();
            if (store.HasValue()) value = store + ":" + value;

            if (!value.StartsWith("[*") && !value.EndsWith("]")) value = "[*" + value + "]";
            return value;
        }
        #endregion
    }
}
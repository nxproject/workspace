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
    /// Default setup
    /// 
    /// </summary>
    public class Init : FNClass
    {
        public override void Initialize(EnvironmentClass env)
        {
            // Set the environment
            env["ui"] = "qx";

            // Load Socket.IO support
            env.Use("Proc.SIO");
            // Load the database
            env.Use("Proc.AO");
            // Access logic
            env.Use("Proc.Access");
            // And extended documents
            //"System.Text.Encoding.CodePages.dll".LoadAssembly(env);
            env.Use("Proc.Docs");

            // Help
            env.Use("Markdig");
            env.Use("Proc.Help");

            // Common
            env.Use("Common.TaskWF");
            env.Use("Proc.Comm");
            // And tasks
            env.Use("Proc.Task");
            // And workflows
            env.Use("Proc.Workflow");

            // Call bootstrap
            env.FN("Office.LoadExtensions");

            // Documentation
            if (env["document"].FromDBBoolean())
            {
                // Write the functions documentation
                env.FNS.GenerateMD(@"C:\Candid Concepts\NX\Office\UI.QX\help\info.\README_FNS.md");

                // Write the eval functions documentation
                FunctionsDefinitions c_Defs = Context.FunctionsTable;
                c_Defs.GenerateMD(@"C:\Candid Concepts\NX\Office\UI.QX\help\info.\README_LE.md");
            }

            base.Initialize(env);
        }
    }
}
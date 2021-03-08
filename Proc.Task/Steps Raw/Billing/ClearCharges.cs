///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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

using NX.Shared;
using NX.Engine;

using Newtonsoft.Json.Linq;

namespace Proc.Task
{
    public class ClearCharges : CommandClass
    {
        #region Constants
        private const string ArgBlock = "block";
        #endregion

        #region Constructor
        public ClearCharges(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParameterDefinitionClass> c_P = new NamedListClass<ParameterDefinitionClass>();

                c_P.Add(ArgBlock, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The block of charges"));

                return new DescriptionClass(CategoriesClass.Array, "Clears the stored charges", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "charges.clear"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;
            
            // Get the parameters
            string sBlock = args.GetDefined(ArgBlock);

            // Do
            ctx.Charges[sBlock].Clear();

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
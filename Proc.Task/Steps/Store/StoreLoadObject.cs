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

using NX.Shared;
using NX.Engine;
using Common.TaskWF;

namespace Proc.Task
{
    public class StoreLoadObject : CommandClass
    {
        #region Constants
        private const string ArgStore = "store";
        private const string ArgObj = "obj";
        #endregion

        #region Constructor
        public StoreLoadObject()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The store to load"));
                c_P.Add(ArgObj, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The object"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Store, "Loads a store from object", c_P);
            } 
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "store.load.object"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sStore = args.Get(ArgStore);
            string sObj = args.Get(ArgObj);

            // Get
            StoreClass c_Store = ctx.Stores[sStore];
            AO.ObjectClass c_Obj = ctx.Objects[sObj];
            if(c_Store != null && c_Obj != null)
            {
                // Get list
                List<string> c_Flds = c_Obj.Fields;
                // Loop thru
                foreach(string sFld in c_Flds)
                {
                    // Save
                    c_Store[sFld] = c_Obj[sFld];
                }
            }

            return eAns;
        }
        #endregion
    }
}
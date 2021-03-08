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

using System.Collections.Generic;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class DDRemove : CommandClass
    {
        #region Constants
        private const string ArgObj = "obj";
        private const string ArgID = "id";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DDRemove(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParameterDefinitionClass> c_P = new NamedListClass<ParameterDefinitionClass>();

                c_P.Add(ArgObj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "An object"));
                c_P.Add(ArgID, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The ID of the dingdong"));

                return new DescriptionClass(CategoriesClass.DingDong, "Deletes a dingdong for an object", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "dingdong.remove"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            string sObj = args.GetDefined(ArgObj);
            string sID = args.GetDefined(ArgID);

            // Do
            MM.AO.ObjectClass c_Obj = ctx.Objects[sObj];
            if (c_Obj != null)
            {
                // Make the query
                MM.AO.XQueryClass c_Qry = new MM.AO.XQueryClass();
                MM.AO.DingDongClass.BelongsTo(c_Qry, c_Obj.UUID);
                MM.AO.DingDongClass.ReferredAs(c_Qry, sID);

                // Get all the items
                List<MM.AO.UUIDClass> c_List = MM.AO.DingDongClass.Query(ctx.User, c_Qry);
                // Do
                foreach (MM.AO.UUIDClass c_UUID in c_List)
                {
                    MM.AO.ObjectClass c_WObj = ctx.User.FetchObject(c_UUID, MM.AO.ObjectClass.Types.Raw);

                    c_WObj.Delete();

                    ctx.User.FreeObject(c_WObj);
                }
            }

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
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

using MongoDB.Bson;
using MongoDB.Driver;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class DDClear : CommandClass
    {
        #region Constants
        private const string ArgObj = "obj";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DDClear(EnvironmentClass env)
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

                c_P.Add(ArgObj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "An object to clear"));

                return new DescriptionClass(CategoriesClass.DingDong, "Deletes all dingdongs for an object", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "dingdong.clear"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            string sObj = args.GetDefined(ArgObj);

            // Do
            AO.ObjectClass c_Obj = ctx.Objects[sObj];
            if(c_Obj != null)
            {
                // Make the query
                AO.QueryClass c_Qry = new AO.QueryClass(ctx.DBManager.DefaultDatabase.DingDongs.Dataset.Collection);
                AO.DingDongClass.BelongsTo(c_Qry, c_Obj.UUID);

                // Get all the items
                List<BsonDocument> c_List = ctx.DBManager.DefaultDatabase.DingDongs.Dataset.Collection.Find(c_Qry);
                // Do
                foreach (BsonDocument c_Doc in c_List)
                {
                    using (AO.ObjectClass c_WObj = new AO.ObjectClass(c_Doc))
                    {
                        c_WObj.Delete();
                    }
                }
            }

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
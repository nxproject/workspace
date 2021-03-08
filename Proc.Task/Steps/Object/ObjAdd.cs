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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class ObjAdd : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgID = "id";
        private const string ArgName = "obj";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjAdd()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The name of the object"));
                c_P.Add(ArgDS, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The dataset of the object"));
                c_P.Add(ArgID, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The id of the object"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, "Adds an object to the working stack", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command => "obj.add";

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sDS = args.Get(ArgDS);
            string sID = args.Get(ArgID);
            string sName = args.Get(ArgName);

            // Adjust for required
            if (!sDS.HasValue() && sID.HasValue())
            {
                if (AO.UUIDClass.IsValid(sID))
                {
                    AO.UUIDClass c_UUID = new AO.UUIDClass(ctx.Database, sID);

                    sDS = c_UUID.Dataset.Name;
                    sID = c_UUID.ID;
                }
            }

            // Add
            ctx.Objects[sName] = ctx.Database[sDS][sID];

            return eAns;
        }
        #endregion
    }
}
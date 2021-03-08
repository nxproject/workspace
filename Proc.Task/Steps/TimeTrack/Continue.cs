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
    public class TTContinue : CommandClass
    {
        #region Constants
        private const string ArgName = "obj";
        private const string ArgType = "type";
        private const string ArgUser = "user";
        private const string ArgReason = "reason";
        #endregion

        #region Constructor
        public TTContinue()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the object"));
                c_P.Add(ArgType, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The type of tag"));
                c_P.Add(ArgUser, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The user"));
                c_P.Add(ArgReason, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The reason of the freeze"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.TimeTrack, "Continues time tracking", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "tt.continue"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sObj = args.Get(ArgName);
            string sType = args.Get(ArgType).IfEmpty("pin");
            string sUser = args.Get(ArgUser).IfEmpty(ctx.User.Name);

            // Get the object
            AO.ObjectClass c_Obj = ctx.Objects[sObj];
            if (c_Obj != null)
            {
                // Get the tag
                TagClass c_Tag = ctx.DBManager.DefaultDatabase.Tagged[sUser, sType, c_Obj.UUID];

                if (!c_Tag.IsFrozen)
                { }
                else
                {
                    c_Tag.Unfreeze();

                    // Tell user
                    ctx.SIO("$$changed.userprofile", "id", ctx.User.Name);
                }
            }

            return eAns;
        }
        #endregion
    }
}
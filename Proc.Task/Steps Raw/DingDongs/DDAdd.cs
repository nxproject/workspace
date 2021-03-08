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

using System;
using System.Collections.Generic;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class DDAdd : CommandClass
    {
        #region Constants
        private const string ArgObj = "obj";
        private const string ArgID = "id";
        private const string ArgOn = "on";
        private const string ArgMsg = "msg";
        private const string ArgVia = "via";
        private const string ArgSubj = "subj";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DDAdd(EnvironmentClass env)
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
                c_P.Add(ArgOn, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "The date and time when to send"));
                c_P.Add(ArgSubj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The subject of the message (email only)"));
                c_P.Add(ArgMsg, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The message to be sent"));
                c_P.Add(ArgVia, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The contact method (SMS number/EMail address)"));

                return new DescriptionClass(CategoriesClass.DingDong, "Adds a dingdong for an object", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "dingdong.add"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            string sObj = args.GetDefined(ArgObj);
            string sID = args.GetDefined(ArgID);
            string sOn = args.GetDefined(ArgOn);
            string sMsg = args.GetDefined(ArgMsg);
            string sVia = args.GetDefined(ArgVia);
            string sSubj = args.GetDefined(ArgSubj);

            DateTime c_On = sOn.ToDateTime(ctx.Now());
            if (sVia.IsEMailAddress() || sVia.IsFormattedPhone())
            {
                // Do
                AO.ObjectClass c_Obj = ctx.Objects[sObj];
                if (c_Obj != null)
                {
                    // Create the object
                    AO.DingDongClass c_New = ctx.DBManager.DefaultDatabase.DingDongs.New();

                    // Fill
                    c_New.On = c_On;
                    c_New.ObjectUUID = c_Obj.UUID;
                    c_New.Reference = sID.IfEmpty();
                    c_New.Subject = sSubj;
                    c_New.Message = sMsg;
                    c_New.Via = sVia;
                    c_New.User = ctx.User.Name;

                    // Save
                    c_New.Save();
                }
            }

            return eAns;
        }
        #endregion
    }
}
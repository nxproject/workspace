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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class ShoutOut : CommandClass
    {
        #region Constants
        private const string ArgTo = "to";
        private const string ArgMsg = "msg";
        private const string ArgDocList = "doclist";
        private const string ArgObjList = "objlist";
        private const string ArgObj = "obj";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ShoutOut(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Code Line
        public override string Description { get { return("Sends a ShoutOut"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            c_P.Add(ArgObj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, ArgTo, "Associate that will receive the shoutout",
            c_P.Add(ArgSubj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, ArgMsg, "The message to be sent",
            c_P.Add(ArgObj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, ArgObj, "An object to attach",
            c_P.Add(ArgObj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, ArgDocList, "List of documents to attach",
            c_P.Add(ArgObj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, ArgObjList, "List of objects to attach");

            }
        }

        public override string Command
        {
            get { return "shout.out"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            string sTo = args.GetDefined(ArgTo).IfEmpty(ctx.User.Name);
            string sMsg = args.GetDefined(ArgMsg);
            string sDocList = args.GetDefined(ArgDocList);
            string sObjList = args.GetDefined(ArgObjList);
            string sObj = args.GetDefined(ArgObj);

            List<string> c_Attachments = new List<string>();
            if (sObjList.HasValue()) c_Attachments.AddRange(ctx.ObjectLists[sObjList].UUIDs(ctx));
            if (sDocList.HasValue()) c_Attachments.AddRange(ctx.DocumentLists[sDocList].Paths(ctx));
            if (sObj.HasValue()) c_Attachments.Add(ctx.Objects[sObj].UUID.ToString());

            // Do
            if (sTo.HasValue() && sMsg.HasValue())
            {
                ctx.User.Caller.ShoutOut(sTo, sMsg, c_Attachments);

                //eAns = ReturnClass.OK;
            }
            else if (sMsg.HasValue())
            {
                ctx.User.Caller.ShoutOut(ctx.User.Name, sMsg, c_Attachments);

                //eAns = ReturnClass.OK;
            }

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
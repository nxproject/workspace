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
    public class SODoc : CommandClass
    {
        #region Constants
        private const string ArgTo = "to";
        private const string ArgDoc = "doc";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public SODoc(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Code Line
        public override string Description { get { return("Open Document window"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            c_P.Add(ArgObj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, ArgTo, "Associate that will receive the shoutout",
            c_P.Add(ArgObj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, ArgDoc, "The document");

            }
        }

        public override string Command
        {
            get { return "window.doc"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            string sTo = args.GetDefined(ArgTo).IfEmpty(ctx.User.Name);
            string sDoc = args.GetDefined(ArgDoc);

            // Do
            NX.Engine.Files.DocumentClass c_Doc = ctx.Documents[sDoc];
            if (c_Doc != null)
            {
                ctx.User.Caller.SendOpenDoc(sTo, c_Doc.Path);

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
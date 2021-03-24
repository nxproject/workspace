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
    public class MessageAddAction : CommandClass
    {
        #region Constants
        private const string ArgMsg = "msg";
        private const string ArgTask = "task";
        private const string ArgName = "name";
        #endregion

        #region Constructor
        public MessageAddAction()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgMsg, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The message to use"));
                c_P.Add(ArgTask, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The task"));
                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name to display"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Comm, "Adds an action to the message", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "msg.action.add"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sMsg = args.Get(ArgMsg);
            string sTask = args.Get(ArgTask);
            string sName = args.Get(ArgName);

            // Do
            ctx.Messages[sMsg].Actions.Add(new Communication.eActionClass(sName, sTask));

            return eAns;
        }
        #endregion
    }
}
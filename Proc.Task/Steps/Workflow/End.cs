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
    public class WorkflowEnd : CommandClass
    {
        #region Constants
        private const string ArgWF = "wf";
        private const string ArgSource = "obj";
        private const string ArgName = "name";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public WorkflowEnd()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgSource, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the object"));
                c_P.Add(ArgWF, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The workflow to start"));
                c_P.Add(ArgName, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The user defined name"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Workflow, 
                    "Ends a workflow on a given object", 
                    c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "workflow.end"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // TBD

            return eAns;
        }
        #endregion
    }
}
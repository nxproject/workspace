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

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;

namespace Proc.Workflow
{
    public class ActivityClass : IStep, ITaskWF
    {
        #region Constructor
        public ActivityClass()
        { }
        #endregion

        #region IDisposable
        public virtual void Dispose()
        { }
        #endregion

        #region IPlugIn
        /// <summary>
        /// 
        /// The name
        /// 
        /// </summary>
        public virtual string Name { get { return Command; } }

        /// <summary>
        /// 
        /// Initialize
        /// 
        /// </summary>
        /// <param name="env">The current environamne</param>
        public void Initialize(EnvironmentClass env)
        {
            // Get the manager
            ManagerClass c_Mgr = env.Globals.Get<ManagerClass>();

            // And add the description
            c_Mgr.Parameters[this.Command] = this.Description;
        }
        #endregion

        #region IStep
        /// <summary>
        /// 
        /// The command
        /// 
        /// </summary>
        public virtual string Command { get { return null; } }

        /// <summary>
        /// 
        /// Description used to build Elsa UI
        /// 
        /// </summary>
        public virtual DescriptionClass Description { get { return null; } }

        /// <summary>
        /// 
        /// The process
        /// 
        /// </summary>
        /// <param name="ctx">The context</param>
        /// <param name="args">Parameters</param>
        /// <returns>Retunr code</returns>
        public virtual ReturnClass ExecStep(WorkflowContextClass ctx, ArgsClass args)
        {
            return ReturnClass.Failure("Not implmented");
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Adds system items
        /// </summary>
        /// <param name="pdesc"></param>
        public void AddSystem(NamedListClass<ParamDefinitionClass> pdesc, bool ifx = true, bool commentx = true)
        {
            if (ifx)
            {
                pdesc.Add("if", new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Execute if"));
            }

            if(commentx)
            {
                pdesc.Add("comment", new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "What does this step accomplish", "text"));
            }
        }

        /// <summary>
        /// 
        /// Builds a parameter description
        /// 
        /// </summary>
        /// <param name="values">The values</param>
        /// <returns>The description</returns>
        public string BuildParams(params string[] values)
        {
            // Work buffer
            List<string> c_Buffer = new List<string>();

            // Loop thru
            for (int i = 0; i < values.Length; i += 3)
            {
                // Add the field name
                c_Buffer.Add(values[i + 1]);
                // Add the value
                c_Buffer.Add(values[i]);
            }

            return c_Buffer.Join(" ");
        }
        #endregion
    }
}
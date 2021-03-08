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

namespace Proc.IOTIF
{
    public class IOTRequestClass : IIOTStep, ITaskWF
    {
        #region Constructor
        public IOTRequestClass()
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
        }

        public virtual StoreClass ExecStep(IOTContextClass ctx)
        {
            return null;
        }
        #endregion

        #region IStep
        /// <summary>
        /// 
        /// The command
        /// 
        /// </summary>
        public virtual string Command { get { return null; } }

        public DescriptionClass Description => null;
        #endregion
    }
}
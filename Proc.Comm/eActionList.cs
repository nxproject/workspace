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
using NX.Engine.Files;

namespace Proc.Communication
{
    public class eActiontListClass : ChildOfClass<eMessageClass>
    {
        #region Constructor
        public eActiontListClass(eMessageClass msg)
            : base(msg)
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The list of documents
        /// 
        /// </summary>
        public List<eActionClass> Actions { get; private set; } = new List<eActionClass>();

        /// <summary>
        /// 
        /// The number of actions
        /// 
        /// </summary>
        public int Count
        {
            get { return this.Actions.Count; }
        }
        #endregion

        #region Methods
        public void Add(List<eActionClass> actions)
        {
            foreach (eActionClass c_Action in actions)
            {
                this.Add(c_Action);
            }
        }

        public void Add(eActionClass action)
        {
            //
            this.Actions.Add(action);
        }
        #endregion
    }
}
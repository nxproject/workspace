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
using System.IO;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Proc.AO;

namespace Proc.Task
{
    public class TaskQueryClass : IDisposable
    {
        #region Constants
        #endregion

        #region Constructor
        public TaskQueryClass(string name)
        {
            //
            this.Name = name;
            this.Expressions = new List<AO.QueryElementClass>();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// the name
        /// 
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 
        /// The expresssions
        /// 
        /// </summary>
        public List<AO.QueryElementClass> Expressions { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Adds an expression to the query
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="op"></param>
        public void Add(AO.ExtendedContextClass ctx, string field, string value, AO.QueryElementClass.QueryOps op = AO.QueryElementClass.QueryOps.Eq)
        {
            this.Expressions.Add(new AO.QueryElementClass(ctx, field, value, op));
        }
        #endregion
    }
}
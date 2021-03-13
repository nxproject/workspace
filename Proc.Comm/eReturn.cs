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
using System.Text;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Communication
{
    public class eReturnClass : IDisposable
    {
        #region Constructor
        public eReturnClass()
        { }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public List<string> Success { get; internal set; } = new List<string>();
        public List<string> Failures { get; internal set; } = new List<string>();

        public string Errors { get { return this.Failures.Join(", "); } }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //
            StringBuilder c_Buffer = new StringBuilder();

            if (this.Success.Count > 0)
            {
                c_Buffer.Append("Success: {0}".FormatString(this.Success.Join(", ")));
            }

            if (this.Failures.Count > 0)
            {
                if (c_Buffer.Length > 0) c_Buffer.Append(", ");
                c_Buffer.Append("Failed: {0}".FormatString(this.Failures.Join(", ")));
            }

            return c_Buffer.ToString();
        }

        /// <summary>
        /// 
        /// Logs a submission
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="error"></param>
        /// <param name="e"></param>
        public void Log(eAddressClass addr, string error = null, Exception e = null)
        {
            if (error.HasValue())
            {
                if (e != null) error += e.GetAllExceptions();
                this.Failures.Add(addr.ToString() + ": " + error);
            }
            else
            {
                this.Success.Add(addr.ToString());
            }
        }
        #endregion
    }
}
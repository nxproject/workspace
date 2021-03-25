//--------------------------------------------------------------------------------
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

using NX.Shared;

namespace Common.TaskWF
{
    /// <summary>
    /// 
    /// Return from chore line
    /// 
    /// </summary>
    public class ReturnClass : IDisposable
    {
        #region Constructor
        public ReturnClass(string outcome, string msg = "")
        {
            //
            this.Outcome = outcome;
            this.Message = msg;
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// 
        /// Housekeeping
        /// 
        /// </summary>
        public void Dispose()
        { }
        #endregion

        #region Properties

        /// <summary>
        /// 
        /// The return code
        /// 
        /// </summary>
        public string Outcome { get; internal set; }

        /// <summary>
        /// 
        /// Optional message
        /// 
        /// </summary>
        public string Message { get; internal set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return "OUTCOME: {0}, MESSAGE: {1}".FormatString(this.Outcome, this.Message);
        }
        #endregion

        #region Statics
        public static ReturnClass Done
        {
            get { return new ReturnClass("Done"); }
        }

        public static ReturnClass End
        {
            get { return new ReturnClass("End"); }
        }

        public static ReturnClass Failure(string msg)
        {
            return new ReturnClass("Fail", msg);
        }

        public static ReturnClass Failure(Exception e)
        {
            return Failure(e.GetAllExceptions());
        }

        public static ReturnClass Failure(string msg, Exception e)
        {
            return Failure(msg.FormatString(e.GetAllExceptions()));
        }

        public static ReturnClass None
        {
            get { return new ReturnClass(""); }
        }
        #endregion
    }
}
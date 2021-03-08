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

using NX.Engine;
using NX.Shared;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// A service message
    /// 
    /// </summary>
    public class MessageClass : ChildOfClass<HTTPCallClass>
    {
        #region Constants
        private const string ParamFN = "fn";
        private const string ParamToken = "_token";
        #endregion

        #region Constructor
        public MessageClass(HTTPCallClass call, StoreClass data)
            : base(call)
        {
            //
            this.Params = data;
            this.Request = new StoreClass(call.BodyAsJObject);

            // Set user
            if (call.UserInfo == null) call.UserInfo = new UserInfoClass(call);
            if (!call.UserInfo.Name.HasValue()) call.UserInfo.Name = this.Request["_user"];
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Parameters
        /// 
        /// </summary>
        public StoreClass Params { get; internal set; }

        /// <summary>
        /// 
        /// Request
        /// 
        /// </summary>
        public StoreClass Request { get; internal set; }

        /// <summary>
        /// 
        /// Result back to caller
        /// 
        /// </summary>
        public StoreClass Result { get; internal set; } = new StoreClass();
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Calls the function passed in params
        /// 
        /// </summary>
        public void Do()
        {
            // Handle the token
            using (TokenClass c_Token = new TokenClass(this.Request[ParamToken]))
            {
                // Get the fn
                string sFN = this.Request[ParamFN];
                // Do we need to verify?
                if (!sFN.StartsWith("Access."))
                {
                    // Is the token valid?
                    if(!c_Token.IsValid)
                    {
                        //
                        this.Parent.Env.LogError("{0} requires a security token".FormatString(sFN));

                        // Bad
                        sFN = null;
                    }
                }
                // Any?
                if (sFN.HasValue())
                {
                    // Valid?
                    if (this.Parent.FNExists(sFN))
                    {
                        // Protect
                        try
                        {
                            // Call the fn
                            this.Result = this.Parent.FN(sFN, this.Request);
                            // Assure
                            if (this.Result == null) this.Result = new StoreClass();
                            // And create token
                            this.Result[ParamToken] = c_Token.Till();
                        }
                        catch (Exception e) 
                        {
                            this.Parent.Env.LogException("Message", e);
                        }
                    }
                    else
                    {
                        this.Parent.Env.LogError("{0} is not a valid FN".FormatString(sFN));
                    }
                }
                else
                {
                    this.Parent.Env.LogError("Missing FN");
                }

                // Add outbound token
                this.Result[ParamToken] = c_Token.Till();
            }
        }
        #endregion
    }
}
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
using System.Net;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class HTTPCookieSet : CommandClass
    {
        #region Constants
        private const string ArgHTTP = "conn";
        private const string ArgDomain = "domain";
        private const string ArgKey = "key";
        private const string ArgValue = "value";
        #endregion

        #region Constructor
        public HTTPCookieSet()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgHTTP, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The connection to use"));
                c_P.Add(ArgDomain, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The domain"));
                c_P.Add(ArgKey, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The key"));
                c_P.Add(ArgValue, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The value"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.HTTP, "Sets a cookie value in the connection", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "http.cookie.set"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sHTTP = args.Get(ArgHTTP);
            string sDomain = args.Get(ArgDomain);
            string sKey = args.Get(ArgKey);
            string sValue = args.GetRaw(ArgValue);

            // Get the connection
            HTTPClientClass c_HTTP = ctx.HTTP[sHTTP];

            try
            {
                //
                c_HTTP.CookieContainer.Add(new Uri(sDomain), new Cookie(sKey, sValue));
            }
            catch { }

            return eAns;
        }
        #endregion
    }
}
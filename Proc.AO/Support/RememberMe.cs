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
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
///  

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public static class RememberMeClass
    {
        #region Methods
        public static string RMEncode(this EnvironmentClass env, string user, string pwd)
        {
            JObject c_Wkg = new JObject();
            c_Wkg.Set("r", "".GUID());
            c_Wkg.Set("u", user.IfEmpty());
            c_Wkg.Set("p", pwd.IfEmpty());

            return c_Wkg.ToSimpleString().EncodeToBase64(env.ReachableURL);
        }

        public static Tuple<string , string> RMDecode(this EnvironmentClass env, string rm)
        {
            Tuple<string, string> c_Ans = new Tuple<string, string>(null, null);

            JObject c_Wkg = rm.DecodeFromBase64(env.ReachableURL).ToJObject();
            if(c_Wkg != null)
            {
                c_Ans = new Tuple<string, string>(c_Wkg.Get("u"), c_Wkg.Get("p"));
            }
            return c_Ans;
        }
        #endregion
    }
}

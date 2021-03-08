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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;

namespace Proc.Task
{
    public class AddressValidate : CommandClass
    {
        #region Constants
        private const string ArgAddr = "addr";
        private const string ArgCity = "city";
        private const string ArgState = "state";
        private const string ArgZip = "zip";
        #endregion

        #region Constructor
        public AddressValidate()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgAddr, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The address field"));
                c_P.Add(ArgCity, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The city field"));
                c_P.Add(ArgState, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The state field"));
                c_P.Add(ArgZip, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The zip field"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Address, "Validates an address", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "address.validate"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sAddr = args.GetRaw(ArgAddr);
            string sCity = args.GetRaw(ArgCity);
            string sState = args.GetRaw(ArgState);
            string sZIP = args.GetRaw(ArgZip);

            //
            string sPassed = ctx[sAddr].IfEmpty() + " " + ctx[sCity].IfEmpty() + " " + ctx[sState].IfEmpty() + " " + ctx[sZIP].IfEmpty();

            //
            string sURL = "http://api.positionstack.com/v1/forward";

            JObject c_Data = new JObject();
            c_Data.Set("access_key", ctx.SiteInfo.PSAPIKey);
            c_Data.Set("query", sPassed);
            c_Data.Set("limit", "1");

            // Assume error
            eAns = ReturnClass.Failure("Unable to validate");

            // Call
            JObject c_Ret = sURL.URLGet(c_Data.ToSimpleString()).ToJObject();
            // Get return
            JArray c_Poss = c_Ret.GetJArray("data");
            if(c_Poss != null && c_Poss.Count > 0)
            {
                JObject c_Info = c_Poss.GetJObject(0);
                if(c_Info.Get("confidence").ToFloat() > 0.9F)
                {
                    ctx[sAddr] = c_Info.Get("name");
                    ctx[sCity] = c_Info.Get("locality");
                    ctx[sState] = c_Info.Get("region_code");
                    ctx[sZIP] = c_Info.Get("postal_code");

                    eAns = ReturnClass.Done;
                }
            }

            return eAns;
        }
        #endregion
    }
}
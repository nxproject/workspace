///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Chore
{
    public class DEBase : CommandClass
    {
        #region Constants
        private const string ArgKeep = "keep";
        private const string ArgExcl = "excl";
        private const string ArgMap = "map";
        private const string ArgReMap = "remap";
        private const string ArgReMapID = "remapid";
        private const string ArgExMap = "exmap";
        private const string ArgExMapID = "exmapid";
        private const string ArgNoSend = "nosend";
        private const string ArgMin = "minimize";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DEBase(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Support
        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(
                    CommandClass.Optional, ArgKeep, "Fields to keep",
                    CommandClass.Optional, ArgExcl, "Fields to exclude",
                    CommandClass.Optional, ArgMap, "Fields to rename",
                    CommandClass.Optional, ArgReMap, "Fields to reduce map by suffix",
                    CommandClass.Optional, ArgReMapID, "Reduce map suffix",
                    CommandClass.Optional, ArgExMap, "Fields to expand map by suffix",
                    CommandClass.Optional, ArgExMapID, "Expand map suffix",
                    CommandClass.Optional, ArgMin, "Minimize data packet");
            }
        }

        public JObject MapPacket(ChoreContextClass ctx, ArgsClass args, JObject packet, AO.DatasetClass ds = null)
        {
            //
            if (packet == null) packet = new JObject();

            //
            string sKeep = args.GetDefined(ArgKeep);
            string sExcl = args.GetDefined(ArgExcl);
            string sMap = args.GetDefined(ArgMap);
            string sRemap = args.GetDefined(ArgReMap);
            string sRemapID = args.GetDefined(ArgReMapID);
            string sExmap = args.GetDefined(ArgExMap);
            string sExmapID = args.GetDefined(ArgExMapID);
            string sNoSend = args.GetDefined(ArgNoSend);

            return packet.DEMap(sKeep, sExcl, sMap, sRemap, sRemapID, sExmap, sExmapID, sNoSend);
        }

        public string ReMapID(ChoreContextClass ctx, ArgsClass args)
        {
            return args.GetDefined(ArgReMapID);
        }

        //public void Bill(ChoreContextClass ctx, ChargeClass charge)
        //{
        //    if (charge != null)
        //    {
        //        ctx.User.Caller.PortalCallBill(ctx.User.UserID, charge);
        //    }
        //}

        public JObject Minimize(ChoreContextClass ctx, ArgsClass args, JObject packet)
        {
            if (args.GetDefinedAsBool(ArgMin, false))
            {
                packet = packet.Minimize();
            }

            return packet;
        }
        #endregion
    }
}
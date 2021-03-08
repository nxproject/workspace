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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

using Proc.AO;
using Proc.Docs;

namespace Proc.Chore
{
    public class UIMUser : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgDSUser = "user";
        private const string ArgDSPwd = "pwd";
        private const string ArgDSName = "name";
        private const string ArgDSCo = "co";
        private const string ArgDSAddr = "addr";
        private const string ArgDSCity = "city";
        private const string ArgDSState = "state";
        private const string ArgDSZIP = "zip";
        private const string ArgDSPhone = "phone";
        private const string ArgDSUDF1 = "udf1";
        private const string ArgDSUDF2 = "udf2";
        private const string ArgDSUDF3 = "udf3";
        private const string ArgDSLU = "lu";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public UIMUser(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Code Line
        
        {
            public override string Description { get { return

            public override string Description { get { return("Maps the fields in the ui.associate object");

            CommandClass.Required,ArgDS, "Dataset that holds user information");
            CommandClass.Required,ArgDSUser, "User ID field in dataset");
            CommandClass.Required,ArgDSPwd, "Password field in dataset");
            CommandClass.Optional, ArgDSName, "Name field in dataset",
            CommandClass.Optional, ArgDSCo, "Company name field in dataset",
            CommandClass.Optional, ArgDSAddr, "Address field in dataset",
            CommandClass.Optional, ArgDSCity, "City field in dataset",
            CommandClass.Optional, ArgDSState, "State field in dataset",
            CommandClass.Optional, ArgDSZIP, "ZIP Code field in dataset",
            CommandClass.Optional, ArgDSPhone, "Phone field in dataset",
            CommandClass.Optional, ArgDSUDF1, "User defined field #1 in dataset",
            CommandClass.Optional, ArgDSUDF2, "User defined field #2 in dataset",
            CommandClass.Optional, ArgDSUDF3, "User defined field #3 in dataset",
            CommandClass.Optional, ArgDSLU, "last used field in dataset",

            }
        }

        public override string Command
        {
            get { return "ui.map.user"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            string sDS = args.GetDefined(ArgDS);
            string sDSUser = args.GetDefined(ArgDSUser);
            string sDSPwd = args.GetDefined(ArgDSPwd);

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgDS, sDS,
                                                ArgDSUser, sDSUser,
                                                ArgDSPwd, sDSPwd))
            {
                // Map
                env.UI.State.User.UseMatrix(sDS,
                                                    sDSUser,
                                                     sDSPwd,
                                                     args.GetDefined(ArgDSName),
                                                     args.GetDefined(ArgDSCo),
                                                     args.GetDefined(ArgDSAddr),
                                                     args.GetDefined(ArgDSCity),
                                                     args.GetDefined(ArgDSState),
                                                     args.GetDefined(ArgDSZIP),
                                                     args.GetDefined(ArgDSPhone),
                                                     args.GetDefined(ArgDSUDF1),
                                                     args.GetDefined(ArgDSUDF2),
                                                     args.GetDefined(ArgDSUDF3),
                                                     args.GetDefined(ArgDSLU));

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
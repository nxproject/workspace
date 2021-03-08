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
    public class JFStepInitialize : CommandClass
    {
        #region Constructor
        public JFStepInitialize(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Setsup the parameters for JETFile"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams();

            //CommandClass.Optional, JETFileInfoClass.ArgObj, "The object",
            //CommandClass.Optional, JETFileInfoClass.ArgSubmID, "The submission ID field",
            //CommandClass.Optional, JETFileInfoClass.ArgDate, "The date field",
            //CommandClass.Optional, JETFileInfoClass.ArgJobStatus, "The job status field",
            //CommandClass.Optional, JETFileInfoClass.ArgHB, "The hertbeat field",
            //CommandClass.Optional, JETFileInfoClass.ArgStatus, "The status field",
            //CommandClass.Optional, JETFileInfoClass.ArgJSON, "The JSON document field",
            //CommandClass.Optional, JETFileInfoClass.ArgXML, "The XML document field",
            //CommandClass.Optional, JETFileInfoClass.ArgSFTJF, "The JETFile SFTP server",
            //CommandClass.Optional, JETFileInfoClass.ArgSFTPP, "The IBM SFTP server",

            }
        }

        public override string Command
        {
            get { return "_jetfile.initialize"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            //
            env.JETFileInfo.InitializeFromArgs(arguments);

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
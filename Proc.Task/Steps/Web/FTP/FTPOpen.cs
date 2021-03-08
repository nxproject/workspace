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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class FTPOpen : CommandClass
    {
        #region Constants
        private const string ArgHTTP = "conn";
        private const string ArgURL = "url";
        private const string ArgPort = "port";
        private const string ArgUser = "user";
        private const string ArgPwd = "pwd";
        private const string ArgCert = "cert";
        private const string ArgMode = "sftp";
        #endregion

        #region Constructor
        public FTPOpen()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgHTTP, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The connection to use"));
                c_P.Add(ArgURL, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The URl to connect to"));
                c_P.Add(ArgPort, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The port"));
                c_P.Add(ArgUser, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The user name"));
                c_P.Add(ArgPwd, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The user password"));
                c_P.Add(ArgMode, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "True if SFTP"));
                c_P.Add(ArgCert, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The SSH certificate if SFTP (from system wallet)"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.FTP, "Opens an sftp connection", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "ftp.open"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sHTTP = args.Get(ArgHTTP);
            string sURL = args.Get(ArgURL);
            int iPort = args.Get(ArgPort).ToInteger(22);
            string sUser = args.Get(ArgUser);
            string sPwd = args.Get(ArgPwd);
            string sCert = args.Get(ArgCert);
            bool bIsSFTP = args.GetAsBool(ArgMode, false);

            // Do
            if (ctx.FTP.Has(sHTTP))
            {
                List<AO.SSHKeyClass> c_Key = null;
                if (sCert.HasValue())
                {
                    // TBD
                    //AO.WalletClass c_Wallet = new AO.WalletClass(ctx.User.SynchObject);
                    //c_Key = c_Wallet.GetSSHKeys(sCert);
                }

                if (!ctx.FTP[sHTTP].Open(ctx, sURL, iPort, c_Key, sUser, sPwd, bIsSFTP))
                {
                    eAns = ReturnClass.Failure("Unable to connect");
                }
            }

            return eAns;
        }
        #endregion
    }
}
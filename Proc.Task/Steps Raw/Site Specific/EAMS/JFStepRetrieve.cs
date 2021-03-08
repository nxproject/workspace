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
    public class JFStepRetrieve : CommandClass
    {
        #region Constants
        private const string ArgList = "list";
        #endregion

        #region Enums

        #endregion

        #region Constructor
        public JFStepRetrieve(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Retrieve responses from JETFile"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams();


            }
        }

        public override string Command
        {
            get { return "_jetfile.retrieve"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            //
            string sList = args.GetDefined(ArgList);

            // From before
            foreach(string sFile in DocReferenceClass.GetUserDocs(env, env.JETFileInfo.InboundFolder))
            {
                env.DocumentLists.Add(sList, sFile);
                //env.Env.LogInfo("SAW: {0}".FormatString(sFile));
            }

            //
            switch(env.Env.VUUID)
            {
                case EnvironmentClass.SiteEAMS:
                case EnvironmentClass.SiteQE:
                    // From payments            
                    this.GetFiles(env, sList, env.JETFileInfo.SFTPPayment(), env.JETFileInfo.SFTPInboundFolder(true, false));
                    // And DIR/DWC
                    this.GetFiles(env, sList, env.JETFileInfo.SFTPJETFile(), env.JETFileInfo.SFTPInboundFolder(false, false));
                    break;

                default:
                    using (BaseInfoClass c_Info = BaseInfoClass.Get(env.Env, true))
                    {
                        if (c_Info.InTest)
                        {
                            // From payments            
                            this.GetFiles(env, sList, env.JETFileInfo.SFTPPayment(), env.JETFileInfo.SFTPInboundFolder(true, true));
                            // And DIR/DWC
                            this.GetFiles(env, sList, env.JETFileInfo.SFTPJETFile(), env.JETFileInfo.SFTPInboundFolder(false, true));
                        }
                        else
                        {
                            // And DIR/DWC
                            this.GetFiles(env, sList, env.JETFileInfo.SFTPJETFile(), env.JETFileInfo.SFTPInboundFolder(false, false));
                        }
                    }
                    break;
            }

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion

        #region Support
        private void GetFiles(SandboxClass env, string list, FTPClientClass sftp, string dir)
        {
            // List the files
            if (sftp != null)
            {
                env.Env.LogInfo("JFStepRetrieve: {0}".FormatString(sftp.ToString()));

                    List<string> c_Files = sftp.Files(dir);
                // Each
                foreach (string sFile in c_Files)
                {
                    string sName = sFile.GetFileNameFromPath();
                    if (!sName.EndsWith(".xml")) sName += ".xml";

                    using (DocReferenceClass c_Doc = DocReferenceClass.CreateUserDoc(env, sName, env.JETFileInfo.InboundFolder))
                    {
                        c_Doc.Path.AssurePath();

                        if (sftp.Download(sFile, c_Doc.Location))
                        {
                            env.DocumentLists.Add(list, c_Doc.Location);

                            sftp.Delete(sFile);

                            //env.Env.LogInfo("JETFILE RETRIEVE: Downloaded {0} to {1}".FormatString(sFile, c_Doc.Location));
                        }
                        else
                        {
                            env.Env.LogError("JETFILE RETRIEVE: Unable to download {0} to {1}".FormatString(sFile, c_Doc.Location));
                        }
                    }
                }
            }
            else
            {
                env.Env.LogError("JFStepRetrieve: Unable to connect to JETFile SFTP server");
            }
        }
        #endregion
    }
}
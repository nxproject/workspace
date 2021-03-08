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
    public class JFStepSubmit : CommandClass
    {
        #region Constants
        #endregion

        #region Enums

        #endregion

        #region Constructor
        public JFStepSubmit(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Submits a JETFile XML packet"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams();


            }
        }

        public override string Command
        {
            get { return "_jetfile.submit"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            //
            env.JETFileInfo.SetObject(arguments);

            using (JETEngineClass c_Eng = new JETEngineClass(env.Env, env.Associate))
            {
                // Get the object
                if (env.JETFileInfo.Object != null)
                {
                    using (BaseInfoClass c_Info = BaseInfoClass.Get(env.Env, true))
                    {
                        bool bIsTest = c_Info.InTest;

                        // Get the XML
                        string sXML = env.JETFileInfo.GetXMLPacket(JETEngineClass.ReturnTypes.Submission);

                        bool bOK = false;

                        // Make the connection
                        if (env.JETFileInfo.PacketType == JETEngineClass.PacketTypes.LienWFee)
                        {
                            // Do the wallet stuff
                            AO.WalletClass c_Wallet = env.JETFileInfo.Object.Wallet;
                            if (c_Wallet != null)
                            {
                                AO.CreditCardClass c_CC = c_Wallet[1];
                                if (c_CC != null)
                                {
                                    sXML = sXML.Replace(JETEngineClass.XMLPlaceholder("CREDITCARD"), c_CC.Number);
                                    sXML = sXML.Replace(JETEngineClass.XMLPlaceholder("EXPDATE"), c_CC.Expiration.Replace("/", ""));

                                    if (env.Env.VUUID.IsSameValue("lawdemo"))
                                    {
                                        env.Env.LogInfo("CC {0} used".FormatString(c_CC.Number));
                                        env.Env.LogInfo("Exp {0}".FormatString(c_CC.Expiration));

                                        env.JETFileInfo.SetXMLPacket(JETEngineClass.ReturnTypes.Private, null, sXML);
                                    }
                                }
                                else
                                {
                                    env.Env.LogInfo("No credit card found");
                                }
                            }
                            else
                            {
                                env.Env.LogInfo("No wallet found");
                            }

                            // Get the connection
                            Protocol.FTPClientClass c_SFTP = env.JETFileInfo.SFTPPayment();
                            if (c_SFTP == null)
                            {
                                env.Env.LogError("Unable to connect to Payment SFTP server");
                            }
                            else
                            {
                                env.Env.LogInfo("JFStepSubmit: {0}".FormatString(c_SFTP.ToString()));

                                // PGP
                                using (PGPCrypto c_Crypto = new PGPCrypto(env.JETFileInfo.PGPKeys))
                                {
                                    // Send
                                    byte[] abEncoded = c_Crypto.Encrypt(sXML.ToBytes());
                                    if (abEncoded == null || abEncoded.Length == 0)
                                    {
                                        env.Env.LogError("Unable to encode payment");
                                    }
                                    else
                                    {
                                        // The file name
                                        string sOutFile = env.JETFileInfo.SFTPOutboundFolder(true, bIsTest).CombinePath(env.JETFileInfo.GetXMLDocument(JETEngineClass.ReturnTypes.Submission).GetFileNameFromPath());
                                        sOutFile = sOutFile.SetExtensionFromPath("pgp");
                                        //env.Env.LogInfo("Uploading {0} with msg of {1} bytes".FormatString(sOutFile, abEncoded.Length));

                                        // Send
                                        bOK = c_SFTP.Upload(sOutFile, abEncoded);
                                        if (!bOK)
                                        {
                                            env.Env.LogError("SFTP payment upload failed for {0}".FormatString(env.JETFileInfo.GetSubmissionID()));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Get the connection
                            Protocol.FTPClientClass c_SFTP = env.JETFileInfo.SFTPJETFile();
                            if (c_SFTP == null)
                            {
                                env.Env.LogError("Unable to connect to JETFile SFTP server");
                            }
                            else
                            {
                                env.Env.LogInfo("JFStepSubmit: {0}".FormatString(c_SFTP.ToString()));

                                // The file name
                                string sOutFile = env.JETFileInfo.SFTPOutboundFolder(false, bIsTest).CombinePath(env.JETFileInfo.GetXMLDocument(JETEngineClass.ReturnTypes.Submission).GetFileNameFromPath());
                                //env.Env.LogInfo("Uploading {0}".FormatString(sOutFile));

                                // Send
                                bOK = c_SFTP.Upload(sOutFile, sXML.ToBytes());
                                if (!bOK)
                                {
                                    env.Env.LogError("SFTP upload failed for {0}".FormatString(env.JETFileInfo.GetSubmissionID()));
                                }
                            }
                        }

                        // If we have packet, submit
                        if (bOK)
                        {
                            env.JETFileInfo.SetStatus(JETEngineClass.ReturnTypes.Submission, "OK");
                            env.JETFileInfo.SetJobStatus("Submitted");
                            env.JETFileInfo.SetHeartBeat(JETFileHBClass.InMinutes(60));
                        }
                        else
                        {
                            // Store errors
                            env.JETFileInfo.SetStatus(JETEngineClass.ReturnTypes.Submission, "Failed, please resubmit");
                            env.JETFileInfo.SetJobStatus("Failed");
                            env.JETFileInfo.ClearHeartBeat();
                        }
                    }
                }

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
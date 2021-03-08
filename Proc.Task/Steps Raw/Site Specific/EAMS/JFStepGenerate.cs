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
    public class JFStepGenerate : CommandClass
    {
        #region Constants
        #endregion

        #region Enums

        #endregion

        #region Constructor
        public JFStepGenerate(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Generates a JETFile XML packet"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams();

            }
        }

        public override string Command
        {
            get { return "_jetfile.generate"; }
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
                        // Call the packet maker
                        bool bIsTest = c_Info.InTest;

                        //env.Env.LogInfo("JFStepGenerate: {0}")    

                        ProcessResultClass c_Rslt = c_Eng.Build(env.JETFileInfo.GetSubmissionID(),
                                                                    env.JETFileInfo.RawJSON,
                                                                    bIsTest);

                        // If we have packet, submit
                        if (c_Rslt.Errors.Count == 0)
                        {
                            if (env.JETFileInfo.RawJSON.eServe)
                            {
                                // TBD
                            }

                            env.JETFileInfo.SetXMLPacket(JETEngineClass.ReturnTypes.Submission, null, c_Rslt.XMLPacket);

                            env.JETFileInfo.SetStatus(JETEngineClass.ReturnTypes.Generated, "OK");
                            env.JETFileInfo.SetJobStatus("Generated");
                            env.JETFileInfo.SetHeartBeat(JETFileHBClass.InMinutes(60));
                        }
                        else
                        {
                            // Store errors
                            env.JETFileInfo.SetStatus(JETEngineClass.ReturnTypes.Generated, "Failed: {0}".FormatString(ErrorClass.Join(c_Rslt.Errors, ", ")));
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
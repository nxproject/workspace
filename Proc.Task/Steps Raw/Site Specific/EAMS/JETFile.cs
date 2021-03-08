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
    public class JETFile : CommandClass
    {
        #region Constants
        private const string ArgStore = "store";
        //private const string ArgCC = "cc";
        private const string ArgDocs = "docs";
        private const string ArgSubmFld = "submfld";
        private const string ArgErrorFld = "errorfld";
        #endregion

        #region Enums

        #endregion

        #region Constructor
        public JETFile(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a JETFile packet"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required,ArgStore, "The values store",
            //CommandClass.Required,ArgCC, "The credit card store");
            CommandClass.Required,ArgDocs, "The documents list",
            CommandClass.Required,ArgSubmFld, "The field to store the submission ID");

            }
        }

        public override string Command
        {
            get { return "_eams.jetfile"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);
            
            // Get the parameters
            string sData = args.GetDefined(ArgStore);
            //string sCC = args.GetDefined(ArgCC);
            string sDocs = args.GetDefined(ArgDocs);
            string sSubm = args.GetDefinedRaw(ArgSubmFld);
            string sError = args.GetDefinedRaw(ArgErrorFld);

            // Validate
            if (this.CheckValidity(ctx,
                                            ArgDocs, sDocs,
                                            ArgSubmFld, sSubm,
                                            ArgErrorFld, sError))
            {
                using (JETEngineClass c_Eng = new JETEngineClass(env.Associate))
                {
                    // The data to be used
                    JObject c_Data = new JObject();

                    // Get the type of submission
                    JETEngineClass.PacketTypes eType = JETEngineClass.PacketTypes.Unknown;
                    string sUFO = null;
                    //
                    List<DocReferenceClass> c_Docs = env.DocumentLists.Documents(sDocs);
                    List<string> c_Paths = new List<string>();
                    foreach (DocReferenceClass c_Doc in c_Docs)
                    {
                        switch (c_Doc.ShortName)
                        {
                            case "APPLICATION FOR ADJUDICATION":
                                eType = JETEngineClass.PacketTypes.App;
                                c_Data = c_Doc.MergedData;
                                break;

                            case "DECLARATION OF READINESS TO PROCEED":
                                eType = JETEngineClass.PacketTypes.DOR;
                                c_Data = c_Doc.MergedData;
                                break;

                            case "EXPEDITED DECLARATION OF READINESS":
                                eType = JETEngineClass.PacketTypes.ExpDOR;
                                break;

                            case "LIEN":
                                eType = JETEngineClass.PacketTypes.Lien;
                                c_Data = c_Doc.MergedData;
                                break;

                            case "CR":
                                eType = JETEngineClass.PacketTypes.CR;
                                c_Data = c_Doc.MergedData;
                                break;

                            case "STIP":
                                eType = JETEngineClass.PacketTypes.Stip;
                                c_Data = c_Doc.MergedData;
                                break;

                            case "PROOF OF SERVICE":
                                c_Paths.Add(c_Doc.Location);
                                break;

                            default:
                                if (eType == JETEngineClass.PacketTypes.Unknown)
                                {
                                    eType = JETEngineClass.PacketTypes.UFO;
                                    c_Data = c_Doc.MergedData;
                                    sUFO = c_Doc.ShortName;
                                }
                                else
                                {
                                    c_Paths.Add(c_Doc.Location);
                                }
                                break;
                        }
                    }

                    // The submission ID
                    string sSubmissionID = env.Associate.Caller.NextJETFileID(MM.EAMS.JETFile.Information.JETFileAcct);

                    // Call the packet maker
                    ProcessResultClass c_Rslt = c_Eng.Build(sSubmissionID, 
                                                                eType,
                                                                c_Data, 
                                                                c_Paths,
                                                                env.Wallet);
                    // If we have packet, submit
                    if(c_Rslt.Errors.Count == 0)
                    {
                        if (c_Eng.Submit(c_Rslt))
                        {
                            // Store the submission ID
                            ctx[sSubm] = sSubmissionID;
                        }
                        else
                        {
                            c_Rslt.Errors.Add(new ErrorClass("Unable to submit"));
                        }
                    }

                    if(c_Rslt.Errors.Count > 0)
                    {
                        // Store errors
                        ctx[sError] = ErrorClass.Join( c_Rslt.Errors, ", ");
                    }                    

                    eAns = ReturnClass.OK;
                }
            }

            return eAns;
        }
        #endregion
    }
}
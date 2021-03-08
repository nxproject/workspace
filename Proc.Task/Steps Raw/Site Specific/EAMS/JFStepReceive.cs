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
    public class JFStepReceive : CommandClass
    {
        #region Constants
        #endregion

        #region Enums

        #endregion

        #region Constructor
        public JFStepReceive(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Receives a JETFile request"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams();

            }
        }

        public override string Command
        {
            get { return "_jetfile.receive"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            //
            env.JETFileInfo.SetObject(arguments);

            using (JETEngineClass c_Eng = new JETEngineClass(env.Env, env.Associate))
            {
                // Get the list of documents
                List<string> c_DocNames = env.JETFileInfo.Object.Documents("");
                List<DocReferenceClass> c_Docs = new List<DocReferenceClass>();
                foreach (string sDocX in c_DocNames) c_Docs.Add(new DocReferenceClass(env, sDocX.GetFileNameFromPath(), env.JETFileInfo.Object));

                // Get the valid document titles
                this.DocumentTitles = c_Eng.DocumentTitles();

                //
                env.JETFileInfo.SetObject(arguments);

                // The data to be used
                JETEngineRawData c_Data = new JETEngineRawData();

                // RAW INFO
                DocReferenceClass c_SpecialInfo = null;

                //
                foreach (DocReferenceClass c_Doc in c_Docs)
                {
                    switch (c_Doc.ShortName)
                    {
                        case "APPLICATION FOR ADJUDICATION":
                            c_Data.PacketType = JETEngineClass.PacketTypes.App;
                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "AppCaseNumber";
                            break;

                        case "ANSWER TO APPLICATION FOR ADJUDICATION OF CLAIM":
                            c_Data.PacketType = JETEngineClass.PacketTypes.AnswerApp;
                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "AppClaimCaseNumber";
                            break;

                        case "DECLARATION OF READINESS TO PROCEED":
                            c_Data.PacketType = JETEngineClass.PacketTypes.DOR;
                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "DORCaseNumber";
                            break;

                        case "DECLARATION OF READINESS TO PROCEED TO EXPEDITED HEARING":
                            c_Data.PacketType = JETEngineClass.PacketTypes.ExpDOR;
                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "DORExpCaseNumber";
                            break;

                        case "NOTICE AND REQUEST FOR ALLOWANCE OF LIEN":

                            if (c_Doc.MergedData.Get("LienExempt4903b").FromDBBoolean() ||
                                c_Doc.MergedData.Get("LienExempt490305d7").FromDBBoolean())
                            {
                                c_Data.PacketType = JETEngineClass.PacketTypes.LienExempt;
                            }
                            else
                            {
                                c_Data.PacketType = JETEngineClass.PacketTypes.LienWFee;
                            }

                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "LienCaseNumber";
                            break;

                        case "COMPROMISE AND RELEASE":
                            c_Data.PacketType = JETEngineClass.PacketTypes.CR;
                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "CNRCaseNo1";
                            break;

                        case "STIPULATIONS WITH REQUEST FOR AWARD":
                        case "STIPULATIONS WITH REQUEST FOR AWARD DOI post 1-1-2013":
                            c_Data.PacketType = JETEngineClass.PacketTypes.Stip;
                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "StipPostCaseNumber";
                            break;

                        case "REQUEST FOR SUMMARY RATING DETERMINATION OF PRIMARY TREATING PHYSICIAN REPORT":
                            c_Data.PacketType = JETEngineClass.PacketTypes.DEU102;
                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "DEUEmpCaseNo";
                            c_Data.OpeningInjuryTypeField = "DEUInjuryType";
                            c_Data.OpeningInjuryStartDateField = "DEUInjuryStartDate";
                            c_Data.OpeningInjuryEndDateField = "DEUInjuryEndDate";
                            c_Data.WCABUnit = "DEU";
                            break;

                        case "REQUEST FOR CONSULTATIVE RATING":
                            c_Data.PacketType = JETEngineClass.PacketTypes.DEU104;
                            c_Data.Use(c_Doc.MergedData);
                            c_Data.ADJField = "DEUIWCase1";
                            c_Data.OpeningInjuryTypeField = "DEUInjuryType";
                            c_Data.OpeningInjuryStartDateField = "DEUInjuryStartDate";
                            c_Data.OpeningInjuryEndDateField = "DEUInjuryEndDate";
                            c_Data.WCABUnit = "DEU";
                            break;

                        case "PROOF OF SERVICE":
                            this.Attachments.Add(c_Doc);
                            break;

                        case "INFO_ONLY":
                            c_SpecialInfo = c_Doc;
                            break;

                        default:
                            string sTitle = c_Doc.ShortName.ToUpper();
                            if (this.DocumentTitles.Contains(sTitle))
                            {
                                if (this.UFO == null)
                                {
                                    this.UFO = c_Doc;
                                }
                                else
                                {
                                    if (this.ComputeTypeIndex(c_Doc) < this.ComputeTypeIndex(this.UFO))
                                    {
                                        this.Attachments.Add(this.UFO);
                                        this.UFO = c_Doc;
                                    }
                                    else
                                    {
                                        this.Attachments.Add(c_Doc);
                                    }
                                }
                            }
                            else
                            {
                                this.Attachments.Add(c_Doc);
                            }
                            break;
                    }
                }

                // If not known, see if UFO
                if (c_Data.PacketType == JETEngineClass.PacketTypes.Unknown && this.UFO != null)
                {
                    c_Data.PacketType = JETEngineClass.PacketTypes.UFO;
                    c_Data.AddAttachment(this.UFO.Location, this.GetAttachmentType(this.UFO));
                    c_Data.Use(this.UFO.MergedData);
                    c_Data.ADJField = "CaseNumber";

                    this.UFO = null;
                }

                // If we still have UFO, add to attachments
                if (this.UFO != null)
                {
                    this.Attachments.Add(this.UFO);
                    this.UFO = null;
                }

                // Do we have a special file
                if (c_SpecialInfo != null && c_SpecialInfo.MergedData != null)
                {
                    // Get the merge data
                    JObject c_MD = c_Data.Values;
                    // Merge
                    c_MD = c_SpecialInfo.MergedData.SmartMerge(c_MD);
                    //
                    c_Data.Use(c_MD);
                }

                // Reset the document titles
                if (c_Data.WCABUnit.HasValue())
                {
                    this.DocumentTitles = c_Eng.DocumentTitles(c_Data.WCABUnit);
                }

                // Process attachments
                foreach (DocReferenceClass c_Doc in this.Attachments)
                {
                    c_Data.AddAttachment(c_Doc.Location, this.GetAttachmentType(c_Doc));
                }

                // Now for memory items
                if (c_Data.PacketType != JETEngineClass.PacketTypes.Unknown)
                {
                    string sSvcInfo = env.JETFileInfo.Object.SvcInfoGet("");
                    if (sSvcInfo.HasValue())
                    {
                        // Make the info
                        SvcInfoClass c_InfoX = new SvcInfoClass(env, sSvcInfo);
                        using (JETFileMemoryClass c_Mem = new JETFileMemoryClass(env.Associate, c_InfoX.UUID, c_Data.ADJ, c_Data.PacketType.ToString()))
                        {
                            // Handle odd cases
                            string sResubmissionID = c_Data.Values.Get("resubmissionid");
                            string sPaymentReference = c_Data.Values.Get("paymentreference");

                            // Get the resubmit ID
                            c_Data.ResubmitID = sResubmissionID.IfEmpty(c_Mem.ResubmissionID);
                            if (c_Data.ResubmitID.IsSameValue("-")) c_Data.ResubmitID = "";

                            // Get the payment reference
                            c_Data.PaymentRef = sPaymentReference.IfEmpty(c_Mem.PaymentReference);
                            if (c_Data.PaymentRef.IsSameValue("-")) c_Data.PaymentRef = "";

                            c_Mem.Delete();
                        }
                    }
                }

                // Set the working data
                env.JETFileInfo.SetStatus(JETEngineClass.ReturnTypes.Received);
                env.JETFileInfo.SetJobStatus("Received");
                env.JETFileInfo.SetHeartBeat(JETFileHBClass.InMinutes(60));
                env.JETFileInfo.RawJSON = c_Data;
                env.JETFileInfo.PacketType = c_Data.PacketType;

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion

        #region Support
        private JObject DocumentTitles { get; set; }
        private List<DocReferenceClass> Attachments { get; set; } = new List<DocReferenceClass>();

        private DocReferenceClass UFO { get; set; }

        private string GetAttachmentType(DocReferenceClass doc)
        {
            string sAns = "LEGAL DOCS";

            string sTitle = doc.ShortName.ToUpper();
            if (this.DocumentTitles.Contains(sTitle))
            {
                sAns = this.DocumentTitles.Get(sTitle);
            }

            return sAns;
        }

        private int ComputeTypeIndex(DocReferenceClass doc)
        {
            int iAns = 999;

            switch (this.GetAttachmentType(doc))
            {
                case "IBR":
                    iAns = 1;
                    break;

                case "IMR":
                    iAns = 2;
                    break;

                case "LEGAL DOCS":
                    iAns = 3;
                    break;

                case "LIENS AND BILLS":
                    iAns = 4;
                    break;

                case "MEDICAL DOCS":
                    iAns = 5;
                    break;

                case "MISC":
                    iAns = 6;
                    break;
            }

            return iAns;
        }
        #endregion
    }
}
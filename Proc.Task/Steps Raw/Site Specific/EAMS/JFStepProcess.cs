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
    public class JFStepProcessIn : CommandClass
    {
        #region Constants
        private const string ArgFilename = "file";


        private const string ElePayload = "DwcPacketPayload";
        private const string EleResp1 = "EAMSPacketReceiveResponse";
        private const string EleResp2 = "EAMSPacketValidationResponse";
        private const string EleResp3 = "EAMSFilingResponse";

        private const string EleHeader = "DwcPacketHeader";
        private const string ElePID = "PacketId";

        private const string EleSRC = "SRC";

        private const string ElePTRX = "TRX";
        private const string ElePBAT = "BAT";
        private const string ElePXIE = "XIE";
        private const string ElePXID = "XID";

        private const string ElePBAC = "BAC";
        private const string ElePSAN = "SAN";
        private const string ElePRTN = "RTN";

        private const string ElePCRD = "CRD";
        private const string ElePSCN = "SCN";
        private const string ElePEXP = "EXP";
        private const string ElePCLV = "CLV";
        private const string ElePMPY = "MPY";

        private const string ElePRES = "RES";
        private const string ElePRCD = "RCD";
        private const string ElePMSG = "MSG";
        private const string ElePSTM = "STM";
        private const string ElePETM = "ETM";
        private const string ElePPRC = "PRC";
        private const string ElePTEL = "TEL";
        private const string ElePBRC = "BRC";
        private const string ElePCVR = "CVR";
        private const string ElePAVR = "AVR";

        private const string ElePAUT = "AUT";
        private const string ElePMID = "MID";
        private const string ElePAUC = "AUC";

        private const string EleERR = "ERR";

        private static string MessageDelimiter = "".RPad(20, "-");
        #endregion

        #region Constructor
        public JFStepProcessIn(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Processes inbound JETFile XML packet"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Optional, ArgFilename, "The file name", "[*l:doc]");

            }
        }

        public override string Command
        {
            get { return "_jetfile.process"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sDocname = args.GetDefined(ArgFilename);

            // Validate
            if (this.CheckValidity(ctx,
                                            ArgFilename, sDocname))
            {
                DocReferenceClass c_Doc = env.Documents[sDocname];
                if (c_Doc == null)
                {
                    env.Env.LogInfo("JFStepProcessIn: Invalid name - {0}".FormatString(sDocname));
                }
                else
                {
                    string sFilename = c_Doc.Name;

                    env.Env.LogInfo("JFStepProcessIn: Raw file name - {0}".FormatString(sFilename));

                    // Just the name form now on
                    sFilename = sFilename.GetFileNameFromPath();

                    env.Env.LogInfo("JFStepProcessIn: Working file name - {0}".FormatString(sFilename));

                    JETEngineClass.ReturnTypes eType = JETEngineClass.FileNameToReturnType(sFilename);

                    //env.Env.LogInfo("JETFILE PROCESS: {0} - {1}".FormatString(sFilename, eType));

                    // Is it a valid file?
                    if (eType != JETEngineClass.ReturnTypes.Unknown)
                    {
                        // Get the data
                        string sData = c_Doc.ValueAsString;

                        // Map the object
                        env.JETFileInfo.SetObject(arguments, sFilename);

                        // Do we have a valid submission ID?
                        if (env.JETFileInfo.Object == null)
                        {
                            env.Env.LogInfo("JFStepProcessIn: No submission found - {0}".FormatString(sFilename));

                            // Nope!  Move it elsewhere
                            using (DocReferenceClass c_Junk = DocReferenceClass.CreateUserDoc(env, c_Doc.Name, env.JETFileInfo.BadFolder))
                            {
                                c_Junk.Value = c_Doc.Value;
                            }
                        }
                        else
                        {
                            // Save the file
                            env.JETFileInfo.SetXMLPacket(eType, sFilename, sData);

                            // Set the holder
                            XMLDocumentClass c_Data = new XMLDocumentClass();

                            // Adjust if payment and non-parsable
                            if (eType == JETEngineClass.ReturnTypes.Payment && !c_Data.Parse(sData))
                            {
                                sData = "<DATA><TRX>" + sData + "</TRX></DATA>";
                            }

                            // Parse the data                            
                            if (c_Data.Parse(sData))
                            {
                                // Get the submission ID
                                string sSubmIDV = JETEngineClass.FileNameToSubmissionID(sFilename);

                                env.Env.LogInfo("JFStepProcessIn: {0} {1} - {2}".FormatString("SUBMID", sFilename, sSubmIDV));
                                env.Env.LogInfo("JFStepProcessIn: {0} {1} - {2}".FormatString("TYPE", sFilename, eType));

                                // Assume failure
                                ProcessReturnClass c_Ret = new ProcessReturnClass();

                                try
                                {
                                    // And process accordingly
                                    switch (eType)
                                    {
                                        case JETEngineClass.ReturnTypes.Payment:
                                            c_Ret = this.ProcessPayment(env, sFilename, c_Data);
                                            break;

                                        case JETEngineClass.ReturnTypes.L1:
                                            c_Ret = this.ProcessL1(env, sFilename, c_Data);
                                            break;

                                        case JETEngineClass.ReturnTypes.L2:
                                            c_Ret = this.ProcessL2(env, sFilename, c_Data);
                                            break;

                                        case JETEngineClass.ReturnTypes.L3:
                                            c_Ret = this.ProcessL3(env, sFilename, c_Data);
                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    env.Env.LogException("JFStepProcessIn: {0}".FormatString(sFilename), e);
                                }

                                // 
                                string sTS = env.JETFileInfo.SetStatus(eType, c_Ret.Ok.ToString());
                                env.JETFileInfo.SetMessage(eType, c_Ret.Message, sTS);
                                env.JETFileInfo.SetJobStatus(c_Ret.JobStatus);
                                env.JETFileInfo.SetADJ(c_Ret.ADJ);
                                env.JETFileInfo.SetPaymentConfirmation(c_Ret.PaymentConfirmation);
                                env.JETFileInfo.SetResubmissionID(c_Ret.ResubmissionID);
                                env.JETFileInfo.SetCCProvider(c_Ret.CCProvider);
                                env.JETFileInfo.SetCCNo(c_Ret.CCNo);
                                env.JETFileInfo.SetCCExp(c_Ret.CCExp);

                                env.Env.LogInfo("JFStepProcessIn: {0} {1} - {2}".FormatString("OK", sFilename, c_Ret.Ok));
                                env.Env.LogInfo("JFStepProcessIn: {0} {1} - {2}".FormatString("MSG", sFilename, c_Ret.Message));
                                env.Env.LogInfo("JFStepProcessIn: {0} {1} - {2}".FormatString("JOBSTATUS", sFilename, c_Ret.JobStatus));
                                env.Env.LogInfo("JFStepProcessIn: {0} {1} - {2}".FormatString("ADJ", sFilename, c_Ret.ADJ));

                                if (c_Ret.Ok == ProcessReturnClass.OKStates.Failed || c_Ret.NextHB.Mode == JETFileHBClass.Modes.None)
                                {
                                    env.JETFileInfo.ClearHeartBeat();
                                }
                                else
                                {
                                    env.JETFileInfo.SetHeartBeat(c_Ret.NextHB);
                                }
                            }
                            else
                            {
                                env.Env.LogInfo("JFStepProcessIn: Unable to parse file {0}".FormatString(sFilename));
                            }
                        }
                    }
                    else
                    {
                        env.Env.LogInfo("JFStepProcessIn: Unknown file type - {0}".FormatString(sFilename));
                    }

                    // Delete
                    c_Doc.Delete();
                }
            }
            else
            {
                env.Env.LogInfo("JFStepProcessIn: Missing file name");
            }

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion

        #region Methods
        private string AddLineIf(StringBuilder buffer,
                                        string label,
                                        XMLElementClass ele,
                                        string item)
        {
            string sAns = null;

            if (ele.ElementExists(item))
            {
                sAns = ele[item].Value.IfEmpty();

                buffer.AppendLine(label.IfEmpty() + ": " + sAns);
            }
            return sAns;
        }

        private void TryToExplain(StringBuilder buffer,
                                    string code,
                                    string message,
                                    string details)

        {
            int bSize = buffer.Length;
            string sSnazzy = "Explanation:";

            try
            {
                if (details.IndexOf("cvc-complex-type") != -1)
                {
                    Match c_Match = Regex.Match(details, @"'(?<field>[^\s]*)@");
                    string sValue = c_Match.Groups["field"].Value;

                    string sSection = null;
                    Match c_Section = Regex.Match(details, @"before the end of the content in element (?<field>[^\s]*)@");
                    if (c_Section.Success)
                    {
                        sSection = c_Match.Groups["field"].Value;
                        if (sSection.HasValue() && !sValue.IsSameValue(sSection))
                        {
                            sValue += " in " + sSection;
                        }
                    }
                    else
                    {
                        c_Section = Regex.Match(details, @" here in element (?<field>[^\s]*)@");
                        if (c_Section.Success)
                        {
                            sSection = c_Match.Groups["field"].Value;
                            if (sSection.HasValue())
                            {
                                sValue += " in " + sSection;
                            }
                        }
                    }

                    string sEnding = "";
                    if (sValue.HasValue()) if ("aeiou".IndexOf(sValue.Substring(0, 1), StringComparison.InvariantCultureIgnoreCase) != -1) sEnding = "n";

                    if (sValue.IsSameValue("SCN")) sValue += ", Check your credit card/check information";
                    if (sValue.IsSameValue("employerName"))
                    {
                        if (!sSection.HasValue())
                        {
                            sValue += ", Check all the names as 'employerName' is a misnomer";
                        }
                    }
                    if (sValue.IsSameValue("DocTitle")) sValue += ", Check document names to make sure that they are valid";

                    if (c_Match.Success) buffer.Append(string.Format("{0} You are missing a{2} {1}\r\n", sSnazzy, sValue, sEnding));
                }
                else if (details.IndexOf("cvc-datatype-valid") != -1)
                {
                    Match c_Match = Regex.Match(details, @"(?<value>'[^']*')\sdoes\snot\smatch\spattern\sfor\stype\sof\s(?<field>[^\s]*)");
                    if (!c_Match.Success) c_Match = Regex.Match(details, @"(?<value>'[^']*')\sdoes\snot\smatch\spattern\sfor\s(?<field>[^\s]*)");

                    if (c_Match.Success)
                    {
                        string sWhat = c_Match.Groups["value"].Value;
                        string sValue = c_Match.Groups["field"].Value;

                        if (sWhat.IsSameValue("''"))
                        {
                            string sEnding = "";
                            if (sValue.HasValue()) if ("aeiou".IndexOf(sValue.Substring(0, 1), StringComparison.InvariantCultureIgnoreCase) != -1) sEnding = "n";

                            buffer.Append(string.Format("{0} You are missing a{2} {1}\r\n", sSnazzy, sValue, sEnding));
                        }
                        else
                        {
                            buffer.Append(string.Format("{0} The value of {1} is not valid for {2}\r\n", sSnazzy, sWhat, sValue));
                        }
                    }
                }
                else if (details.IndexOf("cvc-maxLength-valid") != -1)
                {
                    Match c_Match = Regex.Match(details, @"\x28(?<have>\d+)\x29.*\x28(?<max>\d+)\x29\sfor\stype\sof\s(?<field>[^\s]*)");
                    if (!c_Match.Success)
                    {
                        c_Match = Regex.Match(details, @"length\s\x28(?<have>\d+)\x29.*facet\s\x28(?<max>\d+)\x29\sfor\s(?<field>[^\s]*)");
                    }

                    if (c_Match.Success)
                    {
                        buffer.Append(string.Format("{0} {1} cannot be longer than {2} character(s), you have {3}\r\n", sSnazzy, c_Match.Groups["field"].Value, c_Match.Groups["max"].Value, c_Match.Groups["have"].Value));
                    }
                    else
                    {
                        buffer.Append(string.Format("{0} There is a problem with the number of character(s) in a field\r\n", sSnazzy));
                    }
                }
                //else if (details.IndexOf("cvc-minLength-valid") != -1)
                //{
                //    Match c_Match = Regex.Match(details, @"length\s\x28(?<have>\d+)\x29.*facet\s\x28(?<max>\d+)\x29\sfor\s(?<field>[^\s]*)");
                //    if(c_Match.Success)
                //    {
                //        buffer.Append(string.Format("{0} {1} cannot be smaller than {2} character(s), you have {3}\r\n", sSnazzy, c_Match.Groups["field"].Value, c_Match.Groups["max"].Value, c_Match.Groups["have"].Value));
                //    }
                //    else
                //    {
                //        buffer.Append(string.Format("{0} There is a problem with the number of character(s) in a field.", sSnazzy));
                //    }
                //}
                else if (details.IndexOf("cvc-minLength-valid") != -1)
                {
                    Match c_Match = Regex.Match(details, @"\x28(?<have>\d+)\x29.*\x28(?<max>\d+)\x29\sfor\stype\sof\s(?<field>[^\s]*)");

                    if (c_Match.Success)
                    {
                        buffer.Append(string.Format("{0} {1} cannot be shorter than {2} characters, you have {3}\r\n", sSnazzy, c_Match.Groups["field"].Value, c_Match.Groups["max"].Value, c_Match.Groups["have"].Value));
                    }
                    else
                    {
                        c_Match = Regex.Match(details, @"is\sless\sthan\sminLength\sfacet\s\x28\d+\x29\sfor\s(?<field>[^\s]*)");

                        if (c_Match.Success)
                        {
                            buffer.Append(string.Format("{0} {1} is missing text\r\n", sSnazzy, c_Match.Groups["field"].Value));
                        }
                        else
                        {
                            buffer.Append(string.Format("{0} There is a problem with the number of character(s) in a field\r\n", sSnazzy));
                        }
                    }
                }
                else if (details.IndexOf("There is no integrated case for this claimant") != -1)
                {
                    buffer.Append(string.Format("{0} The case is not fully active in the DIR/DWC databases. Please contact Support\r\n", sSnazzy));
                }
                else if (details.IndexOf("This case has been archived") != -1)
                {
                    buffer.Append(string.Format("{0} The case is not active in the DIR/DWC databases.  Please contact Support\r\n", sSnazzy));
                }
                else if (details.IndexOf("Invalid double value") != -1)
                {
                    Match c_Match = Regex.Match(details, @"error:\sdouble:\sInvalid\sdouble\svalue:(?<value>.*)");
                    if (c_Match.Success)
                    {
                        string sValue = "";
                        try
                        {
                            if (c_Match.Groups["value"] != null) sValue = c_Match.Groups["value"].Value;
                        }
                        catch { }
                        if (!sValue.HasValue()) sValue = "Value entered";
                        buffer.Append(string.Format("{0} {1} is not a valid number\r\n", sSnazzy, sValue.IfEmpty()));
                    }
                    else
                    {
                        buffer.Append(string.Format("{0} A field that is supposed to be a number is not. Use whole numbers\r\n", sSnazzy));
                    }
                }
                else if (message.IndexOf("Conditional Mandatory") != -1 ||
                            message.IndexOf("Invalid Uniform Assigned Name") != -1 ||
                            details.IndexOf("This is a duplicate application") != -1)
                {
                    buffer.Append(string.Format("{0} See Details below\r\n", sSnazzy));
                }
            }
            catch { }

            if (bSize == buffer.Length)
            {
                buffer.Append(string.Format("{0} See Details below\r\n", sSnazzy));
            }

            buffer.AppendLine("Code: {0}".FormatString(code));
            buffer.AppendLine("Msg: {0}".FormatString(message));
            buffer.AppendLine("Details: {0}".FormatString(details));

            buffer.AppendLine();
        }

        private bool CheckPayment(ProcessReturnClass ret, XMLElementClass ele)
        {
            bool bFound = false;

            ret.Ok = ProcessReturnClass.OKStates.Failed;

            if (ele.ElementExists(ElePXID))
            {
                string sXID = ele[ElePXID].Value;

                StringBuilder c_Msg = new StringBuilder();

                c_Msg.AppendLine("Payment Notification");

                if (sXID.HasValue())
                {
                    c_Msg.AppendLine("Confirmation ID: " + sXID);
                    ret.PaymentConfirmation = sXID;
                }

                c_Msg.AppendLine(MessageDelimiter);

                if (ele.ElementExists(ElePBAC))
                {
                    XMLElementClass c_Sub = ele[ElePBAC];
                    if (c_Sub != null)
                    {
                        c_Msg.AppendLine("Using Bank Account");
                        this.AddLineIf(c_Msg, "Account", c_Sub, ElePSAN);
                        this.AddLineIf(c_Msg, "Routing", c_Sub, ElePRTN);
                        c_Msg.AppendLine("");
                    }
                }

                if (ele.ElementExists(ElePCRD))
                {
                    XMLElementClass c_Sub = ele[ElePCRD];
                    if (c_Sub != null)
                    {
                        c_Msg.AppendLine("Using Credit Card");
                        ret.CCProvider = this.AddLineIf(c_Msg, "Card Provider", ele, ElePMPY);
                        ret.CCNo = this.AddLineIf(c_Msg, "Card Number", c_Sub, ElePSCN);
                        ret.CCExp = this.AddLineIf(c_Msg, "Exp", c_Sub, ElePEXP);
                        this.AddLineIf(c_Msg, "Card Level", c_Sub, ElePCLV);
                        c_Msg.AppendLine("");
                    }
                }

                this.AddLineIf(c_Msg, "Code", ele, ElePRCD);
                string sMsg = this.AddLineIf(c_Msg, "Message", ele, ElePMSG);
                this.AddLineIf(c_Msg, "Start time", ele, ElePSTM);
                this.AddLineIf(c_Msg, "End time", ele, ElePETM);
                this.AddLineIf(c_Msg, "Processor", ele, ElePPRC);
                this.AddLineIf(c_Msg, "Telephone", ele, ElePTEL);
                this.AddLineIf(c_Msg, "Bank response code", ele, ElePBRC);
                this.AddLineIf(c_Msg, "CVR response code", ele, ElePCVR);
                this.AddLineIf(c_Msg, "AVS response code", ele, ElePAVR);

                if (ele.ElementExists(ElePRES))
                {
                    XMLElementClass c_Sub = ele[ElePRES];
                    if (c_Sub != null)
                    {
                        string sCode = c_Sub[ElePRCD].Value;
                        if (sCode.IsSameValue("0")) ret.Ok = ProcessReturnClass.OKStates.Success;

                        this.AddLineIf(c_Msg, "Code", c_Sub, ElePRCD);
                        this.AddLineIf(c_Msg, "Message", c_Sub, ElePMSG);
                        this.AddLineIf(c_Msg, "Start time", c_Sub, ElePSTM);
                        this.AddLineIf(c_Msg, "End time", c_Sub, ElePETM);
                        this.AddLineIf(c_Msg, "Processor", c_Sub, ElePPRC);
                        this.AddLineIf(c_Msg, "Telephone", c_Sub, ElePTEL);
                        this.AddLineIf(c_Msg, "Bank response code", c_Sub, ElePBRC);
                        this.AddLineIf(c_Msg, "CVR response code", c_Sub, ElePCVR);
                        this.AddLineIf(c_Msg, "AVS responce code", c_Sub, ElePAVR);
                        c_Msg.AppendLine("");
                    }
                }

                if (ele.ElementExists(ElePAUT))
                {
                    XMLElementClass c_Sub = ele[ElePAUT];
                    if (c_Sub != null)
                    {
                        this.AddLineIf(c_Msg, "Merchant ID", c_Sub, ElePMID);
                        this.AddLineIf(c_Msg, "Authorization code", c_Sub, ElePAUC);
                        c_Msg.AppendLine("");
                    }
                }

                c_Msg.AppendLine(ret.Ok == ProcessReturnClass.OKStates.Failed ? "!! Payment failed !!" : "Payment posted");

                if (sMsg.HasValue())
                {
                    if (sMsg.IndexOf("Missing", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        c_Msg.AppendLine("\r\nExplanation: You are missing a credit card");
                    }
                    else if (sMsg.IndexOf("Invalid", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        c_Msg.AppendLine("\r\nExplanation: The credit card provided is invalid");
                    }
                    else if (sMsg.IndexOf("Declined", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        c_Msg.AppendLine("\r\nExplanation: The credit card was declined");
                    }
                    else if (sMsg.IndexOf("Expired", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        c_Msg.AppendLine("\r\nExplanation: The credit card has expired");
                    }
                }

                ret.Message = c_Msg.ToString();
            }
            else
            {
                foreach (string sChild in ele.ElementNames)
                {
                    bFound = this.CheckPayment(ret, ele[sChild]);
                    if (bFound) break;
                }
            }
            return bFound;
        }

        private ProcessReturnClass ProcessPayment(SandboxClass env, string filename, XMLDocumentClass data)
        {
            ProcessReturnClass c_Ans = new ProcessReturnClass();

            // Handle not matter format
            this.CheckPayment(c_Ans, data.RootElement);

            // Save the confirmation ID
            using (JETFileMemoryClass c_Mem = new JETFileMemoryClass(env.Associate,
                                                                            env.JETFileInfo.GetSite(),
                                                                            env.JETFileInfo.RawJSON.ADJ,
                                                                            env.JETFileInfo.PacketType.ToString()))
            {
                c_Mem.PaymentReference = c_Ans.PaymentConfirmation;
                c_Mem.Put();
            }

            c_Ans.JobStatus = c_Ans.Ok == ProcessReturnClass.OKStates.Success ? "Paid" : "Failed";
            c_Ans.NextHB = JETFileHBClass.InMinutes(40);

            return c_Ans;
        }

        private ProcessReturnClass ProcessJETFile(SandboxClass env, string filename, XMLDocumentClass data)
        {
            ProcessReturnClass c_Ans = new ProcessReturnClass();

            try
            {
                StringBuilder c_Msg = new StringBuilder();
                //JETEngineClass.ReturnTypes eStage = JETEngineClass.ReturnTypes.Unknown;

                if (data.RootElement != null)
                {
                    XMLElementClass c_Payload = data.RootElement[ElePayload];
                    if (c_Payload != null)
                    {
                        bool bEnded = false;

                        try
                        {
                            List<string> c_Items = c_Payload.ElementNames;
                            if (c_Items.Count == 1)
                            {
                                if (EleResp1.IsSameValue(c_Items[0]))
                                {
                                    //eStage = JETEngineClass.ReturnTypes.L1;

                                    c_Msg.AppendLine("Packet received");
                                    c_Msg.AppendLine(MessageDelimiter);

                                    XMLElementClass c_Sub = c_Payload.Node(@"\EAMSPacketReceiveResponse");
                                    if (c_Sub != null)
                                    {
                                        List<XMLElementClass> c_Values = c_Sub.ElementList("Message");
                                        foreach (XMLElementClass c_Value in c_Values)
                                        {
                                            c_Msg.AppendLine(c_Value.Value);
                                        }
                                    }

                                    c_Sub = data.RootElement["DwcPacketExceptions"];
                                    if (c_Sub != null)
                                    {
                                        List<XMLElementClass> c_Values = c_Sub.ElementList("Exception");
                                        if (c_Values.Count > 0)
                                        {
                                            c_Msg.AppendLine(MessageDelimiter);
                                            c_Msg.AppendLine("!! Errors !!");
                                            foreach (XMLElementClass c_Value in c_Values)
                                            {
                                                this.TryToExplain(c_Msg,
                                                                    c_Value.ElementValueIf("ErrorCode"),
                                                                    c_Value.ElementValueIf("ErrorPrimary"),
                                                                    c_Value.ElementValueIf("ErrorSecondary"));

                                                bEnded = true;
                                            }
                                        }
                                    }

                                    c_Ans.Message = c_Msg.ToString();
                                    c_Ans.Ok = !bEnded ? ProcessReturnClass.OKStates.Success : ProcessReturnClass.OKStates.Failed;

                                }
                                else if (EleResp2.IsSameValue(c_Items[0]))
                                {
                                    //eStage = JETEngineClass.ReturnTypes.L2;

                                    c_Msg.AppendLine("Packet validation");
                                    c_Msg.AppendLine(MessageDelimiter);

                                    XMLElementClass data2 = c_Payload.Node(@"\EAMSPacketValidationResponse");
                                    if (data2 != null)
                                    {
                                        XMLElementClass c_Sub1 = data2.Node(@"\HeaderSection\Acknowledgement");
                                        if (c_Sub1 != null)
                                        {
                                            c_Msg.AppendLine(c_Sub1.Value);
                                        }

                                        XMLElementClass c_Sub = data2.Node(@"\Errors\Transactions\Transaction\TransactionErrors");
                                        if (c_Sub != null)
                                        {
                                            List<XMLElementClass> c_Values = c_Sub.ElementList("TransactionError");
                                            if (c_Values.Count > 0)
                                            {
                                                c_Msg.AppendLine(MessageDelimiter);
                                                c_Msg.AppendLine("!! Errors !!");
                                                foreach (XMLElementClass c_Value in c_Values)
                                                {
                                                    string sSecMsg = c_Value.ElementValueIf("ErrorSecondary");

                                                    this.TryToExplain(c_Msg,
                                                                        c_Value.ElementValueIf("ErrorCode"),
                                                                        c_Value.ElementValueIf("ErrorPrimary"),
                                                                        sSecMsg);

                                                    bEnded = true;
                                                }
                                            }
                                        }

                                        c_Sub = data2.Node(@"\Errors\Transactions\Transaction\Forms\Form\FormErrors");
                                        if (c_Sub != null)
                                        {
                                            List<XMLElementClass> c_Values = c_Sub.ElementList("FormError");
                                            if (c_Values.Count > 0)
                                            {
                                                c_Msg.AppendLine(MessageDelimiter);
                                                c_Msg.AppendLine("!! Errors !!");
                                                foreach (XMLElementClass c_Value in c_Values)
                                                {
                                                    this.TryToExplain(c_Msg,
                                                                        c_Value.ElementValueIf("ErrorCode"),
                                                                        c_Value.ElementValueIf("ErrorPrimary"),
                                                                        c_Value.ElementValueIf("ErrorSecondary"));

                                                    bEnded = true;
                                                }
                                            }
                                        }

                                        XMLElementClass c_RForms = data2.Node(@"\Errors\Transactions\Transaction\Forms");
                                        if (c_RForms != null)
                                        {
                                            List<XMLElementClass> c_Forms = c_RForms.ElementList(@"Form");
                                            if (c_Forms != null)
                                            {
                                                foreach (XMLElementClass c_Form in c_Forms)
                                                {
                                                    string sFName = "Unknown form";
                                                    XMLElementClass c_FName = c_Form.Node(@"\FormShortName");
                                                    if (c_FName != null) sFName = c_FName.Value;

                                                    XMLElementClass c_RID = c_Form.Node(@"\ResubmissionID");
                                                    if (c_RID != null)
                                                    {
                                                        using (JETFileMemoryClass c_Mem = new JETFileMemoryClass(env.Associate,
                                                                                                env.JETFileInfo.GetSite(),
                                                                                                env.JETFileInfo.RawJSON.ADJ,
                                                                                                env.JETFileInfo.PacketType.ToString()))
                                                        {
                                                            c_Mem.ResubmissionID = c_RID.Value;
                                                            c_Mem.Put();

                                                            c_Ans.ResubmissionID = c_RID.Value; ;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    c_Ans.Message = c_Msg.ToString();
                                    c_Ans.Ok = !bEnded ? ProcessReturnClass.OKStates.Success : ProcessReturnClass.OKStates.Failed;
                                }
                                else if (EleResp3.IsSameValue(c_Items[0]))
                                {
                                    //eStage = JETEngineClass.ReturnTypes.L3;
                                    //bEnded = true;

                                    c_Msg.AppendLine("Filing");
                                    c_Msg.AppendLine(MessageDelimiter);

                                    // Assume the best
                                    c_Ans.Ok = ProcessReturnClass.OKStates.Success;

                                    XMLElementClass c_Sub = c_Payload.Node(@"\EAMSFilingResponse\Summary\Transactions\Transaction\Forms");
                                    if (c_Sub != null)
                                    {
                                        List<XMLElementClass> c_Values = c_Sub.ElementList("Form");
                                        foreach (XMLElementClass c_Value in c_Values)
                                        {
                                            string sADJ = c_Value.ElementValueIf("CaseNumber");

                                            c_Msg.AppendLine("Form: " + c_Value.ElementValueIf("FormShortName"));
                                            c_Msg.AppendLine("Msg: " + c_Value.ElementValueIf("FormMessage"));
                                            c_Msg.AppendLine("Filing Date: " + c_Value.ElementValueIf("FiledDate"));
                                            c_Msg.AppendLine("Case#: " + sADJ);

                                            c_Ans.ADJ = sADJ;
                                        }
                                    }

                                    bool bInPendingQueue = false;

                                    c_Sub = c_Sub.Node(@"\Errors\Transactions\Transaction\Forms\Form\ErrorList");
                                    if (c_Sub != null)
                                    {
                                        List<XMLElementClass> c_Values = c_Sub.ElementList("Error");
                                        if (c_Values.Count > 0)
                                        {
                                            // But not
                                            c_Ans.Ok = ProcessReturnClass.OKStates.Failed;

                                            c_Msg.AppendLine(MessageDelimiter);
                                            c_Msg.AppendLine("!! Errors !!");
                                            foreach (XMLElementClass c_Value in c_Values)
                                            {
                                                string sSecMsg = c_Value.ElementValueIf("ErrorSecondary");
                                                string sCode = c_Value.ElementValueIf("ErrorCode");

                                                this.TryToExplain(c_Msg,
                                                                    sCode,
                                                                    c_Value.ElementValueIf("ErrorPrimary"),
                                                                    sSecMsg);

                                                if (sCode.IsSameValue("30020")) bInPendingQueue = true;

                                                //bEnded = true;
                                            }
                                        }
                                    }

                                    //
                                    if (bInPendingQueue) c_Ans.Ok = ProcessReturnClass.OKStates.Pending;

                                    // Always save message
                                    c_Ans.Message = c_Msg.ToString();

                                    if (c_Ans.Ok == ProcessReturnClass.OKStates.Success)
                                    {
                                        // Cannot resubmit
                                        using (JETFileMemoryClass c_Mem = new JETFileMemoryClass(env.Associate,
                                                                                                env.JETFileInfo.GetSite(),
                                                                                                env.JETFileInfo.RawJSON.ADJ,
                                                                                                env.JETFileInfo.PacketType.ToString()))
                                        {
                                            c_Mem.Delete();
                                        }
                                    }

                                    if ((bInPendingQueue || c_Ans.Ok == ProcessReturnClass.OKStates.Success) && env.JETFileInfo.RawJSON.eServe)
                                    {
                                        // TBD

                                        env.JETFileInfo.SeteServe();
                                    }

                                    // And set the deletion date
                                    env.JETFileInfo.SetDeletion();
                                }
                            }
                        }
                        catch { }
                    }

                    XMLElementClass c_ExtraErrors = data.RootElement["DwcPacketExceptions"];
                    if (c_ExtraErrors != null)
                    {
                        List<XMLElementClass> c_Values = c_ExtraErrors.ElementList("Exception");
                        foreach (XMLElementClass c_Value in c_Values)
                        {
                            this.TryToExplain(c_Msg,
                                                c_Value.ElementValueIf("ErrorCode"),
                                                c_Value.ElementValueIf("ErrorPrimary"),
                                                c_Value.ElementValueIf("ErrorSecondary"));
                        }
                    }
                }
            }
            catch (Exception e1)
            {
                c_Ans.Message = "Internal Error: {0}".FormatString(e1.Message);
                c_Ans.Ok = ProcessReturnClass.OKStates.Failed;

                env.Env.LogInfo("ProcessJETFile: {0}".FormatString(c_Ans.Message));
            }

            return c_Ans;
        }

        private ProcessReturnClass ProcessL1(SandboxClass env, string filename, XMLDocumentClass data)
        {
            ProcessReturnClass c_Ans = this.ProcessJETFile(env, filename, data);

            c_Ans.JobStatus = c_Ans.Ok == ProcessReturnClass.OKStates.Success ? "Acknowledged" : "Failed";
            c_Ans.NextHB = JETFileHBClass.InMinutes(15);

            return c_Ans;
        }

        private ProcessReturnClass ProcessL2(SandboxClass env, string filename, XMLDocumentClass data)
        {
            ProcessReturnClass c_Ans = this.ProcessJETFile(env, filename, data);

            c_Ans.JobStatus = c_Ans.Ok == ProcessReturnClass.OKStates.Success ? "Validated" : "Failed";

            c_Ans.NextHB = JETFileHBClass.InBusinessDays(1);

            return c_Ans;
        }

        private ProcessReturnClass ProcessL3(SandboxClass env, string filename, XMLDocumentClass data)
        {
            ProcessReturnClass c_Ans = this.ProcessJETFile(env, filename, data);

            switch (c_Ans.Ok)
            {
                case ProcessReturnClass.OKStates.Success:
                    c_Ans.JobStatus = "Filed";
                    break;

                case ProcessReturnClass.OKStates.Failed:
                    c_Ans.JobStatus = "Failed";
                    break;

                case ProcessReturnClass.OKStates.Pending:
                    c_Ans.JobStatus = "In Pending Queue";
                    break;
            }
            c_Ans.NextHB = JETFileHBClass.NoMore();

            return c_Ans;
        }
        #endregion

        public class ProcessReturnClass
        {
            #region Properties
            public OKStates Ok { get; set; } = OKStates.Success;
            public string Message { get; set; } = "";
            public string JobStatus { get; set; } = "Failed";
            public JETFileHBClass NextHB { get; set; }
            public string ADJ { get; set; } = "";
            public string PaymentConfirmation { get; set; } = "";
            public string ResubmissionID { get; set; } = "";

            public string CCProvider { get; set; } = "";
            public string CCNo { get; set; } = "";
            public string CCExp { get; set; } = "";
            #endregion

            #region Enums
            public enum OKStates
            {
                Success,
                Failed,
                Pending
            }
            #endregion
        }
    }
}
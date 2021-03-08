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
    public class DEReceive : DEBase
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgID = "as";
        private const string ArgName = "obj";
        private const string ArgCC = "cc";
        private const string ArgMergeData = "md";
        private const string ArgMDFolder = "mdfolder";
        private const string ArgSetParent = "setparent";
        private const string ArgAsParent = "asparent";
        private const string ArgMerge = "mergestore";

        private const string ArgSvcUUID = "svcuuid";
        private const string ArgSvcUser = "svcuser";

        private const string ArgSvcInfo = "svc";
        private const string ArgLink = "link";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DEReceive(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Receives an object from remote"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required, ArgName, "The name of the object",
            CommandClass.Optional, ArgDS, "The dataset of the object",
            CommandClass.Optional, ArgID, "The pseudo id of the object (force)",
            CommandClass.Optional, ArgMergeData, "Get documents merge data", "#0#",
            CommandClass.Optional, ArgMDFolder, "Get documents merge data as document in folder",
            CommandClass.Optional, ArgSetParent, "Set parent as based object ID", "#0#",
            //
            //this.DEDocumentation(c_Def);
            //
            CommandClass.Optional, ArgSvcUUID, "Sender UUID field",
            CommandClass.Optional, ArgSvcUser, "Sender User field",
            //
            CommandClass.Optional, ArgCC, "Get credit card info", "#0#",
            //
            CommandClass.Optional, ArgSvcInfo, "Service info to use",
            CommandClass.Optional, ArgLink, "Create Chat link");

            }
        }

        public override string Command
        {
            get { return "de.recv"; }
        }

        public override ReturnClass ExecStep(ChoreContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, args);

            // Get the parameters
            string sDS = args.GetDefined(ArgDS);
            string sID = args.GetDefined(ArgID);
            string sName = args.GetDefined(ArgName);
            bool bCC = args.GetDefinedAsBool(ArgCC, false);
            bool bMergeData = args.GetDefinedAsBool(ArgMergeData, false);
            string sMDFolder = args.GetDefined(ArgMDFolder);
            string sSvcUUID = args.GetDefinedRaw(ArgSvcUUID);
            string sSvcUser = args.GetDefinedRaw(ArgSvcUser);
            bool bAsParent = args.GetDefinedAsBool(ArgAsParent, false);
            bool bSetParent = args.GetDefinedAsBool(ArgSetParent, false);
            string sMerge = args.GetDefined(ArgMerge);

            string sSvcInfoID = args.GetDefined(ArgSvcInfo);
            bool bLink = args.GetDefinedAsBool(ArgLink, false);

            string sSource = Names.Passed;

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgName, sName))
            {
                // Get the parameters
                SvcInfoClass c_Info = ctx.Info.SvcInfo;
                if (c_Info == null)
                {
                    ctx.Parent.LogError("Missing service info");
                }
                else
                {
                    //ctx.ctx.LogInfo("SvcInfo: {0}".FormatString(c_Info.ToString()));

                    // Get the object
                    AO.ObjectClass c_Source = ctx.Objects[sSource];
                    if (c_Source != null)
                    {
                        // Default the name
                        sName = sName.IfEmpty(Names.Passed);

                        // Make sure we have a ds
                        if (!sDS.HasValue() && c_Info.ObjectUUID != null)
                        {
                            sDS = c_Info.ObjectUUID.Dataset.Name;
                        }

                        // Our target object
                        AO.ObjectClass c_Obj = null;
                        bool bIsNew = false;

                        if (sDS.HasValue())
                        {
                            // If we are the copy
                            if (!c_Info.UUID.IsSameValue(ctx.Parent.Hive.Name) || c_Info.ReturnToOriginal)
                            {
                                // Make the query
                                JObject c_Query = new JObject();

                                // Forced?
                                if (c_Info.ReturnToOriginal)
                                {
                                    ctx.Objects.Add(sName, c_Info.ObjectUUID);
                                    c_Query.Set("_id", c_Info.ObjectUUID.ID);
                                    //ctx.ctx.LogInfo("Query by _ID: {0}:{1}".FormatString(sDS, c_Info.ObjectUUID.ID));
                                }
                                else if (sID.HasValue())
                                {
                                    c_Query.Set("_forced", sID);
                                    //ctx.ctx.LogInfo("Query by _FORCED: {0}:{1}".FormatString(sDS, sID));
                                }
                                else
                                {
                                    c_Query.Set("_basedon", c_Info.BasedOn(sSvcInfoID));
                                    //ctx.ctx.LogInfo("Query by _BASEDON: {0}:{1}".FormatString(sDS, c_Info.BasedOn(sSvcInfoID)));
                                }

                                // Store URL
                                //ctx.User.Caller.SetDEUrl(c_Info.UUID, c_Info.URL);
                                // Query
                                ctx.User.Caller.ObjectGet(sDS, c_Query, delegate (string id)
                               {
                                   // Any?
                                   if (id.HasValue())
                                   {
                                       // Set 
                                       ctx.Objects.Add(sName, new AO.UUIDClass(sDS, id, ""));
                                       // Into memory
                                       c_Obj = ctx.Objects[sName];

                                       //ctx.ctx.LogInfo("FOUND: {0}".FormatString(id));
                                   }
                                   else
                                   {
                                       // Create object
                                       c_Obj = ctx.User.CreateObject(sDS, AO.ObjectClass.Types.Data);

                                       // Make into working object
                                       ctx.Objects.Add(sName, c_Obj);

                                       //ctx.ctx.LogInfo("CREATED: {0}".FormatString(c_Obj.UUID.ToString()));

                                       bIsNew = true;
                                   }
                               });

                                // And store the call
                                if (c_Obj != null && !c_Info.ReturnToOriginal)
                                {
                                    if (sID.HasValue()) c_Obj.Set("_forced", sID);
                                    if (!c_Obj["_basedon"].HasValue()) c_Obj.Set("_basedon", c_Info.BasedOn(sSvcInfoID));
                                    if (!c_Obj.SvcInfoGet(sSvcInfoID).HasValue()) c_Obj.SvcInfoSet(sSvcInfoID, c_Info.ToString());

                                    if (bAsParent)
                                    {
                                        c_Obj["_parent"] = c_Info.ObjectID;
                                    }

                                    // Set the wallet
                                    if (bCC && c_Info.Wallet != null)
                                    {
                                        c_Info.Wallet.Give(c_Obj);
                                    }
                                }
                            }
                            else
                            {
                                // Use what was passed back
                                c_Obj = ctx.Objects[Names.Passed];
                            }
                        }
                        else
                        {
                            ctx.ctx.LogError("No dataset provided");
                        }

                        if (c_Obj != null && !bIsNew)
                        {
                            bIsNew = c_Obj.IsNew;
                        }

                        // Do we have a valid object?
                        if (c_Obj != null)
                        {
                            // Apply changes
                            JObject c_Changes = ctx.Stores[Names.Changes];

                            // Map
                            c_Changes = this.MapPacket(env, args, c_Changes, c_Obj.Dataset);

                            if (bSetParent)
                            {
                                string sBO = c_Changes.Get("_basedon");
                                if (sBO.HasValue())
                                {
                                    AO.BasedOnUUIDClass c_BUUID = new BasedOnUUIDClass(sBO);
                                    if (ctx.ctx.UUID.IsSameValue(c_BUUID.Site))
                                    {
                                        c_Obj["_parent"] = c_BUUID.ToShortString();
                                    }
                                }
                            }

                            foreach (string sKey in c_Changes.Keys())
                            {
                                c_Obj.Set(sKey, c_Changes.Get(sKey));
                            }

                            if (sSvcUUID.HasValue()) c_Obj.Set(sSvcUUID, c_Info.UUID);
                            if (sSvcUser.HasValue()) c_Obj.Set(sSvcUser, c_Info.User);

                            // Merge store
                            if (sMerge.HasValue())
                            {
                                JObject c_MStore = ctx.Stores[sMerge];
                                if (c_MStore != null)
                                {
                                    foreach (string sKey in c_MStore.Keys())
                                    {
                                        c_Obj.Set(sKey, c_MStore.Get(sKey));
                                    }
                                }
                            }

                            // Copy attachments
                            if (c_Info.AttachedDocs.Count > 0)
                            {
                                // Get Attachments
                                JObject c_Att = c_Info.AttachedDocs;
                                foreach (string sFile in c_Att.Keys())
                                {
                                    // Get the path
                                    string sPath = c_Obj.DocumentPath(c_Info.AttachedFolder, sFile);
                                    c_Info.AttachedDocs.Get(sFile).URLGetFile(sPath);

                                    // Merge data?
                                    if (bMergeData)
                                    {
                                        // Get the path
                                        sPath = c_Obj.DocumentDataPath(c_Info.AttachedFolder, sFile);
                                        c_Info.AttachedDocs.Get(sFile).DocumentDataPath().URLGetFile(sPath);
                                    }

                                    if (sMDFolder.HasValue())
                                    {
                                        // Get the path
                                        sPath = c_Obj.DocumentPath(sMDFolder, sFile.GetFileNameOnlyFromPath() + ".json");
                                        c_Info.AttachedDocs.Get(sFile).DocumentDataPath().URLGetFile(sPath);
                                    }
                                }
                            }

                            // Save
                            ctx.Objects.Save(c_Obj, false);

                            if (c_Info.Link && bLink)
                            {
                                JObject c_Resp = ctx.User.Caller.PortalCall("DEX_Set",
                                                                        "objuuid", ctx.ctx.VUUID,
                                                                        "objid", c_Obj.UUID.ToString(),
                                                                        "tgtuuid", c_Info.UUID,
                                                                        "tgtid", c_Info.ObjectUUID.ToString(),
                                                                        "cmd", "setfwd");
                            }

                            this.Bill(env, ChargeClass.DataExchangeReceive(ctx.User.Caller, c_Info.UUID, c_Info.Dataset));

                            if (bIsNew)
                            {
                                this.Bill(env, ChargeClass.DataExchangeCreate(ctx.User.Caller, c_Info.UUID, c_Info.Dataset));
                            }

                            if (c_Info.ExternalCharges.Count > 0)
                            {
                                ctx.User.Caller.PortalCallBill(ctx.User.UserID, c_Info.ExternalCharges.AsList.ToArray());
                            }

                            //this.FlagDEOp(env, ctx.ctx.UUID, c_Info.UUID, c_Info.ObjectID, "remote", c_Obj.UUID.ToString());

                            eAns = ReturnClass.OK;
                        }
                        else
                        {
                            ctx.ctx.LogError("No object provided");
                        }
                    }
                    else
                    {
                        ctx.ctx.LogError("Unable to get source object");
                    }
                }
            }
            return eAns;
        }
        #endregion
    }
}
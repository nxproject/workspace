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
    public class DESend : DEBase
    {
        #region Constants
        //private const string ArgStore = "store";
        private const string ArgTo = "to";
        private const string ArgChore = "chore";
        private const string ArgList = "list";
        private const string ArgCB = "cb";
        //private const string ArgPriv = "priv";
        private const string ArgObj = "obj";
        //private const string ArgChanges = "changes";
        private const string ArgIncFolder = "incldocs";
        private const string ArgFolder = "folder";
        private const string ArgCC = "cc";
        private const string ArgLink = "link";

        private const string ArgSvcInfo = "svc";
        private const string ArgSvcType = "svctype";

        //private const string ArgIncCharges = "inclchgs";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DESend(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Sends object to a remote service"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Optional, ArgTo, "The site to call",
            CommandClass.Required, ArgChore, "The chore",
            CommandClass.Optional, ArgCB, "The callback chore",
            //
            CommandClass.Optional, ArgObj, "The object reference",
            //
            CommandClass.Optional, ArgIncFolder, "Attach folder of object",
            CommandClass.Optional, ArgFolder, "Folder of object to attach",
            CommandClass.Optional, ArgList, "The document list to attach",
            //
            //this.DEDocumentation(c_Def);
            //
            CommandClass.Optional, ArgCC, "Credit card number to include",
            //
            CommandClass.Optional, ArgSvcInfo, "Service info to use",
            CommandClass.Optional, ArgSvcType, "Service type to use",
            CommandClass.Optional, ArgLink, "Create Chat link");

            //CommandClass.Optional, ArgIncCharges, "Include charges",

            }
        }

        public override string Command
        {
            get { return "de.send"; }
        }

        public override ReturnClass ExecStep(ChoreContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, args);

            // Get
            string sTo = args.GetDefined(ArgTo).IfEmpty(ctx.ctx.SiteInfo.Owner);
            string sChore = args.GetDefined(ArgChore);
            //string sStore = args.GetDefined(ArgStore).IfEmpty("changes");
            string sList = args.GetDefined(ArgList);
            string sCB = args.GetDefined(ArgCB);
            //string sPriv = args.GetDefined(ArgPriv);
            string sObj = args.GetDefined(ArgObj).IfEmpty(Names.Passed);
            //string sChanges = args.GetDefined(ArgChanges).IfEmpty("changes");
            bool bIncFolder = args.GetDefinedAsBool(ArgIncFolder);
            string sFolder = args.GetDefined(ArgFolder).IfEmpty("");
            string sCC = args.GetDefined(ArgCC);

            string sSvcInfoID = args.GetDefined(ArgSvcInfo);
            string sSvcType = args.GetDefined(ArgSvcType);
            bool bLink = args.GetDefinedAsBool(ArgLink, false);

            // Assure list 
            if (bIncFolder && !sList.HasValue()) sList = Names.Passed;

            //bool bIncCharges = args.GetDefinedAsBool(ArgIncCharges, false);

            //
            //if (!sTo.HasValue())
            //{
            //    sTo = ctx[@"[*sys:owner]"];
            //}

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgTo, sTo,
                                                ArgChore, sChore))
            {
                // Get the site settings
                if (ctx.ctx.SiteInfo.URLReachable.HasValue())
                {
                    if (sTo.IsSiteNameTarget())
                    {
                        //// Get the URL for the service
                        ////string sRmtURL = ctx.User.Caller.ServiceCall(ctx.User.Caller.Site.URLService, "Access_SvcMap", "uuid", sTo).Get("data");
                        //string sRmtURL = ctx.User.Caller.GetDEUrl(sTo);

                        MMURLService c_URL = this.GetURL(ctx.Env, sTo);
                        if (c_URL.HasValue())
                        {
                            // Get the store
                            JObject c_Store = new JObject();

                            // Get the changesstore
                            JObject c_ChangesStore = new JObject();

                            // Private
                            JObject c_Priv = new JObject();

                            // Reference object
                            string sObjID = "";
                            AO.ObjectClass c_Obj = ctx.Objects[sObj];
                            if (c_Obj != null)
                            {
                                sObjID = c_Obj.UUID.ToString();
                            }

                            // And the attachments
                            JObject c_Att = new JObject();
                            List<string> c_Docs = new List<string>();

                            if (bIncFolder && c_Obj != null)
                            {
                                c_Docs.AddRange(c_Obj.Documents(sFolder));
                            }

                            if (sList.HasValue())
                            {
                                c_Docs.AddRange(ctx.DocumentLists.Paths(sList));
                            }

                            foreach (string sDoc in c_Docs)
                            {
                                using (DocumentClass c_Doc = new DocumentClass(ctx.User.Caller, ctx.ctx.SiteInfo.URLReachable, sDoc))
                                {
                                    c_Att.Set(c_Doc.Name, c_Doc.URL);
                                    //ctx.Trace.Add("DOC: {0}".FormatString(c_Doc.URL));
                                }
                            }

                            //
                            string sDS = "";

                            // Load the store (make it the same as changes)
                            if (c_Obj != null)
                            {
                                c_ChangesStore = c_Obj.Values.ToSimpleString().ToJObject();
                                sDS = c_Obj.UUID.Dataset;
                            }

                            // Map
                            c_ChangesStore = this.MapPacket(env, args, c_ChangesStore);
                            // Never update the parent
                            c_ChangesStore.Remove("_parent");

                            string sWallet = null;
                            if (sCC.HasValue())
                            {
                                using (AO.WalletClass c_Wallet = new AO.WalletClass(ctx.User, false))
                                {
                                    sWallet = c_Wallet.Transport(sCC.ToInteger(0), sTo);
                                }
                            }

                            //this.ctx.LogInfo("Calling {0} @ {1}".FormatString(sChore, c_URL.RawURL));

                            // Minimize data
                            c_ChangesStore = this.Minimize(env, args, c_ChangesStore);

                            string sCharges = "";
                            //if (bIncCharges) sCharges = ctx.Charges.ToString();

                            bool bOk = true;

                            // Call
                            if (!ctx.User.Caller.ServiceCallYesNo(c_URL, "Service_Call",
                                                                    SvcInfoClass.KeyChore, sChore,
                                                                    SvcInfoClass.KeyCB, sCB,
                                                                    SvcInfoClass.KeyUUID, ctx.ctx.VUUID,
                                                                    SvcInfoClass.KeyURL, ctx.ctx.SiteInfo.URLReachable.RawURL,
                                                                    SvcInfoClass.KeyUser, ctx.User.Name,
                                                                    SvcInfoClass.KeyStore, c_Store.ToSimpleString(),
                                                                    SvcInfoClass.KeyPriv, c_Priv.ToSimpleString(),
                                                                    SvcInfoClass.KeyAttachedFolder, sFolder,
                                                                    SvcInfoClass.KeyAttached, c_Att.ToSimpleString(),
                                                                    SvcInfoClass.KeyChanges, c_ChangesStore.ToSimpleString(),
                                                                    SvcInfoClass.KeyObj, sObjID,
                                                                    SvcInfoClass.KeyCC, sWallet,
                                                                    SvcInfoClass.KeyRemapID, this.ReMapID(env, args).IfEmpty(""),
                                                                    SvcInfoClass.KeyDataset, sDS,
                                                                    SvcInfoClass.KeyType, sSvcType,
                                                                    SvcInfoClass.KeyLink, bLink.ToDBBoolean(),
                                                                    SvcInfoClass.KeyCharges, sCharges
                                                                    ))
                            {
                                bOk = false;

                                if (this.CanRetry(env, args))
                                {
                                    bOk = this.Retry(env, args);
                                }
                            }
                            else
                            {
                                this.Bill(env, ChargeClass.DataExchangeSend(ctx.User.Caller, sTo, sDS));
                            }

                            if (bOk)
                            {
                                eAns = ReturnClass.OK;
                            }
                            else
                            {
                                eAns = ReturnClass.Failure("In DE.Send");
                            }
                            //if (c_URL.Tag.HasValue())
                            //{
                            //    ctx.Trace.Add(c_URL.Tag);
                            //}
                        }
                        else
                        {
                            ctx.ctx.LogError("Unable to compute to URL");
                        }
                    }
                    else if (sTo.IsEMailAddress())
                    {
                        JObject c_Values = new JObject();
                        c_Values.Set("uuid", ctx.ctx.UUID);
                        c_Values.Set("user", ctx.User.Name);

                        AO.ObjectClass c_Obj = null;
                        if (sObj.HasValue())
                        {
                            c_Obj = ctx.Objects[sObj];
                            if (c_Obj != null)
                            {
                                c_Values.Set("obj", c_Obj.Values);
                            }
                        }

                        JObject c_Att = new JObject();
                        List<string> c_Docs = new List<string>();

                        if (bIncFolder && c_Obj != null)
                        {
                            c_Docs.AddRange(c_Obj.Documents(sFolder));
                        }

                        if (sList.HasValue())
                        {
                            c_Docs.AddRange(ctx.DocumentLists.Paths(sList));
                        }

                        foreach (string sDoc in c_Docs)
                        {
                            using (DocumentClass c_Doc = new DocumentClass(ctx.User.Caller, ctx.ctx.SiteInfo.URLReachable, sDoc))
                            {
                                c_Att.Set(c_Doc.Name, c_Doc.URL);
                            }
                        }

                        if (c_Docs.Count > 0)
                        {
                            c_Values.Set("att", c_Att);
                        }

                        string sFinal = MM.Vendors.MimeKitIF.ExtensionsM.TextToHTML(c_Values.ToSimpleString());

                        using (MM.Vendors.EMailIF.EngineClass c_Client = new Vendors.EMailIF.EngineClass(ctx.User.FullName.IfEmpty(ctx.User.Name),
                                                                                                            ctx["*sys:eMail"],
                                                                                                            ctx["*sys:eMailPwd"],
                                                                                                            ctx["*sys:eMailProv"]))
                        {
                            if (!c_Client.SendHTML(sTo,
                                            "{0} Packet".FormatString(SysConstantClass.OwnerName),
                                            sFinal,
                                            "",
                                            null,
                                            false,
                                            null
                                            ).HasValue())
                            {
                                eAns = ReturnClass.OK;
                            }

                        }
                    }
                    else
                    {
                        ctx.ctx.LogError("Invalid to: '{0}".FormatString(sTo));
                    }
                }
                else
                {
                    ctx.ctx.LogError("No reachable URL found for this site");
                }
            }

            return eAns;
        }
        #endregion
    }
}
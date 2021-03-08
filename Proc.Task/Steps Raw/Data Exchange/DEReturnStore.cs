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
    public class DEReturnStore : DEBase
    {
        #region Constants
        private const string ArgStore = "store";
        private const string ArgTo = "to";
        private const string ArgChore = "chore";
        private const string ArgList = "list";
        private const string ArgCB = "cb";
        //private const string ArgPriv = "priv";
        private const string ArgObj = "obj";
        private const string ArgChanges = "changes";
        private const string ArgIncFolder = "incldocs";
        private const string ArgFolder = "folder";
        private const string ArgIsReturn = "rettoorig";

        private const string ArgSvcInfo = "svc";

        private const string ArgIncCharges = "inclchgs";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DEReturnStore(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Returns a store to a remote service"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required, ArgTo, "The site to call",
            CommandClass.Required, ArgChore, "The chore",
            CommandClass.Optional, ArgCB, "The callback chore",
            CommandClass.Optional, ArgChanges, "The store to pass as changes",
            CommandClass.Optional, ArgStore, "The store to use as data",
            //
            CommandClass.Optional, ArgObj, "The object reference",
            //
            //CommandClass.Optional, ArgPriv, "The store to be returned on callback",
            CommandClass.Optional, ArgIncFolder, "Attach folder of object",
            CommandClass.Optional, ArgFolder, "Folder of object to attach",
            CommandClass.Optional, ArgList, "The document list to attach",
            CommandClass.Optional, ArgIsReturn, "Return to original object",
            //
            //this.DEDocumentation(c_Def);
            //
            CommandClass.Optional, ArgSvcInfo, "Service info to use",

            CommandClass.Optional, ArgIncCharges, "Include charges");

            }
        }

        public override string Command
        {
            get { return "de.return.store"; }
        }

        public override ReturnClass ExecStep(ChoreContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, args);

            // Get
            string sChore = args.GetDefined(ArgChore);
            string sStore = args.GetDefined(ArgStore);
            string sList = args.GetDefined(ArgList);
            string sCB = args.GetDefined(ArgCB);
            //string sPriv = args.GetDefined(ArgPriv);
            string sObj = args.GetDefined(ArgObj).IfEmpty(Names.Passed);
            string sChanges = args.GetDefined(ArgChanges).IfEmpty("changes");
            bool bIncFolder = args.GetDefinedAsBool(ArgIncFolder);
            string sFolder = args.GetDefined(ArgFolder).IfEmpty("");
            string sReturn = args.GetDefinedAsBool(ArgIsReturn).ToDBBoolean();

            string sSvcInfoID = args.GetDefined(ArgSvcInfo);

            bool bIncCharges = args.GetDefinedAsBool(ArgIncCharges, false);

            // Assure list 
            if (bIncFolder && !sList.HasValue()) sList = Names.Passed;

            // Get the site settings
            if (ctx.ctx.SiteInfo.URLReachable.HasValue())
            {
                // Get the object
                AO.ObjectClass c_Obj = ctx.Objects[sObj];

                // Must have service info
                if (c_Obj != null)
                {
                    string sSvcInfo = c_Obj.SvcInfoGet(sSvcInfoID);
                    if (sSvcInfo.HasValue())
                    {
                        // Mke the info
                        SvcInfoClass c_Info = new SvcInfoClass(env, sSvcInfo);

                        //// Get the URL for the service
                        ////string sRmtURL = ctx.User.Caller.ServiceCall(ctx.User.Caller.Site.URLService, "Access_SvcMap", "uuid", c_Info.UUID).Get("data");
                        //string sRmtURL = ctx.User.Caller.GetDEUrl(c_Info.UUID);
                        //// If none. use from svc info
                        //sRmtURL = sRmtURL.IfEmpty(c_Info.URL);

                        MMURLService c_URL = this.GetURL(ctx.Env, c_Info.UUID);
                        if (c_URL.HasValue())
                        {
                            // Assure chore
                            sChore = sChore.IfEmpty(c_Info.Callback);

                            // Get the store
                            JObject c_Store = new JObject();
                            if (sStore.HasValue())
                            {
                                c_Store = ctx.Stores[sStore];
                            }

                            // Get the changesstore
                            JObject c_ChangesStore = new JObject();
                            if (sChanges.HasValue() && ctx.Stores.Has(sChanges))
                            {
                                c_ChangesStore = ctx.Stores[sChanges];
                            }

                            // Map
                            c_ChangesStore = this.MapPacket(env, args, c_ChangesStore);

                            // Private
                            JObject c_Priv = new JObject();

                            // Reference object
                            string sObjID = c_Info.ObjectID;

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
                                }
                            }

                            bool bOk = true;

                            // Minimize data
                            c_ChangesStore = this.Minimize(env, args, c_ChangesStore);

                            string sCharges = "";
                            if (bIncCharges) sCharges = ctx.Charges.ToString(sObjID);

                            // Call
                            if (!ctx.User.Caller.ServiceCallYesNo(c_URL, "Service_Call",
                                                                    SvcInfoClass.KeyChore, sChore,
                                                                    SvcInfoClass.KeyCB, sCB,
                                                                    SvcInfoClass.KeyUUID, ctx.ctx.VUUID,
                                                                    SvcInfoClass.KeyURL, ctx.ctx.SiteInfo.URLReachable.RawURL,
                                                                    SvcInfoClass.KeyUser, c_Info.User,
                                                                    SvcInfoClass.KeyStore, c_Store.ToSimpleString(),
                                                                    SvcInfoClass.KeyPriv, c_Priv.ToSimpleString(),
                                                                    SvcInfoClass.KeyAttachedFolder, sFolder,
                                                                    SvcInfoClass.KeyAttached, c_Att.ToSimpleString(),
                                                                    SvcInfoClass.KeyChanges, c_ChangesStore.ToSimpleString(),
                                                                    SvcInfoClass.KeyObj, sObjID,
                                                                    SvcInfoClass.KeyCharges, sCharges,
                                                                    SvcInfoClass.KeyReturnToOriginal, sReturn
                                                                    ))
                            {
                                bOk = false;

                                if (this.CanRetry(env, args))
                                {
                                    bOk = this.Retry(env, args);
                                }
                            }

                            if (bOk)
                            {
                                eAns = ReturnClass.OK;
                            }

                            if (c_URL.Tag.HasValue())
                            {
                                ctx.ctx.LogInfo(c_URL.Tag);
                            }
                        }
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
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

using System;
using System.Collections.Generic;

using NX.Engine;
using NX.Shared;
using NX.Engine.Files;

using Proc.AO;
using Proc.Office;
using Proc.Communication;

namespace Proc.Stripe
{
    /// <summary>
    /// 
    /// Expands allowed definition
    /// 
    /// </summary>
    public class AsPDF : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            call.Env.Debug();

            //
            using (Proc.Office.AsToolClass c_WEnv = new AsToolClass(call, store))
            {
                // Get the manager
                AO.ManagerClass c_Mgr = call.Env.Globals.Get<AO.ManagerClass>();

                // Get the site info
                AO.SiteInfoClass c_SI = c_Mgr.DefaultDatabase.SiteInfo;

                // Make the UUID
                using (AO.UUIDClass c_UUID = new UUIDClass(c_Mgr.DefaultDatabase, c_WEnv.ObjDS, c_WEnv.ObjID))
                {
                    // Get the invoice
                    using (AO.ObjectClass c_Inv = c_UUID.AsObject)
                    {
                        // Set the request date
                        c_Inv["reqon"] = DateTime.Now.ToDBDate();
                        c_Inv.Save();

                        // Get the accout UUID
                        using (AO.UUIDClass c_AUUID = new UUIDClass(c_Mgr.DefaultDatabase, c_Inv["acct"]))
                        {
                            // Get the account
                            using (AO.ObjectClass c_Acct = c_AUUID.AsObject)
                            {
                                // Get the account
                                string sAcct = c_Acct["name"];
                                string sSubject = "";
                                string sMsg = "";

                                using (StoreClass c_Params = new StoreClass())
                                {
                                    // Make the context
                                    using (ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, null, null, call.UserInfo.Name))
                                    {
                                        // Do handlebars
                                        HandlebarDataClass c_HData = new HandlebarDataClass(call.Env);
                                        // Add the object
                                        c_HData.Merge(c_WEnv.Object.Explode(ExplodeMakerClass.ExplodeModes.Yes, c_Ctx));

                                        // Fill store
                                        c_Params[eMessageClass.KeyTo] = "sample@gmail.com";
                                        c_Params[eMessageClass.KeyCommand] = "email";
                                        c_Params[eMessageClass.KeySubj] = c_SI.PayReqSubject.IfEmpty(sSubject);
                                        c_Params[eMessageClass.KeyMsg] = c_SI.PayReqMessage.IfEmpty(sMsg);
                                        c_Params["user"] = call.UserInfo.Name;
                                        c_Params[eMessageClass.KeyEMailTemplate] = c_SI.PayReqTemplate;
                                        c_Params[eMessageClass.KeyInvoice] = c_Inv["code"];

                                        // Make message
                                        using (eMessageClass c_Msg = eMessageClass.FromStore(call.Env, c_Params, c_HData))
                                        {
                                            NX.Engine.Files.ManagerClass c_DocMgr = call.Env.Globals.Get<NX.Engine.Files.ManagerClass>();

                                            using (DocumentClass c_Doc = new DocumentClass(c_DocMgr, c_Inv.Folder, "Invoice.pdf"))
                                            {
                                                c_Msg.AsPDF(c_Doc);

                                                c_WEnv.ReturnDocument(c_Doc);

                                                return c_WEnv.Return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //}
            }
        }
    }
}
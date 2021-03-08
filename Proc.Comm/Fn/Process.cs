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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
/// 

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MongoDB.Driver;
using MongoDB.Bson;

using NX.Engine;
using NX.Shared;
using Proc.AO;

namespace Proc.Comm
{
    /// <summary>
    /// 
    /// Gets an object
    /// 
    /// </summary>
    public class Process : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the params
            string sTo = store["to"];
            string sFn = store["fn"];
            string sSubj = store["subject"];
            string sMsg = store["message"];
            string sAtt = store["attachments"];

            // Handle to
            JArray c_To = sTo.ToJArrayOptional();
            // Handle attachments
            JArray c_Att = sAtt.ToJArrayOptional();

            // Create preference
            eAddressClass.AddressTypes eType = eAddressClass.AddressTypes.User;
            switch(sFn)
            {
                case "voice":
                    eType = eAddressClass.AddressTypes.Voice;
                    break;
                case "sms":
                    eType = eAddressClass.AddressTypes.SMS;
                    break;
                case "email":
                    eType = eAddressClass.AddressTypes.EMail;
                    break;
                case "fedex":
                    eType = eAddressClass.AddressTypes.FedEx;
                    break;
            }

            // Make the context
            using (AO.ExtendedContextClass c_Ctx = new ExtendedContextClass(call.Env, null, null, call.UserInfo.Name))
            {
               // Make the message
                using(eMessageClass c_Msg = new eMessageClass(c_Ctx))
                {
                    for (int i = 0; i < c_To.Count; i++)
                    {
                        string sWkg = c_To.Get(i);
                        if(sWkg.HasValue()) c_Msg.To.Add(sWkg, eType);
                    }

                    // Fill
                    c_Msg.Subject = sSubj;
                    c_Msg.Message = sMsg;

                    for (int i = 0; i < c_Att.Count; i++)
                    {
                        string sWkg = c_To.Get(i);
                        if (sWkg.HasValue()) c_Msg.Attachments.Add(sWkg);
                    }

                    eReturnClass c_Ret = c_Msg.Send();

                    string sQM = "Result of {0} request: {1}".FormatString(eType, c_Ret.ToString());
                    c_Ctx.SendQM(c_Ctx.User.Name, sQM, null);
                }
            }

            // TBD

            return c_Ans;
        }
    }
}
﻿///--------------------------------------------------------------------------------
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

namespace Proc.Communication
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

            // Push back
            store["user"] = call.UserInfo.Name;

            // Make message
            using (eMessageClass c_Msg = eMessageClass.FromStore(call.Env, store))
            {
                c_Msg.Send(true);                
            }

            return c_Ans;
        }
    }
}
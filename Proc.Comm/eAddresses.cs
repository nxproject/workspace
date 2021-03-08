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
/// 

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Comm
{
    public class eAddressesClass : ChildOfClass<eMessageClass>
    {
        #region Constructor
        public eAddressesClass(eMessageClass msg, string key)
            : base(msg)
        {
            //
            this.Key = key;
            this.Parse(this.Message.Values.Get(this.Key), eAddressClass.AddressTypes.SMS);
        }
        #endregion

        #region Properties
        public eMessageClass Message { get { return this.Root as eMessageClass; } }
        private string Key { get; set; }

        public eAddressListClass EMail { get; set; } = new eAddressListClass();
        public eAddressListClass SMS { get; set; } = new eAddressListClass();
        public eAddressListClass FedEx { get; set; } = new eAddressListClass();
        public eAddressListClass Users { get; set; } = new eAddressListClass();
        public eAddressListClass Voice { get; set; } = new eAddressListClass();
        #endregion

        #region Methods
        public void Add(string value, eAddressClass.AddressTypes pref)
        {
            if (value.HasValue())
            {
                eAddressClass c_Addr = new eAddressClass(value, pref);

                switch (c_Addr.Type)
                {
                    case eAddressClass.AddressTypes.EMail:
                        this.EMail.Add(c_Addr);
                        break;
                    case eAddressClass.AddressTypes.SMS:
                        this.SMS.Add(c_Addr);
                        break;
                    case eAddressClass.AddressTypes.FedEx:
                        this.FedEx.Add(c_Addr);
                        break;
                    case eAddressClass.AddressTypes.User:
                        this.Users.Add(c_Addr);
                        break;
                    case eAddressClass.AddressTypes.Voice:
                        this.Voice.Add(c_Addr);
                        break;
                }
            }
        }

        public void Parse(string value, eAddressClass.AddressTypes pref)
        {
            foreach (string sTo in value.Split(';'))
            {
                this.Add(sTo, pref);
            }
        }
        #endregion
    }
}
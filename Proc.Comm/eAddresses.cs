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

namespace Proc.Communication
{
    public class eAddressesClass : ChildOfClass<eMessageClass>
    {
        #region Constructor
        public eAddressesClass(eMessageClass msg)
            : base(msg)
        { }
        #endregion

        #region Properties
        public eAddressListClass EMail { get; set; } = new eAddressListClass();
        public eAddressListClass SMS { get; set; } = new eAddressListClass();
        public eAddressListClass FedEx { get; set; } = new eAddressListClass();
        public eAddressListClass Users { get; set; } = new eAddressListClass();
        public eAddressListClass Voice { get; set; } = new eAddressListClass();
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Adds a to to the address list
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pref"></param>
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

        /// <summary>
        /// 
        /// Parses a string
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pref"></param>
        public void Parse(string value, eAddressClass.AddressTypes pref)
        {
            foreach (string sTo in value.Split(';'))
            {
                this.Add(sTo, pref);
            }
        }

        /// <summary>
        /// 
        /// Resets all tos
        /// 
        /// </summary>
        public void Clear()
        {
            this.EMail = new eAddressListClass();
            this.SMS = new eAddressListClass();
            this.FedEx = new eAddressListClass();
            this.Users = new eAddressListClass();
            this.Voice = new eAddressListClass();
        }
        #endregion
    }
}
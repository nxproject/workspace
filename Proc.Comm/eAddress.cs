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

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Communication
{
    public class eAddressClass : IDisposable
    {
        #region Constants
        public const string EMail = "email";
        public const string SMS = "sms";
        public const string FedEx = "fedex";
        public const string User = "user";
        public const string Voice = "voice";
        #endregion

        #region Constructor
        public eAddressClass(string value, AddressTypes pref)
        {
            string sType = "";
            string sTo = value.IfEmpty();
            string sAttn = "";

            int iPos = sTo.IndexOf(":");
            if (iPos != -1)
            {
                sType = sTo.Substring(0, iPos);
                sTo = sTo.Substring(iPos + 1);
            }

            iPos = sTo.IndexOf(" // ");
            if (iPos != -1)
            {
                sAttn = sTo.Substring(iPos + 4);
                sTo = sTo.Substring(0, iPos);
            }

            if (sTo.IsEMailAddress())
            {
                sType = EMail;
            }
            else if (sTo.IsFormattedPhone())
            {
                sType = sType.IfEmpty(SMS);

                if (sType.IsSameValue(SMS))
                { }
                else
                {
                    sType = Voice;
                }
            }
            else if (sTo.IsSiteNameValid())
            {
                sType = User;
            }

            this.Type = AddressTypes.Invalid;
            if (sType.HasValue())
            {
                try
                {
                    this.Type = (AddressTypes)Enum.Parse(typeof(AddressTypes), sType, true);
                }
                catch { }
            }

            this.To = sTo;
            this.Attn = sAttn;
        }
        #endregion

        #region Enum
        public enum AddressTypes
        {
            Invalid,

            User,
            EMail,
            SMS,
            FedEx,
            Voice
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public AddressTypes Type { get; private set; }
        public string To { get; private set; }
        public string Attn { get; private set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            string sAns = "";

            if (this.Type != AddressTypes.Invalid) sAns += this.Type.ToString().ToLower() + ":";
            sAns += this.To;
            if (this.Attn.HasValue()) sAns += " // " + this.Attn;

            return sAns;
        }

        /// <summary>
        /// 
        /// Compare two addresses
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool IsSameValue(eAddressClass addr)
        {
            return this.To.IsSameValue(addr.To) && this.Type == addr.Type;
        }
        #endregion
    }
}
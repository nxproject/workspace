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

using NX.Shared;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Security token
    /// 
    /// </summary>
    public class TokenClass : IDisposable
    {
        #region Constants
        private byte[] KeyEnc = "gpEdAvWDSLFReoJx-QrCXyB9qt68h$k1MKbiZljP025NsYTa3wfGHnUmuzVcO7+I4".ToBytes();
        private byte[] KeyDec = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".ToBytes();
        #endregion

        #region Constructor
        public TokenClass(string value)
        {
            // Do we have a token?
            if (value.HasValue())
            {
                // Decode
                long iDecoded = this.Decode(value).ToLong();
                // Make room
                DateTime c_Till = new DateTime(iDecoded);
                // Still in range?
                this.IsValid = DateTime.Now <= c_Till;
            }
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// 
        /// Housekeeping
        /// 
        /// </summary>
        public void Dispose()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// True if passed token is valid
        /// 
        /// </summary>
        public bool IsValid { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Translates from one cha set to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void Translate(byte[] value, byte[] from, byte[] to)
        {
            // Loop thru
            for (int i = 0; i < value.Length; i++)
            {
                // Do the byte
                for (int j = 0; j < from.Length; j++)
                {
                    // Same?
                    if (from[j] == value[i])
                    {
                        // Convert
                        value[i] = to[j];
                        // Only once
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// Encodes a string
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string Encode(string value)
        {
            // Convert to bytes
            byte[] abValue = value.ToBase64().ToBytes();
            // Translate
            this.Translate(abValue, KeyDec, KeyEnc);
            //And back
            return abValue.FromBytes();
        }

        /// <summary>
        /// 
        /// Decodes a string
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string Decode(string value)
        {
            // Convert to bytes
            byte[] abValue = value.ToBytes();
            // Translate
            this.Translate(abValue, KeyEnc, KeyDec);
            // And back
            return abValue.FromBytes().FromBase64();
        }

        /// <summary>
        /// 
        /// Returns a token valid for the amount of time given
        /// 
        /// </summary>
        /// <param name="length">The amount of time</param>
        /// <returns></returns>
        public string Till(TimeSpan? length = null)
        {
            // Use default?
            if (length == null) length = 8.HoursAsTimeSpan();
            // Now (UTC)
            DateTime c_Now = DateTime.Now.ToUniversalTime().Add((TimeSpan)length);
            // Encode
            return this.Encode(c_Now.Ticks.ToString());
        }
        #endregion
    }
}

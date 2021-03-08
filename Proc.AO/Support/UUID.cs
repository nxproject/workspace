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
using System.Text;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    public class UUIDClass : ChildOfClass<DatabaseClass>
    {
        #region Constants
        public const string DELIMITER = ":";
        #endregion

        #region Constructor
        public UUIDClass(DatabaseClass mgr, string value)
            : base(mgr)
        {
            // Parse
            string[] asPieces = value.IfEmpty().Split(DELIMITER);
            // Acording to length
            switch(asPieces.Length)
            {
                case 6:
                    this.Dataset = this.Parent[asPieces[2]];
                    this.ID = asPieces[3];
                    break;

                case 7:
                    this.Dataset = this.Parent[asPieces[2]];
                    this.ID = asPieces[3];
                    this.Extra = asPieces[4];
                    break;
            }

        }

        public UUIDClass(DatabaseClass mgr, string dataset, string id, string extra="")
            : base(mgr)
        {
            //
            this.Dataset = mgr[dataset];
            this.ID = id;
            this.Extra = extra;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The dataset
        /// 
        /// </summary>
        public DatasetClass Dataset { get; private set; }

        /// <summary>
        /// 
        /// The ID
        /// 
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 
        /// Extra information
        /// 
        /// </summary>
        public string Extra { get; set; }

        /// <summary>
        /// 
        /// Return the object itself
        /// 
        /// </summary>
        public ObjectClass AsObject {  get { return this.Dataset[this.ID]; } }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// The storeable UUID
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DELIMITER+ DELIMITER + this.Dataset.Name + DELIMITER + this.ID.IfEmpty() + (this.Extra.HasValue() ? DELIMITER + this.Extra : "") + DELIMITER+ DELIMITER;
        }
        #endregion

        #region Statics
        public static string MakeString(string ds, string id)
        {
            return DELIMITER + DELIMITER + ds + DELIMITER + id + DELIMITER + DELIMITER;
        }

        public static bool IsValid(string value)
        {
            return value.StartsWith(DELIMITER + DELIMITER) && value.EndsWith(DELIMITER + DELIMITER) && value.Split(DELIMITER[0]).Length == 6;
        }
        #endregion
    }
}
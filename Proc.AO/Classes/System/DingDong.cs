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
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
/// 

using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    public class DingDongsClass : ChildOfClass<DatabaseClass>
    {
        #region Constructor
        public DingDongsClass(DatabaseClass db)
            : base(db)
        {
            this.Dataset = this.Parent[DatabaseClass.DatasetDingDong];
        }
        #endregion

        #region Indexer
        public DingDongClass this[string index]
        {
            get { return new DingDongClass(this.Dataset[index]); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The dataset
        /// 
        /// </summary>
        public DatasetClass Dataset { get; private set; }
        #endregion

        #region Methods
        public DingDongClass New()
        {
            return new DingDongClass(this.Dataset.New());
        }
        #endregion
    }

    public class DingDongClass : ChildOfClass<ObjectClass>
    {
        #region Constants
        private const string FieldOn = "on";
        private const string FieldUUID = "obj";
        private const string FieldReference = "refid";
        private const string FieldMessage = "msg";
        private const string FieldVia = "via";
        private const string FieldUser = "user";
        private const string FieldSubject = "subj";
        #endregion

        #region Constructor
        internal DingDongClass(ObjectClass obj)
            : base(obj)
        { }
        #endregion

        #region Properties
        public DateTime On
        {
            get { return this.Parent[FieldOn].FromDBDate(); }
            set { this.Parent[FieldOn] = value.ToDBDate(); }
        }

        public UUIDClass ObjectUUID
        {
            get { return new UUIDClass(this.Parent.Parent.Parent, this.Parent[FieldUUID]); }
            set { this.Parent[FieldUUID] = value.ToString(); }
        }

        public string Reference
        {
            get { return this.Parent[FieldReference]; }
            set { this.Parent[FieldReference] = value; }
        }

        public string Subject
        {
            get { return this.Parent[FieldSubject]; }
            set { this.Parent[FieldSubject] = value; }
        }

        public string Message
        {
            get { return this.Parent[FieldMessage]; }
            set { this.Parent[FieldMessage] = value; }
        }

        public string Via
        {
            get { return this.Parent[FieldVia]; }
            set { this.Parent[FieldVia] = value; }
        }

        public string User
        {
            get { return this.Parent[FieldUser]; }
            set { this.Parent[FieldUser] = value; }
        }

        public string Hash
        {
            get { return this.Parent.ToString().MD5HashString(); }
        }

        public bool IsDone
        {
            get { return this.Parent[DatabaseClass.DatasetDingDong].IndexOf(this.Hash) != -1; }
        }
        #endregion

        #region Methods
        public void Save()
        {
            this.Parent.Save();
        }

        public void SetDone()
        {
            this.Parent[DatabaseClass.DatasetDingDong] = this.Parent[DatabaseClass.DatasetDingDong] + " " + this.Hash;
            this.Parent.Save();
        }
        #endregion

        #region Statics
        public static void Before(QueryClass qry, DateTime on)
        {
            qry.Add(FieldOn, QueryElementClass.QueryOps.Lte, on.ToDBDate());
        }

        public static void BelongsTo(QueryClass qry, UUIDClass uuid)
        {
            qry.Add(FieldUUID, uuid.ToString());
        }

        public static void ReferredAs(QueryClass qry, string refid)
        {
            qry.Add(FieldReference, refid);
        }
        #endregion
    }
}

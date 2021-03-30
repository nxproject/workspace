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
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public class IndexStoreClass : ChildOfClass<DatabaseClass>
    {
        #region Constants
        private const int MaxIndex = 1000000;
        #endregion

        #region Constructor
        public IndexStoreClass(DatabaseClass db)
            : base(db)
        {
            // Make generator
            this.RNGenerator = new Random();
            // And grab the dataset
            this.Dataset = this.Parent[DatabaseClass.DatasetIndexStore];
        }
        #endregion

        #region Indexer
        public IndexItemClass this[string index]
        {
            get
            {
                // Create
                IndexItemClass c_Item = new IndexItemClass(this.Dataset[index]);
                // Assure valid
                if (!c_Item.IsValid)
                {
                    c_Item.Delete();
                    c_Item.Dispose();
                    c_Item = null;
                }
                else if(c_Item.Uses > 0)
                {
                    c_Item.Uses--;
                    if (c_Item.Uses == 0)
                    {
                        c_Item.Delete();
                    }
                    else
                    {
                        c_Item.Save();
                    }
                }

                return c_Item;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The random number generator
        /// 
        /// </summary>
        private Random RNGenerator { get; set; }

        /// <summary>
        /// 
        /// The dataset
        /// 
        /// </summary>
        private DatasetClass Dataset { get; set; }
        #endregion

        #region Methods
        public IndexItemClass Next(string owner)
        {
            IndexItemClass c_Ans = null;

            while (c_Ans == null)
            {
                // Get a number
                string sPoss = "{0:000000}".FormatString(this.RNGenerator.Next(MaxIndex));
                // If not valid
                using (IndexItemClass c_Poss = this[sPoss])
                {
                    // Null?
                    if (c_Poss == null)
                    {
                        // Make
                        using (IndexItemClass c_Item = new IndexItemClass(this.Dataset[sPoss]))
                        {
                            // Set 
                            c_Item.Owner = owner;
                            // Save
                            c_Item.Save();

                            // Read
                            IndexItemClass c_ItemR = new IndexItemClass(this.Dataset[sPoss]);

                            // Ours?
                            if (c_ItemR.Owner.IsSameValue(owner))
                            {
                                // Got one
                                c_Ans = c_ItemR;
                            }
                            else
                            {
                                c_ItemR.Dispose();
                            }
                        }
                    }
                }
            }

            return c_Ans;
        }
        #endregion
    }

    public class IndexItemClass : ChildOfClass<ObjectClass>
    {
        #region Constants
        private const string KeyOwner = "owner";
        private const string KeyType = "type";
        private const string KeyExpiration = "exp";
        private const string KeyValue = "val";
        private const string KeyUses = "uses";
        #endregion

        #region Constructor
        internal IndexItemClass(ObjectClass obj)
            : base(obj)
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The entry ID
        /// 
        /// </summary>
        public string ID { get { return this.Parent.ID; } }

        /// <summary>
        /// 
        /// The owner of the entry
        /// 
        /// </summary>
        public string Owner
        {
            get { return this.Parent[KeyOwner]; }
            set { this.Parent[KeyOwner] = value; }
        }

        /// <summary>
        /// 
        /// The type of the entry
        /// 
        /// </summary>
        public string Type
        {
            get { return this.Parent[KeyType]; }
            set { this.Parent[KeyType] = value; }
        }

        /// <summary>
        /// 
        /// The expiration of the entry
        /// 
        /// </summary>
        public DateTime Expiration
        {
            get { return this.Parent[KeyExpiration].FromDBDate(); }
            set { this.Parent[KeyExpiration] = value.ToDBDate(); }
        }

        /// <summary>
        /// 
        /// Is it a one time value?
        /// 
        /// </summary>
        public int Uses
        {
            get { return this.Parent[KeyUses].ToInteger(1); }
            set { this.Parent[KeyUses] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// User values
        /// 
        /// </summary>
        public StoreClass Values
        {
            get { return new StoreClass(this.Parent[KeyValue].ToJObject()); }
            set { this.Parent[KeyValue] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// Is the entry valid?
        /// 
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool bAns = false;

                // Get the expiration date
                string sExp = this.Parent[KeyExpiration];
                if (sExp.HasValue())
                {
                    // After now?
                    bAns = DateTime.Now < sExp.FromDBDate();
                }
                return bAns && this.Uses != 0;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Saves the entry
        /// 
        /// </summary>
        public void Save()
        {
            if (this.Parent != null) this.Parent.Save();
        }

        /// <summary>
        /// 
        /// Deletes the entry
        /// 
        /// </summary>
        public void Delete()
        {
            if(this.Parent != null) this.Parent.Delete();
        }
        #endregion
    }
}
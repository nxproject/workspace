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

using NX.Shared;

namespace Proc.AO
{
    public class CollectionClass : ChildOfClass<DatasetClass>
    {
        #region Constructor
        internal CollectionClass(DatasetClass ds, string prefix, string suffix = "")
            : base(ds)
        {
            //
            this.Prefix = prefix;
            this.Suffix = suffix;

            //
            this.Name = MakeName(this.Parent, prefix , suffix);
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The collection name
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// The prefix for the collection name, if none, the collection is data
        /// 
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// 
        /// The suffix for the collection name, if none, the collection is data
        /// 
        /// </summary>
        public string Suffix { get; private set; }

        /// <summary>
        /// 
        /// True if the collection is data
        /// 
        /// </summary>
        public bool IsData {  get { return !this.Prefix.HasValue() && !this.Suffix.HasValue(); ; } }

        /// <summary>
        /// 
        /// The physical collection
        /// 
        /// </summary>
        private IMongoCollection<BsonDocument> IDocuments { get; set; }
        public IMongoCollection<BsonDocument> Documents
        {
            get
            {
                if (this.IDocuments == null && this.Parent.Parent.Database != null)
                {
                    this.IDocuments = this.Parent.Parent.Database.GetCollection<BsonDocument>(this.Name);
                }

                return this.IDocuments;
            }
        }

        /// <summary>
        /// 
        /// Returns the count of documents
        /// 
        /// </summary>
        public long Count {  get { return this.Documents.CountDocuments(new BsonDocument()); } }
        #endregion

        #region Support
        /// <summary>
        /// 
        /// List of items with a given prefix
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public List<string> Names(string prefix)
        {
            // Assume none
            List<string> c_Ans = new List<string>();

            // Make the filter
            using (QueryClass c_Filter = new QueryClass(this))
            {
                // By ID
                c_Filter.Add("_id", QueryElementClass.QueryOps.Like, this.Prefix + prefix + ".+");
                // Find
                foreach (BsonDocument c_Doc in c_Filter.Find())
                {
                    // Get the name
                    string sName = c_Doc.GetField("_id").Substring(this.Prefix.Length + prefix.Length);
                    // Add
                    c_Ans.Add(sName);
                }
            }

            return c_Ans;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Used by packager
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void AddDirect(string value)
        {
            try
            {
                // Make
                BsonDocument c_Doc = BsonDocument.Parse(value);
                // Filter
                using (QueryClass c_Qry = new QueryClass(this))
                {
                    // Use the id
                    c_Qry.AddByID(c_Doc.GetField("_id"));
                    // Delete
                    this.Documents.DeleteMany(c_Qry.Filter);
                    // Add
                    this.Documents.InsertOne(c_Doc);
                }
            }
            catch(Exception e)
            {
                this.Parent.Parent.Parent.Parent.LogException(e);
            }
        }

        /// <summary>
        /// 
        /// Deletes a collection
        /// 
        /// </summary>
        /// <returns></returns>
        public long Delete()
        {
            this.Parent.Parent.Database.DropCollection(this.Name);

            return 0;
        }

        /// <summary>
        /// 
        /// Creates an index
        /// 
        /// </summary>
        /// <param name="fld"></param>
        public void CreateIndex(string fld)
        {
            FieldDefinition<BsonDocument, string> c_Fld = fld;
            var c_IDef = Builders<BsonDocument>.IndexKeys.Ascending(c_Fld);
            this.Documents.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(c_IDef));
        }
        #endregion

        #region Statics
        public static string MakeName(DatasetClass ds, string prefix, string suffix)
        {
            return prefix.IfEmpty() + ds.Name + suffix.IfEmpty();
        }
        #endregion
    }
}
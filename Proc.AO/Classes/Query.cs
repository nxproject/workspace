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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Newtonsoft.Json.Linq;

using MongoDB.Driver;
using MongoDB.Bson;

using NX.Shared;

namespace Proc.AO
{
    public class QueryClass : ChildOfClass<CollectionClass>, IEnumerable
    {
        #region Constructor
        public QueryClass(CollectionClass coll)
            : base(coll)
        {
            //
            this.Values = new List<QueryElementClass>();
        }

        public QueryClass(ExtendedContextClass ctx,
            CollectionClass coll,
            string value,
            QueryElementClass.QueryOps op = QueryElementClass.QueryOps.Eq)
            : this(ctx, coll, value.ToJArray(), op)
        { }

        public QueryClass(ExtendedContextClass ctx,
            CollectionClass coll,
            JArray value,
            QueryElementClass.QueryOps op = QueryElementClass.QueryOps.Eq)
            : this(coll)
        {
            //
            if (value != null)
            {
                // Loop thru
                for (int i = 0; i < value.Count; i++)
                {
                    // Get entry
                    JObject c_Entry = value.GetJObject(i);
                    // Any?
                    if (c_Entry != null)
                    {
                        // Parse
                        this.Values.Add(new QueryElementClass(ctx, c_Entry, op));
                    }
                    else
                    {
                        // Do we have a list?
                        JArray c_Qrys = value.GetJArray(i);
                        // If so, make a subquery
                        if (c_Qrys != null)
                        {
                            // Parse
                            this.Values.Add(new QueryElementClass(ctx, c_Qrys, op));
                        }
                    }
                }
            }
        }

        public QueryClass(ExtendedContextClass ctx,
            CollectionClass coll,
            List<QueryElementClass> value)
            : this(coll)
        {
            //
            if (value != null)
            {
                // Loop thru
                for (int i = 0; i < value.Count; i++)
                {
                    // Get entry
                    QueryElementClass c_Entry = value[i];
                    // Any?
                    if (c_Entry != null)
                    {
                        // Parse
                        this.Values.Add(c_Entry);
                    }
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The array of filter elements
        /// 
        /// </summary>
        private List<QueryElementClass> Values { get; set; }

        /// <summary>
        /// 
        /// The MongoDb filter
        /// 
        /// </summary>
        public FilterDefinition<BsonDocument> Filter
        {
            get
            {
                return this.Make(this.Values, false);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Makes a query statement
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="any"></param>
        /// <returns></returns>
        private FilterDefinition<BsonDocument> Make(List<QueryElementClass> items, bool any)
        {
            // Default
            FilterDefinition<BsonDocument> c_Ans = Builders<BsonDocument>.Filter.Empty;

            //
            List<FilterDefinition<BsonDocument>> c_Entries = new List<FilterDefinition<BsonDocument>>();

            // Loop thru
            foreach (QueryElementClass c_Ele in items)
            {
                // Get field definition
                Definitions.DatasetFieldClass c_Def = null;
                // Skip id checks
                if (!c_Ele.Field.IsSameValue("_id"))
                {
                    c_Def = this.Parent.Parent.Definition[c_Ele.Field];
                }

                // Make
                var c_Entry = this.Make(c_Ele, c_Def);
                // Add
                if (c_Entry != null) c_Entries.Add(c_Entry);
            }

            // Any?
            switch (c_Entries.Count)
            {
                case 0:
                    break;

                case 1:
                    c_Ans = c_Entries[0];
                    break;

                default:
                    if (any)
                    {
                        c_Ans = Builders<BsonDocument>.Filter.Or(c_Entries);
                    }
                    else
                    {
                        c_Ans = Builders<BsonDocument>.Filter.And(c_Entries);
                    }
                    break;
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Makes  filter definition according to the type and op
        /// 
        /// </summary>
        /// <param name="element">The filter element</param>
        /// <param name="type">The field type</param>
        /// <returns></returns>
        private FilterDefinition<BsonDocument> Make(QueryElementClass element, Definitions.DatasetFieldClass fdef)
        {
            // Assume none
            FilterDefinition<BsonDocument> c_Ans = null;

            // Sub-queries
            if (element.Queries != null && element.Queries.Count > 0)
            {
                c_Ans = this.Make(element.Queries, element.SubOperator == QueryElementClass.SubqueryOps.Any);
            }
            else
            {
                // Global
                switch (element.Operator)
                {
                    case QueryElementClass.QueryOps.Exists:
                        c_Ans = Builders<BsonDocument>.Filter.Exists(element.Field);
                        break;

                    case QueryElementClass.QueryOps.Notexists:
                        c_Ans = Builders<BsonDocument>.Filter.Not(Builders<BsonDocument>.Filter.Exists(element.Field));
                        break;

                    case QueryElementClass.QueryOps.Like:
                        c_Ans = Builders<BsonDocument>.Filter.Regex(element.Field, new BsonRegularExpression(element.Value));
                        break;

                    case QueryElementClass.QueryOps.Notlike:
                        c_Ans = Builders<BsonDocument>.Filter.Not(Builders<BsonDocument>.Filter.Regex(element.Field, new BsonRegularExpression(element.Value)));
                        break;

                    case QueryElementClass.QueryOps.Any:
                        c_Ans = Builders<BsonDocument>.Filter.Regex(element.Field, new BsonRegularExpression((" " + element.Value + " ").Replace(" ", ".*")));
                        break;

                    default:
                        // Assume no definition
                        Definitions.DatasetFieldClass.QueryTypes eType = Definitions.DatasetFieldClass.QueryTypes.String;
                        if (fdef != null)
                        {
                            eType = fdef.QueryType;
                        }
                        else
                        {
                            if(element.Field.IsNumOnly())
                            {
                                eType = Definitions.DatasetFieldClass.QueryTypes.Double;
                            }
                        }

                        switch (eType)
                        {
                            case Definitions.DatasetFieldClass.QueryTypes.String:
                                switch (element.Operator)
                                {
                                    case QueryElementClass.QueryOps.Eq:
                                        c_Ans = Builders<BsonDocument>.Filter.Eq<string>(element.Field, element.Value);
                                        break;

                                    case QueryElementClass.QueryOps.Ne:
                                        c_Ans = Builders<BsonDocument>.Filter.Ne<string>(element.Field, element.Value);
                                        break;

                                    case QueryElementClass.QueryOps.Gt:
                                        c_Ans = Builders<BsonDocument>.Filter.Gt<string>(element.Field, element.Value);
                                        break;

                                    case QueryElementClass.QueryOps.Gte:
                                        c_Ans = Builders<BsonDocument>.Filter.Gte<string>(element.Field, element.Value);
                                        break;

                                    case QueryElementClass.QueryOps.Lt:
                                        c_Ans = Builders<BsonDocument>.Filter.Lt<string>(element.Field, element.Value);
                                        break;

                                    case QueryElementClass.QueryOps.Lte:
                                        c_Ans = Builders<BsonDocument>.Filter.Lte<string>(element.Field, element.Value);
                                        break;

                                    case QueryElementClass.QueryOps.In:
                                        c_Ans = Builders<BsonDocument>.Filter.In<string>(element.Field, element.Value.SplitSpaces(true));
                                        break;

                                    case QueryElementClass.QueryOps.Nin:
                                        c_Ans = Builders<BsonDocument>.Filter.Nin<string>(element.Field, element.Value.SplitSpaces(true));
                                        break;
                                }
                                break;

                            case Definitions.DatasetFieldClass.QueryTypes.Double:
                                switch (element.Operator)
                                {
                                    case QueryElementClass.QueryOps.Eq:
                                        c_Ans = Builders<BsonDocument>.Filter.Eq<double>(element.Field, element.Value.ToDouble());
                                        break;

                                    case QueryElementClass.QueryOps.Ne:
                                        c_Ans = Builders<BsonDocument>.Filter.Ne<double>(element.Field, element.Value.ToDouble());
                                        break;

                                    case QueryElementClass.QueryOps.Gt:
                                        c_Ans = Builders<BsonDocument>.Filter.Gt<double>(element.Field, element.Value.ToDouble());
                                        break;

                                    case QueryElementClass.QueryOps.Gte:
                                        c_Ans = Builders<BsonDocument>.Filter.Gte<double>(element.Field, element.Value.ToDouble());
                                        break;

                                    case QueryElementClass.QueryOps.Lt:
                                        c_Ans = Builders<BsonDocument>.Filter.Lt<double>(element.Field, element.Value.ToDouble());
                                        break;

                                    case QueryElementClass.QueryOps.Lte:
                                        c_Ans = Builders<BsonDocument>.Filter.Lte<double>(element.Field, element.Value.ToDouble());
                                        break;

                                    case QueryElementClass.QueryOps.In:
                                        c_Ans = Builders<BsonDocument>.Filter.In<double>(element.Field, element.Value.SplitSpaces(true).ToDouble());
                                        break;

                                    case QueryElementClass.QueryOps.Nin:
                                        c_Ans = Builders<BsonDocument>.Filter.Nin<double>(element.Field, element.Value.SplitSpaces(true).ToDouble());
                                        break;
                                }
                                break;

                            case Definitions.DatasetFieldClass.QueryTypes.Boolean:
                                switch (element.Operator)
                                {
                                    case QueryElementClass.QueryOps.Eq:
                                        c_Ans = Builders<BsonDocument>.Filter.Eq<string>(element.Field, element.Value.ToBoolean().ToDBBoolean());
                                        break;

                                    case QueryElementClass.QueryOps.Ne:
                                        c_Ans = Builders<BsonDocument>.Filter.Ne<string>(element.Field, element.Value.ToBoolean().ToDBBoolean());
                                        break;
                                }
                                break;

                            case Definitions.DatasetFieldClass.QueryTypes.DateTime:
                                switch (element.Operator)
                                {
                                    case QueryElementClass.QueryOps.Eq:
                                        c_Ans = Builders<BsonDocument>.Filter.Eq<DateTime>(element.Field, element.Value.FromDBDate());
                                        break;

                                    case QueryElementClass.QueryOps.Ne:
                                        c_Ans = Builders<BsonDocument>.Filter.Ne<DateTime>(element.Field, element.Value.FromDBDate());
                                        break;

                                    case QueryElementClass.QueryOps.Gt:
                                        c_Ans = Builders<BsonDocument>.Filter.Gt<DateTime>(element.Field, element.Value.FromDBDate());
                                        break;

                                    case QueryElementClass.QueryOps.Gte:
                                        c_Ans = Builders<BsonDocument>.Filter.Gte<DateTime>(element.Field, element.Value.FromDBDate());
                                        break;

                                    case QueryElementClass.QueryOps.Lt:
                                        c_Ans = Builders<BsonDocument>.Filter.Lt<DateTime>(element.Field, element.Value.FromDBDate());
                                        break;

                                    case QueryElementClass.QueryOps.Lte:
                                        c_Ans = Builders<BsonDocument>.Filter.Lte<DateTime>(element.Field, element.Value.FromDBDate());
                                        break;

                                    case QueryElementClass.QueryOps.In:
                                        c_Ans = Builders<BsonDocument>.Filter.In<DateTime>(element.Field, element.Value.SplitSpaces(true).FromDBDate());
                                        break;

                                    case QueryElementClass.QueryOps.Nin:
                                        c_Ans = Builders<BsonDocument>.Filter.Nin<DateTime>(element.Field, element.Value.SplitSpaces(true).FromDBDate());
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// COnverts query to string
        /// 
        /// </summary>
        /// <returns>The string representation of the query</returns>
        public override string ToString()
        {
            // Into JSON array
            JArray c_Wkg = new JArray();

            // Loop thru
            foreach (QueryElementClass c_Ele in this.Values)
            {
                // Make JSON object
                c_Wkg.Add(c_Ele.ToJObject());
            }

            return c_Wkg.ToSimpleString();
        }

        /// <summary>
        /// 
        /// Clears the query
        /// 
        /// </summary>
        public void Reset()
        {
            this.Values = new List<QueryElementClass>();
        }

        /// <summary>
        ///  Adds a filter entry
        ///  
        /// </summary>
        /// <param name="field">The field</param>
        /// <param name="op">The operator</param>
        /// <param name="value">The value</param>
        public void Add(string field, string value, QueryElementClass.QueryOps ops = QueryElementClass.QueryOps.Eq)
        {
            // Parse the value
            Tuple<QueryElementClass.QueryOps, string> c_Parsed = QueryElementClass.Parse(value, ops);

            //
            this.Add(field, c_Parsed.Item1, c_Parsed.Item2);
        }

        /// <summary>
        ///  Adds a filter entry
        ///  
        /// </summary>
        /// <param name="field">The field</param>
        /// <param name="op">The operator</param>
        /// <param name="value">The value</param>
        public void Add(string field, QueryElementClass.QueryOps op, string value)
        {
            // Make a new entry
            QueryElementClass c_Ele = new QueryElementClass(null, field, value, op);

            this.Values.Add(c_Ele);
        }

        /// <summary>
        /// 
        /// Adds an ID match
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void AddByID(string id)
        {
            this.Add("_id", QueryElementClass.QueryOps.Eq, id);
        }
        #endregion

        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ObjectClass> FindObjects(int max = -1, int skip = 0, string sort = null, bool asc = true)
        {
            // Assume none
            List<ObjectClass> c_Ans = new List<ObjectClass>();

            // Loop thru
            foreach(BsonDocument c_Doc in this.Find(max, skip, sort, asc))
            {
                c_Ans.Add(new ObjectClass(this.Parent.Parent, c_Doc));
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<BsonDocument> Find(int max = -1, int skip = 0, string sort = null, bool asc = true, string fields = null)
        {
            // Assume none
            List<BsonDocument> c_Ans = new List<BsonDocument>();

            //
            ProjectionDefinition<BsonDocument> c_Fields = null;
            if (fields.HasValue())
            {
                // Make list
                foreach(string sField in fields.SplitSpaces(true))
                {
                    if(c_Fields == null)
                    {
                        c_Fields = Builders<BsonDocument>.Projection.Include(new StringFieldDefinition<BsonDocument>(sField));
                    }
                    else
                    {
                        c_Fields = c_Fields.Include<BsonDocument>(new StringFieldDefinition<BsonDocument>(sField));
                    }
                }
            }

            // Find
            var c_Wkg = this.Parent.Documents.Find<BsonDocument>(this.Filter);
            if(c_Fields != null)  c_Wkg = c_Wkg.Project<BsonDocument>(c_Fields);

            // Sort
            if (sort.HasValue())
            {
                //
                SortDefinition<BsonDocument> c_Sort = Builders<BsonDocument>.Sort.Ascending(sort);
                if (!asc)
                {
                    c_Sort = Builders<BsonDocument>.Sort.Descending(sort);
                }
                //
                c_Wkg = c_Wkg.Sort(c_Sort);
            }
            // Set paging
            if (max != -1) c_Wkg.Limit(max);
            if (skip > 0) c_Wkg.Skip(skip);

            // Add
            return c_Wkg.ToList<BsonDocument>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long ComputeCount(int max = -1)
        {
            // Assume none
            List<BsonDocument> c_Ans = new List<BsonDocument>();

            // Find
            var c_Wkg = this.Parent.Documents.Find<BsonDocument>(this.Filter);
            // Set paging
            if (max != -1) c_Wkg.Limit(max);

            // Add
            return c_Wkg.CountDocuments();
        }

        /// <summary>
        /// 
        /// Updates any number of documents
        /// 
        /// </summary>
        /// <param name="updates"></param>
        /// <param name="filter"></param>
        /// <param name="many"></param>
        /// <returns></returns>
        public long Update(BsonDocument updates, bool many = false)
        {
            // Assume none
            long iAns = 0;

            // According
            if (!many)
            {
                iAns = this.Parent.Documents.UpdateOne(this.Filter, updates).ModifiedCount;
            }
            else
            {
                iAns = this.Parent.Documents.UpdateMany(this.Filter, updates).ModifiedCount;
            }

            return iAns;
        }

        /// <summary>
        /// 
        /// Deletes any number of documents
        /// 
        /// </summary>
        /// <param name="updates"></param>
        /// <param name="filter"></param>
        /// <param name="many"></param>
        /// <returns></returns>
        public long Delete(bool many = false)
        {
            // Assume none
            long iAns = 0;

            // Protext
            try
            {
                // According
                if (!many)
                {
                    iAns = this.Parent.Documents.DeleteOne(this.Filter).DeletedCount;
                }
                else
                {
                    iAns = this.Parent.Documents.DeleteMany(this.Filter).DeletedCount;
                }
            }
            catch { }

            return iAns;
        }

        /// <summary>
        /// 
        /// Pops an object from the collection, makig it a FIFO queue
        /// 
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public ObjectClass PopObject(string sort = null, bool asc = true)
        {
            // Assume none
            ObjectClass c_Ans = null;

            FindOneAndDeleteOptions<BsonDocument> c_Opts = new MongoDB.Driver.FindOneAndDeleteOptions<BsonDocument>();
            if (sort.HasValue())
            {
                //
                SortDefinition<BsonDocument> c_Sort = Builders<BsonDocument>.Sort.Ascending(sort);
                if (!asc)
                {
                    c_Sort = Builders<BsonDocument>.Sort.Descending(sort);
                }
                c_Opts.Sort = c_Sort;
            }

            // Find
            var c_Wkg = this.Parent.Documents.FindOneAndDelete<BsonDocument>(this.Filter, c_Opts);

            // Any?
            if (c_Wkg != null)
            {
                // Add
                c_Ans = new ObjectClass(this.Parent.Parent, c_Wkg);
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Returns true if the query matches at least one object
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Contains()
        {
            return this.Find(1).Count == 1;
        }
        #endregion

        #region IEnumerable
        /// <summary>
        /// 
        /// So we can loop thru
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }
        #endregion

        #region Statics
        public static QueryClass ByID(CollectionClass coll, string id)
        {
            QueryClass c_Ans = new QueryClass(coll);
            c_Ans.AddByID("id");

            return c_Ans;
        }
        #endregion
    }
}
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
/// Install-Package MongoDb.Bson -Version 2.11.0
///  

using MongoDB.Bson;

using NX.Shared;
using NX.Engine;

namespace Proc.AO.Definitions
{
    public class PickListClass : ExtraClass
    {
        #region Constants
        public const string Prefix = "pick_";
        #endregion

        #region Constructor
        internal PickListClass(AO.DatasetClass ds, string name)
            : base(ds, Prefix + name)
        {
            //
            this.OriginalName = name;

            // Defaults
            if (!this.AllAny.HasValue()) this.AllAny = "All";
            if (!this.Icon.HasValue()) this.Icon = "cog";
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The passed name
        /// 
        /// </summary>
        private string OriginalName { get; set; }

        /// <summary>
        /// 
        /// The code ID
        /// 
        /// </summary>
        public string Code
        {
            get { return this.Object["_id"]; }
        }

        /// <summary>
        /// 
        /// The display label
        /// 
        /// </summary>
        public string Label
        {
            get { return this.Object["label"]; }
            set { this.Object["label"] = value; }
        }

        /// <summary>
        /// 
        /// The icon
        /// 
        /// </summary>
        public string Icon
        {
            get { return this.Object["icon"]; }
            set { this.Object["icon"] = value; }
        }

        /// 
        /// The All/Any
        /// 
        /// </summary>
        public string AllAny
        {
            get { return this.Object["allany"]; }
            set { this.Object["allany"] = value; }
        }

        /// <summary>
        /// 
        /// The field
        /// 
        /// </summary>
        public string Field1
        {
            get { return this.Object["field1"]; }
            set { this.Object["field1"] = value; }
        }

        /// <summary>
        /// 
        /// The op
        /// 
        /// </summary>
        public string Op1
        {
            get { return this.Object["op1"]; }
            set { this.Object["op1"] = value; }
        }

        /// <summary>
        /// 
        /// The query
        /// 
        /// </summary>
        public string Value1
        {
            get { return this.Object["value1"]; }
            set { this.Object["value1"] = value; }
        }

        /// <summary>
        /// 
        /// The field
        /// 
        /// </summary>
        public string Field2
        {
            get { return this.Object["field2"]; }
            set { this.Object["field2"] = value; }
        }

        /// <summary>
        /// 
        /// The op
        /// 
        /// </summary>
        public string Op2
        {
            get { return this.Object["op2"]; }
            set { this.Object["op2"] = value; }
        }

        /// <summary>
        /// 
        /// The query
        /// 
        /// </summary>
        public string Value2
        {
            get { return this.Object["value2"]; }
            set { this.Object["value2"] = value; }
        }

        /// <summary>
        /// 
        /// The field
        /// 
        /// </summary>
        public string Field3
        {
            get { return this.Object["field3"]; }
            set { this.Object["field3"] = value; }
        }

        /// <summary>
        /// 
        /// The op
        /// 
        /// </summary>
        public string Op3
        {
            get { return this.Object["op3"]; }
            set { this.Object["op3"] = value; }
        }

        /// <summary>
        /// 
        /// The query
        /// 
        /// </summary>
        public string Value3
        {
            get { return this.Object["value3"]; }
            set { this.Object["value3"] = value; }
        }

        /// <summary>
        /// 
        /// The field
        /// 
        /// </summary>
        public string Field4
        {
            get { return this.Object["field4"]; }
            set { this.Object["field4"] = value; }
        }

        /// <summary>
        /// 
        /// The op
        /// 
        /// </summary>
        public string Op4
        {
            get { return this.Object["op4"]; }
            set { this.Object["op4"] = value; }
        }

        /// <summary>
        /// 
        /// The query
        /// 
        /// </summary>
        public string Value4
        {
            get { return this.Object["value4"]; }
            set { this.Object["value4"] = value; }
        }

        /// <summary>
        /// 
        /// The field
        /// 
        /// </summary>
        public string Field5
        {
            get { return this.Object["field5"]; }
            set { this.Object["field5"] = value; }
        }

        /// <summary>
        /// 
        /// The op
        /// 
        /// </summary>
        public string Op5
        {
            get { return this.Object["op5"]; }
            set { this.Object["op5"] = value; }
        }

        /// <summary>
        /// 
        /// The query
        /// 
        /// </summary>
        public string Value5
        {
            get { return this.Object["value5"]; }
            set { this.Object["value5"] = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Saves the definition
        /// 
        /// </summary>
        public void Save()
        {
            // Save
            this.Object.Save(force:true);

            // Signal
            this.Parent.Parent.Parent.SignalChange(this);

            this.Parent.Parent.RemoveFromCache(this.OriginalName);
        }

        /// <summary>
        /// 
        /// Delete the view
        /// 
        /// </summary>
        public void Delete()
        {
            //
            this.Object.Delete();

            // Signal
            this.Parent.Parent.Parent.SignalChange(this, true);
        }
        #endregion
    }
}
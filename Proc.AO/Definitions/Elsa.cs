//--------------------------------------------------------------------------------
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

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MongoDB.Bson;

using NX.Engine;
using NX.Shared;

namespace Proc.AO.Definitions
{
    public class ElsaClass : IDisposable
    {
        #region Constructor
        internal ElsaClass(AO.ObjectClass obj, string name, string type) //  Proc.AO.DatasetClass ds, string prefix, string name)
                                                                         //: base(ds, prefix + name)
        {
            //
            this.OriginalName = name;
            this.Steps = new NamedListClass<ElsaActivityClass>();
            this.Type = type;
            this.Value = new StoreClass(obj.AsJObject);
            this.Object = obj;
            this.Name = obj.UUID.ID;
            this.Dataset = obj.UUID.Dataset.Name;

            // Get values
            JArray c_Activities = this.Value["activities"].ToJArray();
            JArray c_Connections = this.Value["connections"].ToJArray();

            // Build
            for (int i = 0; i < c_Activities.Count; i++)
            {
                // Make
                ElsaActivityClass c_Step = new ElsaActivityClass(this, c_Activities.GetJObject(i), c_Connections);
                // Add
                this.Steps[c_Step.ID] = c_Step;
            }

            // Loop thru
            for (int i = 0; i < c_Connections.Count; i++)
            {
                // Get
                JObject c_Entry = c_Connections.GetJObject(i);
                // Get target
                string sTarget = c_Entry.Get("destinationActivityId");
                // Map
                ElsaActivityClass c_Target = this.Steps[sTarget];
                // Set
                if (c_Target != null) c_Target.IsCalled = true;
            }
        }

        public ElsaClass()
        { }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Indexer
        public ElsaActivityClass this[string id]
        {
            get { return this.Steps[id]; }
        }
        #endregion

        #region Properties
        public StoreClass Value { get; private set; }

        #region If object based
        public AO.ObjectClass Object { get; private set; }

        public string Dataset { get; private set; }

        public string Name { get; private set; }
        #endregion

        /// <summary>
        /// 
        /// The passed name
        /// 
        /// </summary>
        public string OriginalName { get; private set; }

        /// <summary>
        /// 
        /// The passed type
        /// 
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// 
        /// The steps
        /// 
        /// </summary>
        public NamedListClass<ElsaActivityClass> Steps { get; private set; } = new NamedListClass<ElsaActivityClass>();

        /// <summary>
        /// 
        /// The number of lines
        /// 
        /// </summary>
        public int Count { get { return this.Steps.Count; } }

        /// <summary>
        /// 
        /// Caption if tab
        /// 
        /// </summary>
        public string Caption
        {
            get { return this.Value["caption"]; }
            set { this.Value["caption"] = value; }
        }
        #endregion
    }

    public class ElsaActivityClass : ChildOfClass<ElsaClass>
    {
        #region Constructor
        public ElsaActivityClass(ElsaClass task, JObject def, JArray conn)
            : base(task)
        {
            //
            this.Values = def;

            // Build parameters
            JObject c_PRaw = this.Values.GetJObject("state");
            // Loop thru
            foreach (string sFld in c_PRaw.Keys())
            {
                // Get the value
                JObject c_PValue = c_PRaw.GetJObject(sFld);
                // Save
                this.Parameters[sFld] = c_PValue.Get("expression");
            }

            // Build connectors
            for (int i = 0; i < conn.Count; i++)
            {
                // Map
                JObject c_Conn = conn.GetJObject(i);

                // Ours?
                if (c_Conn.Get("sourceActivityId").IsSameValue(this.ID))
                {
                    // Map
                    this.Outcomes[c_Conn.Get("outcome")] = c_Conn.Get("destinationActivityId");
                }

                // Into us?
                if (c_Conn.Get("destinationActivityId").IsSameValue(this.ID))
                {
                    //Map
                    this.Inputs[c_Conn.Get("outcome")] = c_Conn.Get("sourceActivityId");
                }
            }
        }
        #endregion

        #region Indexer
        /// <summary>
        /// 
        /// Returns a field value
        /// 
        /// </summary>
        /// <param name="fld"></param>
        /// <returns></returns>
        public string this[string fld]
        {
            get { return this.Parameters[fld]; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Store
        /// 
        /// </summary>
        public JObject Values { get; private set; }

        /// <summary>
        /// 
        /// The step ID
        /// 
        /// </summary>
        public string ID { get { return this.Values.Get("id"); } }

        /// <summary>
        /// 
        /// The step type
        /// 
        /// </summary>
        public string Type { get { return this.Values.Get("type"); } }

        /// <summary>
        /// 
        /// The connections out
        /// 
        /// </summary>
        public NamedListClass<string> Outcomes { get; private set; } = new NamedListClass<string>();

        /// <summary>
        /// 
        /// The connections in
        /// 
        /// </summary>
        public NamedListClass<string> Inputs { get; private set; } = new NamedListClass<string>();

        /// <summary>
        /// 
        /// Parameters
        /// 
        /// </summary>
        public NamedListClass<string> Parameters { get; private set; } = new NamedListClass<string>();

        /// <summary>
        /// 
        /// Returns a list of parameters
        /// 
        /// </summary>
        public List<string> Keys { get { return new List<string>(this.Parameters.Keys); } }
        /// <summary>
        /// 
        /// Is it called by another
        /// 
        /// </summary>
        public bool IsCalled { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Returns true if key is a parameter
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return this.Parameters.Contains(key);
        }
        #endregion
    }
}
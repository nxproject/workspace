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

using NX.Shared;
using NX.Engine;

using Newtonsoft.Json.Linq;

namespace Proc.Task
{
    public class ChargesClass : BasedObjectClass
    {
        #region Constructor
        public ChargesClass(JArray values)
            : base(values)
        { }
        #endregion

        #region Indexer
        public ChargeClass this[int index]
        {
            get
            {
                ChargeClass c_Ans = null;

                if (index >= 0 && index < this.Count)
                {
                    c_Ans = new ChargeClass(this.Values.AssureJObject(index));
                }
                return c_Ans;
            }
        }
        #endregion

        #region Properties
        private JArray Values { get { return this.Root as JArray; } }

        public int Count { get { return this.Values.Count; } }

        public List<ChargeClass> AsList
        {
            get
            {
                List<ChargeClass> c_Ans = new List<ChargeClass>();

                for (int i = 0; i < this.Count; i++)
                {
                    c_Ans.Add(this[i]);
                }

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return this.Values.ToSimpleString();
        }

        public string ToString(string objid)
        {
            for (int i = 0; i < this.Count; i++)
            {
                using (ChargeClass c_Charge = this[i])
                {
                    c_Charge.ObjectID = objid;
                }
            }

            return this.ToString();
        }

        public ChargeClass New(string code,
                                string desc,
                                string details,
                                double units,
                                double rate,
                                bool indv = false)
        {
            ChargeClass c_Ans = new ChargeClass(units, code, desc, details, rate, indv);

            this.Values.Add(c_Ans.AsObject);

            return c_Ans;
        }

        public void Clear()
        {
            this.Values.Clear();
        }
        #endregion
    }

    public class ChargeClass : IDisposable
    {
        #region Constructor
        //private ChargeClass(double units, string code)
        //    : this(units, code, "", null, 0)
        //{ }

        internal ChargeClass(double units, string code, string details)
            : this(units, code, "", details)
        { }

        internal ChargeClass(double units,
                                string code,
                                string desc,
                                string details,
                                double rate = 0,
                                bool indv = false,
                                string orig = "")
        {
            //
            this.Units = units;
            this.Code = code.IfEmpty();
            this.VRate = rate;
            this.Desc = desc;
            this.Details = details.IfEmpty();
            this.Originator = orig;
            this.Individual = indv;
        }

        internal ChargeClass(JObject value)
        {
            //
            this.AsObject = value;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public double Units { get; private set; }
        public string Code { get; private set; }
        public string Desc { get; private set; }
        public string Details { get; private set; }
        public double VRate { get; private set; }

        public bool Individual { get; private set; }
        public DateTime On { get; set; } = DateTime.MinValue;

        public string Originator { get; internal set; }
        public string ObjectID { get; internal set; }

        internal JObject AsObject
        {
            get
            {
                JObject c_Ans = new JObject();

                c_Ans.Set("units", this.Units);
                c_Ans.Set("code", this.Code);
                if (this.Desc.HasValue()) c_Ans.Set("desc", this.Desc);
                if (this.Details.HasValue()) c_Ans.Set("detail", this.Details);
                if (this.VRate != 0) c_Ans.Set("vrate", this.VRate);
                if (this.Originator.HasValue()) c_Ans.Set("source", this.Originator);
                if (this.Individual) c_Ans.Set("indv", "Y");
                if (this.ObjectID.HasValue()) c_Ans.Set("objid", this.ObjectID);
                if (this.On != DateTime.MinValue) c_Ans.Set("on", this.On.ToDBDate());

                return c_Ans;
            }
            set
            {
                this.Units = value.Get("units").ToDouble(0);
                this.Code = value.Get("code");
                this.Desc = value.Get("desc");
                this.Details = value.Get("detail");
                this.VRate = value.Get("vrate").ToDouble();
                this.Originator = value.Get("source");
                this.Individual = value.Get("indv").IsSameValue("Y");
                this.ObjectID = value.Get("objid");
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return this.AsObject.ToSimpleString();
        }

        public void Save(string user)
        {
            // TBD
        }
        #endregion
    }
}
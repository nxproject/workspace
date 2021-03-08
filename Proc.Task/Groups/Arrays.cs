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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class ArrayClass : IDisposable
    {
        #region Constructor
        public ArrayClass()
        {
            //
            this.SynchObject = new List<object>();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Enums
        public enum Types
        {
            Undefined,
            Null,
            String,
            Number,
            JObject,
            JArray,
            Object
        }
        #endregion

        #region Properties
        internal List<object> SynchObject { get; set; }

        public int Count { get { return this.SynchObject.Count; } }
        #endregion

        #region Methods
        public string ToSimpleString()
        {
            JArray c_Ans = new JArray();

            for (int i = 0; i < this.Count; i++)
            {
                switch (this.GetType(i))
                {
                    case Types.Object:
                        c_Ans.Add(this.GetObject(i).ToStringSafe());
                        break;

                    default:
                        c_Ans.Add(this.SynchObject[i]);
                        break;
                }
            }

            return c_Ans.ToSimpleString();
        }

        public void Remove(int index)
        {
            if (index >= 0 && index < this.Count)
            {
                this.SynchObject.RemoveAt(index);
            }
        }

        public Types GetType(int index)
        {
            Types eAns = Types.Undefined;

            if (index >= 0 && index < this.Count)
            {
                object c_Wkg = this.SynchObject[index];

                if (c_Wkg == null)
                {
                    eAns = Types.Number;
                }
                else if (c_Wkg is JObject)
                {
                    eAns = Types.JObject;
                }
                else if (c_Wkg is JArray)
                {
                    eAns = Types.JArray;
                }
                else if (c_Wkg is string)
                {
                    eAns = Types.String;
                }
                else if (c_Wkg is double)
                {
                    eAns = Types.Number;
                }
                else
                {
                    eAns = Types.Object;
                }
            }

            return eAns;
        }

        public string GetString(int index)
        {
            string sAns = null;

            switch (this.GetType(index))
            {
                case Types.String:
                    sAns = this.SynchObject[index].ToStringSafe();
                    break;
            }

            return sAns;
        }

        public double GetNumber(int index)
        {
            double sAns = 0;

            switch (this.GetType(index))
            {
                case Types.String:
                    sAns = this.SynchObject[index].ToDouble();
                    break;
            }

            return sAns;
        }

        public JObject GetJObject(int index)
        {
            JObject c_Ans = null;

            switch (this.GetType(index))
            {
                case Types.JObject:
                    c_Ans = this.SynchObject[index] as JObject;
                    break;
            }

            return c_Ans;
        }

        public JArray GetJArray(int index)
        {
            JArray c_Ans = null;

            switch (this.GetType(index))
            {
                case Types.JArray:
                    c_Ans = this.SynchObject[index] as JArray;
                    break;
            }

            return c_Ans;
        }

        public object GetObject(int index)
        {
            object c_Ans = null;

            switch (this.GetType(index))
            {
                case Types.Object:
                    c_Ans = this.SynchObject[index] as object;
                    break;
            }

            return c_Ans;
        }

        public void Set(int index, string value)
        {
            if (this.GetType(index) != Types.Undefined)
            {
                this.SynchObject[index] = value;
            }
        }

        public void Set(int index, double value)
        {
            if (this.GetType(index) != Types.Undefined)
            {
                this.SynchObject[index] = value;
            }
        }

        public void Set(int index, JObject value)
        {
            if (this.GetType(index) != Types.Undefined)
            {
                this.SynchObject[index] = value;
            }
        }

        public void Set(int index, JArray value)
        {
            if (this.GetType(index) != Types.Undefined)
            {
                this.SynchObject[index] = value;
            }
        }

        public void Set(int index, object value)
        {
            if (this.GetType(index) != Types.Undefined)
            {
                this.SynchObject[index] = value;
            }
        }

        public void Add(object value)
        {
            this.SynchObject.Add(value);
        }

        public void Insert(int index, object value)
        {
            if (this.Count == 0)
            {
                this.Add(value);
            }
            else if (index <= 0)
            {
                this.SynchObject.Insert(0, value);
            }
            else if (index >= this.Count)
            {
                this.Add(value);
            }
            else
            {
                this.SynchObject.Insert(index, value);
            }
        }
        #endregion
    }
}

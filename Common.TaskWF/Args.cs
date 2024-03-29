﻿///--------------------------------------------------------------------------------
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

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Proc.AO;

namespace Common.TaskWF
{
    /// <summary>
    /// 
    /// args to call
    /// 
    /// </summary>
    public class ArgsClass : ChildOfClass<Context>
    {
        #region Constructor
        public ArgsClass(Context ctx, Proc.AO.Definitions.ElsaClass ext, string line, int depth)
            : base(ctx)
        {
            //
            this.Definition = ext;
            this.LineID = line;
            this.Step = this.Definition[this.LineID];
            this.Depth = depth;
        }

        public ArgsClass(Context ctx, string values, int depth)
            : base(ctx)
        {
            //
            this.Depth = depth;
            this.LocalParameters = new NamedListClass<string>();

            //
            if (values.HasValue())
            {
                List<string> c_Passed = values.IfEmpty().SplitSpaces();
                // Loop thru
                for (int iLoop = 0; iLoop < c_Passed.Count; iLoop += 2)
                {
                    this.LocalParameters[c_Passed[iLoop]] = c_Passed[iLoop + 1];
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Internals
        /// 
        /// </summary>
        //public NamedListClass<string> IDefined { get; private set; } = new NamedListClass<string>();

        /// <summary>
        /// 
        /// The definition
        /// 
        /// </summary>
        public Proc.AO.Definitions.ElsaClass Definition { get; private set; }

        /// <summary>
        /// 
        /// The activity ID
        /// 
        /// </summary>
        public string LineID { get; set; }

        /// <summary>
        /// 
        /// The step
        /// 
        /// </summary>
        public Proc.AO.Definitions.ElsaActivityClass Step { get; private set; }

        /// <summary>
        /// 
        /// The depth
        /// 
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// 
        /// Parameters used in original call
        /// 
        /// </summary>
        private NamedListClass<string> LocalParameters { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the un-evaluated field
        /// 
        /// </summary>
        /// <param name="fld">The field</param>
        /// <returns>The raw value</returns>
        public string GetRaw(string fld)
        {
            string sAns = null;

            // Local?
            if (this.LocalParameters != null)
            {
                sAns = this.LocalParameters[fld];
            }
            else if (this.Step != null)
            {
                sAns = this.Step[fld];
            }

            return sAns;
        }

        /// <summary>
        /// 
        /// Gets the evaluated value of a field 
        /// 
        /// </summary>
        /// <param name="fld">The field</param>
        /// <returns>The evaluation</returns>
        public string Get(string fld)
        {
            return DatumClass.Eval(this.Parent, this.GetRaw(fld));
        }

        /// <summary>
        /// 
        /// Gets a evaluated field as a boolean
        /// 
        /// </summary>
        /// <param name="fld">The field</param>
        /// <param name="defval">Default value</param>
        /// <returns>True/false</returns>
        public bool GetAsBool(string fld, bool defval = true)
        {
            bool bAns = false;

            string sWkg = this.Get(fld);

            if (!sWkg.HasValue())
            {
                bAns = defval;
            }
            else
            {
                if (sWkg.IsSameValue("true"))
                {
                    bAns = true;
                }
                else if (sWkg.IsSameValue("false"))
                {
                    bAns = false;
                }
                else
                {
                    bAns = sWkg.ToInteger(0) != 0;
                }
            }

            return bAns;
        }

        /// <summary>
        /// 
        /// Gets evaluated if
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetIf()
        {
            // Default
            bool bIf = true;

            // Local?
            if (this.LocalParameters != null)
            {
                // Check
                if (this.LocalParameters.Contains("if"))
                {
                    bIf = this.GetAsBool("if");
                }
            }
            else if (this.Step != null)
            {
                // Do we have an if?
                if (this.Step.Contains("if"))
                {
                    // Do we do?
                    bIf = this.GetAsBool("if");
                }
            }

            return bIf;
        }

        /// <summary>
        /// 
        /// Converts to string
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // 
            JObject c_Wkg = new JObject();

            // Local?
            if (this.LocalParameters != null)
            {
                // Loop theu
                foreach (string sKey in this.LocalParameters.Keys)
                {
                    // Addthis
                    c_Wkg.Set(sKey, this.GetRaw(sKey));
                }
            }
            else if (this.Step != null)
            {
                // Loop theu
                foreach (string sKey in this.Step.Keys)
                {
                    // Addthis
                    c_Wkg.Set(sKey, this.GetRaw(sKey));
                }
            }

            return c_Wkg.ToSimpleString();
        }
        #endregion
    }
}
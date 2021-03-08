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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class ArrayPop : CommandClass
    {
        #region Constants
        private const string ArgArray = "array";
        private const string ArgStore = "store";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ArrayPop()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgArray, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The name of the array"));
                c_P.Add(ArgStore, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The store name"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Array, "Pops a store from an array (remove last)", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "array.pop"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sArray = args.Get(ArgArray);
            string sStore = args.Get(ArgStore);
            
            // Get the list
                ArrayClass c_Docs = ctx.Arrays[sArray];

                // The target
                JObject c_Wkg = null;

                // Any?
                if(c_Docs.Count > 0)
                {
                    c_Wkg = c_Docs.GetJObject(c_Docs.Count - 1);
                    c_Docs.Remove(c_Docs.Count - 1);
                }

                if (c_Wkg == null) c_Wkg = new JObject();

                // Save
                ctx.Stores[sStore] = new StoreClass(c_Wkg);

            return eAns;
        }
        #endregion
    }
}
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

namespace Proc.Task.String
{
    public class WordPop : CommandClass
    {
        #region Constants
        private const string ArgString = "string";
        private const string ArgValue = "value";
        private const string ArgDelim = "delim";
        #endregion

        #region Constructor
        public WordPop()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgString, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The string"));
                c_P.Add(ArgValue, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The text"));
                c_P.Add(ArgDelim, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The delimiter"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Words, "Remove word from end of string", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "word.pop"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sString = args.Get(ArgString);
            string sDelim = args.Get(ArgDelim);
            string sValue = "";

            //
            int iPos = sString.LastIndexOf(sDelim);
            if(iPos == -1)
            {
                sValue = sString;
                sString = "";
            }
            else
            {
                sValue = sString.Substring(iPos + sDelim.Length);
                sString = sString.Substring(0, iPos);
            }

            // Do
            ctx[args.GetRaw(ArgString)] = sString;
            ctx[args.GetRaw(ArgValue)] = sValue;

            return eAns;
        }
        #endregion
    }
}

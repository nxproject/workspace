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
    public class TextDocList : CommandClass
    {
        #region Constants
        private const string ArgText = "text";
        private const string ArgList = "list";
        private const string ArgDelim = "delim";
        private const string ArgRow = "line";
        #endregion

        #region Constructor
        public TextDocList()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgText, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The text name"));
                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The list"));
                c_P.Add(ArgRow, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The row"));
                c_P.Add(ArgDelim, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The delimiter"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Text, "Appends a block of doc names to a memo", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "text.add.doclist"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sText = args.GetRaw(ArgText);
            string sList = args.Get(ArgList);
            string sDelim = args.Get(ArgDelim).IfEmpty("\r\n");
            string sRow = args.Get(ArgRow);

            // Do
            ASCIITextClass c_Text = ctx.Texts[sText].Text;

            int iRow = sRow.ToInteger(c_Text.LastRow);

            string sBlock = "";

            List<string> c_Docs = ctx.DocumentLists[sList];
            foreach (string sFile in c_Docs)
            {
                sBlock += ctx.Documents[sFile].NameOnly + sDelim;
            }

            if (sBlock.HasValue())
            {
                c_Text.Append(sBlock, iRow);
            }

            return eAns;
        }
        #endregion
    }
}
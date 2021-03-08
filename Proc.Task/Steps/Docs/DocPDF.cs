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

using System.Collections.Generic;

using NX.Shared;
using NX.Engine;
using Proc.Docs.Files;
using Common.TaskWF;

namespace Proc.Task.Docs
{
    public class DocPDF : CommandClass
    {
        #region Constants
        private const string ArgDoc = "doc";
        private const string ArgTo = "to";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public DocPDF()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgDoc, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The document to be merged"));
                c_P.Add(ArgTo, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The PDF document"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Docs, "Converts to PDF", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "doc.pdf"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sDoc = args.Get(ArgDoc);
            string sTo = args.Get(ArgTo);

            // Validate
            if (sDoc.HasValue() && sTo.HasValue())
            {
                //
                using (NX.Engine.Files.DocumentClass c_Source = ctx.Documents[sDoc])
                {
                    using (NX.Engine.Files.DocumentClass c_Target = ctx.Documents[sTo])
                    {
                        c_Source.PDF().Document.CopyTo(c_Target);
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
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
using System.IO;

using Newtonsoft.Json.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.awt.geom;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class OnePDF : CommandClass
    {
        #region Constants
        private const string ArgList = "doclist";
        private const string ArgTo = "to";
        #endregion

        #region Constructor
        public OnePDF()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The document list to be merged"));
                c_P.Add(ArgTo, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The merged document"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.PDF, "Merges multiple .PDF files into one", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "pdf.one"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sList = args.Get(ArgList);
            string sTo = args.Get(ArgTo);

            // Get the list
            List<string> c_Docs = ctx.DocumentLists[sList];

            DocumentClass c_Target = ctx.Documents[sTo];
            c_Target.AsWriteStream(delegate(Stream c_Result)  
            {
                PdfReader.unethicalreading = true;

                Document c_Output = new Document(PageSize.LETTER);
                PdfWriter c_Writer = PdfWriter.GetInstance(c_Output, c_Result);
                c_Output.Open();

                foreach (string sDoc in c_Docs)
                {
                    // Map
                    DocumentClass c_Doc = ctx.Documents[sDoc];
                    // Use stream
                    c_Doc.AsReadStream(delegate (Stream c_Input)
                    {
                        PdfReader c_Reader = new PdfReader(c_Input);

                        for (int index = 1; index <= c_Reader.NumberOfPages; index++)
                        {
                            c_Output.CopyPage(c_Reader, index, c_Writer);
                        }
                    });
                }

                c_Output.Close();
            });

            return eAns;
        }
        #endregion
    }
}
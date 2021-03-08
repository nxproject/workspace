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
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class SplitPDF : CommandClass
    {
        #region Constants
        private const string ArgDoc = "doc";
        private const string ArgPages = "pages";
        #endregion

        #region Constructor
        public SplitPDF()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgDoc, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The document to be split"));
                c_P.Add(ArgPages, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The page count (<0 if skip, * if pages left over)"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.PDF, "Splits one .PDF file into multiple by page count", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "pdf.split"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sDoc = args.Get(ArgDoc);
            string sPages = args.Get(ArgPages);

            // Get the letter
            NX.Engine.Files.DocumentClass c_Source = ctx.Documents[sDoc];
            string sPrefix = c_Source.NameOnly;

            if (sPages.HasValue())
            {
                c_Source.AsReadStream(delegate (Stream c_In)
                {
                    PdfReader.unethicalreading = true;

                    PdfReader c_Reader = new PdfReader(c_In);

                    List<string> c_SPages = sPages.SplitSpaces();

                    int iUsed = 0;
                    List<int> c_Pages = new List<int>();
                    List<int> c_Stars = new List<int>();

                    for (int i = 0; i < c_SPages.Count; i++)
                    {
                        c_Pages.Add(0);

                        string sPages = c_SPages[i];

                        if (sPages.IsSameValue("*"))
                        {
                            c_Stars.Add(i);
                        }
                        else
                        {
                            int iPages = sPages.ToInteger(0);
                            c_Pages[i] = iPages;

                            if (iPages < 0)
                            {
                                iUsed += -iPages;
                            }
                            else
                            {
                                iUsed += iPages;
                            }
                        }
                    }

                    if (c_Stars.Count > 0)
                    {
                        int iAvl = c_Reader.NumberOfPages - iUsed;
                        if (iAvl < 0) iAvl = 0;

                        int iBlk = (int)Math.Round(iAvl / (float)c_Stars.Count);
                        for (int j = 0; j < c_Stars.Count - 1; j++)
                        {
                            c_Pages[c_Stars[j]] = iBlk;
                            iAvl -= iBlk;
                        }

                        c_Pages[c_Stars[c_Stars.Count - 1]] = iAvl;
                    }

                    int iAt = 1;
                    int iFile = 0;

                    foreach (int iCount in c_Pages)
                    {
                        if (iCount <= 0)
                        {
                            iAt -= iCount;
                        }
                        else
                        {
                            if (iAt <= c_Reader.NumberOfPages)
                            {
                                iFile++;

                                string sName = sPrefix + "_" + iFile + ".pdf";

                                using (NX.Engine.Files.DocumentClass c_Output = new NX.Engine.Files.DocumentClass(ctx.DocumentManager, sName))
                                {
                                    c_Output.AsWriteStream(delegate (Stream c_Out)
                                    {
                                        Document c_Doc = new Document(PageSize.LETTER);
                                        PdfWriter c_Writer = PdfWriter.GetInstance(c_Doc, c_Out);
                                        c_Doc.Open();

                                        for (int i = 0; i < iCount; i++)
                                        {
                                            c_Doc.CopyPage(c_Reader, iAt, c_Writer);
                                            iAt++;

                                        }
                                    });
                                }
                            }
                        }
                    }
                });
            }



            return eAns;
        }
        #endregion
    }
}
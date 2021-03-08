///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class SelfSMSSetup : CommandClass
    {
        #region Constants
        private const string ArgMsg = "msg";
        private const string ArgDocs = "doclist";

        private const string ArgWord = "word";
        private const string ArgDelim = "delim";
        private const string ArgPatt = "patt";
        private const string ArgPWord = "pattword";
        private const string ArgSource = "source";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public SelfSMSSetup(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParameterDefinitionClass> c_P = new NamedListClass<ParameterDefinitionClass>();

                c_P.Add(ArgMsg, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "Field where message is to be stored"));
                c_P.Add(ArgDocs, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "Document list to use"));
                c_P.Add(ArgDelim, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "Word delimiter"));
                c_P.Add(ArgSource, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "Source message"));

                return new DescriptionClass(CategoriesClass.Comm, "Handles the receipt of a SelfSMS", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "selfsms.setup"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            //
            string sMsg = args.GetDefinedRaw(ArgMsg);
            string sDocs = args.GetDefinedRaw(ArgDocs);

            string sSource = args.GetDefinedRaw(ArgSource);
            
            // Get the input
            string sMessage = ctx[@"[*obj:msg]"];
            if (sSource.HasValue()) sMessage = ctx[sSource];
            JArray c_Docs = ctx[@"[*obj:docs]"].IfEmpty().ToJArray();

            // Regex
            int p = 1;
            string sPatt = args.GetDefinedRaw(ArgPatt + p);
            string sWord = args.GetDefinedRaw(ArgPWord + p);
            while (sPatt.HasValue() && sWord.HasValue())
            {
                Match c_Match = Regex.Match(sMessage, sPatt);
                if (c_Match.Success)
                {
                    if (c_Match.Groups.Count > 1)
                    {
                        string sPicked = "";
                        for (int i = 1; i < c_Match.Groups.Count; i++)
                        {
                            sPicked += c_Match.Groups[i].Value;
                        }
                        ctx[sWord] = sPicked;
                    }
                    else
                    {
                        ctx[sWord] = c_Match.Value;
                    }
                    sMessage = sMessage.Remove(c_Match.Index, c_Match.Length).Trim();
                }

                p++;
                sPatt = args.GetDefinedRaw(ArgPatt + p);
                sWord = args.GetDefinedRaw(ArgPWord + p);
            }

            // Parse
            string sDelim = args.GetDefined(ArgDelim).IfEmpty(" ");
            int w = 1;
            sWord = args.GetDefinedRaw(ArgWord + w);
            while (sWord.HasValue())
            {
                int iPos = sMessage.IndexOf(sDelim);
                if (iPos == -1)
                {
                    ctx[sWord] = sMessage;
                    sMessage = "";
                }
                else
                {
                    ctx[sWord] = sMessage.Substring(0, iPos);
                    sMessage = sMessage.Substring(iPos + sDelim.Length).Trim();
                }

                w++;
                sWord = args.GetDefinedRaw(ArgWord + w);
            }

            // Save
            if (sMsg.HasValue())
            {
                ctx[sMsg] = sMessage;
            }

            if (sDocs.HasValue() && c_Docs != null)
            {
                for (int i = 0; i < c_Docs.Count; i++)
                {
                    string sDoc = c_Docs.Get(i);
                    if (sDoc.HasValue())
                    {
                        ctx.DocumentLists[sDoc].Add(sDoc);
                    }
                }
            }

            eAns = ReturnClass.OK;

            return eAns;
        }
        #endregion
    }
}
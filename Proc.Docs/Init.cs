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

using System.Text;

using NX.Engine;
using NX.Shared;
using NX.Engine.Files;

namespace Proc.Docx
{
    /// <summary>
    /// 
    /// Default setup
    /// 
    /// </summary>
    public class Init : FNClass
    {
        public override void Initialize(EnvironmentClass env)
        {
            // Load encoding
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Load third party assemblies
            "itextsharp.xmlworker.dll".LoadAssembly(env);

            // Extend eval
            FunctionsDefinitions c_Defs = Context.FunctionsTable;

            // Files
            c_Defs.AddFn(new StaticFunction("docname", delegate (Context ctx, object[] ps)
            {
                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;

                string sAns = "";

                DocumentClass c_Doc = c_Ctx.Documents[XCVT.ToString(ps[0])];
                if (c_Doc != null) sAns = c_Doc.Name;

                return sAns;
            }, 1, 1,
            "Returns the document name",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The document reference")));
            c_Defs.AddFn(new StaticFunction("docshortname", delegate (Context ctx, object[] ps)
            {
                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;

                string sAns = "";

                DocumentClass c_Doc = c_Ctx.Documents[XCVT.ToString(ps[0])];
                if (c_Doc != null) sAns = c_Doc.NameOnly;

                return sAns;
            }, 1, 1,
            "Returns the document name without extension",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The document reference")));
            c_Defs.AddFn(new StaticFunction("docextension", delegate (Context ctx, object[] ps)
            {
                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;

                string sAns = "";

                DocumentClass c_Doc = c_Ctx.Documents[XCVT.ToString(ps[0])];
                if (c_Doc != null) sAns = c_Doc.Extension;

                return sAns;
            }, 1, 1,
            "Returns the document extension",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The document reference")));
            c_Defs.AddFn(new StaticFunction("docexists", delegate (Context ctx, object[] ps)
            {
                AO.ExtendedContextClass c_Ctx = ctx as AO.ExtendedContextClass;

                bool bAns = false;

                DocumentClass c_Doc = c_Ctx.Documents[XCVT.ToString(ps[0])];
                if (c_Doc != null) bAns = c_Doc.Exists;

                return bAns;
            }, 1, 1,
            "Returns true if the document exists",
            new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The document reference")));

            // Conversion
            NX.Engine.Files.ManagerClass.Conversion = delegate (DocumentClass docin)
            {
                // Assume no change
                DocumentClass c_Out = docin;

                // By the extension
                switch (docin.Extension.ToLower())
                {
                    case "doc":
                    case "docx":
                    case "txt":
                    case "rtf":
                    case "html":
                    case "htm":
                    case "odt":
                    case "wps":
                    case "wpd":
                        c_Out = new DocumentClass(docin.Parent, docin.Path.SetExtensionFromPath("odt"));
                        break;
                }

                // Do we need to convert?
                if (!docin.Path.IsSameValue(c_Out.Path))
                {
                    // Convert
                    Proc.Docs.Files.ConversionClass.Convert(docin, c_Out);
                    // If there, delete upload
                    if (c_Out.Exists) docin.Delete();
                }

                return c_Out;
            };

            base.Initialize(env);
        }
    }
}
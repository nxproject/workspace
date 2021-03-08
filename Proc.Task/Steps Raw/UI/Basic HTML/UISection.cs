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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

using Proc.AO;
using Proc.Docs;

namespace Proc.Chore
{
    public class UISct : UITarget
    {
        #region Constants
        private const string ArgTitle = "title";
        private const string ArgTitleType = "ttype";
        private const string ArgText = "text";
        private const string ArgImage = "image";
        #endregion

        #region Constructor
        public UISct(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a section element"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required, ArgTitle, "The title in the menu",
            CommandClass.Optional, ArgTitleType, "Title type (Main/H1/H2/H3/H4/None)",
            CommandClass.Optional, ArgImage, "Document to use as image",
            CommandClass.Optional, ArgText, "Text");

            }
        }

        public override string BaseCommand
        {
            get { return "section"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sTitle = args.GetDefined(ArgTitle);
            string sText = args.GetDefined(ArgText);
            string sImage = args.GetDefined(ArgImage);
            
            string sTType = args.GetDefined(ArgTitleType);
            UICmpClass.SectionTitleTypes eType = UICmpClass.SectionTitleTypes.None;
            if (sTitle.HasValue()) eType = UICmpClass.SectionTitleTypes.H2;
            if (sTType.HasValue())
            {
                try
                {
                    eType = (UICmpClass.SectionTitleTypes)Enum.Parse(typeof(UICmpClass.SectionTitleTypes), sTType, true);
                }
                catch { }
            }

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgTitle, sTitle))
            {
                // Map the image
                DocReferenceClass c_Doc = null;
                if (sImage.HasValue())
                {
                    c_Doc = env.Documents[sImage];
                    if (c_Doc == null || !c_Doc.Exists)
                    {
                        c_Doc = null;
                    }
                }

                //
                HTMLFragmentClass c_Ele = sText.UISection(sTitle, eType, c_Doc);

                // Save
                this.BaseExecStep(env, arguments, c_Ele);

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
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
    public class UIFrame : CommandClass
    {
        #region Constants
        public const string ArgID = "name";
        private const string ArgTitle = "title";
        private const string ArgTitleType = "ttype";
        private const string ArgText = "text";
        private const string ArgImage = "image";
        private const string ArgHRef = "href";
        private const string ArgChore = "chore";
        private const string ArgStore = "store";
        public const string ArgUse = "use";
        #endregion

        #region Constructor
        public UIFrame(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a frame"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required,ArgID, "The name of the frame",
            CommandClass.Required,ArgTitle, "The title in the menu",
            CommandClass.Optional, ArgTitleType, "Title type (Main/H1/H2/H3/H4/None)",
            CommandClass.Optional, ArgImage, "Document to use as image",
            CommandClass.Optional, ArgText, "Text",

            CommandClass.Optional, ArgHRef, "HRef if the frame redirects",
            CommandClass.Optional, ArgChore, "Chore if the frame runs a chore",
            CommandClass.Optional, ArgStore, "Store to pass if the frame runs a chore",
            CommandClass.Optional, ArgUse, "Set as default", "#1#");

            }
        }

        public override string Command
        {
            get { return "ui.frame"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sID = args.GetDefined(ArgID);
            string sTitle = args.GetDefined(ArgTitle);
            string sText = args.GetDefined(ArgText);
            string sImage = args.GetDefined(ArgImage);
            string sHRef = args.GetDefined(ArgHRef);
            string sChore = args.GetDefined(ArgChore);
            string sStore = args.GetDefined(ArgStore);
            bool bUse = args.GetDefinedAsBool(ArgUse, true);

            string sTType = args.GetDefined(ArgTitleType);
            UICtxClass.TitleTypes eType = UICtxClass.TitleTypes.None;
            if (sTitle.HasValue()) eType = UICtxClass.TitleTypes.H2;
            if (sTType.HasValue())
            {
                try
                {
                    eType = (UICtxClass.TitleTypes)Enum.Parse(typeof(UICtxClass.TitleTypes), sTType, true);
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
                UICtxClass c_Ele = null;

                // According to type
                if (sChore.HasValue())
                {
                    JObject c_Store = null;
                    if (sStore.HasValue()) c_Store = env.Stores[sStore];

                    c_Ele = new UICtxChoreClass("", sTitle, sChore, c_Store, env.UI.State.User);
                }
                else if(sHRef.HasValue())
                {
                    c_Ele = new UICtxRefClass("", sTitle, sHRef);
                }
                else
                {
                    c_Ele = new UICtxClass("", sTitle, eType);
                }

                // Append
                if(c_Ele != null)
                {
                    env.UI.Add(c_Ele);
                }

                //
                if (sID.HasValue())
                {
                    // Save
                    env.HTML[sID] = c_Ele;

                    // Use
                    if (bUse)
                    {
                        env.HTML.Use(sID);
                    }
                }

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
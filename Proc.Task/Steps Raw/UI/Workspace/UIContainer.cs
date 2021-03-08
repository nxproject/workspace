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
    public class UICtxCreate : CommandClass
    {
        #region Constants
        private const string ArgID = "name";
        private const string ArgTitle = "title";
        private const string ArgTitleType = "ttype";
        private const string ArgHRef = "href";
        private const string ArgSubTitle = "subtitle";
        private const string ArgImage = "image";
        private const string ArgChore = "chore";
        private const string ArgStore = "store";
        #endregion

        #region Constructor
        public UICtxCreate(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        
        {
            public override string Description { get { return

            public override string Description { get { return("Creates a container");

            CommandClass.Required,ArgID, "The name of the container");
            CommandClass.Required,ArgTitle, "The title in the menu");
            CommandClass.Optional, ArgHRef, "HREF if a link",
            CommandClass.Optional, ArgTitleType, "Title type (Main/H1/H2/H3/H4/None)",
            CommandClass.Optional, ArgSubTitle, "Element to use as a sub-title",
            CommandClass.Optional, ArgImage, "Document to use as image",
            CommandClass.Optional, ArgChore, "Chore to run",
            CommandClass.Optional, ArgStore, "Store to pass to chore",

            }
        }

        public override string Command
        {
            get { return "ui.container"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sID = args.GetDefined(ArgID);
            string sTitle = args.GetDefined(ArgTitle);
            string sHRef = args.GetDefined(ArgHRef);
            string sSubTitle = args.GetDefined(ArgSubTitle);
            string sImage = args.GetDefined(ArgImage);
            string sChore = args.GetDefined(ArgChore);
            string sStore = args.GetDefined(ArgStore);

            string sTType = args.GetDefined(ArgTitleType);
            UICtxClass.TitleTypes eType = UICtxClass.TitleTypes.H2;
            if(sTType.HasValue())
            {
                try
                {
                    eType = (UICtxClass.TitleTypes)Enum.Parse(typeof(UICtxClass.TitleTypes), sTType, true);
                }
                catch { }
            }

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgID, sID,
                                                ArgTitle, sTitle))
            {
                // Set the container
                UICtxClass c_Ctx = null;
                //
                if (sHRef.HasValue())
                {
                    c_Ctx = new UICtxRefClass(sID, sTitle, sHRef);
                }
                else if(sChore.HasValue())
                {
                    //
                    JObject c_Values = null;
                    if (sStore.HasValue()) c_Values = env.Stores[sStore];

                    c_Ctx = new UICtxChoreClass(sID, sTitle, sChore, c_Values, env.UI.State.User);
                }
                else
                {
                    c_Ctx = new UICtxClass(sID, sTitle, eType);

                    if(sImage.HasValue())
                    {
                        DocReferenceClass c_Doc = env.Documents[sImage];
                        if(c_Doc != null && c_Doc.Exists)
                        {
                            c_Ctx.Image = c_Doc;
                        }
                    }

                    if(sSubTitle.HasValue())
                    {
                        HTMLFragmentClass c_Sub = env.HTML[sSubTitle] as HTMLFragmentClass;
                        if(c_Sub != null)
                        {
                            c_Ctx.SubTitle = c_Sub;
                        }
                    }
                }

                // Save
                env.HTML[sID] = c_Ctx;
                env.UI.Add(c_Ctx);

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
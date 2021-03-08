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
    public class UIImage : UITarget
    {
        #region Constants
        private const string ArgImage = "image";
        #endregion

        #region Constructor
        public UIImage(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates an image area"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Optional, ArgImage, "Document to use as image");

            }
        }

        public override string BaseCommand
        {
            get { return "image"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sImage = args.GetDefined(ArgImage);

            // Validate
            if (this.CheckValidity(ctx,
                                                ArgImage, sImage))
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
                if (c_Doc != null)
                {
                    HTMLFragmentClass c_Ele = c_Doc.UIImage();

                    // Save
                    BaseExecStep(env, arguments, c_Ele);
                }

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
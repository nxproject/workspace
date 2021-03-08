﻿///--------------------------------------------------------------------------------
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
    public class UIFFileCreate : CommandClass
    {
        #region Constants
        private const string ArgID = "name";
        private const string ArgLabel = "label";
        private const string ArgField = "field";
        #endregion

        #region Constructor
        public UIFFileCreate(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Code Line
        public override string Description { get { return("Creates a form x field"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required, ArgID, "The name of the area",
            CommandClass.Required, ArgField, "The field name",
            CommandClass.Optional, ArgLabel, "The label");

            }
        }

        public override string Command
        {
            get { return "ui.form.file"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            // Get the parameters
            string sID = args.GetDefined(ArgID);
            string sLabel = args.GetDefined(ArgLabel);
            string sField = args.GetDefined(ArgField);
            
            // Validate
            if (this.CheckValidity(ctx,
                                                ArgID, sID,
                                                ArgField, sField))
            {
                HTMLFragmentClass c_Ele = sID.UIFileField(sLabel, sField, "", false, "", (JSFragmentClass)null);

                // Save
                env.HTML[sID] = c_Ele;

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}
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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class ObjDocCopy : CommandClass
    {
        #region Constants
        private const string ArgObj = "obj";
        private const string ArgTarget = "to";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjDocCopy()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgObj, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The object to use"));
                c_P.Add(ArgTarget, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The target object"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, "Copies all documents from an object", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.doc.copy"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get
            string sObj = args.Get(ArgObj);
            string sTarget = args.Get(ArgTarget);

            // Reference object
            AO.ObjectClass c_Obj = ctx.Objects[sObj];
            if (c_Obj != null)
            {
                //
                AO.ObjectClass c_TargetO = ctx.Objects[sTarget];
                if (c_TargetO != null)
                {
                    List<NX.Engine.Files.DocumentClass> c_Files = c_Obj.Folder.Files;
                    foreach (NX.Engine.Files.DocumentClass c_Source in c_Files)
                    {
                        using (NX.Engine.Files.DocumentClass c_Target = new NX.Engine.Files.DocumentClass(ctx.DocumentManager, c_TargetO.Folder, c_Source.Name))
                        {
                            c_Target.Value = c_Source.Value;
                        }
                    }
                }
            }

            return eAns;
        }
        #endregion
    }
}
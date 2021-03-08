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
    public class ObjOrganizer : CommandClass
    {
        #region Constants
        private const string ArgObj = "obj";
        private const string ArgName = "name";
        private const string ArgOptions = "options";
        private const string ArgDoc = "doc";
        private const string ArgFolder = "folder";
        #endregion

        #region Constructor
        public ObjOrganizer()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgObj, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The object to use"));
                c_P.Add(ArgOptions, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The dataset options"));
                c_P.Add(ArgDoc, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The document"));
                c_P.Add(ArgFolder, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "The folder"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Obj, "Creates an organizer for the object", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.organizer"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sObj = args.Get(ArgObj);
            string sOpts = args.Get(ArgOptions);
            string sDoc = args.Get(ArgDoc).IfEmpty("organizer");
            string sName = args.Get(ArgName);
            string sFolder = args.Get(ArgFolder);

            // Do
            AO.ObjectClass c_Obj = ctx.Objects[sObj];
            if (c_Obj != null)
            {
                using (OrganizerGeneratorClass c_Eng = new OrganizerGeneratorClass(ctx,
                                                                                    c_Obj,
                                                                                    null,
                                                                                    sOpts,
                                                                                    sName,
                                                                                    sFolder ,
                                                                                    null,
                                                                                    null))
                {
                    NX.Engine.Files.DocumentClass c_Doc = c_Eng.GeneratePDF();

                    ctx.Documents[sDoc] = c_Doc;
                }
            }

            return eAns;
        }
        #endregion
    }
}
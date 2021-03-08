﻿///--------------------------------------------------------------------------------
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
    public class ObjClearList : CommandClass
    {
        #region Constants
        private const string ArgList = "list";
        private const string ArgRem = "remove";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public ObjClearList()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The list to be emptied"));
                c_P.Add(ArgRem, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "Remove objects"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.ObjList, "Empties a list", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "obj.list.clear"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get the parameters
            string sList = args.Get(ArgList);
            bool bRem = args.GetAsBool(ArgRem, false);

            // Do we remove?
           if(bRem)
            {
                // Loop thru
                foreach(string sObj in ctx.ObjectLists[sList])
                {
                    ctx.Objects[sObj] = null;
                }
            }

            // 
            ctx.ObjectLists[sList].Clear();

            return eAns;
        }
        #endregion
    }
}
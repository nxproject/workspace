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

using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class DocumentListClass : List<string>
    {
        #region Properties
        public List<string> Paths(TaskContextClass ctx)
        {
            // Assume none
            List<string> c_Ans = new List<string>();

            // Loop thru
            foreach (string sName in this)
            {
                // Get the object
                NX.Engine.Files.DocumentClass c_Doc = ctx.Documents[sName];
                // Any?
                if (c_Doc != null)
                {
                    // Add
                    c_Ans.Add(c_Doc.Path);
                }
            }

            return c_Ans;

        }

        public List<string> Names(TaskContextClass ctx)
        {
            // Assume none
            List<string> c_Ans = new List<string>();

            // Loop thru
            foreach (string sName in this)
            {
                // Get the object
                NX.Engine.Files.DocumentClass c_Doc = ctx.Documents[sName];
                // Any?
                if (c_Doc != null)
                {
                    // Add
                    c_Ans.Add(c_Doc.Name);
                }
            }

            return c_Ans;

        }
        #endregion
    }
}
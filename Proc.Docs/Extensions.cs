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

using System.Collections.Generic;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.AO;
using Proc.Docs.Files;

namespace Proc.Docs
{
    public static class Extensions
    {
        #region Statics
        public static List<string> ExpandEntries(this List<string> paths, NX.Engine.Files.ManagerClass  mgr)
        {
            // Assume none
            List<string> c_Ans = new List<string>();

            // Loop thru
            foreach(string sPath in paths)
            {
                // Folder?
                if(sPath.EndsWith("/"))
                {
                    using(FolderClass c_Folder = new FolderClass(mgr, sPath))
                    {
                        c_Ans.AddRange(c_Folder.Tree(false, true).ToList());
                    }
                }
                else
                {
                    c_Ans.Add(sPath);
                }
            }

            return c_Ans;

        }

        public static void SortFI(this List<FieldInfoClass> fields)
        {
            fields.Sort(delegate (FieldInfoClass x, FieldInfoClass y)
            {
                return x.Name.CompareTo(y.Name);
            });
        }
        #endregion
    }
}
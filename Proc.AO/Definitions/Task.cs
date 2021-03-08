//--------------------------------------------------------------------------------
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

using NX.Shared;

namespace Proc.AO.Definitions
{
    public class TaskClass : ElsaClass
    {
        #region Constants
        public const string Prefix = "tsk_";
        #endregion

        #region Constructor
        internal TaskClass(Proc.AO.DatasetClass ds, string name)
            : base(ds[Prefix + name], name, "task")
        { }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Saves the definition
        /// 
        /// </summary>
        public void Save()
        {
            // Only if a real one
            AO.ObjectClass c_Obj = this.Object;
            if (c_Obj != null)
            {
                // Save
                c_Obj.Save(force: true);

                // Signal
                c_Obj.Parent.Parent.Parent.SignalChange(this);

                c_Obj.Parent.Parent.RemoveFromCache(this.OriginalName);
            }
        }

        /// <summary>
        /// 
        /// Copies to a different view
        /// 
        /// </summary>
        /// <param name="view"></param>
        //public void CopyTo(ViewClass view)
        //{
        //    // Copy document
        //    this.Document.CopyTo(view.Document, null, "_");
        //}

        /// <summary>
        /// 
        /// Delete the view
        /// 
        /// </summary>
        public void Delete()
        {
            // Only if a real one
            AO.ObjectClass c_Obj = this.Object;
            if (c_Obj != null)
            {
                //
                c_Obj.Delete();

                // Signal
                c_Obj.Parent.Parent.Parent.SignalChange(this, true);

                // Get the dataset name
                string sDSName = c_Obj.Parent.Name.AsDatasetName();
                // Get the view name
                string sViewName = this.Name.AsViewName();

                c_Obj.Parent.RemoveFromCache(this.Name.AsViewName());

                // Test
                if (sViewName.IsSameValue("default") || sViewName.StartsWith("_") || sDSName.StartsWith("_"))
                {
                    // Assure exixtance
                    BuiltIn.DefaultClass.Define(c_Obj.Parent);
                }
            }
        }
        #endregion
    }
}
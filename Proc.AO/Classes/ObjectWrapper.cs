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

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public class ObjectWrapperClass : ChildOfClass<ObjectClass>
    {
        #region Constructor
        public ObjectWrapperClass(ObjectClass obj)
            : base(obj)
        { }
        #endregion

        #region Indexer
        public string this[string field]
        {
            get { return this.Parent[field]; }
            set { this.Parent[field] = value; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Wrappers to underlying object
        /// 
        /// </summary>
        public bool IsValid { get { return this.Parent.IsValid; } }
        public bool IsNew { get { return this.Parent.IsNew; } }
        public UUIDClass UUID { get { return this.Parent.UUID; } }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Saves the object
        /// 
        /// </summary>
        public void Save()
        {
            this.Parent.Save();
        }

        /// <summary>
        /// 
        /// Deletes the object
        /// 
        /// </summary>
        public void Delete()
        {
            this.Parent.Delete();
        }
        #endregion
    }
}
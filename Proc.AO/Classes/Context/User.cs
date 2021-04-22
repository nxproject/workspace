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

namespace Proc.AO
{
    public class ExtendedUserClass : StoreClass
    {
        #region Constants
        public const string UserFolder = "_user";
        #endregion

        #region Constructor
        public ExtendedUserClass(AO.ObjectClass values, SiteInfoClass si)
            : base(values.AsJObject)
        {
            //
            this.SiteInfo = si;

            //
            NX.Engine.Files.ManagerClass c_Mgr = si.Parent.Parent.Parent.Globals.Get<NX.Engine.Files.ManagerClass>();
            this.Folder = new NX.Engine.Files.FolderClass(c_Mgr, UserFolder.CombinePath(this.Name));
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The user name
        /// 
        /// </summary>
        public string Name
        {
            get { return this["_id"]; }
        }

        /// <summary>
        /// 
        /// The user password
        /// 
        /// </summary>
        public string Password
        {
            get { return this["pwd"]; }
            set { this["pwd"] = value; }
        }

        /// <summary>
        /// 
        /// The allowed dataset string
        /// 
        /// </summary>
        public string Allowed
        {
            get { return this["allowed"]; }
            set { this["allowed"] = value; }
        }

        /// <summary>
        /// 
        /// The options supported string
        /// 
        /// </summary>
        public string Options
        {
            get { return this["options"]; }
            set { this["options"] = value; }
        }

        /// <summary>
        /// 
        /// The mode to open root windows
        /// 
        /// </summary>
        public string OpenMode
        {
            get { return this["openmode"]; }
            set { this["openmode"] = value; }
        }

        /// <summary>
        /// 
        /// The mode to open child windows
        /// 
        /// </summary>
        public string OpenModeChild
        {
            get { return this["openmodechild"]; }
            set { this["openmodechild"] = value; }
        }

        /// <summary>
        /// 
        /// The default field width
        /// 
        /// </summary>
        public string DefaultFieldWidth
        {
            get { return this["defaultfieldWidth"]; }
            set { this["defaultfieldWidth"] = value; }
        }

        /// <summary>
        /// 
        /// The default puck width
        /// 
        /// </summary>
        public string DefaultPickWidth
        {
            get { return this["defaultpickWidth"]; }
            set { this["defaultpickWidth"] = value; }
        }

        /// <summary>
        /// 
        /// The default pick height
        /// 
        /// </summary>
        public string DefaultPickHeight
        {
            get { return this["defaultpickHeight"]; }
            set { this["defaultpickHeight"] = value; }
        }

        /// <summary>
        /// 
        /// The user displayable name
        /// 
        /// </summary>
        public string Displayable
        {
            get { return this["dispname"]; }
            set { this["dispname"] = value; }
        }

        /// <summary>
        /// 
        /// The user title
        /// 
        /// </summary>
        public string Title
        {
            get { return this["title"]; }
            set { this["title"] = value; }
        }

        /// <summary>
        /// 
        /// The user email name
        /// 
        /// </summary>
        public string EMailName
        {
            get { return this["emailname"]; }
            set { this["emailname"] = value; }
        }

        /// <summary>
        /// 
        /// The user email password
        /// 
        /// </summary>
        public string EMailPassword
        {
            get { return this["emailpwd"]; }
            set { this["emailpwd"] = value; }
        }

        /// <summary>
        /// 
        /// The user email provider
        /// 
        /// </summary>
        public string EMailProvider
        {
            get { return this["emailprov"]; }
            set { this["emailprov"] = value; }
        }

        /// <summary>
        /// 
        /// The user phone number
        /// 
        /// </summary>
        public string Phone
        {
            get { return this["phone"]; }
            set { this["phone"] = value; }
        }

        /// <summary>
        /// 
        /// The user twilio phone number
        /// 
        /// </summary>
        public string TwilioPhone
        {
            get { return this["twiliophone"].IfEmpty(this.SiteInfo.TwilioPhone); }
            set { this["twiliophone"] = value; }
        }

        /// <summary>
        /// 
        /// The footer for comm messages
        /// 
        /// </summary>
        public string CommFooter
        {
            get { return this["commfooter"]; }
            set { this["commfooter"] = value; }
        }

        /// <summary>
        /// 
        /// The signature
        /// 
        /// </summary>
        public string Signature
        {
            get { return this["signature"]; }
            set { this["signature"] = value; }
        }
        #endregion

        #region Links
        private SiteInfoClass SiteInfo { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return base.SynchObject.ToSimpleString();
        }
        #endregion

        #region Files
        /// <summary>
        /// 
        /// The user folder
        /// 
        /// </summary>
        public NX.Engine.Files.FolderClass Folder { get; private set; }

        /// <summary>
        /// 
        /// Returns a file in the user folder
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public NX.Engine.Files.DocumentClass File(string name)
        {
            return new NX.Engine.Files.DocumentClass(this.Folder.Parent, this.Folder, name);
        }
        #endregion
    }
}
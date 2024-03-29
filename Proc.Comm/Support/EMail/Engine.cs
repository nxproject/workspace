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
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using NX.Engine.Files;

namespace Proc.Communication.EMailIF
{
    public class EngineClass : IDisposable
    {
        #region Constants
        #endregion

        #region Constructor
        public EngineClass(string friendly, string name, string pwd, string prov)
        {
            //
            this.Friendly = friendly;
            this.Name = name;
            this.Pwd = pwd;
            this.Provider = prov;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public string Friendly { get; internal set; }
        public string Name { get; internal set; }
        public string Pwd { get; internal set; }
        public string Provider { get; internal set; }
        #endregion

        #region Methods
        public string SendHTML(AO.ExtendedContextClass ctx,
                                string to,
                                string subj,
                                string msg)
        {
            //
            using (ClientClass c_Client = new ClientClass(this.Friendly,
                                                                this.Name,
                                                                this.Pwd,
                                                                ClientClass.ProviderFromString(this.Provider)))
            {
                return c_Client.Send(to,
                                subj,
                                msg
                                );
            }
        }
        #endregion
    }
}
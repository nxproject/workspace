///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
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

using System;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.LibreOffice
{
    public class ManagerClass : BumbleBeeClass
    {
        #region Constructor
        public ManagerClass(EnvironmentClass env)
            : base(env, "libreoffice")
        {
            // Handle NginX
            this.SetNginxInformation("libreoffice", false);

            //
            this.AvailabilityChanged += delegate (bool isavailable)
            {
                // Get the NginX manager
                Proc.NginX.ManagerClass c_Mgr = this.Parent.Globals.Get<Proc.NginX.ManagerClass>();

                // Recycle
                c_Mgr.MakeConfig(true, true);
            };

            this.CheckForAvailability();
        }
        #endregion
    }
}
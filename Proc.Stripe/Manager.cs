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

using Stripe;

using NX.Shared;
using NX.Engine;
using Proc.AO;

namespace Proc.Stripe
{
    public class ManagerClass : ChildOfClass<EnvironmentClass>
    {
        #region Constructor
        public ManagerClass(EnvironmentClass env)
            : base(env)
        {
            //
            AO.ManagerClass c_Mgr = this.Parent.Globals.Get<AO.ManagerClass>();
            //
            this.SiteInfo = c_Mgr.DefaultDatabase.SiteInfo;

            //
            if (this.SiteInfo.StripeSecurity.HasValue())
            {
                // Make
                this.Client = new StripeClient(this.SiteInfo.StripeSecurity);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// Holds the settings
        /// 
        /// </summary>
        private AO.SiteInfoClass SiteInfo { get; set; }

        /// <summary>
        /// 
        /// The Stripe IF
        /// 
        /// </summary>
        public StripeClient Client { get; set; }
        #endregion

        #region Methods
        #endregion
    }
}
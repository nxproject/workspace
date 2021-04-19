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
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Gets the theme options
    /// 
    /// </summary>
    public class GetTheme : RouteClass
    {
        public override List<string> RouteTree => new List<string>() { RouteClass.GET(), "gettheme" };
        public override void Call(HTTPCallClass call, StoreClass store)
        {
            // The database manager
            AO.ManagerClass c_DBMgr = call.Env.Globals.Get<AO.ManagerClass>();

            // Is there a theme file?
            string sTheme = "{" + c_DBMgr.DefaultDatabase.SiteInfo.ThemeOptions.IfEmpty().SplitCRLF().Join(",") + "}";
            // Try to parse
            JObject c_Theme = sTheme.ToJObject();
            // If none, make one
            if (c_Theme == null) c_Theme = new JObject();

            // And return
            call.RespondWithJSON(c_Theme);

        }
    }
}
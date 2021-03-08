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

using NX.Engine;
using NX.Shared;

namespace Fn.ADEX
{
    /// <summary>
    /// 
    /// Calls a site NX,NodeC# Server and sends/receives data 
    /// 
    /// Uses from passed store:
    /// 
    /// url         - The URl of the target site (Example: https://otherparty.com)
    /// value       - The JSON object to be sent
    /// 
    /// Returns in return store:
    /// 
    /// #json#      - The JSON object that was returned by the other site
    /// 
    /// </summary>
    public class Send : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass values)
        {
            // Get the URL 
            string sURL = values["url"];
            // Is there one?
            if (!sURL.HasValue())
            {
                // If not return without error
                return null;
            }
            else
            {
                // Make the call and return the store
                return sURL.URLNX(values["value"].ToJObject(), Route.ADEX.Constants.Route).ToStore();
            }
        }
    }
}
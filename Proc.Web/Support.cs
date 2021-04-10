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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Web
{
    public static class SupportClass
    {
        #region Statics
        /// <summary>
        /// 
        /// The URL that can reach this site
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string PublicURL(this string path) { return "{{publicurl}}" + path; }

        /// <summary>
        /// 
        /// Shortens the URL
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Bitly(this string url, EnvironmentClass env)
        {
           // Convert to relative
            url = url.ToPublicURL(env);
            // One of ours?
            if (url.StartsWith("".PublicURL()))
            {
                // Make the ID
                string sID = "".GUID();
                // Get the path
                JObject c_Value = "value".AsJObject(url.Substring("".PublicURL().Length));
                // And the ID
                c_Value.Set("_id", sID);
                // Set the expiration date
                c_Value.Set("exp", DateTime.Now.AddDays(7).ToDBDate());
                // Get the manager
                AO.ManagerClass c_Mgr = env.Globals.Get<AO.ManagerClass>();
                // Save
                c_Mgr.DefaultDatabase[AO.DatabaseClass.DatasetBitly].DataCollection.AddDirect(c_Value.ToSimpleString());
                // Make new
                url = "/b/{0}".FormatString(sID).PublicURL().FromPublicURL(env);
            }

            return url;
        }

        /// <summary>
        /// 
        /// Converts from absolute to relative
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static string ToPublicURL(this string url, EnvironmentClass env)
        {
            if (url.StartsWith(env.ReachableURL))
            {
                url = url.Substring(env.ReachableURL.Length).PublicURL();
            }
            return url;
        }

        /// <summary>
        /// 
        /// Converts from relative to absolute
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static string FromPublicURL(this string url, EnvironmentClass env)
        {
            //
            if (url.StartsWith("".PublicURL()))
            {
                url = env.ReachableURL + url.Substring("".PublicURL().Length);
            }
            return url;
        }
        #endregion
    }
}
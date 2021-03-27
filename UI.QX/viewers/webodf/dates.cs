///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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
/// Install-Package Newtonsoft.Json -Version 10.0.2
/// Install-Package TimeZoneConverter -Version 3.2.0
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading;

using Newtonsoft.Json.Linq;
using TimeZoneConverter;

using NX.Shared;

namespace NX.Engine
{
    /// <summary>
    /// 
    /// This is my toolkit.  It makes my life easier and I
    /// belive the code more readable.  It has been with me
    /// over many years, languages and OSs.
    /// 
    /// I will document as best as possible but in most
    /// cases the function name tells it all
    /// 
    /// </summary>
    public static class ExtensionsClass
    {
        #region Constants
        public const string FieldDelimiter = ".";
        #endregion

        #region To make coding easier
        public static StoreClass FN(this HTTPCallClass call, string name, StoreClass values = null)
        {
            StoreClass c_Ans = null;

            // Assure values
            if (values == null) values = new StoreClass();
            // Get the function 
            FNClass c_CN = call.Env.FNS.GetFN(name);
            // Only call if we found a function
            if (c_CN != null) c_Ans = c_CN.Do(call, values);

            return c_Ans;
        }

        public static StoreClass FN(this HTTPCallClass call, string name, string[] values)
        {
            StoreClass c_Params = new StoreClass();
            c_Params.Parse(values);

            return call.FN(name, c_Params);
        }

        public static bool FNExists(this HTTPCallClass call, string name)
        {
            // Get the function 
            FNClass c_CN = call.Env.FNS.GetFN(name);
            //
            return c_CN != null;
        }

        public static void ROUTE(this HTTPCallClass call, StoreClass values, params string[] param)
        {
            if (values == null) values = new StoreClass();

            RouteClass c_Route = call.Env.Router.Get(values, param[0], new List<string>(param.SubArray(1, param.Length - 1)));
            if (c_Route != null)
            {
                try
                {
                    c_Route.Call(call, values);
                }
                catch { }
            }
        }
        #endregion

        #region Splitters
        /// <summary>
        /// 
        /// Split by a space as a delimiter.  
        /// Allows for the following characters to 
        /// override the space as a delimiter:
        /// 
        /// 'xxx xxx'
        /// "xxx xxx"
        /// [xxx xxx]
        /// </summary>
        /// <param name="value">The string to parse</param>
        /// <returns>A list of parsed strings</returns>
        public static List<string> SplitSpaces(this string value)
        {
            return value.SplitSpaces(false);
        }

        /// <summary>
        /// 
        /// See above
        /// 
        /// </summary>
        /// <param name="value">The string to parse</param>
        /// <param name="removedelim">If true the delimiters ("'[]) are removed from the parsed words</param>
        /// <returns>A list of parsed strings</returns>
        public static List<string> SplitSpaces(this string value, bool removedelim)
        {
            List<string> c_Ans = new List<string>();

            MatchCollection c_Matches = Regex.Matches(value.IfEmpty().Trim(), @"[^\s\x23\x5B\x22\x27\x5D]+|\x23([^\x23]*)\x23|\x5B([^\x5D\x5B]*)\x5D|\x22([^\x22]*)\x22|\x27([^\x27]*)\x27");
            foreach (Match c_Match in c_Matches)
            {
                string sValue = c_Match.Value.Trim();

                if (removedelim)
                {
                    if (sValue.StartsWith("'") && sValue.EndsWith("'"))
                    {
                        sValue = sValue.Substring(1, sValue.Length - 2);
                    }
                    else if (sValue.StartsWith("\"") && sValue.EndsWith("\""))
                    {
                        sValue = sValue.Substring(1, sValue.Length - 2);
                    }
                }

                if (sValue.HasValue()) c_Ans.Add(sValue);
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Splits a string into words
        /// 
        /// </summary>
        /// <param name="value">The string to parse</param>
        /// <param name="quotedelim">True if single quote delimiters are allowed</param>
        /// <returns>A list of prsed words</returns>
        public static List<string> SplitWords(this string value, bool quotedelim)
        {
            List<string> c_Ans = new List<string>();

            if (value.HasValue())
            {
                Regex c_Reg = new Regex(@"(?<Delim>[^\w" + (quotedelim ? "'" : string.Empty) + "])",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Singleline
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );

                MatchCollection c_Coll = c_Reg.Matches(value);
                foreach (Match c_Match in c_Coll)
                {
                    c_Ans.Add(c_Match.Value);
                }
            }

            return c_Ans;
        }
        #endregion

        #region Formatters
        /// <summary>
        /// 
        /// A string formatter.
        /// 
        /// In today's term, it is basically:
        /// 
        /// string.format(cmd, values...)
        /// 
        /// But things have not always been so easy.
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string FormatString(this string cmd, params object[] values)
        {
            try
            {
                // Do we format?clean
                if (values.Length > 0)
                {
                    //
                    cmd = string.Format(cmd, values);
                }
            }
            catch { }

            return cmd;
        }

        /// <summary>
        /// 
        /// Capitalizes words in a string
        /// 
        /// </summary>
        /// <param name="value">The string to be capitalized</param>
        /// <returns>The capitalized string</returns>
        public static string SmartCaps(this string value)
        {
            string sAns = string.Empty;

            if (value.HasValue())
            {
                Regex c_Reg = new Regex(@"(?<Delim>[^\w])",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Singleline
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );

                Match c_M = c_Reg.Match(value);
                if (c_M.Success)
                {
                    string sDel = "";
                    string[] saWkg = value.Split(c_M.Value[0]);

                    for (int iLoop = saWkg.GetLowerBound(0); iLoop <= saWkg.GetUpperBound(0); iLoop++)
                    {
                        sAns += sDel + saWkg[iLoop].SmartCaps();
                        sDel = c_M.Value;
                    }
                }
                else
                {
                    sAns = value.Trim();
                    if (sAns.Length > 0)
                    {
                        sAns = (sAns[0] + "").ToUpper() + sAns.Substring(1);
                        if (sAns.StartsWith("Mac") && (sAns.Length > 2))
                        {
                            sAns = sAns.Substring(0, 3) + (sAns[3] + "").ToUpper() + sAns.Substring(4);
                        }
                        else if (sAns.StartsWith("Mc") && (sAns.Length > 2))
                        {
                            sAns = sAns.Substring(0, 2) + (sAns[2] + "").ToUpper() + sAns.Substring(3);
                        }
                    }
                }

                sAns = (sAns + " ").Replace("'S ", "'s ").TrimEnd();
            }

            return sAns;
        }
        #endregion

        #region Conversion
        public static int ToInteger(this object value)
        {
            return value.ToInteger(0);
        }

        public static int ToInteger(this object value, int defaultvalue)
        {
            int xAns = defaultvalue;

            try
            {
                if (value != null)
                {
                    if (!System.Int32.TryParse(value.ToString(), out xAns)) xAns = defaultvalue;
                }
            }
            catch { }

            return xAns;
        }

        public static long ToLong(this object value)
        {
            return value.ToLong(0);
        }

        public static long ToLong(this object value, long defaultvalue)
        {
            long xAns = defaultvalue;

            try
            {
                if (!System.Int64.TryParse(value.ToString(), out xAns)) xAns = defaultvalue;
            }
            catch { }

            return xAns;
        }

        public static double ToDouble(this object value)
        {
            return value.ToDouble(0);
        }

        public static double ToDouble(this object value, double defaultvalue)
        {
            double xAns = defaultvalue;

            try
            {
                if (!System.Double.TryParse(value.ToString(), out xAns)) xAns = defaultvalue;
            }
            catch { }

            return xAns;
        }

        public static float ToFloat(this object value)
        {
            return value.ToFloat(0);
        }

        public static float ToFloat(this object value, float defaultvalue)
        {
            float xAns = defaultvalue;

            try
            {
                if (!float.TryParse(value.ToString(), out xAns)) xAns = defaultvalue;
            }
            catch { }

            return xAns;
        }

        public static bool ToBoolean(this object value)
        {
            return value.ToBoolean(false);
        }

        public static bool ToBoolean(this object value, bool defaultvalue)
        {
            bool xAns = defaultvalue;

            try
            {
                if (value is bool)
                {
                    xAns = (bool)value;
                }
                else
                {
                    string sWkg = value.ToString().ToLower();
                    if (sWkg.IndexOf("t") != -1 || sWkg.IndexOf("y") != -1)
                    {
                        xAns = true;
                    }
                    else if (sWkg.IndexOf("f") != -1 || sWkg.IndexOf("n") != -1)
                    {
                        xAns = false;
                    }
                    else
                    {
                        xAns = Convert.ToBoolean(sWkg);
                    }
                }
            }
            catch
            {
                xAns = value.ToDouble() != 0;
            }

            return xAns;
        }

        public static string ToString(this bool value)
        {
            return (value ? "true" : "false");
        }

        public static string ToDBDate(this DateTime value)
        {
            string sAns = "";
            if (!value.Equals(DateTime.MinValue))
            {
                sAns = value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
            return sAns;
        }

        public static string ToDBDate(this DateTime? value)
        {
            string sAns = "";
            if (value != null)
            {
                sAns = ((DateTime)value).ToDBDate();
            }
            return sAns;
        }

        public static DateTime FromDBDate(this string value)
        {
            return value.FromDBDate(ExtensionsClass.Now());
        }

        public static DateTime FromDBDate(this string value, DateTime defvalue)
        {
            DateTime c_Ans = defvalue;

            try
            {
                c_Ans = DateTime.Parse(value);
            }
            catch { }

            return c_Ans;
        }

        public static byte[] ToBytes(this string value)
        {
            byte[] abAns = new byte[0];
            if (value.HasValue()) abAns = System.Text.UTF8Encoding.UTF8.GetBytes(value.IfEmpty());
            return abAns;
        }

        public static string FromBytes(this byte[] value)
        {
            string sAns = "";
            if (value != null) sAns = System.Text.UTF8Encoding.UTF8.GetString(value);
            return sAns;
        }

        public static string FromBytes(this byte[] value, int start, int length)
        {
            return System.Text.ASCIIEncoding.UTF8.GetString(value, start, length);
        }

        public static byte[] ToASCIIBytes(this string value)
        {
            return System.Text.UTF8Encoding.ASCII.GetBytes(value.IfEmpty());
        }

        public static string FromASCIIBytes(this byte[] value)
        {
            return System.Text.ASCIIEncoding.ASCII.GetString(value);
        }

        public static string FromASCIIBytes(this byte[] value, int start, int length)
        {
            return System.Text.ASCIIEncoding.ASCII.GetString(value, start, length);
        }
        #endregion

        #region URL
        private const char URLSeparator = '/';

        public static string CombineURL(this string url, string value)
        {
            if (url.EndsWith(URLSeparator) && value.StartsWith(URLSeparator))
            {
                url = url + value.Substring(1);
            }
            else if (url.EndsWith(URLSeparator) || value.StartsWith(URLSeparator))
            {
                url += value;
            }
            else
            {
                url += URLSeparator + value;
            }
            return url;
        }

        public static string EndURL(this string url)
        {
            return url + (!url.EndsWith(URLSeparator) ? URLSeparator + "" : "");
        }

        public static string URLEncode(this string value)
        {
            return System.Uri.EscapeDataString(value.IfEmpty());
        }

        public static string URLDecode(this string value)
        {
            return System.Uri.UnescapeDataString(value.IfEmpty());
        }

        public static string HTMLEncode(this string value)
        {
            return System.Net.WebUtility.HtmlEncode(value.IfEmpty());
        }

        public static string HTMLDecode(this string value)
        {
            return System.Net.WebUtility.HtmlDecode(value.IfEmpty());
        }

        public static void TlsSet(this string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        public static byte[] URLGet(this string url, params string[] headers)
        {
            byte[] abAns = null;

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    for (int i = 0; i < headers.Length; i += 2)
                    {
                        c_Client.Headers.Add(headers[i], headers[i + 1]);
                    }

                    abAns = c_Client.DownloadData(url);
                }
            }
            catch { }

            if (abAns == null) abAns = new byte[0];

            return abAns;
        }

        public static byte[] URLPost(this string url, byte[] value, params string[] headers)
        {
            byte[] abAns = null;

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    for (int i = 0; i < headers.Length; i += 2)
                    {
                        c_Client.Headers.Add(headers[i], headers[i + 1]);
                    }

                    abAns = c_Client.UploadData(url, value);
                }
            }
            catch { }

            if (abAns == null) abAns = new byte[0];

            return abAns;
        }

        public static byte[] URLPost(this string url, string value, params string[] headers)
        {
            return url.URLPost(value.ToBytes(), headers);
        }

        public static byte[] URLPost(this string url, StoreClass value, params string[] headers)
        {
            return url.URLPost(value.SynchObject, headers);
        }

        public static byte[] URLPost(this string url, JObject value, params string[] headers)
        {
            return url.URLPost(value.ToSimpleString().ToBytes(), headers);
        }

        public static byte[] URLPost(this string url, JArray value, params string[] headers)
        {
            return url.URLPost(value.ToSimpleString().ToBytes(), headers);
        }

        public static bool URLGetFile(this string url, string path)
        {
            bool bAns = false;

            try
            {
                url.TlsSet();

                using (System.Net.WebClient c_Client = new System.Net.WebClient())
                {
                    System.IO.Path.GetDirectoryName(path).AssurePath();
                    if (System.IO.File.Exists(path))
                    {
                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch { }
                    }

                    c_Client.DownloadFile(url, path);
                    bAns = true;
                }
            }
            catch { }

            return bAns;
        }
        #endregion

        #region Object
        private const string BaseKey = "_base";

        public static List<string> Keys(this JObject obj)
        {
            return obj.Properties().Select(p => p.Name).ToList();
        }

        public static JObject Minimize(this JObject obj)
        {
            JObject c_Min = new JObject();
            foreach (string sKey in obj.Keys())
            {
                if (obj.GetJObject(sKey) != null)
                {
                    c_Min.Set(sKey, obj.GetJObject(sKey).Minimize());
                }
                else if (obj.GetJArray(sKey) != null)
                {
                    c_Min.Set(sKey, obj.GetJArray(sKey));
                }
                else if (obj.Get(sKey).HasValue())
                {
                    c_Min.Set(sKey, obj.Get(sKey));
                }
            }

            return c_Min;
        }

        public static List<string> AsList(this JObject obj)
        {
            return obj.Properties().Select(p => p.Value.ToStringSafe()).ToList();
        }

        public static string ToSimpleString(this JObject obj)
        {
            return obj.ToString(Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonConverter[0]);
        }

        public static string ToTFString(this JObject obj)
        {
            return obj.ToSimpleString().Replace(":\"true\"", ":true").Replace(":\"false\"", ":false");
        }

        public static string ApplyTo(this JObject value, string patt)
        {
            foreach (string sKey in value.Keys())
            {
                patt = patt.Replace("{" + sKey + "}", value.Get(sKey));
            }

            return patt;
        }

        public static JObject Format(this JObject value, JObject values)
        {
            JObject c_Ans = value.CloneIfNull();

            if (values != null)
            {
                foreach (string sKey in c_Ans.Keys())
                {
                    try
                    {
                        string sTargetKey = values.ApplyTo(sKey);

                        JObject c_WkgO = c_Ans.GetJObject(sKey);
                        if (c_WkgO != null)
                        {
                            c_Ans[sTargetKey] = c_WkgO.Format(values);
                        }
                        else
                        {
                            JArray c_WkgA = c_Ans.GetJArray(sKey);
                            if (c_WkgA != null)
                            {
                                c_Ans[sTargetKey] = c_WkgA.Format(values);
                            }
                            else
                            {
                                c_Ans[sTargetKey] = values.ApplyTo(value.Get(sKey));
                            }
                        }

                        if (!sTargetKey.IsSameValue(sKey))
                        {
                            c_Ans.Remove(sKey);
                        }
                    }
                    catch { }
                }
            }

            return c_Ans;
        }

        public static JObject Combine(this JObject value, JObject other)
        {
            value = value.CloneIfNull();
            other = other.CloneIfNull();

            foreach (string sKey in other.Keys())
            {
                value[sKey] = other[sKey];
            }

            return value;
        }

        //public static JArray SmartMerge(this JArray value, JArray other)
        //{
        //    value = value.CloneIfNull();
        //    other = other.CloneIfNull();

        //    switch(sett)
        //    {
        //        case MergeArrayHandling.Concat:
        //            value.AddElements(other);
        //            break;

        //        case MergeArrayHandling.Merge:
        //            break;

        //        case MergeArrayHandling.Replace:
        //            value.Clear();
        //            value.AddElements(other);
        //            break;

        //        case MergeArrayHandling.Union:
        //            for(int i =0; i < other.Count;i++)
        //            {
        //                string sValue = other.Get(i);
        //                if (value.IndexOf(sValue) == -1) value.Add(sValue);
        //            }
        //            break;
        //    }

        //    return value;
        //}

        public static JObject SmartMerge(this JObject value, JObject other, bool clone = true)
        {
            if (clone) value = value.CloneIfNull();

            List<string> c_VKeys = value.Keys();

            foreach (string sKey in other.Keys())
            {
                if (c_VKeys.IndexOf(sKey) == -1)
                {
                    value[sKey] = other[sKey];
                }
                else
                {
                    JArray c_ArrO = other.GetJArray(sKey);
                    if (c_ArrO != null)
                    {
                        JArray c_Curr = value.AssureJArray(sKey);
                        foreach (object c_New in c_ArrO)
                        {
                            if (c_New.ToStringSafe().IsSameValue("!"))
                            {
                                c_Curr.Clear();
                            }
                            else
                            {
                                c_Curr.Add(c_New);
                            }
                        }
                    }
                    else
                    {
                        JObject c_ObjO = other.GetJObject(sKey);
                        if (c_ObjO != null)
                        {
                            value.AssureJObject(sKey).SmartMerge(c_ObjO, false);
                        }
                        else
                        {
                            value[sKey] = other[sKey];
                        }
                    }
                }
            }

            return value;
        }


        public static JObject CloneIfNull(this JObject value)
        {
            if (value == null)
            {
                value = new JObject();
            }
            else
            {
                value = value.Clone();
            }

            return value;
        }

        public static JObject Clone(this JObject value)
        {
            if (value != null)
                value = value.ToString().ToJObject();

            return value;
        }

        public static JObject ToJObject(this string value)
        {
            JObject c_Ans = null;

            if (value.HasValue())
            {
                try
                {
                    c_Ans = JObject.Parse(value);
                }
                catch (Exception e)
                {
                    string sMsg = e.GetAllExceptions();
                }
            }

            if (c_Ans == null)
                c_Ans = new JObject();

            return c_Ans;
        }

        public static JObject ToJObjectIf(this string value)
        {
            JObject c_Ans = null;

            if (value.HasValue())
            {
                try
                {
                    c_Ans = JObject.Parse(value);
                }
                catch (Exception e)
                {
                    string sMsg = e.GetAllExceptions();
                }
            }

            return c_Ans;
        }

        public static Dictionary<string, string> ToDictionary(this JObject value)
        {
            Dictionary<string, string> c_Ans = new Dictionary<string, string>();

            foreach (string sKey in value.Keys())
            {
                c_Ans.Add(sKey, value.Get(sKey));
            }

            return c_Ans;
        }

        public static string Get(this JObject obj, string key, string delim = null)
        {
            string sAns = null;

            if (!delim.HasValue()) delim = FieldDelimiter;

            if (key.Contains(delim))
            {
                // Parse the key
                int iPos = key.IndexOf(delim);
                string sParent = key.Substring(0, iPos);
                string sField = key.Substring(iPos + 1);
                // Array?
                iPos = sParent.IndexOf(":");
                if (iPos != -1)
                {
                    int iIndex = sParent.Substring(iPos + 1).ToInteger(0);
                    sParent = sParent.Substring(0, iPos);
                    if (obj.Contains(sParent))
                    {
                        // Get
                        sAns = obj.GetJArray(sParent).GetJObject(iIndex).Get(sField, delim);
                    }
                }
                else
                {
                    // Is there an object?
                    if (obj.Contains(sParent))
                    {
                        // Get
                        sAns = obj.GetJObject(sParent).Get(sField, delim);
                    }
                }
            }
            else if (obj != null)
            {
                try
                {
                    var c_Wkg = obj.GetValue(key);
                    if (c_Wkg != null)
                    {
                        switch (c_Wkg.Type)
                        {
                            case JTokenType.Date:
                                sAns = ((DateTime)c_Wkg).ToDBDate();
                                break;

                            //case JTokenType.Boolean:
                            //    sAns = c_Wkg.ToString().IsSameValue("true") ? "true" : "false";
                            //    break;

                            case JTokenType.Array:
                                sAns = ((JArray)c_Wkg).ToSimpleString();
                                break;

                            case JTokenType.Object:
                                sAns = ((JObject)c_Wkg).ToSimpleString();
                                break;

                            default:
                                sAns = c_Wkg.ToString();
                                break;
                        }
                    }

                    //sAns = obj[key].ToString();
                }
                catch { }
            }

            return (sAns == null ? string.Empty : sAns);
        }

        public static JObject Set(this JObject obj, string key, string value)
        {
            if (key.Contains(FieldDelimiter))
            {
                // Parse the key
                int iPos = key.IndexOf(FieldDelimiter);
                string sParent = key.Substring(0, iPos);
                string sField = key.Substring(iPos + 1);
                // Set
                obj.AssureJObject(sParent).Set(sField, value);
            }
            else
            {
                obj[key] = value;
            }

            return obj;
        }

        public static JObject Set(this JObject obj, string key, object value)
        {
            obj[key] = JToken.FromObject(value);

            return obj;
        }

        public static JObject Fill(this JObject obj, params string[] values)
        {
            for (int i = 0; i < values.Length; i += 2)
            {
                obj.Set(values[i], values[i + 1]);
            }
            return obj;
        }

        public static void CopyFrom(this JObject obj, JObject source, params string[] keys)
        {
            foreach (string sKey in keys)
            {
                obj.Set(sKey, source.Get(sKey));
            }
        }

        public static void CopyFromAsObject(this JObject obj, JObject source, params string[] keys)
        {
            foreach (string sKey in keys)
            {
                obj.Set(sKey, source.Get(sKey).ToJObject());
            }
        }

        public static void CopyFromAsArray(this JObject obj, JObject source, params string[] keys)
        {
            foreach (string sKey in keys)
            {
                obj.Set(sKey, source.Get(sKey).ToJArray());
            }
        }

        public static void SetDiff(this JObject obj)
        {
            if (!obj.Contains(BaseKey) && obj.Contains("_id"))
            {
                obj[BaseKey] = obj.ToSimpleString();
            }
        }

        public static JObject GetDiff(this JObject obj)
        {
            JObject c_Ans = obj;
            if (obj.Contains(BaseKey))
            {
                JObject c_Old = (obj[BaseKey].ToStringSafe()).ToJObject();
                obj.Remove(BaseKey);

                c_Ans = obj.Diff(c_Old);
            }

            return c_Ans;
        }

        public static bool UsesDiff(this JObject obj)
        {
            return obj.Contains(BaseKey);
        }

        public static JObject Diff(this JObject newobj, JObject oldobj)
        {
            // Result
            JObject c_Ans = new JObject();
            // Do we have an old object
            if (oldobj == null)
            {
                c_Ans = newobj;
            }
            else
            {
                // Loop through
                foreach (string sKey in newobj.Keys())
                {
                    // Get values
                    object c_New = newobj[sKey];
                    object c_Old = oldobj[sKey];
                    // If system stuff, leave alone
                    if (sKey.StartsWith("_"))
                    {
                        c_Ans.Set(sKey, c_New);
                    }
                    else
                    {
                        // Was there any?
                        if (c_Old == null)
                        {
                            // New
                            c_Ans.Set(sKey, c_New);
                        }
                        else
                        {
                            // Is it an object?
                            if (c_New is JObject)
                            {
                                // Recurse
                                JObject c_Changes = ((JObject)c_New).Diff(c_Old as JObject);
                                // Any changes?
                                if (c_Changes.Keys().Count > 0)
                                {
                                    // Save
                                    c_Ans[sKey] = c_Changes;
                                }
                            }
                            else if (c_New is JArray)
                            {
                                // Assume has changed
                                c_Ans.Set(sKey, c_New);
                            }
                            else
                            {
                                if (!c_New.Equals(c_Old))
                                {
                                    // It is a change
                                    c_Ans.Set(sKey, c_New);
                                }
                            }
                        }
                    }
                }
            }
            // Exit
            return c_Ans;
        }

        public static JObject RemovePrefix(this JObject obj, string prefix)
        {
            JObject c_Ans = obj.CloneIfNull();

            foreach (string sKey in c_Ans.Keys())
            {
                if (sKey.StartsWith(prefix)) c_Ans.Remove(sKey);
            }

            return c_Ans;
        }

        public static string ToFormattedString(this JObject obj)
        {
            return obj.ToString(Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonConverter[0]);
        }

        public static JObject AssureJObject(this JObject obj)
        {
            if (obj == null) obj = new JObject();

            return obj;
        }

        public static void Load(this JObject obj, params string[] values)
        {
            for (int i = 0; i < values.Length; i += 2)
            {
                obj.Set(values[i], values[i + 1]);
            }
        }

        public static List<string> ToList(this JObject obj)
        {
            List<string> c_Ans = new List<string>();

            foreach (string sFld in obj.Keys())
            {
                c_Ans.Add(sFld);
                c_Ans.Add(obj.Get(sFld));
            }

            return c_Ans;
        }

        public static string[] ToArray(this JObject obj)
        {
            return obj.ToList().ToArray();
        }

        public static JObject IfJObjectEmpty(this JObject value, JObject newval)
        {
            if (value == null || value.Keys().Count == 0)
            {
                value = newval;
            }
            return value;
        }

        public static StoreClass ToStore(this string value)
        {
            return new StoreClass(value);
        }

        public static StoreClass ToStore(this byte[] value)
        {
            return new StoreClass(value.FromBytes());
        }
        #endregion

        #region Exception
        public static string GetAllExceptions(this Exception e)
        {
            string sAns = "";

            if (e != null)
            {
                sAns = e.Message;

                if (e.InnerException != null)
                {
                    string sSub = e.InnerException.GetAllExceptions();
                    if (sSub.HasValue()) sAns += " --> " + sSub;
                }
            }

            return sAns;
        }
        #endregion

        #region Array
        public static JArray ToJArrayOptional(this string value)
        {
            JArray c_Ans = null;

            if (value.HasValue())
            {
                try
                {
                    c_Ans = JArray.Parse(value);
                }
                catch { }
            }

            if (c_Ans == null)
            {
                c_Ans = new JArray();
                c_Ans.Add(value);
            }

            return c_Ans;
        }

        public static JArray ToJArray(this string value)
        {
            JArray c_Ans = null;

            if (value.HasValue())
            {
                try
                {
                    c_Ans = JArray.Parse(value);
                }
                catch { }
            }

            if (c_Ans == null)
                c_Ans = new JArray();

            return c_Ans;
        }

        public static JArray ToJArrayIf(this string value)
        {
            JArray c_Ans = null;

            if (value.HasValue())
            {
                try
                {
                    c_Ans = JArray.Parse(value);
                }
                catch { }
            }

            return c_Ans;
        }

        public static JArray ToJArray(this List<string> value)
        {
            JArray c_Ans = null;

            if (value != null)
            {
                c_Ans = new JArray();

                foreach (string sWkg in value) c_Ans.Add(sWkg);
            }

            if (c_Ans == null)
                c_Ans = new JArray();

            return c_Ans;
        }

        public static JObject ToJObject(this List<string> value)
        {
            JObject c_Ans = null;

            if (value != null)
            {
                c_Ans = new JObject();

                for (int iLoop = 0; iLoop < value.Count; iLoop += 2)
                {
                    c_Ans.Set(value[iLoop], value[iLoop + 1]);
                }
            }

            if (c_Ans == null)
                c_Ans = new JObject();

            return c_Ans;
        }

        public static JObject AsJObject(this string key, string value, params string[] extra)
        {
            JObject c_Ans = new JObject();

            c_Ans.Set(key, value);
            for (int i = 0; i < extra.Length; i += 2)
            {
                c_Ans.Set(extra[i], extra[i + 1]);
            }

            return c_Ans;
        }

        public static JObject AsJObject(this string key, JObject value, params string[] extra)
        {
            JObject c_Ans = new JObject();

            c_Ans.Set(key, value);
            for (int i = 0; i < extra.Length; i += 2)
            {
                c_Ans.Set(extra[i], extra[i + 1]);
            }

            return c_Ans;
        }

        public static JObject AsJObject(this string key, JArray value, params string[] extra)
        {
            JObject c_Ans = new JObject();

            c_Ans.Set(key, value);
            for (int i = 0; i < extra.Length; i += 2)
            {
                c_Ans.Set(extra[i], extra[i + 1]);
            }

            return c_Ans;
        }

        public static string Get(this JArray obj, int index)
        {
            string sAns = null;

            if (obj != null)
            {
                if (index >= 0 && index < obj.Count)
                {
                    try
                    {
                        object c_Wkg = obj[index];
                        if (c_Wkg is JObject)
                        {
                            sAns = ((JObject)c_Wkg).ToSimpleString();
                        }
                        else if (c_Wkg is JArray)
                        {
                            sAns = ((JArray)c_Wkg).ToSimpleString();
                        }
                        else
                        {
                            sAns = c_Wkg.ToStringSafe();
                        }
                    }
                    catch { }
                }
            }

            return (sAns == null ? string.Empty : sAns);
        }

        public static JArray ParamsToArray(params string[] value)
        {
            JArray c_Ans = new JArray();

            foreach (string sValue in value) c_Ans.Add(sValue);

            return c_Ans;
        }

        public static JArray Format(this JArray value, JObject values)
        {
            JArray c_Ans = value.CloneIfNull();

            if (values != null && value.Count > 0)
            {
                for (int iLoop = 0; iLoop < c_Ans.Count; iLoop++)
                {
                    JObject c_WkgO = c_Ans.GetJObject(iLoop);
                    if (c_WkgO != null)
                    {
                        c_Ans[iLoop] = c_WkgO.Format(values);
                    }
                    else
                    {
                        JArray c_WkgA = c_Ans.GetJArray(iLoop);
                        if (c_WkgA != null)
                        {
                            c_Ans[iLoop] = c_WkgA.Format(values);
                        }
                        else
                        {
                            c_Ans[iLoop] = values.ApplyTo(value.Get(iLoop));
                        }
                    }
                }
            }

            return c_Ans;
        }

        public static JArray CloneIfNull(this JArray values)
        {
            if (values == null)
            {
                values = new JArray();
            }
            else
            {
                values = values.Clone();
            }

            return values;
        }

        public static JArray Clone(this JArray values)
        {
            JArray c_Ans = new JArray();

            if (values != null)
            {
                foreach (object c_Value in values)
                    c_Ans.Add(c_Value);
            }

            return c_Ans;
        }

        public static JArray ToArray(this List<string> values)
        {
            JArray c_Ans = new JArray();

            if (values != null)
            {
                foreach (string sValue in values)
                    c_Ans.Add(sValue);
            }

            return c_Ans;
        }

        public static List<string> ToList(this JArray values)
        {
            List<string> c_Ans = new List<string>();

            if (values != null)
            {
                foreach (object c_Value in values)
                    c_Ans.Add(c_Value.ToStringSafe());
            }

            return c_Ans;
        }

        public static Dictionary<string, string> ToDictionary(this JArray values)
        {
            Dictionary<string, string> c_Ans = new Dictionary<string, string>();

            if (values != null)
            {
                for (int iLoop = 0; iLoop < values.Count; iLoop += 2)
                {
                    string sKey = values[iLoop].ToString();
                    string sValue = values[iLoop + 1].ToString();

                    if (c_Ans.ContainsKey(sKey))
                    {
                        c_Ans[sKey] = sValue;
                    }
                    else
                    {
                        c_Ans.Add(sKey, sValue);
                    }
                }
            }

            return c_Ans;
        }

        public static bool Has(this JArray values, string value)
        {
            bool bAns = false;
            if (values != null)
            {
                for (int iLoop = 0; iLoop < values.Count; iLoop++)
                {
                    if (value.IsSameValue(values[iLoop].ToString()))
                    {
                        bAns = true;
                        break;
                    }
                }
            }
            return bAns;
        }

        public static bool Contains(this JObject obj, string key)
        {
            bool bAns = false;

            try
            {
                bAns = obj[key] != null;
            }
            catch { }


            return bAns;
        }

        public static object GetObject(this JObject obj, string key)
        {
            object c_Ans = null;

            try
            {
                c_Ans = obj[key].Value<object>();
            }
            catch { }

            return c_Ans;
        }

        public static JObject GetJObject(this JObject obj, string key)
        {
            JObject c_Ans = null;

            try
            {
                c_Ans = obj[key].Value<JObject>();
            }
            catch { }

            return c_Ans;
        }

        public static JArray GetJArray(this JObject obj, string key)
        {
            JArray c_Ans = null;

            try
            {
                c_Ans = obj[key].Value<JArray>();
            }
            catch { }

            return c_Ans;
        }

        public static JObject GetJObject(this JArray obj, int index)
        {
            JObject c_Ans = null;

            try
            {
                c_Ans = obj[index].Value<JObject>();
            }
            catch { }

            return c_Ans;
        }

        public static T GetObject<T>(this JObject obj, string key)
        {
            T c_Ans = default(T);

            try
            {
                c_Ans = obj[key].Value<T>();
            }
            catch { }

            return c_Ans;
        }

        public static JArray GetJArray(this JArray obj, int index)
        {
            JArray c_Ans = null;

            try
            {
                c_Ans = obj[index].Value<JArray>();
            }
            catch { }

            return c_Ans;
        }

        public static JObject AssureJObject(this JObject obj, string key)
        {
            JObject c_Ans = null;

            try
            {
                c_Ans = obj[key].Value<JObject>();
            }
            catch { }

            if (c_Ans == null)
            {
                c_Ans = new JObject();

                if (obj != null) obj[key] = c_Ans;
            }

            return c_Ans;
        }

        public static JArray AssureJArray(this JObject obj, string key)
        {
            JArray c_Ans = null;

            try
            {
                c_Ans = obj[key].Value<JArray>();
            }
            catch { }

            if (c_Ans == null)
            {
                c_Ans = new JArray();
                if (obj != null) obj[key] = c_Ans;
            }

            return c_Ans;
        }

        public static JObject AssureJObject(this JArray obj, int index)
        {
            JObject c_Ans = null;

            try
            {
                c_Ans = obj[index].Value<JObject>();
            }
            catch { }

            if (c_Ans == null)
            {
                c_Ans = new JObject();
                obj[index] = c_Ans;
            }

            return c_Ans;
        }

        public static JArray AssureJArray(this JArray obj, int index)
        {
            JArray c_Ans = null;

            try
            {
                c_Ans = obj[index].Value<JArray>();
            }
            catch { }

            if (c_Ans == null)
            {
                c_Ans = new JArray();
                obj[index] = c_Ans;
            }

            return c_Ans;
        }

        public static void Set(this JArray obj, int index, string value)
        {
            if (index < 0)
            {
                obj.Insert(0, value);
            }
            else if (index > obj.Count)
            {
                obj.Add(value);
            }
            else
            {
                obj[index] = value;
            }
        }

        public static void Set(this JArray obj, int index, object value)
        {
            if (index < 0)
            {
                obj.Insert(0, JToken.FromObject(value));
            }
            else if (index > obj.Count)
            {
                obj.Add(value);
            }
            else
            {
                obj[index] = JToken.FromObject(value);
            }
        }

        public static void AddElements(this JArray obj, JArray values)
        {
            if (values != null)
            {
                for (int iLoop = 0; iLoop < values.Count; iLoop++)
                {
                    obj.Add(values.Get(iLoop));
                }
            }
        }

        public static string ToSimpleString(this JArray obj)
        {
            return obj.ToString(Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonConverter[0]);
        }

        public static void ForEach(this JArray values, Action<string> cb)
        {
            if (values != null && cb != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    cb(values.Get(i));
                }
            }
        }
        #endregion

        #region Delays
        public static void Sleep(this TimeSpan span)
        {
            System.Threading.Thread.Sleep(span);
        }

        public static int RandomValue(this int max)
        {
            Random c_Rnd = new Random();

            return c_Rnd.Next(max);
        }
        #endregion

        #region Types
        public static bool CanBeTreatedAsType(this Type CurrentType, Type TypeToCompareWith)
        {
            // Always return false if either Type is null
            if (CurrentType == null || TypeToCompareWith == null)
                return false;

            // Return the result of the assignability test
            return TypeToCompareWith.IsAssignableFrom(CurrentType);
        }
        #endregion

        #region Dates
        public static string FormattedAs(this DateTime value, string fmt)
        {
            return value.ToString(fmt);
        }
        #endregion

        #region Object Names
        public static string ObjectName(this object value)
        {
            // Get the type name
            string sAns = value.GetType().Name;

            int iPos = sAns.LastIndexOf(".");
            if (iPos != -1) sAns = sAns.Substring(iPos + 1);

            return sAns;
        }

        public static string ObjectFullName(this object value)
        {
            // Get theh assembly name
            Assembly c_Assm = value.GetType().Assembly;
            // Retrieve the module name
            string sModule = null;
            Match c_Match = Regex.Match(c_Assm.FullName, @"[^\x2E]+\x2E(?<name>[^,]+)\x2C");
            if (c_Match.Success) sModule = c_Match.Groups["name"].Value;
            sModule = sModule.IfEmpty("Other") + ".";

            return sModule + value.ObjectName();
        }
        #endregion

        #region Images
        public static System.Drawing.Image ToImage(this byte[] byteArrayIn)
        {
            System.Drawing.Image c_Ans = null;

            try
            {
                using (MemoryStream ms = new MemoryStream(byteArrayIn))
                {
                    c_Ans = System.Drawing.Image.FromStream(ms);
                }
            }
            catch { }

            return c_Ans;
        }

        public static System.Drawing.Image Crop(this System.Drawing.Image image, System.Drawing.Color fill)
        {
            return Crop(image, 0, fill);
        }

        public static System.Drawing.Image Crop(this System.Drawing.Image image, int border, System.Drawing.Color fill)
        {
            return Crop(image, border, border, border, border, fill);
        }

        public static System.Drawing.Image Crop(this System.Drawing.Image image, int left, int top, int right, int bottom, System.Drawing.Color fill)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image);

            int w = bmp.Width;
            int h = bmp.Height;

            System.Drawing.Color c_Fill = fill;

            // Find the most common color in a 10 by 10 area
            //Color c_Fill = System.Drawing.Color.White;
            //int iCount = -1;
            //Dictionary<Color, int> c_Found = new Dictionary<Color, int>();
            //for(int iRow =0;iRow < 10;iRow++)
            //{
            //    for (int iCol = 0; iCol < 10; iCol++)
            //    {
            //        System.Drawing.Color c_Pixel = bmp.GetPixel(iRow, iCol);
            //        if(c_Found.ContainsKey(c_Pixel))
            //        {
            //            c_Found[c_Pixel]++;
            //        }
            //        else
            //        {
            //            c_Found.Add(c_Pixel, 1);
            //        }
            //    }
            //}
            //foreach (Color c_Pixel in c_Found.Keys)
            //{
            //    if (c_Found[c_Pixel] > iCount)
            //    {
            //        c_Fill = c_Pixel;
            //        iCount = c_Found[c_Fill];
            //    }
            //}

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                {
                    //Color c_Clr = bmp.GetPixel(i, row);
                    //if (c_Clr.ToArgb() == 0) bmp.SetPixel(i, row, System.Drawing.Color.White);

                    if (!bmp.GetPixel(i, row).IsMonochromatic())
                    {
                        //if (bmp.GetPixel(i, row).R != 255)
                        return false;
                    }
                }
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                {
                    if (!bmp.GetPixel(col, i).IsMonochromatic())
                    {
                        //if ((bmp.GetPixel(col, i).R % 255) != 0)
                        return false;
                    }
                }
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= topmost; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= leftmost; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == leftmost) rightmost = w; // As reached left
            if (bottommost == topmost) bottommost = h; // As reached top.

            int adj = leftmost - left;
            if (adj < 0) adj = 0;
            leftmost = adj;

            adj = topmost - top;
            if (adj < 0) adj = 0;
            topmost = adj;

            adj = rightmost + right;
            if (adj > w) adj = w;
            rightmost = adj;

            adj = bottommost + bottom;
            if (adj > h) adj = h;
            bottommost = adj;

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new System.Drawing.Bitmap(croppedWidth, croppedHeight);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(target))
                {
                    System.Drawing.Imaging.ColorMap c_Bkg = new System.Drawing.Imaging.ColorMap();
                    c_Bkg.OldColor = c_Fill;
                    c_Bkg.NewColor = System.Drawing.Color.White;

                    System.Drawing.Imaging.ColorMap[] c_Map = new System.Drawing.Imaging.ColorMap[1];
                    c_Map[0] = c_Bkg;

                    System.Drawing.Imaging.ImageAttributes c_Attr = new System.Drawing.Imaging.ImageAttributes();
                    c_Attr.SetRemapTable(c_Map);

                    g.DrawImage(bmp,
                                new System.Drawing.Rectangle(0, 0, croppedWidth, croppedHeight),
                                  leftmost, topmost, croppedWidth, croppedHeight,
                                  System.Drawing.GraphicsUnit.Pixel, c_Attr);
                }
                bmp = target;
            }
            catch { }

            return bmp;
        }

        public static System.Drawing.Color BackgroundColor(this System.Drawing.Image image, int width = 10, int height = 10)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image);

            int w = bmp.Width;
            int h = bmp.Height;

            // Find the most common color in a 10 by 10 area
            System.Drawing.Color c_Fill = System.Drawing.Color.White;
            int iCount = -1;
            Dictionary<System.Drawing.Color, int> c_Found = new Dictionary<System.Drawing.Color, int>();
            for (int iRow = 0; iRow < 10; iRow++)
            {
                for (int iCol = 0; iCol < 10; iCol++)
                {
                    System.Drawing.Color c_Pixel = bmp.GetPixel(iRow, iCol);
                    if (c_Found.ContainsKey(c_Pixel))
                    {
                        c_Found[c_Pixel]++;
                    }
                    else
                    {
                        c_Found.Add(c_Pixel, 1);
                    }
                }
            }
            foreach (System.Drawing.Color c_Pixel in c_Found.Keys)
            {
                if (c_Found[c_Pixel] > iCount)
                {
                    c_Fill = c_Pixel;
                    iCount = c_Found[c_Fill];
                }
            }

            return c_Fill;
        }

        public static bool IsMonochromatic(this System.Drawing.Color pixel)
        {
            return (pixel.R == 0 && pixel.G == 0 && pixel.B == 0) ||
                (pixel.R == 255 && pixel.G == 255 && pixel.B == 255);
        }
        #endregion
    
        #region Files and Directories
        public static string AssurePath(this string path)
        {
            string[] asValues = path.Split(new char[] { System.IO.Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            string sWkg = "";
            foreach (string sSub in asValues)
            {
                sWkg += (sSub.IndexOf(":") == -1 ? System.IO.Path.PathSeparator + "" : "") + sSub;
                try
                {
                    Directory.CreateDirectory(sWkg);
                }
                catch { }
            }

            return path;
        }

        public static string CombinePath(this string path1, string path2)
        {
            path1 = path1.IfEmpty();
            path2 = path2.IfEmpty();
            if (path2.StartsWith(System.IO.Path.DirectorySeparatorChar))
            {
                while (path1.EndsWith(System.IO.Path.DirectorySeparatorChar)) path1 = path1.Substring(0, path1.Length - 1);
            }
            else
            {
                if (!path1.EndsWith(System.IO.Path.DirectorySeparatorChar)) path1 += System.IO.Path.DirectorySeparatorChar;
            }

            return path1 + path2;
        }

        public static bool IsPath(this string path)
        {
            return path.IfEmpty().StartsWith(System.IO.Path.PathSeparator);
        }

        public static void ClearPath(this string path)
        {
            // Get files
            string[] asFiles = System.IO.Directory.GetFiles(path);
            foreach (string sFile in asFiles) sFile.DeleteFile();
        }

        public static void DeletePath(this string path)
        {
            try
            {
                System.IO.Directory.Delete(path, true);
            }
            catch { }
        }

        public static bool PathIsDirectory(this string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public static bool PathIsFile(this string path)
        {
            return System.IO.File.Exists(path);
        }

        public static List<string> GetFilesInPath(this string path)
        {
            List<string> c_Ans = null;

            if (path.DirectoryExists())
            {
                c_Ans = new List<string>(System.IO.Directory.GetFiles(path));
            }
            if (c_Ans == null) c_Ans = new List<string>();

            return c_Ans;
        }

        public static List<string> GetMatchingFilesInPath(this string path, string patt)
        {
            List<string> c_Ans = null;

            if (path.DirectoryExists())
            {
                string[] asFiles = System.IO.Directory.GetFiles(path, patt);
                if (asFiles != null) c_Ans = new List<string>(asFiles);
            }
            if (c_Ans == null) c_Ans = new List<string>();

            return c_Ans;
        }

        public static List<string> GetDirectoriesInPath(this string path)
        {
            List<string> c_Ans = null;

            if (path.DirectoryExists())
            {
                c_Ans = new List<string>(System.IO.Directory.GetDirectories(path));
            }
            if (c_Ans == null) c_Ans = new List<string>();

            return c_Ans;
        }

        public static string GetFileNameFromPath(this string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public static string GetFileNameOnlyFromPath(this string path)
        {
            string sAns = path.GetFileNameFromPath().IfEmpty();

            int iPos = sAns.LastIndexOf(".");
            if (iPos != -1) sAns = sAns.Substring(0, iPos);

            return sAns;
        }

        public static string SetFileNameOnlyFromPath(this string path, string name)
        {
            if (path.GetExtensionFromPath().HasValue()) name += "." + path.GetExtensionFromPath();

            return path.GetDirectoryFromPath().IfEmpty().CombinePath(name);
        }

        public static string GetDirectoryNameFromPath(this string path)
        {
            string sPath = System.IO.Path.GetDirectoryName(path);

            return path.Substring(sPath.Length + 1);
        }

        public static string GetDirectoryFromPath(this string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }

        public static string SetExtensionFromPath(this string path, string ext)
        {
            path = path.IfEmpty();

            int iPos = path.LastIndexOf(".");
            if (iPos != -1)
            {
                path = path.Substring(0, iPos);
            }

            return path + "." + ext;
        }

        public static string GetExtensionFromPath(this string path)
        {
            string sAns = "";

            path = path.IfEmpty().GetFileNameFromPath();

            int iPos = path.LastIndexOf(".");
            if (iPos != -1)
            {
                sAns = path.Substring(iPos + 1);
            }

            return sAns;
        }

        public static ExtensionTypes GetExtensionTypeFromPath(this string path)
        {
            ExtensionTypes eAns = ExtensionTypes.Unknown;

            path = path.GetExtensionFromPath();

            switch (path.ToLower())
            {
                case "css":
                    eAns = ExtensionTypes.CSS;
                    break;

                case "odt":
                    eAns = ExtensionTypes.ODT;
                    break;

                case "html":
                case "htm":
                    eAns = ExtensionTypes.HTML;
                    break;

                case "mhtml":
                    eAns = ExtensionTypes.MHTML;
                    break;

                case "csv":
                    eAns = ExtensionTypes.CSV;
                    break;

                case "jpg":
                case "jpeg":
                    eAns = ExtensionTypes.JPEG;
                    break;

                case "gif":
                    eAns = ExtensionTypes.GIF;
                    break;

                case "tif":
                case "tiff":
                    eAns = ExtensionTypes.TIFF;
                    break;

                case "bmp":
                    eAns = ExtensionTypes.BMP;
                    break;

                case "png":
                    eAns = ExtensionTypes.PNG;
                    break;

                case "json":
                case "txt":
                case "cs":
                case "js":
                case "java":
                case "bat":
                case "cmd":
                    eAns = ExtensionTypes.Text;
                    break;

                case "pdf":
                    eAns = ExtensionTypes.PDF;
                    break;

                case "fdf":
                    eAns = ExtensionTypes.FDF;
                    break;

                case "doc":
                    eAns = ExtensionTypes.Doc;
                    break;

                case "docx":
                    eAns = ExtensionTypes.Docx;
                    break;

                case "xls":
                case "xlsx":
                    eAns = ExtensionTypes.XLS;
                    break;

                case "ppt":
                    eAns = ExtensionTypes.PPT;
                    break;

                case "wpd":
                    eAns = ExtensionTypes.WPD;
                    break;

                case "rtf":
                    eAns = ExtensionTypes.RTF;
                    break;

                case "xml":
                    eAns = ExtensionTypes.XML;
                    break;
            }

            return eAns;
        }

        public static DateTime GetLastWriteFromPath(this string path)
        {
            DateTime c_Ans = DateTime.MinValue;

            if (path.FileExists())
            {
                c_Ans = File.GetLastWriteTime(path);
            }

            return c_Ans;
        }

        public static long GetSizeFromPath(this string path)
        {
            long lAns = 0;

            if (path.FileExists())
            {
                FileInfo c_Info = new FileInfo(path);
                lAns = c_Info.Length;
            }

            return lAns;
        }

        public static void TouchLastWriteFromPath(this string path)
        {
            if (path.FileExists())
            {
                File.SetLastWriteTime(path, DateTime.Now);
            }
        }

        public static int CopyDirectoryTree(this string source, string target)
        {
            // The count
            int iAns = 0;

            // Make sure that they exists
            source.AssurePath();
            target.AssurePath();

            // Copy all of the files
            List<string> asFiles = source.GetFilesInPath();
            foreach (string sFile in asFiles)
            {
                string sTarget = target.CombinePath(sFile.GetFileNameFromPath());
                try
                {
                    sFile.CopyFile(sTarget);
                    // One more
                    iAns++;
                }
                catch { }
            }

            // And the directories
            List<string> asDirectories = source.GetDirectoriesInPath();
            foreach (string sDirectory in asDirectories)
            {
                // Get the name
                string sDirName = sDirectory.GetDirectoryNameFromPath();
                // Copy
                iAns += sDirectory.CopyDirectoryTree(target.CombinePath(sDirName));
            }

            //if(!string.IsNullOrEmpty( donefile))
            //{
            //    target.CombinePath(donefile).WriteFile("1");
            //}

            return iAns;
        }

        public static bool DirectoryExists(this string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public static void xcopy(this string path, string target)
        {
            try
            {
                path = path.Replace('\\', System.IO.Path.PathSeparator);
                target = target.Replace('\\', System.IO.Path.PathSeparator);

                // Make the target directory if it does not exist
                target.AssurePath();

                // Split the source
                int iPos = path.LastIndexOf(@System.IO.Path.PathSeparator);
                var source = path.Substring(0, iPos);
                var filemask = path.Substring(iPos + 1);

                // Copy each file into the new directory.
                foreach (string fi in source.GetMatchingFilesInPath(filemask))
                {
                    fi.CopyFile(target.CombinePath(fi.GetFileNameFromPath()));
                }

                // Copy each subdirectory using recursion.
                foreach (string diSourceSubDir in source.GetDirectoriesInPath())
                {
                    diSourceSubDir.CombinePath(filemask).xcopy(target.CombinePath(diSourceSubDir.Substring(source.Length + 1)));
                }
            }
            catch { }
        }
        public static bool FileExists(this string filename)
        {
            return System.IO.File.Exists(filename);
        }

        public static bool CopyFile(this string from, string to)
        {
            bool bAns = false;

            try
            {
                System.IO.File.Copy(from, to, true);
                bAns = true;
            }
            catch { }

            return bAns;
        }

        public static bool WriteFile(this string filename, string value)
        {
            // Assume we could not write
            bool bAns = false;

            // Make sure path is there
            filename.GetDirectoryFromPath().AssurePath();

            try
            {
                // Delete if it exists
                if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);
                // Write
                System.IO.File.WriteAllText(filename, value);
                // And flag
                bAns = true;
            }
            catch { }

            //
            return bAns;
        }

        public static bool WriteFileAsBytes(this string filename, byte[] value)
        {
            // Assume we could not write
            bool bAns = false;

            // Make sure path is there
            filename.GetDirectoryFromPath().AssurePath();

            try
            {
                // Delete if it exists
                if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);
                // Write
                System.IO.File.WriteAllBytes(filename, value);
                // And flag
                bAns = true;
            }
            catch { }

            //
            return bAns;
        }

        public static bool AppendFile(this string filename, string value)
        {
            // Assume we could not write
            bool bAns = false;

            // Make sure path is there
            filename.GetDirectoryFromPath().AssurePath();

            try
            {
                // Write
                System.IO.File.AppendAllText(filename, value);
                // And flag
                bAns = true;
            }
            catch { }

            //
            return bAns;
        }

        public static bool AppendFileAsBytes(this string filename, byte[] value)
        {
            // Assume we could not write
            bool bAns = false;

            // Make sure path is there
            filename.GetDirectoryFromPath().AssurePath();

            try
            {
                // Write
                System.IO.File.AppendAllText(filename, value.FromBytes());
                // And flag
                bAns = true;
            }
            catch { }

            //
            return bAns;
        }

        public static long FileSize(this string filename)
        {
            // Assume we could not read
            long lAns = -1;

            // Make sure path is there
            filename.GetDirectoryFromPath().AssurePath();

            try
            {
                // Read
                FileInfo c_Info = new FileInfo(filename);
                lAns = c_Info.Length;
            }
            catch { }

            //
            return lAns;
        }

        public static string ReadFile(this string filename)
        {
            // Assume we could not read
            string sAns = null;

            try
            {
                if (filename.PathIsFile())
                {
                    // Read
                    sAns = System.IO.File.ReadAllText(filename);
                }
            }
            catch { }

            //
            return sAns;
        }

        public static string LastError { get; set; }

        public static byte[] ReadFileAsBytes(this string filename)
        {
            // Assume we could not read
            byte[] c_Ans = null;

            LastError = null;
            int iTries = 10;

            while (iTries > 0 && c_Ans == null)
            {
                try
                {
                    if (filename.PathIsFile())
                    {
                        // Read
                        c_Ans = System.IO.File.ReadAllBytes(filename);
                    }
                    else
                    {
                        LastError = "{0} is not a file".FormatString(filename);
                        iTries = 0;
                    }
                }
                catch (Exception e)
                {
                    if (e.GetAllExceptions().IndexOf("Sharing violation") != -1)
                    {
                        iTries--;
                        1.SecondsAsTimeSpan().Sleep();
                    }
                    else
                    {
                        LastError = "While reading {0}: {1}".FormatString(filename, e.GetAllExceptions());
                        iTries = 0;
                    }
                }
            }

            //
            return c_Ans;
        }

        /// <summary>
        /// 
        /// Returns the HTTP Content-Type from a file path
        /// 
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>The content-type</returns>

        public static string ContentTypeFromPath(this string path)
        {
            return path.GetExtensionTypeFromPath().ContentTypeFromExtensionType();
        }

        public static string ContentTypeFromExtensionType(this ExtensionTypes type)
        {
            string sContent = "";

            switch (type)
            {
                case ExtensionTypes.CSS:
                    sContent = "text/css";
                    break;

                case ExtensionTypes.BMP:
                case ExtensionTypes.JPEG:
                case ExtensionTypes.GIF:
                case ExtensionTypes.TIFF:
                case ExtensionTypes.PNG:
                    sContent = "image/" + type;
                    break;

                case ExtensionTypes.Text:
                case ExtensionTypes.CSV:
                    sContent = "text/plain";
                    break;

                case ExtensionTypes.PDF:
                    sContent = "application/pdf";
                    break;

                case ExtensionTypes.FDF:
                    sContent = "application/vnd.fdf";
                    break;

                case ExtensionTypes.Doc:
                case ExtensionTypes.Docx:
                case ExtensionTypes.ODT:
                    sContent = "application/msword";
                    break;

                case ExtensionTypes.XLS:
                    sContent = "application/vnd.ms-excel";
                    break;

                case ExtensionTypes.PPT:
                    sContent = "application/mspowerpoint";
                    break;

                case ExtensionTypes.WPD:
                    sContent = "application/wordperfect";
                    break;

                case ExtensionTypes.RTF:
                    sContent = "application/rtf";
                    break;

                case ExtensionTypes.HTML:
                    sContent = "text/html";
                    break;

                case ExtensionTypes.MHTML:
                    sContent = "text/mhtml";
                    break;

                case ExtensionTypes.XML:
                    sContent = "text/xml";
                    break;

                default:
                    sContent = "application/octect-stream";
                    break;
            }

            return sContent;
        }

        public static string DeleteFile(this string filename)
        {
            string sAns = null;

            try
            {
                System.IO.File.Delete(filename);
            }
            catch (Exception e)
            {
                sAns = e.GetAllExceptions();
            }

            return sAns.IfEmpty();
        }

        public static void RenameFile(this string oldname, string newname)
        {
            try
            {
                newname.DeleteFile();
            }
            catch { }

            try
            {
                System.IO.File.Move(oldname, newname);
            }
            catch { }
        }

        public static string WorkingDirectory(this string x)
        {
            return System.IO.Directory.GetCurrentDirectory();
        }
        
        public enum ExtensionTypes
        {
            Unknown,
            Text,
            PDF,
            FDF,
            XML,
            Doc,
            Docx,
            XLS,
            PPT,
            WPD,
            RTF,
            JPEG,
            GIF,
            TIFF,
            BMP,
            PNG,
            CSV,
            HTML,
            MHTML,
            ODT,
            CSS
        }
        #endregion

        #region Threading
        public static string StartThread(this ParameterizedThreadStart code, params object[] values)
        {
            return SafeThreadManagerClass.StartThread(code, values);
        }
        #endregion

        #region Time Spans
        public static TimeSpan MillisecondsAsTimeSpan(this int value)
        {
            return TimeSpan.FromMilliseconds(value);
        }

        public static TimeSpan SecondsAsTimeSpan(this int value)
        {
            return TimeSpan.FromSeconds(value);
        }

        public static TimeSpan MinutesAsTimeSpan(this int value)
        {
            return TimeSpan.FromMinutes(value);
        }

        public static TimeSpan HoursAsTimeSpan(this int value)
        {
            return TimeSpan.FromHours(value);
        }

        public static TimeSpan DaysAsTimeSpan(this int value)
        {
            return TimeSpan.FromDays(value);
        }

        public static TimeSpan WeeksAsTimeSpan(this int value)
        {
            return TimeSpan.FromDays(value * 7);
        }

        public static TimeSpan SecondsAsTimeSpan(this double value)
        {
            return TimeSpan.FromSeconds(value);
        }

        public static TimeSpan MinutesAsTimeSpan(this double value)
        {
            return TimeSpan.FromMinutes(value);
        }

        public static TimeSpan HoursAsTimeSpan(this double value)
        {
            return TimeSpan.FromHours(value);
        }

        public static TimeSpan DaysAsTimeSpan(this double value)
        {
            return TimeSpan.FromDays(value);
        }

        public static TimeSpan WeeksAsTimeSpan(this double value)
        {
            return TimeSpan.FromDays(value * 7);
        }

        public static TimeSpan MonthsAsTimeSpan(this double value)
        {
            return TimeSpan.FromDays(value * 30);
        }

        public static TimeSpan AtNextHourAsTimeSpan(this int value)
        {
            return ((double)value).AtNextHourAsTimeSpan();
        }

        public static TimeSpan AtNextHourAsTimeSpan(this double value)
        {
            DateTime c_Now = ExtensionsClass.Now();

            double iMinutes = 60 - c_Now.Minute;
            //(24 * 60) - (c_Now.Minute + (60 * c_Now.Hour));
            //iMinutes += (60 * value);

            return iMinutes.MinutesAsTimeSpan();
        }
        #endregion

        #region Timezone
        private static Dictionary<string, TimeZoneInfo> ITimeZones { get; set; }
        public static Dictionary<string, TimeZoneInfo> TimeZones
        {
            get
            {
                if (ITimeZones == null)
                {
                    ITimeZones = new Dictionary<string, TimeZoneInfo>();

                    var timeZones = TimeZoneInfo.GetSystemTimeZones();
                    foreach (var timeZone in timeZones)
                    {
                        ITimeZones.Add(timeZone.DisplayName, timeZone);
                    }
                }

                return ITimeZones;
            }
        }

        public static string DefaultTimeZone
        {
            get { return "".IsLinux() ? "US/Pacific" : "(UTC-08:00) Pacific Time (US & Canada)"; }
        }

        public static TimeZoneInfo GetTimezone(this string tz)
        {
            TimeZoneInfo c_Ans = null;

            // Use default
            tz = tz.IfEmpty(DefaultTimeZone);
            // Adjust for windows
            if (!"".IsLinux()) tz = TZConvert.IanaToWindows(tz);

            if (TimeZones.ContainsKey(tz))
            {
                c_Ans = TimeZones[tz];
            }
            else if (!tz.IsSameValue(DefaultTimeZone))
            {
                c_Ans = DefaultTimeZone.GetTimezone();
            }

            return c_Ans;
        }

        public static DateTime AdjustTimezone(this DateTime dt)
        {
            return dt.AdjustTimezone(DefaultTimeZone);
        }

        public static DateTime AdjustTimezone(this DateTime dt, string timezone, bool reverse = false)
        {
            DateTime c_Ans = dt.ToUniversalTime();

            try
            {
                TimeZoneInfo c_TZ = timezone.IfEmpty(DefaultTimeZone).GetTimezone();

                if (c_TZ != null)
                {
                    if (reverse)
                    {
                        c_Ans = c_Ans.Subtract(c_TZ.BaseUtcOffset);
                    }
                    else
                    {
                        c_Ans = c_Ans.Add(c_TZ.BaseUtcOffset);
                    }

                    if (c_TZ.IsDaylightSavingTime(c_Ans))
                    {
                        if (reverse)
                        {
                            c_Ans = c_Ans.AddHours(-1);
                        }
                        else
                        {
                            c_Ans = c_Ans.AddHours(1);
                        }
                    }
                }
            }
            catch { }

            return c_Ans;
        }
        #endregion

        #region DateTime
        public enum Holidays
        {
            None,
            All,
            NewYear,
            MLK,
            President,
            Memorial,
            Independence,
            Labor,
            Columbus,
            Veteran,
            Thanksgiving,
            Christmas
        }

        public static DateTime DTNow
        {
            get { return DateTime.Now; }
        }

        public static string Timestamp(int days = 0)
        {
            return ExtensionsClass.Now().AddDays(days).ToString("MM/dd/yyyy hh:mm tt");
        }

        public static DateTime Now()
        {
            return DefaultTimeZone.Now();
        }

        public static DateTime Now(this string tz)
        {
            return ExtensionsClass.DTNow.AdjustTimezone(tz);
        }

        public static DateTime Today()
        {
            return ExtensionsClass.Now().DateOnly();
        }

        public static DateTime Today(this string tz)
        {
            return tz.Now().DateOnly();
        }

        public static bool IsWeekend(this DateTime date)
        {
            return (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        public static DateTime ToBusinessDay(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday) date = date.AddDays(1);
            if (date.DayOfWeek == DayOfWeek.Sunday) date = date.AddDays(1);
            if (date.IsFederalHoliday()) date = date.AddDays(1);

            return date;
        }

        /// <summary>
        /// Determines if this date is a federal holiday.
        /// </summary>
        /// <param name="date">This date</param>
        /// <returns>True if this date is a federal holiday</returns>
        public static bool IsFederalHoliday(this DateTime date)
        {
            return date.FederalHoliday() != Holidays.None;
        }

        //public static bool IsFederalHoliday(this DateTime date)
        //{
        //    // to ease typing
        //    int nthWeekDay = (int)(Math.Ceiling((double)date.Day / 7.0d));
        //    DayOfWeek dayName = date.DayOfWeek;
        //    bool isThursday = dayName == DayOfWeek.Thursday;
        //    bool isFriday = dayName == DayOfWeek.Friday;
        //    bool isMonday = dayName == DayOfWeek.Monday;
        //    bool isWeekend = dayName == DayOfWeek.Saturday || dayName == DayOfWeek.Sunday;

        //    // New Years Day (Jan 1, or preceding Friday/following Monday if weekend)
        //    if ((date.Month == 12 && date.Day == 31 && isFriday) ||
        //        (date.Month == 1 && date.Day == 1 && !isWeekend) ||
        //        (date.Month == 1 && date.Day == 2 && isMonday)) return true;

        //    // MLK day (3rd monday in January)
        //    if (date.Month == 1 && isMonday && nthWeekDay == 3) return true;

        //    // President’s Day (3rd Monday in February)
        //    if (date.Month == 2 && isMonday && nthWeekDay == 3) return true;

        //    // Memorial Day (Last Monday in May)
        //    if (date.Month == 5 && isMonday && date.AddDays(7).Month == 6) return true;

        //    // Independence Day (July 4, or preceding Friday/following Monday if weekend)
        //    if ((date.Month == 7 && date.Day == 3 && isFriday) ||
        //        (date.Month == 7 && date.Day == 4 && !isWeekend) ||
        //        (date.Month == 7 && date.Day == 5 && isMonday)) return true;

        //    // Labor Day (1st Monday in September)
        //    if (date.Month == 9 && isMonday && nthWeekDay == 1) return true;

        //    // Columbus Day (2nd Monday in October)
        //    if (date.Month == 10 && isMonday && nthWeekDay == 2) return true;

        //    // Veteran’s Day (November 11, or preceding Friday/following Monday if weekend))
        //    if ((date.Month == 11 && date.Day == 10 && isFriday) ||
        //        (date.Month == 11 && date.Day == 11 && !isWeekend) ||
        //        (date.Month == 11 && date.Day == 12 && isMonday)) return true;

        //    // Thanksgiving Day (4th Thursday in November)
        //    if (date.Month == 11 && isThursday && nthWeekDay == 4) return true;

        //    // Christmas Day (December 25, or preceding Friday/following Monday if weekend))
        //    if ((date.Month == 12 && date.Day == 24 && isFriday) ||
        //        (date.Month == 12 && date.Day == 25 && !isWeekend) ||
        //        (date.Month == 12 && date.Day == 26 && isMonday)) return true;

        //    return false;
        //}

        public static Holidays FederalHoliday(this DateTime date)
        {
            // to ease typing
            int nthWeekDay = (int)(Math.Ceiling((double)date.Day / 7.0d));
            DayOfWeek dayName = date.DayOfWeek;
            bool isThursday = dayName == DayOfWeek.Thursday;
            bool isFriday = dayName == DayOfWeek.Friday;
            bool isMonday = dayName == DayOfWeek.Monday;
            bool isWeekend = dayName == DayOfWeek.Saturday || dayName == DayOfWeek.Sunday;

            // New Years Day (Jan 1, or preceding Friday/following Monday if weekend)
            if ((date.Month == 12 && date.Day == 31 && isFriday) ||
                (date.Month == 1 && date.Day == 1 && !isWeekend) ||
                (date.Month == 1 && date.Day == 2 && isMonday)) return Holidays.NewYear;

            // MLK day (3rd monday in January)
            if (date.Month == 1 && isMonday && nthWeekDay == 3) return Holidays.MLK;

            // President’s Day (3rd Monday in February)
            if (date.Month == 2 && isMonday && nthWeekDay == 3) return Holidays.President;

            // Memorial Day (Last Monday in May)
            if (date.Month == 5 && isMonday && date.AddDays(7).Month == 6) return Holidays.Memorial;

            // Independence Day (July 4, or preceding Friday/following Monday if weekend)
            if ((date.Month == 7 && date.Day == 3 && isFriday) ||
                (date.Month == 7 && date.Day == 4 && !isWeekend) ||
                (date.Month == 7 && date.Day == 5 && isMonday)) return Holidays.Independence;

            // Labor Day (1st Monday in September)
            if (date.Month == 9 && isMonday && nthWeekDay == 1) return Holidays.Labor;

            // Columbus Day (2nd Monday in October)
            if (date.Month == 10 && isMonday && nthWeekDay == 2) return Holidays.Columbus;

            // Veteran’s Day (November 11, or preceding Friday/following Monday if weekend))
            if ((date.Month == 11 && date.Day == 10 && isFriday) ||
                (date.Month == 11 && date.Day == 11 && !isWeekend) ||
                (date.Month == 11 && date.Day == 12 && isMonday)) return Holidays.Veteran;

            // Thanksgiving Day (4th Thursday in November)
            if (date.Month == 11 && isThursday && nthWeekDay == 4) return Holidays.Thanksgiving;

            // Christmas Day (December 25, or preceding Friday/following Monday if weekend))
            if ((date.Month == 12 && date.Day == 24 && isFriday) ||
                (date.Month == 12 && date.Day == 25 && !isWeekend) ||
                (date.Month == 12 && date.Day == 26 && isMonday)) return Holidays.Christmas;

            return Holidays.None;
        }

        public static DateTime DateOnly(this DateTime dt)
        {
            return dt.Date; // new DateTime(dt.Year, dt.Month, dt.Day);
        }

        public static DateTime AsUTC(this DateTime dt)
        {
            return (new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, new TimeSpan(0, 0, 0))).DateTime;
        }

        public static DateTime OnOrAfter(this DateTime date, DateTime mindate)
        {
            return date >= mindate ? date : mindate;
        }

        public static DateTime OnOrBefore(this DateTime date, DateTime mindate)
        {
            return date <= mindate ? date : mindate;
        }

        public static DateTime After(this DateTime date, DateTime mindate)
        {
            return date > mindate ? date : mindate;
        }

        public static DateTime Before(this DateTime date, DateTime mindate)
        {
            return date < mindate ? date : mindate;
        }

        public static bool IsOnOrAfter(this DateTime date, DateTime mindate)
        {
            return date >= mindate;
        }

        public static bool IsOnOrBefore(this DateTime date, DateTime mindate)
        {
            return date <= mindate;
        }

        public static bool IsAfter(this DateTime date, DateTime mindate)
        {
            return date > mindate;
        }

        public static bool IsBefore(this DateTime date, DateTime mindate)
        {
            return date < mindate;
        }

        public static int DaysDiff(this DateTime date, DateTime mindate)
        {
            return (int)date.Date.Subtract(mindate.Date).TotalDays;
        }

        public static int AgeInDays(this DateTime date)
        {
            return (int)Math.Abs(DateTime.Now.Subtract(date).TotalDays);
        }

        public static DateTime NextDay(this DateTime dt)
        {
            return dt.DateOnly().MoveByDays(1);
        }

        public static DateTime PrevDay(this DateTime dt)
        {
            return dt.DateOnly().MoveByDays(-1);
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            return dt.StartOfMonth().MoveByMonths(1).MoveBy(-1.DaysAsTimeSpan());
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime At(this DateTime dt, int hour)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, hour, 0, 0);
        }
        public static DateTime MoveTo(this DateTime dt, DayOfWeek day)
        {
            int iDays = (int)day - (int)dt.DayOfWeek;
            if (iDays < 0) iDays += 7;

            return dt.AddDays(iDays);
        }

        public static DateTime BackTo(this DateTime dt, DayOfWeek day)
        {
            dt = dt.MoveTo(day);
            return dt.AddDays(-7);
        }

        public static DateTime MoveBy(this DateTime dt, TimeSpan span)
        {
            return dt.Add(span);
        }

        public static DateTime MoveByDays(this DateTime dt, int days)
        {
            return dt.AddDays(days);
        }

        public static DateTime MoveByMonths(this DateTime dt, int months)
        {
            return dt.AddMonths(months);
        }

        public static DateTime MoveByYears(this DateTime dt, int years)
        {
            return dt.AddYears(years);
        }

        public static DateTime Parse(this string date)
        {
            return DateTime.Parse(date);
        }
        #endregion

        #region Resources
        public static Stream GetResourceStream(this object obj, string name)
        {
            Stream c_Stream = null;

            Type c_Type = obj.GetType();
            if (c_Type != null)
            {
                string[] c_Names = c_Type.Assembly.GetManifestResourceNames();
                foreach (string sName in c_Names)
                {
                    if (sName.EndsWith("." + name))
                    {
                        c_Stream = c_Type.Assembly.GetManifestResourceStream(sName);
                        break;
                    }
                }
            }

            return c_Stream;
        }

        public static byte[] GetResource(this object obj, string name)
        {
            byte[] abAns = null;

            try
            {
                Stream c_Stream = GetResourceStream(obj, name);
                abAns = new byte[c_Stream.Length];
                c_Stream.Read(abAns, 0, abAns.Length);
            }
            catch { }

            return abAns;
        }
        #endregion
    }
}
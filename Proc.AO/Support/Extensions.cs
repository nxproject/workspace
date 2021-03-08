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
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
///  

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public static class Extensions
    {
        #region BsonDocument
        /// <summary>
        /// 
        /// Gets a field value
        /// 
        /// </summary>
        /// <param name="doc">The BSON document</param>
        /// <param name="field">The field</param>
        /// <returns>The value</returns>
        public static string GetField(this BsonDocument doc, string field)
        {
            // Assume none
            string sAns = null;

            // Protect
            try
            {
                // Do we have it?
                if (doc.Contains(field))
                {
                    sAns = doc.GetValue(field).AsString;
                }
            }
            catch { }

            return sAns;
        }

        /// <summary>
        /// 
        /// Sets a field value
        /// 
        /// </summary>
        /// <param name="doc">The BSON document</param>
        /// <param name="field">The field</param>
        /// <param name="value">The value</param>
        /// <returns>The BSON document</returns>
        public static BsonDocument SetField(this BsonDocument doc, string field, string value)
        {
            // Protect
            try
            {
                doc.Set(field, value);
            }
            catch { }

            return doc;
        }

        /// <summary>
        /// 
        /// Gets a field value
        /// 
        /// </summary>
        /// <param name="doc">The BSON document</param>
        /// <param name="field">The field</param>
        /// <returns>The value</returns>
        public static BsonDocument GetDocument(this BsonDocument doc, string field)
        {
            // Assume none
            BsonDocument c_Ans = null;

            // Protect
            try
            {
                // Do we have it?
                if (!doc.Contains(field))
                {
                    doc.Set(field, new BsonDocument());
                }

                // Get
                c_Ans = doc.GetValue(field).AsBsonDocument;
            }
            catch { }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Gets a field value
        /// 
        /// </summary>
        /// <param name="doc">The BSON document</param>
        /// <param name="field">The field</param>
        /// <returns>The value</returns>
        public static BsonArray GetList(this BsonDocument doc, string field)
        {
            // Assume none
            BsonArray c_Ans = null;

            // Protect
            try
            {
                // Do we have it?
                if (!doc.Contains(field))
                {
                    doc.Set(field, new BsonArray());
                }

                // Get
                c_Ans = doc.GetValue(field).AsBsonArray;
            }
            catch { }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Gets a JSON object 
        /// 
        /// </summary>
        /// <param name="doc">The BSON Document</param>
        /// <returns>A JSON object</returns>
        public static JObject ToJObject(this BsonDocument doc)
        {
            return doc.ToString().ToJObject();
        }

        /// <summary>
        /// 
        /// Gets a JSON array 
        /// 
        /// </summary>
        /// <param name="doc">The BSON Document</param>
        /// <returns>A JSON object</returns>
        public static JArray ToJArray(this BsonArray doc)
        {
            return doc.ToString().ToJArray();
        }

        /// <summary>
        /// 
        /// Copies a BsonDocument from a store
        /// 
        /// </summary>
        /// <param name="doc">The BSON Document</param>
        /// <param name="obj">The source object</param>
        public static void FromJObject(this BsonDocument doc, JObject obj)
    {
        // Convert to BsonDocument
        BsonDocument c_Wkg = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(obj.ToSimpleString());

        // Loop thru
        foreach (string sKey in c_Wkg.Keys())
        {
            // Set
            doc[sKey] = c_Wkg[sKey];
        }
    }

        /// <summary>
        /// 
        /// Copies a BsonDocument into a store
        /// 
        /// </summary>
        /// <param name="doc">The BSON Document</param>
        /// <param name="store">The target store</param>
        public static void ToStore(this BsonDocument doc, StoreClass store)
        {
            //
            store.Load(doc.ToJObject());
        }

        /// <summary>
        /// 
        /// Copies a BsonDocument from a store
        /// 
        /// </summary>
        /// <param name="doc">The BSON Document</param>
        /// <param name="store">The target store</param>
        public static void FromStore(this BsonDocument doc, StoreClass store)
        {
            //
            doc.FromJObject(store.SynchObject);
        }

        /// <summary>
        /// 
        /// Returns the field names
        /// 
        /// </summary>
        /// <param name="doc">The BSON document</param>
        /// <returns></returns>
        public static List<string> Keys(this BsonDocument doc)
        {
            List<string> c_Ans = new List<string>();

            foreach (var c_Entry in doc)
            {
                c_Ans.Add(c_Entry.Name);
            }

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Copies contents of document
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="target"></param>
        /// <param name="include"></param>
        /// <param name="skip"></param>
        public static void CopyTo(this BsonDocument doc,  BsonDocument target, string include = "", string skip = "")
        {
            // Lopp thru
            foreach (string sField in doc.Keys())
            {
                bool bOK = true;

                // 
                if (include.HasValue())
                {
                    // Match?
                    bOK = sField.StartsWith(include);
                }

                if(bOK)
                {
                    // Do we have a skip?
                    if (skip.HasValue())
                    {
                        // Not match?
                        bOK = !sField.StartsWith(skip);
                    }
                    
                    if(bOK)
                    {
                        // Copy
                        target = target.Set(sField, doc.GetValue(sField));
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// Assure that a field is a JObject
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="field"></param>
        public static void AssureJObject(this BsonDocument doc, string field)
        {
            // Try
            try
            {
                // Make
                doc[field] = doc[field].AsBsonDocument;
            }
            catch
            {
                try
                {
                    doc[field] = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(doc[field].AsString);
                }
                catch { }
            }
        }

        /// <summary>
        /// 
        /// Assure that a field is a JArray
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="field"></param>
        public static void AssureJArray(this BsonDocument doc, string field)
        {
            // Try
            try
            {
                // Make
                doc[field] = doc[field].AsBsonArray;
            }
            catch
            {
                try
                {
                    doc[field] = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(doc[field].AsString);
                }
                catch { }
            }
        }
        #endregion

        #region Context
        public static string Get(this Context ctx, string field)
        {
            using (DatumClass c_Datum = new DatumClass(ctx, field))
            {
                return c_Datum.Value;
            }
        }

        public static Context Set(this Context ctx, string field, string value)
        {
            using (DatumClass c_Datum = new DatumClass(ctx, field))
            {
                c_Datum.Value = value;

                return ctx;
            }
        }
        #endregion

        #region Formatting
        public static string AsKeyword(this string value)
        {
            return Regex.Replace(value.IfEmpty().ToLower(), @"[^a-z]", "");
        }

        public static string AsExtendedKeyword(this string value)
        {
            bool bSys = value.IfEmpty().StartsWith("_");

            return (bSys ? "_" : "") + Regex.Replace(value.IfEmpty().ToLower(), @"[^a-z]", "");
        }

        public static string AsDatasetName(this string value)
        {
            //
            value = value.IfEmpty();
            if (value.StartsWith("#")) value = value.Substring(1);

            return value.AsExtendedKeyword();
        }

        public static string AsViewName(this string value)
        {
            //
            value = value.IfEmpty();

            if (value.StartsWith("#view_")) value = value.Substring(6);

            return value.AsExtendedKeyword();
        }

        public static string AsFieldName(this string value)
        {
            //
            if(value.HasValue())
            {
                // Turn into keyword
                string sWkg = value.AsKeyword();
                // Any?
                if(!sWkg.HasValue())
                {
                    value = "";
                }
                else
                {
                    // Save first character
                    string sLead = sWkg.Substring(0, 1);
                    // Turn into valid
                    sWkg = Regex.Replace(value.IfEmpty().ToLower(), @"[^a-z0-9]", "");
                    // Now remove lead
                    sWkg = sWkg.Remove(sWkg.IndexOf(sLead), 1);
                    // Make
                    value = sLead + sWkg;
                }
            }
            //
            return value;
        }

        public static string Secured(this string value)
        {
            if (value.HasValue() && value.Length > 4)
            {
                value = value.Substring(value.Length - 4, 4).LPad(value.Length, "*");
            }

            return value;
        }

        /// <summary>
        /// 
        /// Format as a phone
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPhone(this string value)
        {
            string raw = value.IfEmpty();
            if (!raw.StartsWith("+"))
            {
                raw = Regex.Replace(raw, @"[^0-9]", "");
                if (raw.Length < 10) raw = raw.LPad(10, "0");
                value = '(' + raw.Substring(0, 3) + ") " + raw.Substring(3, 3) + "-" + raw.Substring(6, 4);
            }
            return value;
        }

        /// <summary>
        /// 
        /// Format as a field name [xxx]
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string AsObjField(this string field)
        {
            return "[{0}]".FormatString(field.AsFieldName());
        }
        #endregion

        #region Files
        public static string PathToObjectDataset(this string path)
        {
            // Assume nobe
            string sAns = null;

            // Split 
            string[] asPieces = path.Split("/");
            // Validate
            if(asPieces.Length >= 3 && asPieces[1].IsSameValue("ao"))
            {
                // Get
                sAns = asPieces[2];
            }

            return sAns;
        }

        public static string PathToObjectID(this string path)
        {
            // Assume nobe
            string sAns = null;

            // Split 
            string[] asPieces = path.Split("/");
            // Validate
            if (asPieces.Length >= 3 && asPieces[1].IsSameValue("ao"))
            {
                // Get
                sAns = asPieces[2];
            }

            return sAns;
        }
        #endregion

        #region Fields
        public static string GetTRAILINGNUMBER(this string value)
        {
            string ans = "";
            Match matches = Regex.Match(value, @"\d+$");
            if (matches.Success)
            {
                ans = matches.Value;
            }
            return ans;
        }

        public static string RemoveTRAILINGNUMBER(this string value)
        {
            string end = value.GetTRAILINGNUMBER();
            if (end.HasValue())
            {
                value = value.Substring(0, value.Length - end.Length);
            }
            return value;
        }
        #endregion

        #region Support
        public static int Min(this int value, int minval)
        {
            if (value < minval) value = minval;
            return value;
        }

        public static int Max(this int value, int maxval)
        {
            if (value > maxval) value = maxval;
            return value;
        }
        #endregion

        #region Dates
        public static DateTime MongoIDtoDate(this string id)
        {
            DateTime c_Ans = DateTime.MinValue;

            if (id.HasValue())
            {
                long iAns = 0;

                try
                {
                    iAns = long.Parse(id.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                c_Ans = new DateTime(1970, 1, 1).AddSeconds(iAns).ToLocalTime();
            }

            return c_Ans;
        }

        public static bool IsISODate(this string value)
        {
            return Regex.Match(value.IfEmpty(), @"^\d{4}\x2D\d{2}\x2D\d{2}\x54\d{2}\x3A\d{2}\x3A\d{2}\x2E\d{3}\x5A$").Success;
        }

        //public static string ToDBDate(this DateTime value)
        //{
        //    string sAns = "";
        //    if (!value.Equals(DateTime.MinValue))
        //    {
        //        sAns = value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        //    }
        //    return sAns;
        //}

        //public static string ToDBDate(this DateTime? value)
        //{
        //    string sAns = "";
        //    if (value != null)
        //    {
        //        sAns = ((DateTime)value).ToDBDate();
        //    }
        //    return sAns;
        //}

        //public static DateTime FromDBDate(this string value)
        //{
        //    return value.FromDBDate(DateTime.Now);
        //}

        public static string AsLocalTimeString(this DateTime value)
        {
            string sAns = "";
            if (!value.Equals(DateTime.MinValue))
            {
                sAns = value.ToDBDate().Replace("T", " ").Replace("Z", "");
            }
            return sAns;
        }

        public static DateTime AsLocalTime(this DateTime value)
        {
            return DateTime.Parse(value.AsLocalTimeString());
        }

        //public static DateTime FromDBDate(this string value, DateTime defvalue)
        //{
        //    DateTime c_Ans = defvalue;

        //    try
        //    {
        //        c_Ans = DateTime.Parse(value);
        //    }
        //    catch { }

        //    return c_Ans;
        //}

        public static string ToTZString(this DateTime value, string tz)
        {
            string sAns = "";
            if (!value.Equals(DateTime.MinValue))
            {
                sAns = value.ToString("yyy-MM-ddTHH:mm:ss") + tz;
            }
            return sAns;
        }

        public static string FormattedAs(this DateTime value, string fmt)
        {
            return value.ToString(fmt);
        }

        public static DateTime AdjustUTC(this DateTime value, TimeZoneInfo to)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(value, to);
        }

        public static string ToTimeStamp(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm");
        }

        public static string ToTimeStampSecs(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToSortable(this DateTime value)
        {
            return value.ToString("yyyyMMddHHmm");
        }

        public static string ToSortableDate(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd");
        }

        public static string ToReverseSortable(this DateTime value)
        {
            string sAns = "";

            string sWkg = value.ToSortable();
            for (int iLoop = 0; iLoop < sWkg.Length; iLoop++)
            {
                sAns += "9876543210"["0123456789".IndexOf(sWkg.Substring(iLoop, 1))];
            }

            return sAns;
        }
        #endregion

        #region Duration
        public static string ToDBDuration(this string value)
        {
            if (value.HasValue())
            {
                var re = new Regex(@"(?<days>\d+d)|(?<hours>\d+h)|(?<minutes>\d+m)|(?<secs>\d+s)");
                var p = re.Matches(value.ToLower().Replace(" ", ""));
                var days = "";
                var hours = "";
                var mins = "";
                var secs = "";
                foreach (Match c_Piece in p)
                {
                    //
                    string piece = c_Piece.Value;

                    var last = piece.Substring(piece.Length - 1, 1);
                    switch (last)
                    {
                        case "d":
                            days = piece;
                            break;
                        case "h":
                            hours = piece;
                            break;
                        case "m":
                            mins = piece;
                            break;
                        case "s":
                            secs = piece;
                            break;
                    }
                }
                value = "";
                if (days.HasValue()) value += ":" + days;
                if (hours.HasValue()) value += ":" + hours;
                if (mins.HasValue()) value += ":" + mins;
                if (secs.HasValue()) value += ":" + secs;
                if (value.Length> 0) value = value.Substring(1);
            }

            return value;
        }

        public static double ToSeconds(this string duration)
        {
            double dAns = 0;

            if (duration.HasValue())
            {
                var re = new Regex(@"(?<days>\d+d)|(?<hours>\d+h)|(?<minutes>\d+m)|(?<secs>\d+s)");
                var p = re.Matches(duration.ToLower().Replace(" ", ""));
                var days = "";
                var hours = "";
                var mins = "";
                var secs = "";
                foreach (Match c_Piece in p)
                {
                    //
                    string piece = c_Piece.Value;

                    var last = piece.Substring(piece.Length - 1, 1);
                    switch (last)
                    {
                        case "d":
                            days = piece;
                            break;
                        case "h":
                            hours = piece;
                            break;
                        case "m":
                            mins = piece;
                            break;
                        case "s":
                            secs = piece;
                            break;
                    }
                }
                
                if (days.HasValue()) dAns += (days.ToInteger(0) * 24 * 60* 60);
                if (hours.HasValue()) dAns += (hours.ToInteger(0) *  60 * 60);
                if (mins.HasValue()) dAns += (mins.ToInteger(0) * 60);
                if (secs.HasValue()) dAns += secs.ToInteger(0);
            }

            return dAns ;
        }

        public static string ToDuration(this double secs)
        {
            string ans = "";

            // Compute days
            var days = Math.Floor(secs / 86400);
            secs -= days * 86400;
            // Compute hours
            var hours = Math.Floor(secs / 3600);
            secs -= hours * 3600;
            // Minutes
            var minutes = Math.Floor(secs / 60);
            // Seconds
            var seconds = secs - (minutes * 60);

            if (ans.Length > 0 || days > 0) ans += ':' + days;
            if (ans.Length > 0 || hours > 0) ans += ':' + "{0:00}".FormatString(hours);
            if (ans.Length > 0 || minutes > 0) ans += ':' + "{0:00}".FormatString(minutes);
            if (ans.Length > 0 || seconds > 0) ans += ':' + "{0:00}".FormatString(seconds);

            if (ans.Length > 0) ans = ans.Substring(1);

            return ans;
        }
        #endregion
    }
}
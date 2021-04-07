//--------------------------------------------------------------------------------
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

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.AO.BuiltIn
{
    public static class DefaultClass
    {
        #region Constants
        private const string Release = "0001";
        #endregion

        #region Definer
        public static void Define(this DatasetClass ds)
        {
            // Accordng to type
            switch (ds.Name)
            {
                case DatabaseClass.DatasetSys:
                    ds.Define_Sys();
                    break;

                case DatabaseClass.DatasetHelp:
                    ds.Define_Help();
                    break;

                case DatabaseClass.DatasetWeb:
                    ds.Define_Web();
                    break;

                case DatabaseClass.DatasetEMailTemplates:
                    ds.Define_EMailTemplate();
                    break;


                case DatabaseClass.DatasetAllowed:
                    ds.Define_Allowed();
                    break;

                case DatabaseClass.DatasetUser:
                    ds.Define_User();
                    break;

                case DatabaseClass.DatasetCron:
                    ds.Define_Cron();
                    break;


                case DatabaseClass.DatasetIOTAgent:
                    ds.Define_IOT("Agent", "user");
                    break;
                case DatabaseClass.DatasetIOTClient:
                    ds.Define_IOT("Client", "group");
                    break;
                case DatabaseClass.DatasetIOTUnit:
                    ds.Define_IOT("Unit", "plugin");
                    break;
                case DatabaseClass.DatasetIOTKeyboard:
                    ds.Define_IOT("Keyboard", "keyboard");
                    break;
                case DatabaseClass.DatasetIOTVerb:
                    ds.Define_IOT("Verb", "star");
                    break; ;
                case DatabaseClass.DatasetIOTMacro:
                    ds.Define_IOT("Macro", "script");
                    break;


                case DatabaseClass.DatasetTelemetry:
                    ds.Define_Telemetry();
                    break;

                case DatabaseClass.DatasetTelemetryData:
                    ds.Define_TelemetryData();
                    break;


                case DatabaseClass.DatasetBillAccess:
                    ds.Define_BillAccess();
                    break;

                case DatabaseClass.DatasetBiilCharge:
                    ds.Define_BillCharge();
                    break;

                case DatabaseClass.DatasetBiilInvoice:
                    ds.Define_BillInvoice();
                    break;

                case DatabaseClass.DatasetBiilRate:
                    ds.Define_BillRate();
                    break;

                case DatabaseClass.DatasetBiilSubscription:
                    ds.Define_BillSubscription();
                    break;



                case DatabaseClass.DatasetQuorum:
                    ds.Define_Quorum();
                    break;

                case DatabaseClass.DatasetQuorumTopic:
                    ds.Define_QuorumTopic();
                    break;

                case DatabaseClass.DatasetQuorumRating:
                    ds.Define_QuorumRating();
                    break;

                case DatabaseClass.DatasetQuorumComment:
                    ds.Define_QuorumComment();
                    break;

                case DatabaseClass.DatasetQuorumOption:
                    ds.Define_QuorumOption();
                    break;

                default:
                    ds.Define_Generic();
                    break;
            }
        }
        #endregion

        #region System
        private static void Define_Sys(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.07a"))
            {
                //
                ds.Definition.Caption = "Site Settings";
                ds.Definition.Placeholder = "[_id]";
                ds.Definition.Privileges = "*";
                ds.Definition.IDAlias = "id";
                ds.Definition.Icon = "computer";
                ds.Definition.SIOEventsAtSave = "$$changed.systemprofile";
                ds.Definition.Selector = "SYS";

                //
                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["id"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Hidden;
                c_Field.DefaultValue = "_info";

                c_Field = ds.Definition["name"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Name;

                c_Field = ds.Definition["addr1"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Address;

                c_Field = ds.Definition["addr2"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Address;

                c_Field = ds.Definition["city"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.City;

                c_Field = ds.Definition["state"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.State;

                c_Field = ds.Definition["zip"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.ZIP;

                c_Field = ds.Definition["phone"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Phone;

                c_Field = ds.Definition["fax"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Phone;

                c_Field = ds.Definition["taxid"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Tax ID";

                c_Field = ds.Definition["timezone"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Timezone;

                c_Field = ds.Definition["twilioacct"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Acct";

                c_Field = ds.Definition["twiliotoken"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Protected;
                c_Field.Label = "Token";

                c_Field = ds.Definition["twiliophone"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TwilioPhone;
                c_Field.Label = "Phone";

                c_Field = ds.Definition["udf1"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;

                c_Field = ds.Definition["udf2"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;

                c_Field = ds.Definition["udf3"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;

                c_Field = ds.Definition["defaultfieldWidth"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Field Width";

                c_Field = ds.Definition["defaultpickWidth"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Pick Width";

                c_Field = ds.Definition["defaultpickHeight"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Pick Height";

                c_Field = ds.Definition["domain"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Domain";

                c_Field = ds.Definition["certemail"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.EMail;
                c_Field.Label = "Cert. EMail";

                c_Field = ds.Definition["proccount"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Proc. Count";

                c_Field = ds.Definition["officeopen"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Time;
                c_Field.Label = "Office Opens";

                c_Field = ds.Definition["officeclose"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Time;
                c_Field.Label = "Office Closes";

                c_Field = ds.Definition["twitterck"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Consumer Key";

                c_Field = ds.Definition["twittersk"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Protected;
                c_Field.Label = "Secret Key";

                c_Field = ds.Definition["twitterat"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Access Token";

                c_Field = ds.Definition["twitterats"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Protected;
                c_Field.Label = "Access Token Secret";

                c_Field = ds.Definition["vc8by8api"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "API Key";

                c_Field = ds.Definition["iotenabled"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "IOT Enabled";

                c_Field = ds.Definition["stripepub"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Public Key";

                c_Field = ds.Definition["stripesec"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Protected;
                c_Field.Label = "Secret Key";

                c_Field = ds.Definition["ttenabled"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Time Track Enabled";

                c_Field = ds.Definition["psapi"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "API Key";

                c_Field = ds.Definition["sendgridapi"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Protected;
                c_Field.Label = "API Key";

                c_Field = ds.Definition["sendgridemail"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.EMail;
                c_Field.Label = "EMail";

                c_Field = ds.Definition["sendgridfriendly"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Name;
                c_Field.Label = "Friendly Name";

                c_Field = ds.Definition["helproot"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;
                c_Field.Label = "Help Root";

                c_Field = ds.Definition["acctenabled"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Accounts";

                c_Field = ds.Definition["billenabled"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Billing";

                c_Field = ds.Definition["quorumenabled"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Quorum Enabled";

                c_Field = ds.Definition["acctdefallowed"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Allowed;
                c_Field.Label = "Acct.Def.Alwd";

                c_Field = ds.Definition["ahksearch"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Lower;
                c_Field.Label = "Datasets/Fields";

                c_Field = ds.Definition["fburl"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Facebook - https://www.facebook.com/";

                c_Field = ds.Definition["liurl"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "LinkedIn - https://www.linkedin.com/";

                c_Field = ds.Definition["twitterurl"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Twitter - https://twitter.com/";

                c_Field = ds.Definition["teleenabled"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Telemetry";

                c_Field.SaveParent();
            }

            // The pages
            Definitions.ViewClass c_VInfo = ds.View("info");
            if (c_VInfo.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VInfo.Caption = "Info";

                // Clear
                c_VInfo.ClearFields();

                // Map
                c_VInfo.UseFields("name", "addr1", "addr2", "city", "state", "zip",
                    "phone", "fax", "taxid");

                c_VInfo.Save();
            }

            Definitions.ViewClass c_CInfo = ds.View("sysa");
            if (c_CInfo.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_CInfo.Caption = "Options";

                // Clear
                c_CInfo.ClearFields();

                // Map
                c_CInfo.UseFields(
                    "acctenabled", "acctdefallowed",
                    "billenabled",
                    "ttenabled",
                    "teleenabled",
                    "quorumenabled", 
                    "iotenabled",  
                    "helproot",
                    "proccount"
                    );

                c_CInfo.Save();
            }

            c_CInfo = ds.View("ahk");
            if (c_CInfo.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_CInfo.Caption = "AutoHotKeys";

                // Clear
                c_CInfo.ClearFields();

                // Map
                c_CInfo.UseFields("ahksearch");

                c_CInfo.Save();
            }

            c_CInfo = ds.View("sysb");
            if (c_CInfo.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_CInfo.Caption = "Office";

                // Clear
                c_CInfo.ClearFields();

                // Map
                c_CInfo.UseFields("timezone", "officeopen", "officeclose");

                c_CInfo.Save();
            }

            c_CInfo = ds.View("sysc");
            if (c_CInfo.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_CInfo.Caption = "SSL";

                // Clear
                c_CInfo.ClearFields();

                // Map
                c_CInfo.UseFields("domain", "certemail");

                c_CInfo.Save();
            }

            c_CInfo = ds.View("sysd");
            if (c_CInfo.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_CInfo.Caption = "Sizes";

                // Clear
                c_CInfo.ClearFields();

                // Map
                c_CInfo.UseFields("defaultfieldWidth", "defaultpickWidth", "defaultpickHeight");

                c_CInfo.Save();
            }

            Definitions.ViewClass c_VTwitter = ds.View("twitter");
            if (c_VTwitter.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VTwitter.Caption = "Twitter";

                // Clear
                c_VTwitter.ClearFields();

                // Map
                c_VTwitter.UseFields("twitterck", "twittersk", "twitterat", "twitterats");

                c_VTwitter.Save();
            }

            Definitions.ViewClass c_VSocial = ds.View("social");
            if (c_VSocial.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VSocial.Caption = "Social Media";

                // Clear
                c_VSocial.ClearFields();

                string sLWidth = "12";

                // Map
                c_VSocial.UseFields("fburl", "twitterurl", "liurl");
                c_VSocial.Set("labelWidth", "12", "fburl", "twitterurl", "liurl");

                c_VSocial.Save();
            }

            Definitions.ViewClass c_VPS = ds.View("ps");
            if (c_VPS.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VPS.Caption = "PositionStack";

                // Clear
                c_VPS.ClearFields();

                // Map
                c_VPS.UseFields("psapi");

                c_VPS.Save();
            }

            Definitions.ViewClass c_VStripe = ds.View("stripe");
            if (c_VStripe.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VStripe.Caption = "Stripe";

                // Clear
                c_VStripe.ClearFields();

                // Map
                c_VStripe.UseFields("stripepub", "stripesec");

                c_VStripe.Save();
            }

            Definitions.ViewClass c_VTwilio = ds.View("twilio");
            if (c_VTwilio.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VTwilio.Caption = "Twilio";

                // Clear
                c_VTwilio.ClearFields();

                // Map
                c_VTwilio.UseFields("twilioacct", "twiliotoken", "twiliophone");

                c_VTwilio.Save();
            }

            Definitions.ViewClass c_VSendGrid = ds.View("sendgrid");
            if (c_VSendGrid.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VSendGrid.Caption = "SendGrid";

                // Clear
                c_VSendGrid.ClearFields();

                // Map
                c_VSendGrid.UseFields("sendgridapi", "sendgridemail", "sendgridfriendly");

                c_VSendGrid.Save();
            }

            Definitions.ViewClass c_V8x8 = ds.View("vebye");
            if (c_V8x8.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_V8x8.Caption = "8x8";

                // Clear
                c_V8x8.ClearFields();

                // Map
                c_V8x8.UseFields("vc8by8api");

                c_V8x8.Save();
            }

            Definitions.ViewClass c_VUDF = ds.View("user");
            if (c_VUDF.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VUDF.Caption = "User Defined";

                // Clear
                c_VUDF.ClearFields();

                // Map
                c_VUDF.UseFields("udf1", "udf2", "udf3");

                c_VUDF.Save();
            }

            // Sub tabs
            Definitions.ViewClass c_VSys = ds.View("sysv");
            if (c_VSys.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VSys.Caption = "System";

                // Clear
                c_VSys.ClearFields();

                // Make the tabs
                Definitions.ViewFieldClass c_Field = c_VSys.AsTabs("sysvt");
                c_Field.Height = "15";
                c_Field.Views = "sysa sysb sysc sysd";

                c_VSys.Save();
            }

            Definitions.ViewClass c_VTP = ds.View("tpv");
            if (c_VTP.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VTP.Caption = "Services";

                // Clear
                c_VTP.ClearFields();

                // Make the tabs
                Definitions.ViewFieldClass c_Field = c_VTP.AsTabs("sysve");
                c_Field.Height = "14";
                c_Field.Views = "twilio sendgrid ps stripe ahk vebye social twitter";

                c_VTP.Save();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("_info");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // Make the tabs
                Definitions.ViewFieldClass c_Field = c_VDefault.AsTabs("tabs");
                c_Field.Width = "default.parenttabWidth";
                c_Field.Height = "19";
                c_Field.Views = "info user sysv tpv";

                c_VDefault.Save();
            }

        }

        private static void Define_Help(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.29a"))
            {
                //
                ds.Definition.Caption = "Help";
                ds.Definition.Placeholder = "[_id] '-' [desc]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "key";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "110";
                ds.Definition.Selector = "HELP";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["text"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.XMDEditor;
                c_Field.Label = "Text";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                // Give some room
                c_VDefault["text"].Height = "15";
                c_VDefault["text"].Width = "default.fieldWidth@2";

                c_VDefault.Save();
            }
        }

        private static void Define_Web(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.29c"))
            {
                //
                ds.Definition.Caption = "Web";
                ds.Definition.Placeholder = "[_id] '-' [desc]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "key";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "150";
                ds.Definition.Selector = "WEB";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Lower;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["text"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.XHTMLEditor;
                c_Field.Label = "Text";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                // Give some room
                c_VDefault["text"].Height = "15";
                c_VDefault["text"].Width = "default.fieldWidth@2";

                c_VDefault.Save();
            }
        }

        private static void Define_EMailTemplate(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.03c"))
            {
                //
                ds.Definition.Caption = "EMail Templates";
                ds.Definition.Placeholder = "[_id] '-' [desc]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "key";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "150";
                ds.Definition.Selector = "EMAIL";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.AutoCaps;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["text"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.XEMailEditor;
                c_Field.Label = "Text";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                // Give some room
                c_VDefault["text"].Height = "15";
                c_VDefault["text"].Width = "default.fieldWidth@2";

                c_VDefault.Save();
            }
        }

        private static void Define_Allowed(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.18a"))
            {
                //
                ds.Definition.Caption = "Allowed Extensions";
                ds.Definition.Placeholder = "[_id] '-' [desc]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "key";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "100";
                ds.Definition.Selector = "ALLOWED";

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["value"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Allowed;
                c_Field.Label = "Allowed";

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TextArea;
                c_Field.Label = "Description";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_User(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.18a"))
            {
                //
                ds.Definition.Caption = ds.Definition.Caption.IfEmpty("User");
                ds.Definition.Placeholder = "[_id]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "name";
                ds.Definition.Icon = ds.Definition.Icon.IfEmpty("user");
                ds.Definition.SIOEventsAtSave = "$$changed.userprofile";
                ds.Definition.Selector = "USER";

                // Restart
                ds.Definition.ClearFields();

                // 
                Definitions.DatasetFieldClass c_Field = ds.Definition["name"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["pwd"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Password;
                c_Field.Label = "Password";

                c_Field = ds.Definition["allowed"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Allowed;

                c_Field = ds.Definition["openmode"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.XOpenMode;
                c_Field.Label = "Open Mode";
                c_Field.DefaultValue = "stack";

                c_Field = ds.Definition["openmodechild"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.XOpenMode;
                c_Field.Label = "O.M. (child)";
                c_Field.DefaultValue = "right";

                c_Field = ds.Definition["emailname"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Name";

                c_Field = ds.Definition["emailpwd"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Password;
                c_Field.Label = "Password";

                c_Field = ds.Definition["emailprov"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.ComboBox;
                c_Field.DefaultValue = "GMail";
                c_Field.Choices = "GMail Live Office365 Hotmail Yahoo GoDaddy HushMail";
                c_Field.Label = "Provider";

                c_Field = ds.Definition["phone"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Phone;
                c_Field.Label = "Phone";

                c_Field = ds.Definition["twiliophone"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TwilioPhone;
                c_Field.Label = "Phone";

                c_Field = ds.Definition["dispname"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Name;
                c_Field.Label = "Full name";

                c_Field = ds.Definition["title"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.AutoCaps;

                c_Field = ds.Definition["signature"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Signature;

                c_Field = ds.Definition["footer"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TextArea;

                c_Field = ds.Definition["vc8by8jwt"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TextArea;
                c_Field.Label = "JWT";

                c_Field = ds.Definition["childof"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Users;
                c_Field.Label = "Child of";

                c_Field.SaveParent();
            }

            // Pages
            Definitions.ViewClass c_VInfo = ds.View("info");
            if (c_VInfo.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VInfo.Caption = "Info";

                //
                c_VInfo.UseFields("name", "pwd", "allowed", "childof");

                c_VInfo.Save();
            }

            Definitions.ViewClass c_VInfoU = ds.View("infou");
            if (c_VInfoU.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VInfoU.Caption = "Info";

                //
                c_VInfoU.UseFields("pwd");

                c_VInfoU.Save();
            }

            Definitions.ViewClass c_OMode = ds.View("modes");
            if (c_OMode.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_OMode.Caption = "Modes";

                //
                c_OMode.UseFields("openmode", "openmodechild");

                c_OMode.Save();
            }

            Definitions.ViewClass c_EMail = ds.View("email");
            if (c_EMail.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_EMail.Caption = "EMail";
                c_EMail.Selector = "EMAIL";

                //
                c_EMail.UseFields("emailname", "emailpwd", "emailprov", "footer");
                c_EMail["footer"].Height = "10";

                c_EMail.Save();
            }

            Definitions.ViewClass c_Twilio = ds.View("twilio");
            if (c_Twilio.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_Twilio.Caption = "Twilio";
                c_Twilio.Selector = "TWILIO";

                //
                c_Twilio.UseFields("twiliophone");

                c_Twilio.Save();
            }

            Definitions.ViewClass c_V8x8 = ds.View("vebye");
            if (c_V8x8.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_V8x8.Caption = "8x8";
                c_V8x8.Selector = "8X8";

                // Clear
                c_V8x8.ClearFields();

                // Map
                c_V8x8.UseFields("vc8by8jwt");

                c_V8x8.Save();
            }

            Definitions.ViewClass c_Pers = ds.View("personal");
            if (c_Pers.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_Pers.Caption = "Personal";

                //
                c_Pers.UseFields("dispname", "title", "phone", "signature");

                // Adjust height
                c_Pers["signature"].Height = "default.signatureHeight";

                c_Pers.Save();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // Make the tabs
                Definitions.ViewFieldClass c_Field = c_VDefault.AsTabs("tabs");
                c_Field.Height = "17";
                c_Field.Views = "info personal modes email twilio vebye";

                c_VDefault.Save();
            }

            // Add the UserSettings view
            Definitions.ViewClass c_VUSett = ds.View("_usersettings");
            if (c_VUSett.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VUSett.ClearFields();

                // Make the tabs
                Definitions.ViewFieldClass c_Field = c_VUSett.AsTabs("tabsu");
                c_Field.Height = "17";
                c_Field.Views = "infou personal modes email twilio";

                c_VUSett.Save();
            }
        }

        private static void Define_Cron(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.18a"))
            {
                //
                ds.Definition.Caption = ds.Definition.Caption.IfEmpty("CRON");
                ds.Definition.Placeholder = "#datesortable[next])# [name] - [fn]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "name";
                ds.Definition.Icon = ds.Definition.Icon.IfEmpty("user");
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "600";
                ds.Definition.Selector = "CRON";

                // Restart
                ds.Definition.ClearFields();

                // 
                Definitions.DatasetFieldClass c_Field = ds.Definition["name"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["patt"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Pattern";

                c_Field = ds.Definition["start"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Start On";

                c_Field = ds.Definition["next"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Next On";

                c_Field = ds.Definition["nextl"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Next";

                c_Field = ds.Definition["enabled"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Enabled";

                c_Field = ds.Definition["fn"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Function";

                c_Field = ds.Definition["siovalue"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TextArea;
                c_Field.Label = "Values";

                c_Field = ds.Definition["siocode"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "SIO Code";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_IOT(this DatasetClass ds, string caption, string icon)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.02.08b"))
            {
                //
                ds.Definition.Caption = ds.Definition.Caption.IfEmpty(caption);
                ds.Definition.Placeholder = "[name] [desc]";
                ds.Definition.Privileges = "av";
                ds.Definition.Icon = ds.Definition.Icon.IfEmpty(icon);

                ds.Definition.StartGroup = "IOT";
                ds.Definition.StartIndex = "100";
                ds.Definition.Selector = "IOT";

                // 
                Definitions.DatasetFieldClass c_Field = ds.Definition["name"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;

                c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TextArea;

                c_Field.SaveParent();

                ds.Definition.Save();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_Telemetry(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.04a"))
            {
                //
                ds.Definition.Caption = ds.Definition.Caption.IfEmpty("Telemetry");
                ds.Definition.Placeholder = "[_id]";
                ds.Definition.Privileges = "v";
                ds.Definition.Icon = ds.Definition.Icon.IfEmpty("asterisk_yellow");
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "700";
                ds.Definition.Selector = "TELEMETRY";

                // Restart
                ds.Definition.ClearFields();

                // 
                Definitions.DatasetFieldClass c_Field = ds.Definition["s"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Source";

                c_Field = ds.Definition["t"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Type";

                c_Field = ds.Definition["c"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Campaign";

                c_Field = ds.Definition["e"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Created On";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_TelemetryData(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.07a"))
            {
                //
                ds.Definition.Caption = ds.Definition.Caption.IfEmpty("Telemetry Data");
                ds.Definition.Placeholder = "[_id]";
                ds.Definition.Privileges = "v";
                ds.Definition.Icon = ds.Definition.Icon.IfEmpty("asterisk_orange");
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "701";
                ds.Definition.Selector = "TELEMETRY";

                // Restart
                ds.Definition.ClearFields();

                // 
                Definitions.DatasetFieldClass c_Field = ds.Definition["s"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Source";
                c_Field.UseIndex = true;

                c_Field = ds.Definition["t"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Type";

                c_Field = ds.Definition["c"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Campaign";
                c_Field.UseIndex = true;

                c_Field = ds.Definition["e"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Created On";

                c_Field = ds.Definition["x"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Target";
                c_Field.UseIndex = true;

                c_Field = ds.Definition["r"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Route";

                c_Field = ds.Definition["d"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "On";

                c_Field = ds.Definition["i"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "IP";

                // Must be done last
                ds.Definition.AnalyzerAllow = true;
                ds.Definition.AnalyzerBy = ds.Definition.FieldNames.JoinSpaces();

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();
                // They are all read only
                c_VDefault.Set("ro", true.ToDBBoolean());

                c_VDefault.Save();
            }
        }

        private static void Define_Generic(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged(Release.Version()))
            {
                //
                ds.Definition.Caption = ds.Definition.Caption.IfEmpty(WesternNameClass.CapEachWord(ds.Name));
                //ds.Definition.Placeholder = "[name]";
                ds.Definition.Placeholder = "[_id]";
                ds.Definition.Privileges = "av";
                ds.Definition.Icon = ds.Definition.Icon.IfEmpty("application_view_list");
                if (ds.Name.StartsWith("_")) ds.Definition.StartIndex = "hidden";

                // 
                //Definitions.DatasetFieldClass c_Field = ds.Definition["name"];
                //c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;

                //c_Field = ds.Definition["value"];
                //c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;

                //c_Field.SaveParent();

                ds.Definition.Save();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }
        #endregion

        #region Billing
        private static void Define_BillAccess(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.07d"))
            {
                //
                ds.Definition.Caption = "Account";
                ds.Definition.Placeholder = "[name] '-' [allowed] / #linkdscaption([actual])#:#linkdesc([actual])#";
                ds.Definition.Privileges = "av";
                ds.Definition.Icon = "group";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "101";
                ds.Definition.Selector = "ACCESS";

                ds.Definition.ChildDSs = new List<string>() { DatabaseClass.DatasetBiilSubscription, DatabaseClass.DatasetBiilCharge, DatabaseClass.DatasetBiilInvoice }.JoinSpaces();

                ds.Definition.RelatedDSs = DatabaseClass.DatasetTelemetryData + " x";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["name"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.XAccount;
                c_Field.UseIndex = true;

                c_Field = ds.Definition["pwd"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Password;
                c_Field.Label = "Password";

                c_Field = ds.Definition["allowed"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Allowed;
                c_Field.Label = "Allowed";

                c_Field = ds.Definition["childof"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Users;
                c_Field.Label = "Child of";

                c_Field = ds.Definition["billon"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Bill On";

                c_Field = ds.Definition["actual"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Link;
                c_Field.Label = "Actual Obj";

                c_Field = ds.Definition["lastin"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Last login";

                c_Field = ds.Definition["subscribedon"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Subscribed On";

                c_Field = ds.Definition["lastctcout"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Last ctc out";

                c_Field = ds.Definition["lastctcoutvia"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.AutoCaps;
                c_Field.Label = "Ctc out via";

                c_Field = ds.Definition["lastctcin"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Last ctc in";

                c_Field = ds.Definition["lastctcinvia"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.AutoCaps;
                c_Field.Label = "Ctc in via";

                c_Field = ds.Definition["lastctcinsource"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Ctc in source";

                c_Field = ds.Definition["lastctcincmp"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Ctc in cmp";

                c_Field = ds.Definition["lastctcoutsource"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Ctc out source";

                c_Field = ds.Definition["lastctcoutcmp"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Ctc out cmp";

                c_Field.SaveParent();
            }

            // Pages
            Definitions.ViewClass c_VInfo = ds.View("info");
            if (c_VInfo.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VInfo.Caption = "Info";

                //
                c_VInfo.UseFields("name", "pwd", "allowed", "lastin", "subscribedon", "childof");
                c_VInfo.Set("ro", true.ToDBBoolean(), "lastin", "subscribedon");

                c_VInfo.Save();
            }

            Definitions.ViewClass c_VOut = ds.View("out");
            if (c_VOut.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VOut.Caption = "Outbound";

                //
                c_VOut.UseFields("lastctcout", "lastctcoutsource", "lastctcoutvia", "lastctcoutcmp");
                c_VOut.Set("ro", true.ToDBBoolean());

                c_VOut.Save();
            }

            Definitions.ViewClass c_VIn = ds.View("in");
            if (c_VIn.ReleaseChanged(ds.Definition.Release))
            {
                //
                c_VIn.Caption = "Inbound";

                //
                c_VIn.UseFields("lastctcin", "lastctcinsource", "lastctcinvia", "lastctcincmp");
                c_VIn.Set("ro", true.ToDBBoolean());

                c_VIn.Save();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // Make the tabs
                Definitions.ViewFieldClass c_Field = c_VDefault.AsTabs("tabs");
                c_Field.Height = "10";
                c_Field.Views = "info out in";

                c_VDefault.Save();
            }

            // Add the account view
            Definitions.ViewClass c_VAcct = ds.View("_usersettings");
            if (c_VAcct.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VAcct.ClearFields();

                // Only the password
                c_VAcct.UseFields("pwd");

                c_VAcct.Save();
            }
        }

        private static void Define_BillRate(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.06a"))
            {
                //
                ds.Definition.Caption = "Bill. Rate";
                ds.Definition.Placeholder = "[code] '-' [desc]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "money";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "102";
                ds.Definition.Selector = "BILLING";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["rate"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Rate";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_BillCharge(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.06a"))
            {
                //
                ds.Definition.Caption = "Charges";
                ds.Definition.Placeholder = "[code] '-' [desc] @ #linkdesc([acct])#";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "money";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "hidden";
                ds.Definition.Selector = "BILLING";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["acct"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Link;
                c_Field.Label = "Account";
                c_Field.LinkDS = DatabaseClass.DatasetBillAccess;

                c_Field = ds.Definition["units"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Float;
                c_Field.Label = "Units";

                c_Field = ds.Definition["rate"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Rate";

                c_Field = ds.Definition["price"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Price";

                c_Field = ds.Definition["disc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Disc.";

                c_Field = ds.Definition["total"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Total";
                c_Field.Compute = "([rate]*[price])-[disc]";

                c_Field = ds.Definition["inv"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Link;
                c_Field.Label = "Invoice";
                c_Field.LinkDS = DatabaseClass.DatasetBiilInvoice;

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_BillSubscription(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.06a"))
            {
                //
                ds.Definition.Caption = "Subscriptions";
                ds.Definition.Placeholder = "[code] '-' [desc] @ #linkdesc([acct])#";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "money";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "hidden";
                ds.Definition.Selector = "BILLING";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.LU;
                c_Field.LinkDS = DatabaseClass.DatasetBiilRate;
                c_Field.LUMap = "???";

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["acct"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Link;
                c_Field.Label = "Account";
                c_Field.LinkDS = DatabaseClass.DatasetBillAccess;

                c_Field = ds.Definition["units"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Float;
                c_Field.Label = "Units";

                c_Field = ds.Definition["rate"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Rate";

                c_Field = ds.Definition["price"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Price";

                c_Field = ds.Definition["disc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Disc.";

                c_Field = ds.Definition["total"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Total";

                c_Field = ds.Definition["nexton"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Date;
                c_Field.Label = "Next On";
                c_Field.UseIndex = true;

                c_Field = ds.Definition["interval"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.ComboBox;
                c_Field.Label = "Interval";
                c_Field.Choices = "daily weekly monthly twomonths queartly yearly";
                c_Field.UseIndex = true;

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_BillInvoice(this DatasetClass ds)
        { // dataset into
            if (ds.Definition.ReleaseChanged("2021.04.06a"))
            {
                //
                ds.Definition.Caption = "Invoices";
                ds.Definition.Placeholder = "#datesortable([on])# '-' [billed] / [payment]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "money";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "hidden";
                ds.Definition.Selector = "BILLING";
                ds.Definition.SortOrder = "desc";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["on"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "On";
                c_Field.DefaultValue = "#now()#";

                c_Field = ds.Definition["acct"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Link;
                c_Field.Label = "Account";
                c_Field.LinkDS = DatabaseClass.DatasetBillAccess;

                c_Field = ds.Definition["billed"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Billed";

                c_Field = ds.Definition["payment"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Currency;
                c_Field.Label = "Payment";

                c_Field = ds.Definition["paidon"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "Paid On";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }
        #endregion

        #region Survey
        private static void Define_Quorum(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.18a"))
            {
                //
                ds.Definition.Caption = "Quorum";
                ds.Definition.Placeholder = "[_id] '-' [desc]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "group";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "500";
                ds.Definition.Selector = "QUORUM";

                ds.Definition.ChildDSs = DatabaseClass.DatasetQuorumTopic;

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["rtgmin"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Rtg.Min";

                c_Field = ds.Definition["rtgmax"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Rtg.Max";

                c_Field = ds.Definition["userratings"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Ratings";

                c_Field = ds.Definition["usercomments"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Comments";

                c_Field = ds.Definition["useroptions"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Options";

                c_Field = ds.Definition["userdocs"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Documents";

                c_Field = ds.Definition["acctratings"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Ratings";

                c_Field = ds.Definition["acctcomments"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Comments";

                c_Field = ds.Definition["acctoptions"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Options";

                c_Field = ds.Definition["acctdocs"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Documents";

                c_Field = ds.Definition["tabs"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Tabs;
                c_Field.GridView = "info user acct";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                Definitions.ViewFieldClass c_Field = c_VDefault["tabs"];
                c_Field.Height = "6";

                c_VDefault.Save();
            }

            Definitions.ViewClass c_VInfo = ds.View("info");
            if (c_VInfo.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VInfo.ClearFields();

                c_VInfo.Caption = "Info";

                c_VInfo.UseFields("code desc rtgmin rtgmax");

                c_VInfo.Save();
            }

            Definitions.ViewClass c_VUser = ds.View("user");
            if (c_VUser.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VUser.ClearFields();

                c_VUser.Caption = "User";

                c_VUser.UseFields("userratings usercomments useroptions userdocs");

                c_VUser.Save();
            }

            Definitions.ViewClass c_VAcct = ds.View("acct");
            if (c_VAcct.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VAcct.ClearFields();

                c_VAcct.Caption = "Account";

                c_VAcct.UseFields("acctratings acctcomments acctoptions acctdocs");

                c_VAcct.Save();
            }
        }

        private static void Define_QuorumTopic(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.14c"))
            {
                //
                ds.Definition.Caption = "Topic";
                ds.Definition.Placeholder = "#format([count],'{0:#####0}')# @ #format([rating],'{0:##0.0}')# '-' [desc]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "comment";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "hidden";
                ds.Definition.SortOrder = "desc";
                ds.Definition.Selector = "QUORUM";

                ds.Definition.ChildDSs = new List<string>() { DatabaseClass.DatasetQuorumComment, DatabaseClass.DatasetQuorumOption }.JoinSpaces();

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                c_Field.Label = "Description";

                c_Field = ds.Definition["comment"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TextArea;
                c_Field.Label = "Comment";

                c_Field = ds.Definition["defrange"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Boolean;
                c_Field.Label = "Use Q.Range.";
                c_Field.DefaultValue = "y";

                c_Field = ds.Definition["rtgmin"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Min";

                c_Field = ds.Definition["rtgmax"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Max";

                c_Field = ds.Definition["rtgavg"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Float;
                c_Field.Label = "Rating";

                c_Field = ds.Definition["rtgcount"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Rtg.Count";

                c_Field = ds.Definition["commentcount"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Int;
                c_Field.Label = "Comm.Count";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_QuorumRating(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.14c"))
            {
                //
                ds.Definition.Caption = "Response";
                ds.Definition.Placeholder = "#datesortable([on])# '-' [user]:[rating]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "chart_pie";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "hidden";
                ds.Definition.SortOrder = "desc";
                ds.Definition.Selector = "QUORUM";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["user"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.User;
                c_Field.Label = "By";

                c_Field = ds.Definition["rating"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Slider;
                c_Field.Label = "Rating";

                c_Field = ds.Definition["on"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "On";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }

        private static void Define_QuorumComment(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.14c"))
            {
                //
                ds.Definition.Caption = "Comment";
                ds.Definition.Placeholder = "#datesortable([on])# '-' [user]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "comment";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "hidden";
                ds.Definition.SortOrder = "desc";
                ds.Definition.Selector = "QUORUM";

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["user"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.User;
                c_Field.Label = "By";

                c_Field = ds.Definition["on"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.DateTime;
                c_Field.Label = "On";

                c_Field = ds.Definition["comment"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.TextArea;
                c_Field.Label = "Comment";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                //
                c_VDefault["comment"].Height = "5";

                c_VDefault.Save();
            }
        }

        private static void Define_QuorumOption(this DatasetClass ds)
        {
            // dataset into
            if (ds.Definition.ReleaseChanged("2021.03.14c"))
            {
                //
                ds.Definition.Caption = "Option";
                ds.Definition.Placeholder = "#datesortable([on])# '-' [user]";
                ds.Definition.Privileges = "av";
                ds.Definition.IDAlias = "code";
                ds.Definition.Icon = "chart_organization";
                ds.Definition.StartGroup = "System";
                ds.Definition.StartIndex = "hidden";
                ds.Definition.SortOrder = "desc";
                ds.Definition.Selector = "QUORUM";

                ds.Definition.ChildDSs = DatabaseClass.DatasetQuorumRating;

                ds.Definition.ClearFields();

                //  
                Definitions.DatasetFieldClass c_Field = ds.Definition["code"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.Keyword;

                c_Field = ds.Definition["desc"];
                c_Field.Type = Definitions.DatasetFieldClass.FieldTypes.AutoCaps;
                c_Field.Label = "Description";

                c_Field.SaveParent();
            }

            // Add the view
            Definitions.ViewClass c_VDefault = ds.View("default");
            if (c_VDefault.ReleaseChanged(ds.Definition.Release))
            {
                // Clear
                c_VDefault.ClearFields();

                // And from definition
                c_VDefault.FromFields();

                c_VDefault.Save();
            }
        }
        #endregion
    }
}
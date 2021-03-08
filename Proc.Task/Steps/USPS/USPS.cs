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
using System.Text;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;
using Proc.AO;
using Proc.Docs;

namespace Proc.Task
{
    public class USPSValidate : CommandClass
    {
        #region Constants
        private const string ArgAddress = "address";
        private const string ArgCity = "city";
        private const string ArgState = "state";
        private const string ArgZIP = "zip";
        #endregion

        #region Constructor
        public USPSValidate()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgAddress, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The address"));
                c_P.Add(ArgCity, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The city"));
                c_P.Add(ArgState, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The state"));
                c_P.Add(ArgZIP, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The zip"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.USPS, "Validates an address", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "usps.validate"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            using (USPSClass c_Eng = new USPSClass())
            {
                using (USPSClass.Address c_Addr = USPSClass.Address.FromData(args))
                {
                    using (USPSClass.Address c_Valid = c_Eng.ValidateAddress(c_Addr))
                    {
                        USPSClass.Address.ToData(ctx, args, c_Valid);
                    }
                }
            }

            return eAns;
        }
        #endregion
    }

    public class USPSGetZIP : CommandClass
    {
        #region Constants
        private const string ArgAddress = "address";
        private const string ArgCity = "city";
        private const string ArgState = "state";
        private const string ArgZIP = "zip";
        #endregion

        #region Constructor
        public USPSGetZIP()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgCity, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The city"));
                c_P.Add(ArgState, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The state"));
                c_P.Add(ArgZIP, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The zip"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.USPS, "Returns the ZIP for an address", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "usps.zip"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            using (USPSClass c_Eng = new USPSClass())
            {
                using (USPSClass.Address c_Addr = USPSClass.Address.FromData(args))
                {
                    using (USPSClass.Address c_Valid = c_Eng.GetZipcode(c_Addr))
                    {
                        USPSClass.Address.ToData(ctx, args, c_Valid);
                    }
                }
            }

            return eAns;
        }
        #endregion
    }

    public class USPSGetCS : CommandClass
    {
        #region Constants
        private const string ArgAddress = "address";
        private const string ArgCity = "city";
        private const string ArgState = "state";
        private const string ArgZIP = "zip";
        #endregion

        #region Constructor
        public USPSGetCS()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgAddress, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The address"));
                c_P.Add(ArgCity, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The city"));
                c_P.Add(ArgState, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The state"));
                c_P.Add(ArgZIP, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The zip"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.USPS, "Returns the city and state for an address", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "usps.citystate"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            using (USPSClass c_Eng = new USPSClass())
            {
                using (USPSClass.Address c_Addr = USPSClass.Address.FromData(args))
                {
                    using (USPSClass.Address c_Valid = c_Eng.GetCityState(c_Addr))
                    {
                        USPSClass.Address.ToData(ctx, args, c_Valid);
                    }
                }
            }

            return eAns;
        }
        #endregion
    }

    public class USPSClass : IDisposable
    {
        #region Constants
        private const string ProductionUrl = "http://production.shippingapis.com/ShippingAPI.dll";
        private const string TestingUrl = "http://testing.shippingapis.com/ShippingAPITest.dll";
        private const string UserID = "867CANDI7382";
        #endregion

        #region Constructor
        public USPSClass()
            : this(true)
        {
        }

        public USPSClass(bool testmode)
        {
            this.TestMode = testmode;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        private bool TestMode { get; set; }
        #endregion

        #region Address Methods
        /// <summary>
        /// Validate a single address
        /// </summary>
        /// <param name="address">Address object to be validated</param>
        /// <returns>Validated Address</returns>
        public Address ValidateAddress(Address address)
        {
            if (address == null)
            {
                address = new Address();
                address.LastError = "Missing address";
            }
            else
            {
                address.LastError = null;

                try
                {
                    string validateUrl = "?API=Verify&XML=<AddressValidateRequest USERID=\"{0}\"><Address ID=\"{1}\"><Address1>{2}</Address1><Address2>{3}</Address2><City>{4}</City><State>{5}</State><Zip5>{6}</Zip5><Zip4>{7}</Zip4></Address></AddressValidateRequest>";
                    string url = GetURL() + validateUrl;
                    url = System.String.Format(url, UserID, address.ID.ToString(), address.Address1, address.Address2, address.City, address.State, address.Zip, address.ZipPlus4);
                    string addressxml = url.URLGet();
                    if (addressxml.Contains("<Error>"))
                    {
                        int idx1 = addressxml.IndexOf("<Description>") + 13;
                        int idx2 = addressxml.IndexOf("</Description>");
                        int l = addressxml.Length;
                        string errDesc = addressxml.Substring(idx1, idx2 - idx1);

                        address.LastError = errDesc;
                    }
                    else
                    {
                        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                        doc.LoadXml(addressxml);

                        System.Xml.XmlNode element = doc.SelectSingleNode("/AddressValidateResponse/Address");
                        if (element != null)
                        {
                            this.LoadXml(element, address);
                        }
                    }
                }
                catch (Exception ex)
                {
                    address.LastError = "Error: " + ex.Message;
                }
            }

            return address;
        }

        private void LoadXml(System.Xml.XmlNode element, Address address)
        {
            System.Xml.XmlNode celement = element.SelectSingleNode("Address1");
            if (celement != null)
            {
                address.Address1 = celement.InnerText;
            }

            celement = element.SelectSingleNode("Address2");
            if (celement != null)
            {
                address.Address2 = celement.InnerText;
            }

            celement = element.SelectSingleNode("City");
            if (celement != null)
            {
                address.City = celement.InnerText;
            }

            celement = element.SelectSingleNode("State");
            if (celement != null)
            {
                address.State = celement.InnerText;
            }

            celement = element.SelectSingleNode("Zip5");
            if (celement != null)
            {
                address.Zip = celement.InnerText;
            }

            celement = element.SelectSingleNode("Zip4");
            if (celement != null)
            {
                address.ZipPlus4 = celement.InnerText;
            }

            if (string.IsNullOrEmpty(address.Address1))
            {
                address.Address1 = address.Address2;
                address.Address2 = "";
            }
            else if (!string.IsNullOrEmpty(address.Address2))
            {
                address.Address1 = address.Address2;
                address.Address2 = "";
            }

        }

        /// <summary>
        /// Get the zip code by providing an Address object with a city and state
        /// </summary>
        /// <param name="city">City</param>
        /// <param name="state">State</param>
        public Address GetZipcode(string city, string state)
        {
            Address address = new Address();
            address.City = city;
            address.State = state;

            if (address.Validation == null)
            {
                address = GetZipcode(address);
            }

            return address;
        }

        /// <summary>
        /// Get the zip code by providing an Address object with a city and state
        /// </summary>
        /// <param name="address">Address Object</param>
        /// <returns>Address Object</returns>
        public Address GetZipcode(Address address)
        {
            if (address == null)
            {
                address = new Address();
                address.LastError = "Missing address";
            }
            else
            {
                address.LastError = null;

                try
                {
                    //The address must contain a city and state
                    if (address.City == null || address.City.Length < 1 || address.State == null || address.State.Length < 1)
                    {
                        address.LastError = "You must supply a city and state for a zipcode lookup request.";
                    }
                    else
                    {
                        string zipcodeurl = "?API=ZipCodeLookup&XML=<ZipCodeLookupRequest USERID=\"{0}\"><Address ID=\"{1}\"><Address1>{2}</Address1><Address2>{3}</Address2><City>{4}</City><State>{5}</State></Address></ZipCodeLookupRequest>";
                        string url = GetURL() + zipcodeurl;
                        url = System.String.Format(url, UserID, address.ID.ToString(), address.Address1, address.Address2, address.City, address.State, address.Zip, address.ZipPlus4);
                        string addressxml = url.URLGet();
                        if (addressxml.Contains("<Error>"))
                        {
                            int idx1 = addressxml.IndexOf("<Description>") + 13;
                            int idx2 = addressxml.IndexOf("</Description>");
                            int l = addressxml.Length;
                            string errDesc = addressxml.Substring(idx1, idx2 - idx1);

                            address.LastError = errDesc;
                        }
                        else
                        {
                            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                            doc.LoadXml(addressxml);

                            System.Xml.XmlNode element = doc.SelectSingleNode("/ZipCodeLookupResponse/Address");
                            if (element != null)
                            {
                                this.LoadXml(element, address);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    address.LastError = "Error: " + ex.Message;
                }
            }

            return address;
        }

        /// <summary>
        /// Get the city and state by proving the zip code.
        /// </summary>
        /// <param name="zipcode">Zipcode</param>
        public Address GetCityState(string zipcode)
        {
            Address address = new Address();
            address.Zip = zipcode;

            if (address.Validation == null)
            {
                address = GetCityState(address);
            }

            return address;
        }

        /// <summary>
        /// Get the city and state by proving the zip code.
        /// </summary>
        /// <param name="address">Address object</param>
        /// <returns>Address Object</returns>
        public Address GetCityState(Address address)
        {
            if (address == null)
            {
                address = new Address();
                address.LastError = "Missing address";
            }
            else
            {
                address.LastError = null;

                try
                {
                    //The address must contain a city and state
                    if (address.Zip == null || address.Zip.Length < 1)
                    {
                        address.LastError = "You must supply a zipcode for a city/state lookup request.";
                    }
                    else
                    {
                        string citystateurl = "?API=CityStateLookup&XML=<CityStateLookupRequest USERID=\"{0}\"><ZipCode ID= \"{1}\"><Zip5>{2}</Zip5></ZipCode></CityStateLookupRequest>";
                        string url = GetURL() + citystateurl;
                        url = System.String.Format(url, UserID, address.ID.ToString(), address.Zip);
                        string addressxml = url.URLGet();
                        if (addressxml.Contains("<Error>"))
                        {
                            int idx1 = addressxml.IndexOf("<Description>") + 13;
                            int idx2 = addressxml.IndexOf("</Description>");
                            int l = addressxml.Length;
                            string errDesc = addressxml.Substring(idx1, idx2 - idx1);

                            address.LastError = errDesc;
                        }
                        else
                        {
                            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                            doc.LoadXml(addressxml);

                            System.Xml.XmlNode element = doc.SelectSingleNode("/CityStateLookupResponse/ZipCode");
                            if (element != null)
                            {
                                this.LoadXml(element, address);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    address.LastError = "Error: " + ex.Message;
                }
            }

            return address;
        }

        private string GetURL()
        {
            string url = ProductionUrl;
            if (TestMode)
                url = TestingUrl;
            return url;
        }
        #endregion

        public class Address : IDisposable
        {
            #region Constructor
            public Address()
            {
                this.ID = 1;
                this.FirmName = "";
                this.Contact = "";
                this.ContactEmail = "";
                this.Address1 = "";
                this.Address2 = "";
                this.City = "";
                this.State = "";
                this.Zip = "";
                this.ZipPlus4 = "";
            }
            #endregion

            #region IDisposable
            public void Dispose()
            {
            }
            #endregion

            #region Properties
            /// <summary>
            /// ID of this address
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// The last error while processing
            /// </summary>
            public string LastError { get; set; }

            /// <summary>
            /// Returns null if it is valid, or a list of errors
            /// </summary>
            public List<string> Validation
            {
                get
                {
                    List<string> c_Ans = new List<string>();

                    //
                    if (this.Address1.Length > 38) c_Ans.Add("Address1 is is limited to a maximum of 38 characters.");
                    if (this.Address2.Length > 38) c_Ans.Add("Address2 is is limited to a maximum of 38 characters.");
                    if (this.City.Length > 15) c_Ans.Add("City is is limited to a maximum of 15 characters.");
                    if (this.State.Length > 2) c_Ans.Add("State is is limited to a maximum of 2 characters.");
                    if (this.Zip.Length > 5) c_Ans.Add("Zip is is limited to a maximum of 5 characters.");
                    if (this.ZipPlus4.Length > 4) c_Ans.Add("ZipPlus4 is is limited to a maximum of 4 characters.");
                    //
                    if (!string.IsNullOrEmpty(this.LastError)) c_Ans.Add(this.LastError);

                    if (c_Ans.Count == 0) c_Ans = null;

                    return c_Ans;
                }
            }

            /// <summary>
            /// Name of the Firm or Company
            /// </summary>
            public string FirmName { get; set; }

            /// <summary>
            /// The contact is used to send confirmation when a package is shipped
            /// </summary>
            public string Contact { get; set; }

            /// <summary>
            /// The contacts email address is used to send delivery confirmation
            /// </summary>
            public string ContactEmail { get; set; }

            /// <summary>
            /// Address Line 1 is used to provide an apartment or suite
            /// number, if applicable. Maximum characters allowed: 38
            /// </summary>
            public string Address1 { get; set; }

            /// <summary>
            /// Street address
            /// Maximum characters allowed: 38
            /// </summary>
            public string Address2 { get; set; }

            /// <summary>
            /// City
            /// Either the City and State or Zip are required.
            /// Maximum characters allowed: 15
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// State
            /// Either the City and State or Zip are required.
            /// Maximum characters allowed = 2
            /// </summary>
            public string State { get; set; }

            /// <summary>
            /// Zipcode
            /// Maximum characters allowed = 5
            /// </summary>
            public string Zip { get; set; }

            /// <summary>
            /// Zip code extension
            /// Maximum characters allowed = 4
            /// </summary>
            public string ZipPlus4 { get; set; }

            public string AsString
            {
                get { return "{0}, {1}, {2} {3}".FormatString(this.Address1, this.City, this.State, this.Zip); }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Get the Xml representation of this address object
            /// </summary>
            /// <returns>String</returns>
            public string ToXml()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<Address ID=\"" + this.ID.ToString() + "\">");
                sb.Append("<Address1>" + this.Address1 + "</Address1>");
                sb.Append("<Address2>" + this.Address2 + "</Address2>");
                sb.Append("<City>" + this.City + "</City>");
                sb.Append("<State>" + this.State + "</State>");
                sb.Append("<Zip5>" + this.Zip + "</Zip5>");
                sb.Append("<Zip4>" + this.ZipPlus4 + "</Zip4>");
                sb.Append("</Address>");
                return sb.ToString();
            }
            #endregion

            #region Statics
            public static Address FromString(string address)
            {
                Address c_Ans = new Address();

                try
                {
                    using (AddressParser c_Eng = new AddressParser())
                    {
                        AddressParseResult c_Wkg = c_Eng.ParseAddress(address);

                        c_Ans.Address1 = c_Wkg.StreetLine;
                        c_Ans.City = c_Wkg.City;
                        c_Ans.State = c_Wkg.State;
                        c_Ans.Zip = c_Wkg.Zip;

                        // Fixes
                        using (USPSClass c_USPS = new USPSClass())
                        {
                            c_Ans = c_USPS.ValidateAddress(c_Ans);
                        }
                    }
                }
                catch { }

                return c_Ans;
            }

            public static Address FromData(ArgsClass args)
            {
                Address c_Addr = new Address();

                c_Addr.Address1 = args.Get("address");
                c_Addr.City = args.Get("city");
                c_Addr.State = args.Get("state");
                c_Addr.Zip = args.Get("zip");

                return c_Addr;
            }

            public static void ToData(TaskContextClass ctx, ArgsClass args, Address addr)
            {
                if (addr != null)
                {
                    Set(ctx, args, "address", addr.Address1);
                    Set(ctx, args, "city", addr.City);
                    Set(ctx, args, "state", addr.State);
                    Set(ctx, args, "zip", addr.Zip);
                }
            }

            private static void Set(TaskContextClass ctx, ArgsClass args, string key, string value)
            {
                string sField = args.GetRaw(key);
                if (sField.HasValue()) ctx[sField] = value;
            }

            //////////////////////////////////////////////////////////////////////////
            // FromXML medthod provided by viperguynaz via codeproject
            //////////////////////////////////////////////////////////////////////////
            #endregion
        }

        public class AddressObject
        {
            #region Properties
            public string Address { get; set; }
            public string Suite { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZIP { get; set; }
            public string Plus4 { get; set; }
            #endregion
        }
    }
}
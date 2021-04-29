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

using Newtonsoft.Json.Linq;

using NX.Shared;

namespace Proc.AO
{
    public class SiteInfoClass : ChildOfClass<DatabaseClass>
    {
        #region Constructor
        public SiteInfoClass(DatabaseClass ds)
            : base(ds)
        {
            // Get
            this.SynchObject = this.Parent[DatabaseClass.DatasetSys]["_info"];

            //
            this.SynchObject.Volatile(delegate (string fld, string value)
            {
                //
                this.UpdateEnv(true);
            });

            // Save
            this.DomainShadow = this.Domain;
            this.CertEMailShadow = this.CertEMail;
            this.ProcessorCountShadow = this.ProcessorCount;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The underlying object
        /// 
        /// </summary>
        public ObjectClass SynchObject { get; private set; }

        /// <summary>
        /// 
        /// The office name
        /// 
        /// </summary>
        public string Name
        {
            get { return this.SynchObject["name"]; }
            set { this.SynchObject["name"] = value; }
        }

        /// <summary>
        /// 
        /// The address
        /// 
        /// </summary>
        public string Address1
        {
            get { return this.SynchObject["addr1"]; }
            set { this.SynchObject["addr1"] = value; }
        }

        /// <summary>
        /// 
        /// The address second line
        /// 
        /// </summary>
        public string Address2
        {
            get { return this.SynchObject["addr2"]; }
            set { this.SynchObject["addr2"] = value; }
        }

        /// <summary>
        /// 
        /// The city
        /// 
        /// </summary>
        public string City
        {
            get { return this.SynchObject["city"]; }
            set { this.SynchObject["city"] = value; }
        }

        /// <summary>
        /// 
        /// The state
        /// 
        /// </summary>
        public string State
        {
            get { return this.SynchObject["state"]; }
            set { this.SynchObject["state"] = value; }
        }

        /// <summary>
        /// 
        /// The ZIP code
        /// 
        /// </summary>
        public string ZIP
        {
            get { return this.SynchObject["zip"]; }
            set { this.SynchObject["zip"] = value; }
        }

        /// <summary>
        /// 
        /// The phone
        /// 
        /// </summary>
        public string Phone
        {
            get { return this.SynchObject["phone"]; }
            set { this.SynchObject["phone"] = value; }
        }

        /// <summary>
        /// 
        /// The fax
        /// 
        /// </summary>
        public string Fax
        {
            get { return this.SynchObject["fax"]; }
            set { this.SynchObject["fax"] = value; }
        }

        /// <summary>
        /// 
        /// The site domain
        /// 
        /// </summary>
        public string Domain
        {
            get { return this.SynchObject["domain"].IfEmpty(); }
            set
            {
                // Save
                if (this.Domain != value)
                {
                    //
                    this.SynchObject["domain"] = value;
                    this.UpdateEnv(true);
                }
            }
        }
        private string DomainShadow { get; set; }

        /// <summary>
        /// 
        /// The site SSL email
        /// 
        /// </summary>
        public string CertEMail
        {
            get { return this.SynchObject["certemail"].IfEmpty(); }
            set
            {
                // Save
                if (this.CertEMail != value)
                {
                    this.SynchObject["certemail"] = value;
                    this.UpdateEnv(true);
                }
            }
        }
        private string CertEMailShadow { get; set; }

        /// <summary>
        /// 
        /// The number of processors
        /// 
        /// </summary>
        public int ProcessorCount
        {
            get { return this.SynchObject["proccount"].ToInteger(0).Min(1); }
            set
            {
                // Assure at least one
                if (value < 1) value = 1;

                // Save
                if (this.ProcessorCount != value)
                {
                    //
                    this.SynchObject["proccount"] = value.ToString();
                    this.UpdateEnv(false);
                }
            }
        }
        private int ProcessorCountShadow { get; set; }

        /// <summary>
        /// 
        /// Returns the information as JSON object
        /// 
        /// </summary>
        public JObject AsJObject
        {
            get { return this.SynchObject.AsJObject; }
        }

        /// <summary>
        /// 
        /// The time that the office opens
        /// 
        /// </summary>
        public string OfficeOpens
        {
            get { return this.SynchObject["officeopen"]; }
            set { this.SynchObject["officeopen"] = value; }
        }

        /// <summary>
        /// 
        /// The time that the office closes
        /// 
        /// </summary>
        public string OfficeCloses
        {
            get { return this.SynchObject["officeclose"]; }
            set { this.SynchObject["officeclose"] = value; }
        }

        /// <summary>
        /// 
        /// The timezone
        /// 
        /// </summary>
        public string Timezone
        {
            get { return this.SynchObject["timezone"]; }
            set { this.SynchObject["timezone"] = value; }
        }

        /// <summary>
        /// 
        /// Is IOT enabled?
        /// 
        /// </summary>
        public bool IOTEnabled
        {
            get { return this.SynchObject["iotenabled"].FromDBBoolean(); }
            set { this.SynchObject["iotenabled"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// Is Time track enabled?
        /// 
        /// </summary>
        public bool TTEnabled
        {
            get { return this.SynchObject["ttenabled"].FromDBBoolean(); }
            set { this.SynchObject["ttenabled"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// Are accounts enabled?
        /// 
        /// </summary>
        public bool AccountsEnabled
        {
            get { return this.SynchObject["acctenabled"].FromDBBoolean(); }
            set { this.SynchObject["acctenabled"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The tax ID
        /// 
        /// </summary>
        public string TaxID
        {
            get { return this.SynchObject["taxid"]; }
            set { this.SynchObject["taxid"] = value; }
        }

        /// <summary>
        /// 
        /// The tax rate
        /// 
        /// </summary>
        public double TaxRate
        {
            get { return this.SynchObject["taxrate"].IfEmpty().ToDouble(0); }
            set { this.SynchObject["taxrate"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// The number of days of bitly retention
        /// 
        /// </summary>
        public int BitlyDays
        {
            get { return this.SynchObject["bitly"].ToInteger(0); }
            set { this.SynchObject["bitly"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// Update site?
        /// 
        /// </summary>
        public bool AutoUpdate
        {
            get { return this.SynchObject["autoupd"].FromDBBoolean(); }
            set { this.SynchObject["autoupd"] = value.ToDBBoolean(); }
        }
        #endregion

        #region Indexer
        public string this[string key]
        {
            get { return this.SynchObject[key]; }
        }
        #endregion

        #region Internal
        /// <summary>
        /// 
        /// The starting page for help
        /// 
        /// </summary>
        public string HelpRoot
        {
            get { return this.SynchObject["helproot"]; }
            set { this.SynchObject["helproot"] = value; }
        }

        /// <summary>
        /// 
        /// The theme file
        /// 
        /// </summary>
        public string ThemeOptions
        {
            get { return this.SynchObject["themeoptions"]; }
            set { this.SynchObject["themeoptions"] = value; }
        }

        /// <summary>
        /// 
        /// Default allowed for accounts
        /// 
        /// </summary>
        public string AccountDefaultAllowed
        {
            get { return this.SynchObject["acctdefallowed"].IfEmpty(); }
            set { this.SynchObject["acctdefallowed"] = value; }
        }
        #endregion

        #region Telemetry
        /// <summary>
        /// 
        /// Is telemetry enabled?
        /// 
        /// </summary>
        public bool TelemetryEnabled
        {
            get { return this.SynchObject["teleenabled"].FromDBBoolean(); }
            set { this.SynchObject["teleenabled"] = value.ToDBBoolean(); }
        }
        #endregion

        #region Twilio
        /// <summary>
        /// 
        /// The Twilio account
        /// 
        /// </summary>
        public string TwilioAccount
        {
            get { return this.SynchObject["twilioacct"]; }
            set { this.SynchObject["twilioacct"] = value; }
        }

        /// <summary>
        /// 
        /// The Twilio token
        /// 
        /// </summary>
        public string TwilioToken
        {
            get { return this.SynchObject["twiliotoken"]; }
            set { this.SynchObject["twiliotoken"] = value; }
        }

        /// <summary>
        /// 
        /// The Twilio phone number
        /// 
        /// </summary>
        public string TwilioPhone
        {
            get { return this.SynchObject["twiliophone"]; }
            set { this.SynchObject["twiliophone"] = value; }
        }

        /// <summary>
        /// 
        /// The task to run on incoming
        /// 
        /// </summary>
        public string TwilioTask
        {
            get { return this.SynchObject["twiliotask"]; }
            set { this.SynchObject["twiliotask"] = value; }
        }

        /// <summary>
        /// 
        /// The SendGrid API key
        /// 
        /// </summary>
        public string SendGridAPI
        {
            get { return this.SynchObject["sendgridapi"]; }
            set { this.SynchObject["sendgridapi"] = value; }
        }

        /// <summary>
        /// 
        /// The SendGrid email
        /// 
        /// </summary>
        public string SendGridEmail
        {
            get { return this.SynchObject["sendgridemail"]; }
            set { this.SynchObject["sendgridemail"] = value; }
        }

        /// <summary>
        /// 
        /// The SendGrid email
        /// 
        /// </summary>
        public string SendGridFriendlyName
        {
            get { return this.SynchObject["sendgridfriendly"]; }
            set { this.SynchObject["sendgridfriendly"] = value; }
        }
        #endregion

        #region User defined
        /// <summary>
        /// 
        /// The user defined entry #1
        /// 
        /// </summary>
        public string UDF1
        {
            get { return this.SynchObject["udf1"]; }
            set { this.SynchObject["udf1"] = value; }
        }

        /// <summary>
        /// 
        /// The user defined entry #2
        /// 
        /// </summary>
        public string UDF2
        {
            get { return this.SynchObject["udf2"]; }
            set { this.SynchObject["udf2"] = value; }
        }

        /// <summary>
        /// 
        /// The user defined entry #3
        /// 
        /// </summary>
        public string UDF3
        {
            get { return this.SynchObject["udf3"]; }
            set { this.SynchObject["udf3"] = value; }
        }
        #endregion

        #region Social media
        public string FacebookURL
        {
            get { return this.SynchObject["fburl"]; }
            set { this.SynchObject["fburl"] = value; }
        }

        public string LinkedInURL
        {
            get { return this.SynchObject["liurl"]; }
            set { this.SynchObject["liurl"] = value; }
        }

        public string TwitterURL
        {
            get { return this.SynchObject["twitterurl"]; }
            set { this.SynchObject["twitterurl"] = value; }
        }

        public string TwitterConsumerKey
        {
            get { return this.SynchObject["twitterck"]; }
            set { this.SynchObject["twitterck"] = value; }
        }

        public string TwitterSecretKey
        {
            get { return this.SynchObject["twittersk"]; }
            set { this.SynchObject["twittersk"] = value; }
        }

        public string TwitterAccessToken
        {
            get { return this.SynchObject["twitterat"]; }
            set { this.SynchObject["twitterat"] = value; }
        }

        public string TwitterAccessTokenSecret
        {
            get { return this.SynchObject["twitterats"]; }
            set { this.SynchObject["twitterats"] = value; }
        }
        #endregion

        #region PositionStack
        public string PSAPIKey
        {
            get { return this.SynchObject["psapi"]; }
            set { this.SynchObject["psapi"] = value; }
        }
        #endregion

        #region Billing
        /// <summary>
        /// 
        /// Is billing enabled?
        /// 
        /// </summary>
        public bool BillingEnabled
        {
            get { return this.SynchObject["billenabled"].FromDBBoolean(); }
            set { this.SynchObject["billenabled"] = value.ToDBBoolean(); }
        }

        /// <summary>
        /// 
        /// The Stripe public key
        /// 
        /// </summary>
        public string StripePublic
        {
            get { return this.SynchObject["stripepub"]; }
            set { this.SynchObject["stripepub"] = value; }
        }

        /// <summary>
        /// 
        /// The Stripe private key
        /// 
        /// </summary>
        public string StripeSecurity
        {
            get { return this.SynchObject["stripesec"]; }
            set { this.SynchObject["stripesec"] = value; }
        }

        /// <summary>
        /// 
        /// The template to use for a payment request
        /// 
        /// </summary>
        public string PayReqSubject
        {
            get { return this.SynchObject["billinvrsubj"]; }
            set { this.SynchObject["billinvrsubj"] = value; }
        }

        /// <summary>
        /// 
        /// The template to use for a payment request
        /// 
        /// </summary>
        public string PayReqMessage
        {
            get { return this.SynchObject["billinvrmsg"]; }
            set { this.SynchObject["billinvrmsg"] = value; }
        }

        /// <summary>
        /// 
        /// The template to use for a payment request
        /// 
        /// </summary>
        public string PayReqTemplate
        {
            get { return this.SynchObject["billinvrtemplate"]; }
            set { this.SynchObject["billinrvtemplate"] = value; }
        }

        /// <summary>
        /// 
        /// The day of the month when invoicing takes place
        /// 
        /// </summary>
        public int InvoiceDOM
        {
            get { return this.SynchObject["billdom"].IfEmpty().ToInteger(0); }
            set { this.SynchObject["billdom"] = value.ToString(); }
        }
        #endregion

        #region Quorums
        /// <summary>
        /// 
        /// Is billing enabled?
        /// 
        /// </summary>
        public bool QuorumEnabled
        {
            get { return this.SynchObject["quorumenabled"].FromDBBoolean(); }
            set { this.SynchObject["quorumenabled"] = value.ToDBBoolean(); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Sets a value using the field name
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, string value)
        {
            //
            this.SynchObject[key] = value;
        }

        /// <summary>
        /// 
        /// Converts information to string
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.SynchObject.AsJObject.ToSimpleString();
        }

        /// <summary>
        /// 
        /// Saves information
        /// 
        /// </summary>
        public void Save()
        {
            // Save
            this.SynchObject.Save();
        }

        /// <summary>
        /// 
        /// handles updating environment
        /// 
        /// </summary>
        /// <param name="recyclenginx"></param>
        public void UpdateEnv(bool recyclenginx, bool force=false)
        {
            //
            bool bChanged = false;
            bool bNChanged = false;

            // To environment
            if (force || !this.Domain.IsSameValue(this.DomainShadow))
            {
                this.DomainShadow = this.Domain;
                this.Parent.Parent.Parent.Domain = this.Domain;
                this.Parent.Parent.Parent.LogInfo("Domain set to {0}".FormatString(this.Domain));
                bChanged = true;
                bNChanged = true;
            }

            if (force || !this.CertEMail.IsSameValue(this.CertEMailShadow))
            {
                this.CertEMailShadow = this.CertEMail;
                this.Parent.Parent.Parent["certbot_email"] = this.CertEMail;
                this.Parent.Parent.Parent.LogInfo("Cert EMail set to {0}".FormatString(this.CertEMail));
                bChanged = true;
                bNChanged = true;
            }

            if (force || this.ProcessorCount != this.ProcessorCountShadow)
            {
                this.ProcessorCountShadow = this.ProcessorCount;
                this.Parent.Parent.Parent["qd_worker"] = this.ProcessorCount.ToString();
                this.Parent.Parent.Parent.LogInfo("Processor count set to {0}".FormatString(this.ProcessorCount));
                bChanged = true;
            }

            // Save
            if (bChanged) this.SynchObject.Save();

            // Recycle NginX
            if (force || (recyclenginx && bNChanged))
            {
                // Remove old NginX
                this.Parent.Parent.Parent.Globals.Remove<Proc.NginX.ManagerClass>();

                // And remake
                Proc.NginX.ManagerClass c_Mgr = this.Parent.Parent.Parent.Globals.Get<Proc.NginX.ManagerClass>();
                c_Mgr.MakeConfig(true);
            }

            // Relink twilio
            if(this.TwilioPhone.HasValue())
            {
                StoreClass c_Params = new StoreClass();
                c_Params["phone"] = this.TwilioPhone;
                c_Params["ref"] = this.SynchObject.UUID.ToString();

                this.Parent.Parent.Parent.FN("Communication.TwilioRegister", c_Params);
            }

            // Create enabled datsets 
            this.Parent.Initialize(true);
        }

        /// <summary>
        /// 
        /// Reload from database
        /// 
        /// </summary>
        public void Reload()
        {
            //
            this.SynchObject.Load();
        }
        #endregion
    }
}
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    public class WalletClass : ChildOfClass<AO.DatabaseClass>
    {
        #region Constructor
        public WalletClass(AO.ObjectClass obj)
            : this(obj.Dataset.Parent, obj.ObjectDataset, obj.ObjectID)
        { }

        public WalletClass(AO.Definitions.UserClass assoc)
            : this(assoc.Parent)
        { }

        public WalletClass(AO.DatabaseClass db, string ds, string id)
            : base(db)
        {
            //
            this.ObjDataset = ds;
            this.ObjID = id;
            this.HashID = (this.ObjDataset + ":" + this.ObjID).MD5HashString();

            // Get
            this.Values = this.Parent[DatabaseClass.DatasetWallet][this.HashID];

            //
            this.ICreditCards = new WalletSectionClass(this, "cc");
            this.ISSHKeys = new WalletSectionClass(this, "ssh");
        }
        #endregion

        #region Properties
        public AO.Definitions.UserClass Associate { get { return this.Root as AO.Definitions.UserClass; } }

        private string HashID { get; set; }
        private string ObjDataset { get; set; }
        private string ObjID { get; set; }

        internal AO.ObjectClass Values { get; set; }
        #endregion

        #region Credit Cards
        /// <summary>
        /// 
        /// Holder of the credit cards
        /// 
        /// </summary>
        private WalletSectionClass ICreditCards { get; set; }

        /// <summary>
        /// 
        /// Returns a list of credit card indexes
        /// </summary>
        public List<int> CreditCards
        {
            get { return this.ICreditCards.IDs; }
        }

        /// <summary>
        /// 
        /// Returns a credit card from an index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CreditCardClass GetCreditCard(int index)
        {
            return this.ICreditCards[index] as CreditCardClass;
        }

        /// <summary>
        /// 
        /// Get all the creadit cards that match the user ID
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<CreditCardClass> GetCreditCards(string uid)
        {
            List<CreditCardClass> c_Ans = new List<CreditCardClass>();

            // Get
            List<WalletEntryClass> c_List = this.ICreditCards.GetEntries(uid);
            // Loop thru
            foreach (WalletEntryClass c_Entry in c_List) c_Ans.Add(c_Entry as CreditCardClass);

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Get the first entry for a given user ID
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public CreditCardClass GetCreditCard(string uid)
        {
            return this.ICreditCards.GetEntry(uid) as CreditCardClass;
        }
        #endregion

        #region SSH Keys
        /// <summary>
        /// 
        /// Holder of the SSH keys
        /// 
        /// </summary>
        private WalletSectionClass ISSHKeys { get; set; }

        /// <summary>
        /// 
        /// Returns an SSH Key from an index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SSHKeyClass GetSSHKey(int index)
        {
            return this.ISSHKeys[index] as SSHKeyClass;
        }

        /// <summary>
        /// 
        /// Get all the SSH keys that match the user ID
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<SSHKeyClass> GetSSHKeys(string uid)
        {
            List<SSHKeyClass> c_Ans = new List<SSHKeyClass>();

            // Get
            List<WalletEntryClass> c_List = this.ISSHKeys.GetEntries(uid);
            // Loop thru
            foreach (WalletEntryClass c_Entry in c_List) c_Ans.Add(c_Entry as SSHKeyClass);

            return c_Ans;
        }

        /// <summary>
        /// 
        /// Get the first entry for a given user ID
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public SSHKeyClass GetSSHKey(string uid)
        {
            return this.ISSHKeys.GetEntry(uid)as SSHKeyClass;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// The password used to encode an entry
        /// 
        /// </summary>
        /// <param name="salt"></param>
        /// <returns></returns>
        internal string Passcode(string salt)
        {
            return this.HashID.EncodeString(salt).MD5HashString();
        }

        /// <summary>
        /// 
        /// Save the wallet
        /// 
        /// </summary>
        internal void Save()
        {
            this.Values.Save();
        }
        #endregion
    }

    public class WalletSectionClass : ChildOfClass<WalletClass>
    {
        #region Constructor
        internal WalletSectionClass(WalletClass wallet, string section)
            : base(wallet)
        {
            //
            this.Section = section;
        }
        #endregion

        #region Indexer
        public WalletEntryClass this[int index]
        {
            get
            {
                return new WalletEntryClass(this, index);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The field prefix
        /// 
        /// </summary>
        public string Section { get; private set; }

        public List<int> IDs
        {
            get
            {
                List<int> c_Ans = new List<int>();

                foreach (string sFld in this.Parent.Values.Fields)
                {
                    if (sFld.StartsWith(this.Section))
                    {
                        c_Ans.Add(sFld.Substring(this.Section.Length).ToInteger());
                    }
                }

                return c_Ans;
            }
        }
        #endregion

        #region Methods
        public List<WalletEntryClass> GetEntries(string uid)
        {
            List<WalletEntryClass> c_Ans = new List<WalletEntryClass>();

            foreach(int iIndex in this.IDs)
            {
                WalletEntryClass c_Poss = new WalletEntryClass(this, iIndex);

                if (uid.IsSameValue(c_Poss.ID))
                {
                    c_Ans.Add(c_Poss);
                }
                else
                {
                    c_Poss.Dispose();
                }
            }

            return c_Ans;
        }

        public WalletEntryClass GetEntry(string uid)
        {
            WalletEntryClass c_Ans = null;

            foreach (int iIndex in this.IDs)
            {
                WalletEntryClass c_Poss = new WalletEntryClass(this, iIndex);

                if (uid.IsSameValue(c_Poss.ID))
                {
                    c_Ans = c_Poss;
                    break;
                }
                else
                {
                    c_Poss.Dispose();
                }
            }

            return c_Ans;
        }
        #endregion
    }

    public class WalletEntryClass : ChildOfClass<WalletSectionClass>
    {
        #region Constants
        public const string KeyID = "uid";
        #endregion

        #region Constructor
        public WalletEntryClass(WalletSectionClass section, int instance)
            : base(section)
        {
            //
            this.Instance = instance;

            this.Values = this.Parent.Parent.Values[this.Parent.Section + this.Instance].DecodeFromBase64(this.Parent.Parent.Passcode(this.Parent.Section)).ToJObject();
            if (this.Values == null) this.Values = new JObject();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The instance
        /// 
        /// </summary>
        private int Instance { get; set; }

        /// <summary>
        /// 
        /// The decoded values
        /// 
        /// </summary>
        protected JObject Values { get; set; }

        /// <summary>
        /// 
        /// User identifier
        /// 
        /// </summary>
        public string ID
        {
            get { return this.Values.Get(KeyID); }
            set { this.Values.Set(KeyID, value); }
        }
        #endregion

        #region Methods
        public void Save()
        {
            // Push encoded value back
            this.Parent.Parent.Values[this.Parent.Section + this.Instance] = this.Values.ToSimpleString().EncodeToBase64(this.Parent.Parent.Passcode(this.Parent.Section));
            // Save
            this.Parent.Parent.Save();
        }
        #endregion
    }

    public class CreditCardClass : WalletEntryClass
    {
        #region Constants
        public const string KeyNumber = "no";
        public const string KeyExp = "exp";
        public const string KeyName = "name";
        public const string KeyAddress = "addr";
        public const string KeyCity = "city";
        public const string KeyState = "state";
        public const string KeyZIP = "zip";
        public const string KeyPhone = "phone";
        public const string KeyCVC = "cvc";
        public const string KeyAutoPay = "autopay";
        #endregion

        #region Constructor
        internal CreditCardClass(WalletSectionClass section, int index)
            : base(section, index)
        { }
        #endregion

        #region Properties
        public string Number
        {
            get { return this.Values.Get(KeyNumber); }
            set
            {
                if (value.IsNum())
                {
                    this.Values.Set(KeyNumber, value);
                }
            }
        }

        public string Expiration
        {
            get { return this.Values.Get(KeyExp); }
            set
            {
                if (value.HasValue())
                {
                    List<string> c_Wkg = Regex.Replace(value, @"[^\d]", " ").Trim().SplitSpaces();
                    if (c_Wkg.Count < 2) c_Wkg.Add("0");

                    value = "{0:00}/{1:00}".FormatString(c_Wkg[0].ToInteger(0), c_Wkg[1].ToInteger(0));
                }
                this.Values.Set(KeyExp, value);
            }
        }

        public string Name
        {
            get { return this.Values.Get(KeyName); }
            set { this.Values.Set(KeyName, value.ToUpper()); }
        }

        public string Address
        {
            get { return this.Values.Get(KeyAddress); }
            set { this.Values.Set(KeyAddress, value.ToUpper()); }
        }

        public string City
        {
            get { return this.Values.Get(KeyCity); }
            set { this.Values.Set(KeyCity, value.ToUpper()); }
        }

        public string State
        {
            get { return this.Values.Get(KeyState); }
            set { this.Values.Set(KeyState, value.ToUpper()); }
        }

        public string ZIP
        {
            get { return this.Values.Get(KeyZIP); }
            set { this.Values.Set(KeyName, "{0:00000}".FormatString(value.ToInteger(0))); }
        }

        public string Phone
        {
            get { return this.Values.Get(KeyPhone); }
            set { this.Values.Set(KeyPhone, value.ToPhone()); }
        }

        public string CVC
        {
            get { return this.Values.Get(KeyCVC); }
            set { this.Values.Set(KeyCVC, "{0:000}".FormatString(value.ToInteger(0))); }
        }

        public bool AutoPay
        {
            get { return this.Values.Get(KeyAutoPay).FromDBBoolean(); }
            set
            {
                this.Values.Set(KeyAutoPay, value.ToDBBoolean());
            }
        }
        #endregion
    }

    public class SSHKeyClass : WalletEntryClass
    {
        #region Constants
        public const string KeyPublic = "public";
        public const string KeyPrivate = "private";
        public const string KeySSH = "ssh";
        public const string KeyPassword = "pwd";
        public const string KeyCreatedOn = "creon";
        #endregion

        #region Constructor
        internal SSHKeyClass(WalletSectionClass section, int index)
            : base(section, index)
        { }
        #endregion

        #region Properties
        public string Public
        {
            get { return this.Values.Get(KeyPublic); }
            set { this.Values.Set(KeyPublic, value); }
        }

        public string Private
        {
            get { return this.Values.Get(KeyPrivate); }
            set { this.Values.Set(KeyPrivate, value); }
        }

        public string SSH
        {
            get { return this.Values.Get(KeySSH); }
            set { this.Values.Set(KeySSH, value); }
        }

        public string Password
        {
            get { return this.Values.Get(KeyPassword).IfEmpty(); }
            set { this.Values.Set(KeyPassword, value); }
        }

        public DateTime CreatedOn
        {
            get { return this.Values.Get(KeyCreatedOn).FromDBDate(); }
            set { this.Values.Set(KeyCreatedOn, value.ToDBDate()); }
        }

        public PGPKeysClass AsPGPKey
        {
            get
            {
                return new PGPKeysClass(
                                          new PGPKeyClass(this.Public),
                                          new PGPKeyClass(this.Private),
                                          this.Password);
            }
        }
        #endregion
    }

    public class PGPKeysClass : IDisposable
    {
        #region Constants
        private const string KeyPassword = "pwd";
        private const string KeyPublic = "pub";
        private const string KeyPrivate = "priv";
        #endregion

        #region Constructor
        public PGPKeysClass(string value)
        {
            JObject c_Wkg = value.ToJObject();

            this.Password = c_Wkg.Get(KeyPassword);
            this.PublicKey = new PGPKeyClass(c_Wkg.Get(KeyPublic));
            this.PrivateKey = new PGPKeyClass(c_Wkg.Get(KeyPrivate));
        }

        public PGPKeysClass(PGPKeyClass publickey)
            : this(null, publickey, null)
        { }

        public PGPKeysClass(PGPKeyClass publickey = null, PGPKeyClass privatekey = null, string password = null)
        {
            if (password == null) password = "".GUID();

            //
            this.Password = password;
            this.PublicKey = publickey;
            this.PrivateKey = privatekey;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public string Password { get; internal set; }
        public PGPKeyClass PublicKey { get; set; }
        public PGPKeyClass PrivateKey { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            JObject c_Wkg = new JObject();

            c_Wkg.Set(KeyPassword, this.Password);
            c_Wkg.Set(KeyPublic, this.PublicKey.ToString());
            c_Wkg.Set(KeyPrivate, this.PrivateKey.ToString());

            return c_Wkg.ToSimpleString();
        }
        #endregion
    }

    public class PGPKeyClass : IDisposable
    {
        #region Constructor
        public PGPKeyClass(string value)
            : this(value.ToBytes())
        { }

        public PGPKeyClass(byte[] value)
        {
            //
            this.Value = value;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public byte[] Value { get; set; }
        public MemoryStream AsStream { get { return new MemoryStream(this.Value); } }

        public string Password { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return this.Value.FromBytes();
        }
        #endregion
    }
}
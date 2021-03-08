///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    public static class Crypto
    {
        #region Enums
        public enum CryptoAlgorithm
        {
            DES,
            TripleDES,
            RC2,
            Rijiandel
        }

        public enum CryptoHashAlgorithm
        {
            MD5,
            SHA1
        }
        #endregion Enums

        #region MD5
        public static string GenerateMD5(this string value)
        {
            using (var c_MD5 = MD5.Create())
            {
                using (MemoryStream c_Stream = new MemoryStream(value.ToBytes()))
                {
                    return c_MD5.ComputeHash(c_Stream).FromBytes();
                }
            }
        }

        public static string GenerateFileMD5(this string file)
        {
            using (var c_MD5 = MD5.Create())
            {
                using (MemoryStream c_Stream = new MemoryStream(System.IO.File.ReadAllBytes(file)))
                {
                    return c_MD5.ComputeHash(c_Stream).FromBytes();
                }
            }
        }

        public static string GenerateFileMD5Hex(this string file)
        {
            using (var c_MD5 = MD5.Create())
            {
                using (MemoryStream c_Stream = new MemoryStream(System.IO.File.ReadAllBytes(file)))
                {
                    return c_MD5.ComputeHash(c_Stream).ByteArrayToHexString();
                }
            }
        }
        #endregion

        #region Hash
        public static byte[] Hash(this byte[] value)
        {
            return Hash(value, CryptoHashAlgorithm.MD5);
        }

        public static byte[] HashPlus(this byte[] value)
        {
            return Hash(value, CryptoHashAlgorithm.SHA1);
        }

        public static byte[] Hash(this byte[] value, CryptoHashAlgorithm algorithm)
        {
            HashAlgorithm c_Provider = null;

            switch (algorithm)
            {
                case CryptoHashAlgorithm.MD5:

                    c_Provider = new MD5CryptoServiceProvider();
                    break;

                case CryptoHashAlgorithm.SHA1:

                    c_Provider = new SHA1CryptoServiceProvider();
                    break;

            }

            return c_Provider.ComputeHash(value);
        }

        public static string HashString(this string value)
        {
            System.Text.StringBuilder c_Wkg = new System.Text.StringBuilder();

            byte[] baWkg = Hash(value.ToBytes());
            for (int iLoop = 0; iLoop < baWkg.Length; iLoop++)
            {
                c_Wkg.Append(string.Format("{0:X}", baWkg[iLoop]));
            }

            return c_Wkg.ToString();
        }

        public static string HashPlusString(this string value, string addition = "")
        {
            System.Text.StringBuilder c_Wkg = new System.Text.StringBuilder();

            byte[] baWkg = Hash((value + addition).ToBytes());
            for (int iLoop = 0; iLoop < baWkg.Length; iLoop++)
            {
                c_Wkg.Append(string.Format("{0:X}", baWkg[iLoop]));
            }

            return c_Wkg.ToString();
        }

        public static bool IsHash(this string value)
        {
            return value.Length == 32 && Regex.Replace(value, @"[0-9a-fA-F]", "").Length == 0;
        }

        public static bool IsHashPlus(this string value)
        {
            return value.Length == 40 && Regex.Replace(value, @"[0-9a-fA-F]", "").Length == 0;
        }

        public static string MakePwd(this string value)
        {
            return value.HashString().Substring(17, 6);
        }
        #endregion

        #region Encode/Decode
        public static byte[] Encode(this byte[] value, string password, CryptoAlgorithm algorithm)
        {
            byte[] baAns = null;

            SymmetricAlgorithm c_Provider = algorithm.GetAlgorithm();

            if (value != null && value.Length > 0)
            {
                try
                {
                    ComputeKey(c_Provider, password);

                    MemoryStream c_Mem = new MemoryStream();
                    CryptoStream c_Crypto = new CryptoStream(c_Mem, c_Provider.CreateEncryptor(), CryptoStreamMode.Write);
                    c_Crypto.Write(value, 0, value.Length);
                    c_Crypto.Flush();
                    c_Crypto.Close();

                    baAns = c_Mem.ToArray();
                }
                catch { }
            }

            return baAns;
        }

        public static byte[] Encode(this byte[] value, string password)
        {
            return value.Encode(password, CryptoAlgorithm.TripleDES);
        }

        public static byte[] Decode(this byte[] value, string password, CryptoAlgorithm algorithm)
        {
            byte[] baAns = null;

            SymmetricAlgorithm c_Provider = algorithm.GetAlgorithm();

            if (value != null && value.Length > 0)
            {
                try
                {
                    ComputeKey(c_Provider, password);

                    MemoryStream c_Mem = new MemoryStream();
                    CryptoStream c_Crypto = new CryptoStream(c_Mem, c_Provider.CreateDecryptor(), CryptoStreamMode.Write);
                    c_Crypto.Write(value, 0, value.Length);
                    c_Crypto.Flush();
                    c_Crypto.Close();

                    baAns = c_Mem.ToArray();
                }
                catch (Exception e)
                {
                    var a = e;
                }
            }

            return baAns;
        }

        public static byte[] Decode(this byte[] value, string password)
        {
            return value.Decode(password, CryptoAlgorithm.TripleDES);
        }

        private static SymmetricAlgorithm GetAlgorithm(this CryptoAlgorithm algorithm)
        {
            SymmetricAlgorithm c_Ans = null;

            switch (algorithm)
            {
                case CryptoAlgorithm.DES:

                    c_Ans = new DESCryptoServiceProvider();
                    break;

                case CryptoAlgorithm.TripleDES:

                    c_Ans = new TripleDESCryptoServiceProvider();
                    break;

                case CryptoAlgorithm.RC2:

                    c_Ans = new RC2CryptoServiceProvider();
                    break;

                case CryptoAlgorithm.Rijiandel:

                    c_Ans = new RijndaelManaged();
                    break;

            }

            return c_Ans;
        }

        private static void ComputeKey(this SymmetricAlgorithm provider, string password)
        {
            SHA256Managed c_Hash = new SHA256Managed();
            System.Security.Cryptography.KeySizes[] baSizes = provider.LegalKeySizes;
            int iSIV = provider.IV.Length;
            int iSize = 0;
            //provider.Padding = PaddingMode.ANSIX923;

            foreach (System.Security.Cryptography.KeySizes c_KS in baSizes)
            {
                int iNew = (int)(c_KS.MaxSize / 8);
                if (c_KS.MaxSize == (8 * iNew))
                {
                    if (iNew > iSize) iSize = iNew;
                }
            }

            int iMin = iSize + iSIV;
            string sKey = password;

            while (sKey.Length < iMin) sKey += sKey;

            byte[] baPwd = Encoding.UTF8.GetBytes(sKey.Substring(0, iSize));
            byte[] baIV = Encoding.UTF8.GetBytes(sKey.Substring(iSize, iSIV));

            provider.Key = baPwd;
            provider.IV = baIV;
        }
        #endregion

        #region String
        public static string StringEncode(this string value, string pwd)
        {
            return Convert.ToBase64String(Encode(UTF32Encoding.UTF32.GetBytes(value), pwd));
        }

        public static string StringDecode(this string value, string pwd)
        {
            return UTF32Encoding.UTF32.GetString(Decode(Convert.FromBase64String(value), pwd));
        }
        #endregion

        #region UTF8 String
        public static string SecureEncode(this string value, string pwd)
        {
            return value.eCEncode(pwd).flatten();
        }

        public static string SecureDecode(this string value, string pwd)
        {
            // The replace is to fix issues when string used in web page!
            return value.sharpen().Replace(" ", "+").eCDecode(pwd);
        }
        #endregion

        #region eCandidus
        public static string eCDecode(this string value, string password, CryptoAlgorithm algorithm)
        {
            string sAns = null;
            SymmetricAlgorithm c_Provider = GetAlgorithm(algorithm);

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    ComputeKey(c_Provider, password);

                    MemoryStream c_Mem = new MemoryStream();
                    CryptoStream c_Crypto = new CryptoStream(c_Mem, c_Provider.CreateDecryptor(), CryptoStreamMode.Write);
                    byte[] baValue = Convert.FromBase64String(value);
                    c_Crypto.Write(baValue, 0, baValue.Length);
                    c_Crypto.Flush();
                    c_Crypto.Close();

                    sAns = c_Mem.ToArray().FromASCIIBytes();
                }
                catch { }
            }

            return sAns;
        }

        public static string eCDecode(this string value, string password, bool nospace = false)
        {
            if (nospace) value = value.Replace(" ", "+");
            return eCDecode(value, password, CryptoAlgorithm.RC2);
        }

        public static string eCEncode(this string value, string password, CryptoAlgorithm algorithm)
        {
            string sAns = null;
            SymmetricAlgorithm c_Provider = GetAlgorithm(algorithm);

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    ComputeKey(c_Provider, password);

                    MemoryStream c_Mem = new MemoryStream();
                    CryptoStream c_Crypto = new CryptoStream(c_Mem, c_Provider.CreateEncryptor(), CryptoStreamMode.Write);
                    byte[] baValue = value.ToASCIIBytes();
                    c_Crypto.Write(baValue, 0, baValue.Length);
                    c_Crypto.Flush();
                    c_Crypto.Close();

                    sAns = Convert.ToBase64String(c_Mem.ToArray());
                }
                catch { }
            }

            return sAns;
        }

        public static string eCEncode(this string value, string password)
        {
            return eCEncode(value, password, CryptoAlgorithm.RC2);
        }
        #endregion

        #region Hex
        public static string HexDecode(this string value, string password, CryptoAlgorithm algorithm)
        {
            string sAns = null;
            SymmetricAlgorithm c_Provider = GetAlgorithm(algorithm);

            try
            {
                ComputeKey(c_Provider, password);

                MemoryStream c_Mem = new MemoryStream();
                CryptoStream c_Crypto = new CryptoStream(c_Mem, c_Provider.CreateDecryptor(), CryptoStreamMode.Write);
                byte[] baValue = value.HexStringToByteArray();
                c_Crypto.Write(baValue, 0, baValue.Length);
                c_Crypto.Flush();
                c_Crypto.Close();

                sAns = c_Mem.ToArray().FromASCIIBytes();
            }
            catch
            {
            }

            return sAns;
        }

        public static string HexDecode(this string value, string password)
        {
            return HexDecode(value, password, CryptoAlgorithm.RC2);
        }

        public static string HexEncode(this string value, string password, CryptoAlgorithm algorithm)
        {
            string sAns = null;
            SymmetricAlgorithm c_Provider = GetAlgorithm(algorithm);

            try
            {
                ComputeKey(c_Provider, password);

                MemoryStream c_Mem = new MemoryStream();
                CryptoStream c_Crypto = new CryptoStream(c_Mem, c_Provider.CreateEncryptor(), CryptoStreamMode.Write);
                byte[] baValue = value.ToASCIIBytes();
                c_Crypto.Write(baValue, 0, baValue.Length);
                c_Crypto.Flush();
                c_Crypto.Close();

                sAns = c_Mem.ToArray().ByteArrayToHexString();
            }
            catch { }

            return sAns;
        }

        public static string HexEncode(this string value, string password)
        {
            return HexEncode(value, password, CryptoAlgorithm.RC2);
        }

        public static int HexToInt(this string hex, int defaultvalue)
        {
            int iAns = defaultvalue;

            try
            {
                iAns = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            }
            catch { }

            return iAns;
        }

        public static byte HexToByte(this string hex, byte defaultvalue)
        {
            byte iAns = defaultvalue;

            try
            {
                iAns = Byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            }
            catch
            {
            }

            return iAns;
        }

        public static string ByteToHex(this byte value)
        {
            string sWkg = string.Format("{0:X}", value);

            while (sWkg.Length < 2) sWkg = "0" + sWkg;

            return sWkg;
        }

        public static string ByteArrayToHexString(this byte[] value)
        {
            string sAns = string.Empty;

            for (int iLoop = 0; iLoop < value.Length; iLoop++)
            {
                sAns += ByteToHex(value[iLoop]);
            }

            return sAns;
        }

        public static byte[] HexStringToByteArray(this string value)
        {
            int iLen = value.Length / 2;

            byte[] baAns = new byte[iLen];

            int iPos = 0;
            for (int iLoop = 0; iLoop < value.Length; iLoop += 2)
            {
                baAns[iPos++] = HexToByte(value.Substring(iLoop, 2), 0);
            }

            return baAns;
        }
        #endregion

        #region Compression
        public static byte[] CompressArray(this byte[] buffer)
        {
            MemoryStream c_MS = new MemoryStream();
            System.IO.Stream c_Stream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(c_MS);

            c_Stream.Write(buffer, 0, buffer.Length);
            c_Stream.Flush();
            c_Stream.Close();

            return c_MS.ToArray();
        }

        public static byte[] DecompressArray(this byte[] buffer)
        {
            byte[] baAns = null;

            try
            {
                using (System.IO.MemoryStream c_Result = new MemoryStream())
                {
                    using (System.IO.MemoryStream c_Source = new MemoryStream(buffer, 0, buffer.Length))
                    {
                        using (ICSharpCode.SharpZipLib.GZip.GZipInputStream c_Stream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(c_Source))
                        {
                            byte[] abBuffer = new byte[1024];

                            int iSize = c_Stream.Read(abBuffer, 0, abBuffer.Length);
                            while (iSize > 0)
                            {
                                c_Result.Write(abBuffer, 0, iSize);

                                iSize = c_Stream.Read(abBuffer, 0, abBuffer.Length);
                            }
                        }
                    }

                    baAns = c_Result.ToArray();
                }
            }
            catch
            {
            }

            return baAns;
        }

        public static string Compress(this string value)
        {
            return Convert.ToBase64String(CompressArray(value.ToBytes()));
        }

        public static string Decompress(this string value)
        {
            return DecompressArray(Convert.FromBase64String(value)).FromBytes();
        }
        #endregion

        #region Base36
        public static string base36 = "01234567898ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string base62 = "01234567898ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string ToBase(this string basec, long value, int length)
        {
            string sAns = string.Empty;

            while (length > sAns.Length)
            {
                byte bVal = (byte)(value % basec.Length);
                sAns = basec[bVal] + sAns;

                value /= basec.Length;
            }

            return sAns;
        }

        public static long FromBase(this string basec, string value)
        {
            long lAns = 0;

            while (value.Length > 0)
            {
                lAns *= basec.Length;

                byte bWkg = (byte)basec.IndexOf(value[0]);
                lAns += bWkg;

                value = value.Substring(1);
            }

            return lAns;
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
            if (password == null) password = "".UUID();

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

    public class PGPCrypto : IDisposable
    {
        #region Constants
        private const int BufferSize = 0x10000;
        #endregion

        #region Constructor
        public PGPCrypto(PGPKeysClass keys)
        {
            //
            this.Keys = keys;
            if (this.Keys == null)
            {
                this.Keys = PGPCrypto.GenerateKeys("");
            }
        }

        public PGPCrypto(PGPKeyClass publickkey, PGPKeyClass privatekey, string pwd)
        {
            //
            this.Keys = new PGPKeysClass(publickkey, privatekey, pwd);
        }
        #endregion

        #region Enums
        public enum PGPFileType { Binary, Text, UTF8 }
        #endregion

        #region Properties
        private PGPKeysClass Keys { get; set; }
        //private PGPKeyClass IPublicKey { get; set; }
        //private PGPKeyClass IPrivateKey { get; set; }
        //private string IPassword { get; set; }

        private CompressionAlgorithmTag CompressionAlgorithm { get; set; } = CompressionAlgorithmTag.Uncompressed;
        private SymmetricKeyAlgorithmTag SymmetricKeyAlgorithm { get; set; } = SymmetricKeyAlgorithmTag.TripleDes;
        private int PgpSignatureType { get; set; } = PgpSignature.DefaultCertification;
        private PublicKeyAlgorithmTag PublicKeyAlgorithm { get; set; } = PublicKeyAlgorithmTag.RsaGeneral;
        private PGPFileType FileType { get; set; } = PGPFileType.Binary;
        #endregion

        #region Constructor
        public PGPCrypto()
        {
            CompressionAlgorithm = CompressionAlgorithmTag.Uncompressed;
            SymmetricKeyAlgorithm = SymmetricKeyAlgorithmTag.TripleDes;
            PgpSignatureType = PgpSignature.DefaultCertification;
            PublicKeyAlgorithm = PublicKeyAlgorithmTag.RsaGeneral;
            FileType = PGPFileType.Binary;
        }
        #endregion Constructor

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Encrypt
        private void EncryptStream(
            Stream inputStream,
            Stream outputStream,
            Stream publicKeyStream,
            bool armor = true,
            bool withIntegrityCheck = true)
        {
            EncryptStream(inputStream, outputStream, new[] { publicKeyStream }, armor, withIntegrityCheck);
        }

        private void EncryptStream(Stream inputStream, Stream outputStream, IEnumerable<Stream> publicKeyStreams, bool armor = true, bool withIntegrityCheck = true)
        {
            //Avoid multiple enumerations of 'publicKeyFilePaths'
            Stream[] publicKeys = publicKeyStreams.ToArray();

            if (inputStream == null)
                throw new ArgumentException("InputStream");
            if (outputStream == null)
                throw new ArgumentException("OutputStream");
            foreach (Stream publicKey in publicKeys)
            {
                if (publicKey == null)
                    throw new ArgumentException("PublicKeyStream");
            }

            using (MemoryStream @out = new MemoryStream())
            {
                if (CompressionAlgorithm != CompressionAlgorithmTag.Uncompressed)
                {
                    PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(CompressionAlgorithm);
                    Utilities.WriteStreamToLiteralData(comData.Open(@out), FileTypeToChar(), inputStream, "name");
                    comData.Close();
                }
                else
                    Utilities.WriteStreamToLiteralData(@out, FileTypeToChar(), inputStream, "name");

                PgpEncryptedDataGenerator pk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithm, withIntegrityCheck, new SecureRandom());

                foreach (Stream publicKey in publicKeys)
                {
                    pk.AddMethod(Utilities.ReadPublicKey(publicKey));
                }

                byte[] bytes = @out.ToArray();

                if (armor)
                {
                    using (ArmoredOutputStream armoredStream = new ArmoredOutputStream(outputStream))
                    {
                        using (Stream armoredOutStream = pk.Open(armoredStream, bytes.Length))
                        {
                            armoredOutStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                else
                {
                    using (Stream plainStream = pk.Open(outputStream, bytes.Length))
                    {
                        plainStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public byte[] Encrypt(byte[] data, bool armor = true, bool withIntegrityCheck = true)
        {
            using (MemoryStream c_In = new MemoryStream(data))
            {
                using (MemoryStream c_Out = new MemoryStream())
                {
                    using (MemoryStream c_Key = this.Keys.PublicKey.AsStream)
                    {
                        this.EncryptStream(c_In, c_Out, c_Key, armor, withIntegrityCheck);

                        return c_Out.ToArray();
                    }
                }
            }
        }
        #endregion Encrypt

        #region Encrypt and Sign
        private void EncryptFileAndSign(string inputFilePath, string outputFilePath, string publicKeyFilePath,
            string privateKeyFilePath, string passPhrase, bool armor = true, bool withIntegrityCheck = true)
        {
            EncryptFileAndSign(inputFilePath, outputFilePath, new[] { publicKeyFilePath }, privateKeyFilePath, passPhrase, armor, withIntegrityCheck);
        }

        private void EncryptFileAndSign(string inputFilePath, string outputFilePath, IEnumerable<string> publicKeyFilePaths,
            string privateKeyFilePath, string passPhrase, bool armor = true, bool withIntegrityCheck = true)
        {
            //Avoid multiple enumerations of 'publicKeyFilePaths'
            string[] publicKeys = publicKeyFilePaths.ToArray();

            if (String.IsNullOrEmpty(inputFilePath))
                throw new ArgumentException("InputFilePath");
            if (String.IsNullOrEmpty(outputFilePath))
                throw new ArgumentException("OutputFilePath");
            if (String.IsNullOrEmpty(privateKeyFilePath))
                throw new ArgumentException("PrivateKeyFilePath");
            if (passPhrase == null)
                passPhrase = String.Empty;

            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException(String.Format("Input file [{0}] does not exist.", inputFilePath));
            if (!File.Exists(privateKeyFilePath))
                throw new FileNotFoundException(String.Format("Private Key file [{0}] does not exist.", privateKeyFilePath));

            foreach (string publicKeyFilePath in publicKeys)
            {
                if (String.IsNullOrEmpty(publicKeyFilePath))
                    throw new ArgumentException(nameof(publicKeyFilePath));
                if (!File.Exists(publicKeyFilePath))
                    throw new FileNotFoundException(String.Format("Input file [{0}] does not exist.", publicKeyFilePath));
            }

            EncryptionKeys encryptionKeys = new EncryptionKeys(publicKeyFilePaths, privateKeyFilePath, passPhrase);

            if (encryptionKeys == null)
                throw new ArgumentNullException("Encryption Key not found.");

            using (Stream outputStream = File.Create(outputFilePath))
            {
                if (armor)
                {
                    using (ArmoredOutputStream armoredOutputStream = new ArmoredOutputStream(outputStream))
                    {
                        OutputEncrypted(inputFilePath, armoredOutputStream, encryptionKeys, withIntegrityCheck);
                    }
                }
                else
                    OutputEncrypted(inputFilePath, outputStream, encryptionKeys, withIntegrityCheck);
            }
        }

        private void EncryptStreamAndSign(Stream inputStream, Stream outputStream, Stream publicKeyStream,
            Stream privateKeyStream, string passPhrase, bool armor = true, bool withIntegrityCheck = true)
        {
            EncryptStreamAndSign(inputStream, outputStream, new[] { publicKeyStream }, privateKeyStream, passPhrase, armor, withIntegrityCheck);
        }

        private void EncryptStreamAndSign(Stream inputStream, Stream outputStream, IEnumerable<Stream> publicKeyStreams,
            Stream privateKeyStream, string passPhrase, bool armor = true, bool withIntegrityCheck = true)
        {
            if (inputStream == null)
                throw new ArgumentException("InputStream");
            if (outputStream == null)
                throw new ArgumentException("OutputStream");
            if (privateKeyStream == null)
                throw new ArgumentException("PrivateKeyStream");
            if (passPhrase == null)
                passPhrase = String.Empty;

            foreach (Stream publicKey in publicKeyStreams)
            {
                if (publicKey == null)
                    throw new ArgumentException("PublicKeyStream");
            }

            EncryptionKeys encryptionKeys = new EncryptionKeys(publicKeyStreams, privateKeyStream, passPhrase);

            if (encryptionKeys == null)
                throw new ArgumentNullException("Encryption Key not found.");

            if (armor)
            {
                using (ArmoredOutputStream armoredOutputStream = new ArmoredOutputStream(outputStream))
                {
                    OutputEncrypted(inputStream, armoredOutputStream, encryptionKeys, withIntegrityCheck, "name");
                }
            }
            else
                OutputEncrypted(inputStream, outputStream, encryptionKeys, withIntegrityCheck, "name");
        }

        private void OutputEncrypted(string inputFilePath, Stream outputStream, EncryptionKeys encryptionKeys, bool withIntegrityCheck)
        {
            using (Stream encryptedOut = ChainEncryptedOut(outputStream, encryptionKeys, withIntegrityCheck))
            {
                FileInfo unencryptedFileInfo = new FileInfo(inputFilePath);
                using (Stream compressedOut = ChainCompressedOut(encryptedOut))
                {
                    PgpSignatureGenerator signatureGenerator = InitSignatureGenerator(compressedOut, encryptionKeys);
                    using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedFileInfo))
                    {
                        using (FileStream inputFileStream = unencryptedFileInfo.OpenRead())
                        {
                            WriteOutputAndSign(compressedOut, literalOut, inputFileStream, signatureGenerator);
                            inputFileStream.Dispose();
                        }
                    }
                }
            }
        }

        private void OutputEncrypted(Stream inputStream, Stream outputStream, EncryptionKeys encryptionKeys, bool withIntegrityCheck, string name)
        {
            using (Stream encryptedOut = ChainEncryptedOut(outputStream, encryptionKeys, withIntegrityCheck))
            {
                using (Stream compressedOut = ChainCompressedOut(encryptedOut))
                {
                    PgpSignatureGenerator signatureGenerator = InitSignatureGenerator(compressedOut, encryptionKeys);
                    using (Stream literalOut = ChainLiteralStreamOut(compressedOut, inputStream, name))
                    {
                        WriteOutputAndSign(compressedOut, literalOut, inputStream, signatureGenerator);
                        inputStream.Dispose();
                    }
                }
            }
        }

        private void WriteOutputAndSign(Stream compressedOut, Stream literalOut, FileStream inputFilePath, PgpSignatureGenerator signatureGenerator)
        {
            int length = 0;
            byte[] buf = new byte[BufferSize];
            while ((length = inputFilePath.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, length);
                signatureGenerator.Update(buf, 0, length);
            }
            signatureGenerator.Generate().Encode(compressedOut);
        }

        private void WriteOutputAndSign(Stream compressedOut, Stream literalOut, Stream inputStream, PgpSignatureGenerator signatureGenerator)
        {
            int length = 0;
            byte[] buf = new byte[BufferSize];
            while ((length = inputStream.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, length);
                signatureGenerator.Update(buf, 0, length);
            }
            signatureGenerator.Generate().Encode(compressedOut);
        }

        private Stream ChainEncryptedOut(Stream outputStream, EncryptionKeys encryptionKeys, bool withIntegrityCheck)
        {
            PgpEncryptedDataGenerator encryptedDataGenerator;
            encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithm, withIntegrityCheck, new SecureRandom());

            if (encryptionKeys.PublicKey != null)
            {
                encryptedDataGenerator.AddMethod(encryptionKeys.PublicKey);
            }
            else if (encryptionKeys.PublicKeys != null)
            {
                foreach (PgpPublicKey publicKey in encryptionKeys.PublicKeys)
                {
                    encryptedDataGenerator.AddMethod(publicKey);
                }
            }

            return encryptedDataGenerator.Open(outputStream, new byte[BufferSize]);
        }

        private Stream ChainCompressedOut(Stream encryptedOut)
        {
            if (CompressionAlgorithm != CompressionAlgorithmTag.Uncompressed)
            {
                PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                return compressedDataGenerator.Open(encryptedOut);
            }
            else
                return encryptedOut;
        }

        private Stream ChainLiteralOut(Stream compressedOut, FileInfo file)
        {
            PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
            return pgpLiteralDataGenerator.Open(compressedOut, FileTypeToChar(), file.Name, file.Length, DateTime.Now);
        }

        private Stream ChainLiteralStreamOut(Stream compressedOut, Stream inputStream, string name)
        {
            PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
            return pgpLiteralDataGenerator.Open(compressedOut, FileTypeToChar(), name, inputStream.Length, DateTime.Now);
        }

        private PgpSignatureGenerator InitSignatureGenerator(Stream compressedOut, EncryptionKeys encryptionKeys)
        {
            PublicKeyAlgorithmTag tag = encryptionKeys.SecretKey.PublicKey.Algorithm;
            PgpSignatureGenerator pgpSignatureGenerator = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
            pgpSignatureGenerator.InitSign(PgpSignature.BinaryDocument, encryptionKeys.PrivateKey);
            foreach (string userId in encryptionKeys.SecretKey.PublicKey.GetUserIds())
            {
                PgpSignatureSubpacketGenerator subPacketGenerator = new PgpSignatureSubpacketGenerator();
                subPacketGenerator.SetSignerUserId(false, userId);
                pgpSignatureGenerator.SetHashedSubpackets(subPacketGenerator.Generate());
                // Just the first one!
                break;
            }
            pgpSignatureGenerator.GenerateOnePassVersion(false).Encode(compressedOut);
            return pgpSignatureGenerator;
        }

        #endregion Encrypt and Sign

        #region Decrypt
        private void DecryptStream(Stream inputStream, Stream outputStream, Stream privateKeyStream, string passPhrase)
        {
            if (inputStream == null)
                throw new ArgumentException("InputStream");
            if (outputStream == null)
                throw new ArgumentException("outputStream");
            if (privateKeyStream == null)
                throw new ArgumentException("privateKeyStream");
            if (passPhrase == null)
                passPhrase = String.Empty;

            PgpObjectFactory objFactory = new PgpObjectFactory(PgpUtilities.GetDecoderStream(inputStream));
            // find secret key
            PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));

            PgpObject obj = null;
            if (objFactory != null)
                obj = objFactory.NextPgpObject();

            // the first object might be a PGP marker packet.
            PgpEncryptedDataList enc = null;
            if (obj is PgpEncryptedDataList)
                enc = (PgpEncryptedDataList)obj;
            else
                enc = (PgpEncryptedDataList)objFactory.NextPgpObject();

            // decrypt
            PgpPrivateKey privateKey = null;
            PgpPublicKeyEncryptedData pbe = null;
            foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
            {
                privateKey = FindSecretKey(pgpSec, pked.KeyId, passPhrase.ToCharArray());

                if (privateKey != null)
                {
                    pbe = pked;
                    break;
                }
            }

            if (privateKey == null)
                throw new ArgumentException("Secret key for message not found.");

            PgpObjectFactory plainFact = null;

            using (Stream clear = pbe.GetDataStream(privateKey))
            {
                plainFact = new PgpObjectFactory(clear);
            }

            PgpObject message = plainFact.NextPgpObject();

            if (message is PgpOnePassSignatureList)
            {
                message = plainFact.NextPgpObject();
            }

            if (message is PgpCompressedData)
            {
                PgpCompressedData cData = (PgpCompressedData)message;
                PgpObjectFactory of = null;

                using (Stream compDataIn = cData.GetDataStream())
                {
                    of = new PgpObjectFactory(compDataIn);
                }

                message = of.NextPgpObject();
                if (message is PgpOnePassSignatureList)
                {
                    message = of.NextPgpObject();
                    PgpLiteralData Ld = null;
                    Ld = (PgpLiteralData)message;
                    Stream unc = Ld.GetInputStream();
                    Streams.PipeAll(unc, outputStream);
                }
                else
                {
                    PgpLiteralData Ld = null;
                    Ld = (PgpLiteralData)message;
                    Stream unc = Ld.GetInputStream();
                    Streams.PipeAll(unc, outputStream);
                }
            }
            else if (message is PgpLiteralData)
            {
                PgpLiteralData ld = (PgpLiteralData)message;
                string outFileName = ld.FileName;

                Stream unc = ld.GetInputStream();
                Streams.PipeAll(unc, outputStream);

                if (pbe.IsIntegrityProtected())
                {
                    if (!pbe.Verify())
                    {
                        throw new PgpException("Message failed integrity check.");
                    }
                }
            }
            else if (message is PgpOnePassSignatureList)
                throw new PgpException("Encrypted message contains a signed message - not literal data.");
            else
                throw new PgpException("Message is not a simple encrypted file.");
        }

        public byte[] Decrypt(byte[] data)
        {
            using (MemoryStream c_In = new MemoryStream(data))
            {
                using (MemoryStream c_Out = new MemoryStream())
                {
                    using (MemoryStream c_Key = this.Keys.PrivateKey.AsStream)
                    {
                        this.DecryptStream(c_In, c_Out, c_Key, this.Keys.Password);

                        return c_Out.ToArray();
                    }
                }
            }
        }
        #endregion Decrypt

        #region GenerateKey
        public static PGPKeysClass GenerateKeys(string password,
                                                        int strength = 2048,
                                                        string username = null,
                                                        int certainty = 8,
                                                        bool armor = true)
        {
            if (!password.HasValue())
            {
                password = "".UUID();
            }

            using (PGPCrypto c_Eng = new PGPCrypto())
            {
                return c_Eng.IGenerateKeys(strength, username, password, certainty, armor);
            }
        }

        private PGPKeysClass IGenerateKeys(int strength = 4096,
                                                string username = null,
                                                string password = null,
                                                int certainty = 8,
                                                bool armor = true)
        {
            PGPKeysClass c_Ans = new PGPKeysClass(null, null, password);

            username = username == null ? string.Empty : username;
            password = password == null ? string.Empty : password;

            IAsymmetricCipherKeyPairGenerator kpg = new RsaKeyPairGenerator();
            kpg.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(0x13), new SecureRandom(), strength, certainty));
            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            using (MemoryStream c_Priv = new MemoryStream())
            {
                using (MemoryStream c_Pub = new MemoryStream())
                {
                    ExportKeyPair(c_Priv, c_Pub, kp.Public, kp.Private, username, password.ToCharArray(), armor);

                    c_Ans.PublicKey = new PGPKeyClass(c_Pub.ToArray());
                    c_Ans.PrivateKey = new PGPKeyClass(c_Priv.ToArray());

                    return c_Ans;
                }
            }

        }
        #endregion GenerateKey

        #region Private helpers
        private char FileTypeToChar()
        {
            char cAns = PgpLiteralData.Binary;

            switch (this.FileType)
            {
                case PGPFileType.UTF8:
                    cAns = PgpLiteralData.Utf8;
                    break;
                case PGPFileType.Text:
                    cAns = PgpLiteralData.Text;
                    break;
            }
            return cAns;
        }

        private void ExportKeyPair(
                    Stream secretOut,
                    Stream publicOut,
                    AsymmetricKeyParameter publicKey,
                    AsymmetricKeyParameter privateKey,
                    string identity,
                    char[] passPhrase,
                    bool armor)
        {
            if (secretOut == null)
                throw new ArgumentException("secretOut");
            if (publicOut == null)
                throw new ArgumentException("publicOut");

            if (armor)
            {
                secretOut = new ArmoredOutputStream(secretOut);
            }

            PgpSecretKey secretKey = new PgpSecretKey(
                PgpSignatureType,
                PublicKeyAlgorithm,
                publicKey,
                privateKey,
                DateTime.Now,
                identity,
                SymmetricKeyAlgorithm,
                passPhrase,
                null,
                null,
                new SecureRandom()
                //                ,"BC"
                );

            secretKey.Encode(secretOut);

            secretOut.Dispose();

            if (armor)
            {
                publicOut = new ArmoredOutputStream(publicOut);
            }

            PgpPublicKey key = secretKey.PublicKey;

            key.Encode(publicOut);

            publicOut.Dispose();
        }

        /*
        * Search a secret key ring collection for a secret key corresponding to keyId if it exists.
        */
        private PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] pass)
        {
            PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyId);

            if (pgpSecKey == null)
                return null;

            return pgpSecKey.ExtractPrivateKey(pass);
        }
        #endregion Private helpers

        private sealed class Utilities
        {
            #region Statics
            public static MPInteger[] DsaSigToMpi(
                byte[] encoding)
            {
                DerInteger i1, i2;

                try
                {
                    Asn1Sequence s = (Asn1Sequence)Asn1Object.FromByteArray(encoding);

                    i1 = (DerInteger)s[0];
                    i2 = (DerInteger)s[1];
                }
                catch (IOException e)
                {
                    throw new PgpException("exception encoding signature", e);
                }

                return new MPInteger[] { new MPInteger(i1.Value), new MPInteger(i2.Value) };
            }

            public static MPInteger[] RsaSigToMpi(
                byte[] encoding)
            {
                return new MPInteger[] { new MPInteger(new BigInteger(1, encoding)) };
            }

            public static string GetDigestName(
                HashAlgorithmTag hashAlgorithm)
            {
                switch (hashAlgorithm)
                {
                    case HashAlgorithmTag.Sha1:
                        return "SHA1";
                    case HashAlgorithmTag.MD2:
                        return "MD2";
                    case HashAlgorithmTag.MD5:
                        return "MD5";
                    case HashAlgorithmTag.RipeMD160:
                        return "RIPEMD160";
                    case HashAlgorithmTag.Sha224:
                        return "SHA224";
                    case HashAlgorithmTag.Sha256:
                        return "SHA256";
                    case HashAlgorithmTag.Sha384:
                        return "SHA384";
                    case HashAlgorithmTag.Sha512:
                        return "SHA512";
                    default:
                        throw new PgpException("unknown hash algorithm tag in GetDigestName: " + hashAlgorithm);
                }
            }

            public static string GetSignatureName(
                PublicKeyAlgorithmTag keyAlgorithm,
                HashAlgorithmTag hashAlgorithm)
            {
                string encAlg;
                switch (keyAlgorithm)
                {
                    case PublicKeyAlgorithmTag.RsaGeneral:
                    case PublicKeyAlgorithmTag.RsaSign:
                        encAlg = "RSA";
                        break;
                    case PublicKeyAlgorithmTag.Dsa:
                        encAlg = "DSA";
                        break;
                    case PublicKeyAlgorithmTag.ElGamalEncrypt: // in some malformed cases.
                    case PublicKeyAlgorithmTag.ElGamalGeneral:
                        encAlg = "ElGamal";
                        break;
                    default:
                        throw new PgpException("unknown algorithm tag in signature:" + keyAlgorithm);
                }

                return GetDigestName(hashAlgorithm) + "with" + encAlg;
            }

            public static string GetSymmetricCipherName(
                SymmetricKeyAlgorithmTag algorithm)
            {
                switch (algorithm)
                {
                    case SymmetricKeyAlgorithmTag.Null:
                        return null;
                    case SymmetricKeyAlgorithmTag.TripleDes:
                        return "DESEDE";
                    case SymmetricKeyAlgorithmTag.Idea:
                        return "IDEA";
                    case SymmetricKeyAlgorithmTag.Cast5:
                        return "CAST5";
                    case SymmetricKeyAlgorithmTag.Blowfish:
                        return "Blowfish";
                    case SymmetricKeyAlgorithmTag.Safer:
                        return "SAFER";
                    case SymmetricKeyAlgorithmTag.Des:
                        return "DES";
                    case SymmetricKeyAlgorithmTag.Aes128:
                        return "AES";
                    case SymmetricKeyAlgorithmTag.Aes192:
                        return "AES";
                    case SymmetricKeyAlgorithmTag.Aes256:
                        return "AES";
                    case SymmetricKeyAlgorithmTag.Twofish:
                        return "Twofish";
                    default:
                        throw new PgpException("unknown symmetric algorithm: " + algorithm);
                }
            }

            public static int GetKeySize(SymmetricKeyAlgorithmTag algorithm)
            {
                int keySize;
                switch (algorithm)
                {
                    case SymmetricKeyAlgorithmTag.Des:
                        keySize = 64;
                        break;
                    case SymmetricKeyAlgorithmTag.Idea:
                    case SymmetricKeyAlgorithmTag.Cast5:
                    case SymmetricKeyAlgorithmTag.Blowfish:
                    case SymmetricKeyAlgorithmTag.Safer:
                    case SymmetricKeyAlgorithmTag.Aes128:
                        keySize = 128;
                        break;
                    case SymmetricKeyAlgorithmTag.TripleDes:
                    case SymmetricKeyAlgorithmTag.Aes192:
                        keySize = 192;
                        break;
                    case SymmetricKeyAlgorithmTag.Aes256:
                    case SymmetricKeyAlgorithmTag.Twofish:
                        keySize = 256;
                        break;
                    default:
                        throw new PgpException("unknown symmetric algorithm: " + algorithm);
                }

                return keySize;
            }

            public static KeyParameter MakeKey(
                SymmetricKeyAlgorithmTag algorithm,
                byte[] keyBytes)
            {
                string algName = GetSymmetricCipherName(algorithm);

                return ParameterUtilities.CreateKeyParameter(algName, keyBytes);
            }

            public static KeyParameter MakeRandomKey(
                SymmetricKeyAlgorithmTag algorithm,
                SecureRandom random)
            {
                int keySize = GetKeySize(algorithm);
                byte[] keyBytes = new byte[(keySize + 7) / 8];
                random.NextBytes(keyBytes);
                return MakeKey(algorithm, keyBytes);
            }

            public static KeyParameter MakeKeyFromPassPhrase(
                SymmetricKeyAlgorithmTag algorithm,
                S2k s2k,
                char[] passPhrase)
            {
                int keySize = GetKeySize(algorithm);
                byte[] pBytes = Strings.ToByteArray(new string(passPhrase));
                byte[] keyBytes = new byte[(keySize + 7) / 8];

                int generatedBytes = 0;
                int loopCount = 0;

                while (generatedBytes < keyBytes.Length)
                {
                    IDigest digest;
                    if (s2k != null)
                    {
                        string digestName = GetDigestName(s2k.HashAlgorithm);

                        try
                        {
                            digest = DigestUtilities.GetDigest(digestName);
                        }
                        catch (Exception e)
                        {
                            throw new PgpException("can't find S2k digest", e);
                        }

                        for (int i = 0; i != loopCount; i++)
                        {
                            digest.Update(0);
                        }

                        byte[] iv = s2k.GetIV();

                        switch (s2k.Type)
                        {
                            case S2k.Simple:
                                digest.BlockUpdate(pBytes, 0, pBytes.Length);
                                break;
                            case S2k.Salted:
                                digest.BlockUpdate(iv, 0, iv.Length);
                                digest.BlockUpdate(pBytes, 0, pBytes.Length);
                                break;
                            case S2k.SaltedAndIterated:
                                long count = s2k.IterationCount;
                                digest.BlockUpdate(iv, 0, iv.Length);
                                digest.BlockUpdate(pBytes, 0, pBytes.Length);

                                count -= iv.Length + pBytes.Length;

                                while (count > 0)
                                {
                                    if (count < iv.Length)
                                    {
                                        digest.BlockUpdate(iv, 0, (int)count);
                                        break;
                                    }
                                    else
                                    {
                                        digest.BlockUpdate(iv, 0, iv.Length);
                                        count -= iv.Length;
                                    }

                                    if (count < pBytes.Length)
                                    {
                                        digest.BlockUpdate(pBytes, 0, (int)count);
                                        count = 0;
                                    }
                                    else
                                    {
                                        digest.BlockUpdate(pBytes, 0, pBytes.Length);
                                        count -= pBytes.Length;
                                    }
                                }
                                break;
                            default:
                                throw new PgpException("unknown S2k type: " + s2k.Type);
                        }
                    }
                    else
                    {
                        try
                        {
                            digest = DigestUtilities.GetDigest("MD5");

                            for (int i = 0; i != loopCount; i++)
                            {
                                digest.Update(0);
                            }

                            digest.BlockUpdate(pBytes, 0, pBytes.Length);
                        }
                        catch (Exception e)
                        {
                            throw new PgpException("can't find MD5 digest", e);
                        }
                    }

                    byte[] dig = DigestUtilities.DoFinal(digest);

                    if (dig.Length > (keyBytes.Length - generatedBytes))
                    {
                        Array.Copy(dig, 0, keyBytes, generatedBytes, keyBytes.Length - generatedBytes);
                    }
                    else
                    {
                        Array.Copy(dig, 0, keyBytes, generatedBytes, dig.Length);
                    }

                    generatedBytes += dig.Length;

                    loopCount++;
                }

                Array.Clear(pBytes, 0, pBytes.Length);

                return MakeKey(algorithm, keyBytes);
            }

            /// <summary>Write out the passed in file as a literal data packet.</summary>
            public static void WriteFileToLiteralData(
                Stream output,
                char fileType,
                FileInfo file)
            {
                PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
                Stream pOut = lData.Open(output, fileType, file.Name, file.Length, file.LastWriteTime);
                PipeFileContents(file, pOut, 4096);
            }

            /// <summary>Write out the passed in file as a literal data packet in partial packet format.</summary>
            public static void WriteFileToLiteralData(
                Stream output,
                char fileType,
                FileInfo file,
                byte[] buffer)
            {
                PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
                Stream pOut = lData.Open(output, fileType, file.Name, file.LastWriteTime, buffer);
                PipeFileContents(file, pOut, buffer.Length);
            }

            public static void WriteStreamToLiteralData(
                Stream output,
                char fileType,
                Stream input,
                string name)
            {
                PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
                Stream pOut = lData.Open(output, fileType, name, input.Length, DateTime.Now);
                PipeStreamContents(input, pOut, 4096);
            }

            public static void WriteStreamToLiteralData(
                Stream output,
                char fileType,
                Stream input,
                byte[] buffer,
                string name)
            {
                PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
                Stream pOut = lData.Open(output, fileType, name, DateTime.Now, buffer);
                PipeStreamContents(input, pOut, buffer.Length);
            }


            /// <summary>
            /// Opens a key ring file and returns first available sub-key suitable for encryption.
            /// If such sub-key is not found, return master key that can encrypt.
            /// </summary>
            /// <param name="inputStream"></param>
            /// <returns></returns>
            internal static PgpPublicKey ReadPublicKey(Stream inputStream)
            {
                inputStream = PgpUtilities.GetDecoderStream(inputStream);

                PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);

                // we just loop through the collection till we find a key suitable for encryption, in the real
                // world you would probably want to be a bit smarter about this.
                // iterate through the key rings.
                foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
                {
                    List<PgpPublicKey> keys = kRing.GetPublicKeys()
                        .Cast<PgpPublicKey>()
                        .Where(k => k.IsEncryptionKey).ToList();

                    const int encryptKeyFlags = PgpKeyFlags.CanEncryptCommunications | PgpKeyFlags.CanEncryptStorage;

                    foreach (PgpPublicKey key in keys.Where(k => k.Version >= 4 && !k.IsMasterKey))
                    {
                        foreach (PgpSignature s in key.GetSignatures())
                        {
                            if (s.GetHashedSubPackets().GetKeyFlags() == encryptKeyFlags)
                                return key;
                        }
                    }

                    if (keys.Any())
                        return keys.First();
                }

                throw new ArgumentException("Can't find encryption key in key ring.");
            }

            public static PgpPublicKey ReadPublicKey(string publicKeyFilePath)
            {
                if (!File.Exists(publicKeyFilePath))
                    throw new FileNotFoundException(String.Format("File {0} was not found", publicKeyFilePath));
                using (FileStream fs = new FileStream(publicKeyFilePath, FileMode.Open))
                    return ReadPublicKey(fs);
            }

            private static void PipeFileContents(FileInfo file, Stream pOut, int bufSize)
            {
                using (FileStream inputStream = file.OpenRead())
                {
                    byte[] buf = new byte[bufSize];

                    int len;
                    while ((len = inputStream.Read(buf, 0, buf.Length)) > 0)
                    {
                        pOut.Write(buf, 0, len);
                    }
                }
            }

            private static void PipeStreamContents(Stream input, Stream pOut, int bufSize)
            {
                byte[] buf = new byte[bufSize];

                int len;
                while ((len = input.Read(buf, 0, buf.Length)) > 0)
                {
                    pOut.Write(buf, 0, len);
                }
            }

            private const int ReadAhead = 60;

            private static bool IsPossiblyBase64(
                int ch)
            {
                return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z')
                        || (ch >= '0' && ch <= '9') || (ch == '+') || (ch == '/')
                        || (ch == '\r') || (ch == '\n');
            }

            /// <summary>
            /// Return either an ArmoredInputStream or a BcpgInputStream based on whether
            /// the initial characters of the stream are binary PGP encodings or not.
            /// </summary>
            public static Stream GetDecoderStream(
                Stream inputStream)
            {
                // TODO Remove this restriction?
                if (!inputStream.CanSeek)
                    throw new ArgumentException("inputStream must be seek-able", "inputStream");

                long markedPos = inputStream.Position;

                int ch = inputStream.ReadByte();
                if ((ch & 0x80) != 0)
                {
                    inputStream.Position = markedPos;

                    return inputStream;
                }
                else
                {
                    if (!IsPossiblyBase64(ch))
                    {
                        inputStream.Position = markedPos;

                        return new ArmoredInputStream(inputStream);
                    }

                    byte[] buf = new byte[ReadAhead];
                    int count = 1;
                    int index = 1;

                    buf[0] = (byte)ch;
                    while (count != ReadAhead && (ch = inputStream.ReadByte()) >= 0)
                    {
                        if (!IsPossiblyBase64(ch))
                        {
                            inputStream.Position = markedPos;

                            return new ArmoredInputStream(inputStream);
                        }

                        if (ch != '\n' && ch != '\r')
                        {
                            buf[index++] = (byte)ch;
                        }

                        count++;
                    }

                    inputStream.Position = markedPos;

                    //
                    // nothing but new lines, little else, assume regular armoring
                    //
                    if (count < 4)
                    {
                        return new ArmoredInputStream(inputStream);
                    }

                    //
                    // test our non-blank data
                    //
                    byte[] firstBlock = new byte[8];
                    Array.Copy(buf, 0, firstBlock, 0, firstBlock.Length);
                    byte[] decoded = Base64.Decode(firstBlock);

                    //
                    // it's a base64 PGP block.
                    //
                    bool hasHeaders = (decoded[0] & 0x80) == 0;

                    return new ArmoredInputStream(inputStream, hasHeaders);
                }
            }
            #endregion
        }

        private sealed class EncryptionKeys
        {
            #region Instance Members (Public)

            public PgpPublicKey PublicKey { get; private set; }
            public IEnumerable<PgpPublicKey> PublicKeys { get; private set; }
            public PgpPrivateKey PrivateKey { get; private set; }
            public PgpSecretKey SecretKey { get; private set; }

            #endregion Instance Members (Public)

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the EncryptionKeys class.
            /// Two keys are required to encrypt and sign data. Your private key and the recipients public key.
            /// The data is encrypted with the recipients public key and signed with your private key.
            /// </summary>
            /// <param name="publicKeyFilePath">The key used to encrypt the data</param>
            /// <param name="privateKeyFilePath">The key used to sign the data.</param>
            /// <param name="passPhrase">The password required to access the private key</param>
            /// <exception cref="ArgumentException">Public key not found. Private key not found. Missing password</exception>
            public EncryptionKeys(string publicKeyFilePath, string privateKeyFilePath, string passPhrase)
            {
                if (String.IsNullOrEmpty(publicKeyFilePath))
                    throw new ArgumentException("PublicKeyFilePath");
                if (String.IsNullOrEmpty(privateKeyFilePath))
                    throw new ArgumentException("PrivateKeyFilePath");
                if (passPhrase == null)
                    throw new ArgumentNullException("Invalid Pass Phrase.");

                if (!File.Exists(publicKeyFilePath))
                    throw new FileNotFoundException(String.Format("Public Key file [{0}] does not exist.", publicKeyFilePath));
                if (!File.Exists(privateKeyFilePath))
                    throw new FileNotFoundException(String.Format("Private Key file [{0}] does not exist.", privateKeyFilePath));

                PublicKey = Utilities.ReadPublicKey(publicKeyFilePath);
                SecretKey = ReadSecretKey(privateKeyFilePath);
                PrivateKey = ReadPrivateKey(passPhrase);
            }

            /// <summary>
            /// Initializes a new instance of the EncryptionKeys class.
            /// Two keys are required to encrypt and sign data. Your private key and the recipients public key.
            /// The data is encrypted with the recipients public key and signed with your private key.
            /// </summary>
            /// <param name="publicKeyFilePath">The key used to encrypt the data</param>
            /// <param name="privateKeyFilePath">The key used to sign the data.</param>
            /// <param name="passPhrase">The password required to access the private key</param>
            /// <exception cref="ArgumentException">Public key not found. Private key not found. Missing password</exception>
            public EncryptionKeys(IEnumerable<string> publicKeyFilePaths, string privateKeyFilePath, string passPhrase)
            {
                //Avoid multiple enumerations of 'publicKeyFilePaths'
                string[] publicKeys = publicKeyFilePaths.ToArray();

                if (String.IsNullOrEmpty(privateKeyFilePath))
                    throw new ArgumentException("PrivateKeyFilePath");
                if (passPhrase == null)
                    throw new ArgumentNullException("Invalid Pass Phrase.");

                if (!File.Exists(privateKeyFilePath))
                    throw new FileNotFoundException(String.Format("Private Key file [{0}] does not exist.", privateKeyFilePath));

                foreach (string publicKeyFilePath in publicKeys)
                {
                    if (String.IsNullOrEmpty(publicKeyFilePath))
                        throw new ArgumentException(nameof(publicKeyFilePath));
                    if (!File.Exists(publicKeyFilePath))
                        throw new FileNotFoundException(String.Format("Input file [{0}] does not exist.", publicKeyFilePath));
                }

                PublicKeys = publicKeys.Select(x => Utilities.ReadPublicKey(x)).ToList();
                SecretKey = ReadSecretKey(privateKeyFilePath);
                PrivateKey = ReadPrivateKey(passPhrase);
            }

            public EncryptionKeys(Stream publicKeyStream, Stream privateKeyStream, string passPhrase)
            {
                if (publicKeyStream == null)
                    throw new ArgumentException("PublicKeyStream");
                if (privateKeyStream == null)
                    throw new ArgumentException("PrivateKeyStream");
                if (passPhrase == null)
                    throw new ArgumentNullException("Invalid Pass Phrase.");

                PublicKey = Utilities.ReadPublicKey(publicKeyStream);
                SecretKey = ReadSecretKey(privateKeyStream);
                PrivateKey = ReadPrivateKey(passPhrase);
            }

            public EncryptionKeys(IEnumerable<Stream> publicKeyStreams, Stream privateKeyStream, string passPhrase)
            {
                //Avoid multiple enumerations of 'publicKeyFilePaths'
                Stream[] publicKeys = publicKeyStreams.ToArray();

                if (privateKeyStream == null)
                    throw new ArgumentException("PrivateKeyStream");
                if (passPhrase == null)
                    throw new ArgumentNullException("Invalid Pass Phrase.");
                foreach (Stream publicKey in publicKeys)
                {
                    if (publicKey == null)
                        throw new ArgumentException("PublicKeyStream");
                }

                PublicKeys = publicKeys.Select(x => Utilities.ReadPublicKey(x)).ToList();
                SecretKey = ReadSecretKey(privateKeyStream);
                PrivateKey = ReadPrivateKey(passPhrase);
            }

            #endregion Constructors

            #region Secret Key

            private PgpSecretKey ReadSecretKey(string privateKeyPath)
            {
                using (Stream sr = File.OpenRead(privateKeyPath))
                {
                    using (Stream inputStream = PgpUtilities.GetDecoderStream(sr))
                    {
                        PgpSecretKeyRingBundle secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                        PgpSecretKey foundKey = GetFirstSecretKey(secretKeyRingBundle);
                        if (foundKey != null)
                            return foundKey;
                    }
                }
                throw new ArgumentException("Can't find signing key in key ring.");
            }

            private PgpSecretKey ReadSecretKey(Stream privateKeyStream)
            {
                using (Stream inputStream = PgpUtilities.GetDecoderStream(privateKeyStream))
                {
                    PgpSecretKeyRingBundle secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                    PgpSecretKey foundKey = GetFirstSecretKey(secretKeyRingBundle);
                    if (foundKey != null)
                        return foundKey;
                }
                throw new ArgumentException("Can't find signing key in key ring.");
            }

            /// <summary>
            /// Return the first key we can use to encrypt.
            /// Note: A file can contain multiple keys (stored in "key rings")
            /// </summary>
            private PgpSecretKey GetFirstSecretKey(PgpSecretKeyRingBundle secretKeyRingBundle)
            {
                foreach (PgpSecretKeyRing kRing in secretKeyRingBundle.GetKeyRings())
                {
                    PgpSecretKey key = kRing.GetSecretKeys()
                        .Cast<PgpSecretKey>()
                        .Where(k => k.IsSigningKey)
                        .FirstOrDefault();
                    if (key != null)
                        return key;
                }
                return null;
            }

            #endregion Secret Key

            #region Public Key

            private PgpPublicKey GetFirstPublicKey(PgpPublicKeyRingBundle publicKeyRingBundle)
            {
                foreach (PgpPublicKeyRing kRing in publicKeyRingBundle.GetKeyRings())
                {
                    PgpPublicKey key = kRing.GetPublicKeys()
                        .Cast<PgpPublicKey>()
                        .Where(k => k.IsEncryptionKey)
                        .FirstOrDefault();
                    if (key != null)
                        return key;
                }
                return null;
            }

            #endregion Public Key

            #region Private Key

            private PgpPrivateKey ReadPrivateKey(string passPhrase)
            {
                PgpPrivateKey privateKey = SecretKey.ExtractPrivateKey(passPhrase.ToCharArray());
                if (privateKey != null)
                    return privateKey;

                throw new ArgumentException("No private key found in secret key.");
            }

            #endregion Private Key
        }
    }
}

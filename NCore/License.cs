using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using Newtonsoft.Json;
using System.Security;
using System.Security.Cryptography;
using MongoDB.Bson;

namespace NCore
{
    public class License
    {
        private static bool _isValid;
        public static bool IsValid
        {
            get { return _isValid; }
        }

        private static List<LicenseKey> licenses;

        public static List<LicenseKey> LicenseKeys
        {
            get
            {
                if (licenses == null) licenses = new List<LicenseKey>();
                return licenses;
            }
        }

        public static string ConfigFile { get; set; }
        public static string KeyFile { get; set; }
        private static byte[] _salt;
        private static string _token;

        public static LicenseKey GetLicenseKey(string LicenseKeyId)
        {
            LicenseKey lm = LicenseKeys.Find(d => d.KeyId.Equals(LicenseKeyId));
            return lm == null ? new LicenseKey(LicenseKeyId, false) : lm;
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static string _saltOrigin;
        private static string _configFolder;
        public static string ConfigFolder
        {
            get
            {
                return _configFolder;
            }

            set
            {
                String folder = value;
                _configFolder = folder;
                if (!Directory.Exists(folder)) throw new Exception(String.Format("Unable to load configuration file. Folder {0} is not exists", folder));

                KeyFile = Path.Combine(folder, "key.ecis");
                ConfigFile = Path.Combine(folder, "config.ecis");

                if (!File.Exists(KeyFile))
                {
                    throw new Exception(String.Format("Invalid configuration key file. {0} is not exists", KeyFile));
                }
                var alltexts = File.ReadAllText(KeyFile).Split(new char[] { '\n' });
                _token = alltexts[0];
                _saltOrigin = alltexts[1];
                _salt = Encoding.ASCII.GetBytes(String.Join("*", Reverse( alltexts[1])));

                if (!File.Exists(ConfigFile))
                {//throw new Exception(String.Format("Invalid configuration file. {0} is not exists", file));
                    FileStream fs = File.Open(ConfigFile, FileMode.OpenOrCreate);
                    fs.Close();
                }
                LoadLicense();
            }
        }

        public static bool Validate(string keyId, string reference1 = "", string reference2 = "", string reference3 = "", int qty = 0)
        {
            if (String.IsNullOrEmpty(keyId)) keyId = "Global";
            bool ret = false;
            var key = GetLicenseKey(keyId);
            if (key != null
                && key.Valid
                && (key.KeyExpired.CompareTo(DateTime.Now) >= 0 || key.KeyExpired.Equals(Tools.DefaultDate)))
            {
                ret = true;
                if (qty > 0 && key.KeyQuantity.CompareTo(qty) < 0) return false;
                if (String.IsNullOrEmpty(reference1) == false && key.Reference1.Equals(reference1) == false) return false;
                if (String.IsNullOrEmpty(reference2) == false && key.Reference1.Equals(reference2) == false) return false;
                if (String.IsNullOrEmpty(reference3) == false && key.Reference1.Equals(reference3) == false) return false;
            }
            return ret;
        }

        public static void LoadLicense()
        {
            if (_configFolder == null) throw new Exception("Please specify configuration folder");

            try
            {
                var configtext = File.ReadAllText(ConfigFile);
                if (configtext != "")
                {
                    var outstr = DecryptStringAES(configtext, _token);
                    licenses = JsonConvert.DeserializeObject<List<LicenseKey>>(outstr);
                }
                //var globalLicKey = GetLicenseKey("Global");
                //if (Validate("Global",_saltOrigin))
                //{
                //    _isValid = true;
                //}
                //else
                //{
                //    _isValid = false;
                //}
            }
            catch (Exception e)
            {
                throw new Exception("Unable to load license => " + Tools.PushException(e, false));
            }
        }

        public static ResultInfo SetLicense(List<LicenseKey> Lics)
        {
            if (_configFolder == null) throw new Exception("Please specify configuration folder");
            licenses = Lics;
            return Save();
        }

        private static ResultInfo Save()
        {
            bool oldIsValid = _isValid;
            _isValid = true;
            ResultInfo ri = new ResultInfo { Result = "OK" };
            try
            {
                string jsonLicense = JsonConvert.SerializeObject(LicenseKeys.FindAll(d => d.Valid == true));
                var outstr = EncryptStringAES(jsonLicense, _token);
                File.WriteAllText(ConfigFile, outstr);
                _isValid = oldIsValid;
            }
            catch (Exception ex)
            {
                ri.PushException(ex);
            }
            return ri;
        }

        public static T GetConfig<T>(string configkey)
        {
            var ret = default(T);
            BsonDocument doc = new BsonDocument();
            return ret;
        }

        public static string Encyp(string plainTex)
        {
            return EncryptStringAES(plainTex, _token);
        }
        public static string Decyp(string cryptedStr)
        {
            return DecryptStringAES(cryptedStr, _token);
        }

        #region "Encryption"
        private static string EncryptStringAES(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        private static string DecryptStringAES(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
        #endregion
    }

    public class LicenseKey
    {
        public String KeyId { get; set; }
        public DateTime KeyExpired { get; set; }
        public String ModuleId { get; set; }
        public String Reference1 { get; set; }
        public String Reference2 { get; set; }
        public String Reference3 { get; set; }
        public Int32 KeyQuantity { get; set; }
        public bool Valid { get; set; }

        public LicenseKey()
        {
            KeyId = "";
            Valid = false;
            KeyExpired = new DateTime(1900, 1, 1);
            ModuleId = "";
            Reference1 = "";
            Reference2 = "";
            Reference3 = "";
            KeyQuantity = 0;
        }

        public LicenseKey(string LicenseKeyId, bool LicenseValid)
        {
            KeyId = LicenseKeyId;
            Valid = LicenseValid;
            KeyExpired = new DateTime(1900, 1, 1);
            ModuleId = "";
            Reference1 = "";
            Reference2 = "";
            Reference3 = "";
            KeyQuantity = 0;
        }
    }
}

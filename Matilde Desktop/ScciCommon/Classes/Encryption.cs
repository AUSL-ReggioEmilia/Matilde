using System;
using System.Security.Cryptography;
using System.Text;

namespace UnicodeSrl.Scci
{

    public class Encryption
    {

        private SymmetricAlgorithm encryptionAlgorithm;
        private string _key = "";

        private string _IV = "";

        public Encryption(string CryptKey, string IV)
        {
            _key = CryptKey;
            _IV = IV;
            encryptionAlgorithm = InitializeEncryptionAlgorithm();

        }

        public string CryptKey
        {
            get { return GetKey(); }
        }

        public string IV
        {
            get { return GetIV(); }
        }

        public string EncryptString(string clearText)
        {


            try
            {
                //The purpose of this function is to take plain text and encrypt it using rijaendel algorithm
                byte[] clearTextBytes = Encoding.UTF8.GetBytes(clearText);
                byte[] encrypted = encryptionAlgorithm.CreateEncryptor().TransformFinalBlock(clearTextBytes, 0, clearTextBytes.Length);
                return Convert.ToBase64String(encrypted);

            }
            catch (Exception)
            {
                return "";
            }

        }

        public string DecryptString(string encryptedEncodedText)
        {

            try
            {
                //The purpose of this function is to take encrypted text and decrypt it using rijaendel algorithm
                byte[] encryptedBytes = Convert.FromBase64String(encryptedEncodedText);
                byte[] decryptedBytes = encryptionAlgorithm.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (FormatException)
            {
                return "";
            }
            catch (CryptographicException)
            {
                return "";
            }

        }

        private SymmetricAlgorithm InitializeEncryptionAlgorithm()
        {
            try
            {
                SymmetricAlgorithm rijaendel = RijndaelManaged.Create();

                rijaendel.Key = GetLegalKey(_key);
                rijaendel.IV = new byte[] {
                    0xf,
                    0x6f,
                    0x13,
                    0x2e,
                    0x35,
                    0xc2,
                    0xcd,
                    0xf9,
                    0x5,
                    0x46,
                    0x9c,
                    0xea,
                    0xa8,
                    0x4b,
                    0x73,
                    0xcc
                };

                return rijaendel;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private string GetKey()
        {
            return Convert.ToBase64String(encryptionAlgorithm.Key);
        }

        private string GetIV()
        {
            return Convert.ToBase64String(encryptionAlgorithm.IV);
        }

        private byte[] GetLegalKey(string KeyString)
        {


            try
            {
                SymmetricAlgorithm rijaendel = RijndaelManaged.Create();
                // Adjust key if necessary, and return a valid key
                if (rijaendel.LegalKeySizes.Length > 0)
                {
                    // Key sizes in bits
                    int keySize = KeyString.Length * 8;
                    int minSize = rijaendel.LegalKeySizes[0].MinSize;
                    int maxSize = rijaendel.LegalKeySizes[0].MaxSize;
                    int skipSize = rijaendel.LegalKeySizes[0].SkipSize;

                    if (keySize > maxSize)
                    {
                        // Extract maximum size allowed
                        KeyString = KeyString.Substring(0, maxSize / 8);

                    }
                    else
                    {
                        // Set valid size
                        int validSize = 0;
                        if ((keySize <= minSize))
                        {
                            validSize = minSize;
                        }
                        else
                        {
                            if ((keySize % skipSize) != 0)
                            {
                                validSize = (keySize - keySize % skipSize) + skipSize;
                            }
                            else
                            {
                                validSize = keySize;
                            }
                        }

                        // Pad the key with asterisk to make up the size
                        KeyString = KeyString.PadRight(validSize / 8, '*');
                    }
                }

                return UTF8Encoding.UTF8.GetBytes(KeyString);

            }
            catch (Exception)
            {
                return null;
            }

        }

    }

}
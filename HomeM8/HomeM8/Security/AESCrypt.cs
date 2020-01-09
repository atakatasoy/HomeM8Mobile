using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HomeM8
{
    public class AESCrypt
    {
        AesCryptoServiceProvider AES;

        public AESCrypt(byte[] Key, byte[] IV)
        {
            AES = new AesCryptoServiceProvider();
            AES.Key = Key;
            AES.IV = IV;
        }

        public string Encrypt(string secretMessage)
        {
            byte[] encryptedMessage;

            using (var cipherText = new MemoryStream())
            {
                using (var encryptor = AES.CreateEncryptor(AES.Key,AES.IV))
                {
                    using (var cryptoStream = new CryptoStream(cipherText, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] ciphertextMessage = Encoding.UTF8.GetBytes(secretMessage);
                        cryptoStream.Write(ciphertextMessage, 0, ciphertextMessage.Length);
                    }
                }

                encryptedMessage = cipherText.ToArray();
            }

            return Convert.ToBase64String(encryptedMessage);
        }
        public string Decrypt(string base64)
        {
            byte[] encryptedMessage = Convert.FromBase64String(base64);
            string decryptedMessage;

            using (var plainText = new MemoryStream())
            {
                using (var decryptor = AES.CreateDecryptor(AES.Key,AES.IV))
                {
                    using (var cryptoStream = new CryptoStream(plainText, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedMessage, 0, encryptedMessage.Length);
                    }
                }

                decryptedMessage = Encoding.UTF8.GetString(plainText.ToArray());
            }

            return decryptedMessage;
        }
    }
}

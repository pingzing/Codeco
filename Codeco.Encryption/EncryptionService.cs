using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Codeco.Encryption
{


    public class EncryptionService : IEncryptionService
    {
        public (string encryptedData, string initializationVector) Encrypt(string data, string password)
        {
            if (String.IsNullOrEmpty(data))
            {
                throw new ArgumentException("The data to encrypt must not be null, or an empty string.", nameof(data));
            }
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException("The password for encrypting data must not be null, or an empty string.", nameof(password));
            }

            byte[] encryptedBytes = null; // This will be filled in after the write operations are complete.
            byte[] ivToReturn = null; // This will be filled in below, and returned at the end.
            using (Aes aes = Aes.Create())
            {                
                aes.Key = Encoding.UTF8.GetBytes(password);                

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(data);
                        }
                    }
                    encryptedBytes = memoryStream.ToArray();
                    ivToReturn = aes.IV;
                }
            }
            string encryptedByteString = Convert.ToBase64String(encryptedBytes);
            string ivString = Convert.ToBase64String(ivToReturn);
            return (encryptedByteString, ivString);
        }

        public string Decrypt(string encryptedData, string password, string initializationVector)
        {
            if (String.IsNullOrEmpty(encryptedData))
            {
                throw new ArgumentException("The data to decrypt must not be null, or an empty string.", nameof(encryptedData));
            }
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException("The password for decrypting data must not be null, or an empty string.", nameof(password));
            }
            if (String.IsNullOrEmpty(initializationVector))
            {
                throw new ArgumentException("The initialization vectory must not be null, or an empty string.", nameof(initializationVector));
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] ivBytes = Convert.FromBase64String(initializationVector);

            string decryptedString = null; // Will be filled in below.
            using (Aes aes = Aes.Create())
            {
                aes.Key = passwordBytes;
                aes.IV = ivBytes;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            decryptedString = reader.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedString;
        }
    }
}

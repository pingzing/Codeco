using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Codeco.Encryption
{


    public class EncryptionService : IEncryptionService
    {
        public (string encryptedData, string salt, string initializationVector) Encrypt(string data, string password)
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
            byte[] saltToReturn = null; // This will be filled in below, and returned at the end.
            byte[] ivToReturn = null;
            using (Aes aes = Aes.Create())
            {
                (byte[] key, byte[] salt) = GetKeyAndSaltFromPassword(password, aes.Key.Length);
                saltToReturn = salt;
                ivToReturn = aes.IV;
                aes.Key = key;

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
                }
            }
            string encryptedByteString = Convert.ToBase64String(encryptedBytes);
            string saltString = Convert.ToBase64String(saltToReturn);
            string ivString = Convert.ToBase64String(ivToReturn);
            return (encryptedByteString, saltString, ivString);
        }

        public string Decrypt(string encryptedData, string password, string salt, string initializationVector)
        {
            if (String.IsNullOrEmpty(encryptedData))
            {
                throw new ArgumentException("The data to decrypt must not be null, or an empty string.", nameof(encryptedData));
            }
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException("The password for decrypting data must not be null, or an empty string.", nameof(password));
            }
            if (String.IsNullOrEmpty(salt))
            {
                throw new ArgumentException("The salt must not be null, or an empty string.", nameof(salt));
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            string decryptedString = null; // Will be filled in below.
            using (Aes aes = Aes.Create())
            {
                (byte[] key, _) = GetKeyAndSaltFromPassword(password, aes.Key.Length, salt);
                byte[] ivBytes = Convert.FromBase64String(initializationVector);
                aes.Key = key;
                aes.IV = ivBytes;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
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

        private (byte[] key, byte[] salt) GetKeyAndSaltFromPassword(string password, int keyLength, string salt = null)
        {
            using (RandomNumberGenerator csrng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[64];
                if (salt == null)
                {                    
                    csrng.GetBytes(saltBytes);
                }
                else
                {
                    saltBytes = Convert.FromBase64String(salt);
                }
                using (Rfc2898DeriveBytes generator = new Rfc2898DeriveBytes(password, saltBytes, 10000))
                {
                    byte[] key = generator.GetBytes(keyLength);
                    return (key, saltBytes);
                }
            }
        }
    }
}

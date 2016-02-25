using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Codeco.Windows10
{
    public static class EncryptionManager
    {
         public static string Encrypt(string plainText, string pw, string iv)
        {
            IBuffer pwBuffer = CryptographicBuffer.ConvertStringToBinary(pw, BinaryStringEncoding.Utf8);
            IBuffer ivBuffer = CryptographicBuffer.ConvertStringToBinary(iv, BinaryStringEncoding.Utf16LE);
            IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf16LE);

            // Derive key material for password size 32 bytes for AES256 algorithm
            KeyDerivationAlgorithmProvider keyDerivationProvider = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            // using iv and 1000 iterations
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(ivBuffer, 1000);

            // create a key based on original key and derivation parmaters
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

            // derive buffer to be used for encryption IV from derived password key
            IBuffer ivMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);
            

            SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            // create symmetric key from derived password key
            CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

            // encrypt data buffer using symmetric key and derived iv material
            IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, plainBuffer, ivMaterial);            
            return CryptographicBuffer.EncodeToBase64String(resultBuffer);
        }

        public static string Decrypt(string encryptedData, string pw, string iv)
        {
            try {
                IBuffer pwBuffer = CryptographicBuffer.ConvertStringToBinary(pw, BinaryStringEncoding.Utf8);
                IBuffer ivBuffer = CryptographicBuffer.ConvertStringToBinary(iv, BinaryStringEncoding.Utf16LE);
                IBuffer cipherBuffer = CryptographicBuffer.DecodeFromBase64String(encryptedData);

                // Derive key material for password size 32 bytes for AES256 algorithm
                KeyDerivationAlgorithmProvider keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
                // using IV and 1000 iterations
                KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(ivBuffer, 1000);

                // create a key based on original key and derivation parmaters
                CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
                IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
                CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

                // derive buffer to be used for encryption iv from derived password key
                IBuffer ivMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

                SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
                // create symmetric key from derived password material
                CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

                // decrypt data buffer using symmetric key and derived IV material
                IBuffer resultBuffer = CryptographicEngine.Decrypt(symmKey, cipherBuffer, ivMaterial);
                string result = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf16LE, resultBuffer);
                return result;
            }
            catch(System.Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to decrypt file. Throwing exception up the chain.");
                throw;
            }
        }        
    }
}

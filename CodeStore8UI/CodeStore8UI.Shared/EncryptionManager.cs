using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace CodeStore8UI
{
    public static class EncryptionManager
    {
        public static string Encrypt(String input, string password)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be empty.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            password = PadPassword(password);
            IBuffer iv = CreateInitializationVector(password);
            CryptographicKey key = CreateKey(password);
            var encryptedBuffer = CryptographicEngine.Encrypt(
                key,
                CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8),
                iv);
            return CryptographicBuffer.EncodeToBase64String(encryptedBuffer);
        }

        public static string Decrypt(string input, string password)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be empty.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            password = PadPassword(password);
            IBuffer iv = CreateInitializationVector(password);
            CryptographicKey key = CreateKey(password);
            var decryptedBuffer = CryptographicEngine.Decrypt(key,
                CryptographicBuffer.DecodeFromBase64String(input),
                iv);
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decryptedBuffer);
        }

        private static IBuffer CreateInitializationVector(string password)
        {
            password = PadPassword(password);
            var provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            IBuffer iv = CryptographicBuffer.CreateFromByteArray(UTF8Encoding.UTF8.GetBytes(password));
            return iv;
        }


        private static CryptographicKey CreateKey(string password)
        {
            password = PadPassword(password);
            var provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var buffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            var key = provider.CreateSymmetricKey(buffer);
            return key;
        }

        private static string PadPassword(string password)
        {
            uint passMultiple = 32;
            uint passByteLength = (uint)password.ToCharArray().Length * sizeof(char);
            //Password byte-length must be a multiple of 32, so let's pad it if it's not
            while (passByteLength < GetNearestMultipleOf(passMultiple, passByteLength))
            {
                password += password;
                passByteLength = (uint)password.ToCharArray().Length * sizeof(char);
            }

            password = new string(password
                .ToCharArray()
                .Take((int)GetNearestMultipleOf(passMultiple, (uint)password.Length)) //Clamp length to a multiple of passMultiple.
                .ToArray());

            return password;
        }

        private static decimal GetNearestMultipleOf(uint multiple, uint value)
        {
            if(value < multiple)
            {
                return multiple;
            }

            uint rounded = ((value + multiple - 1) / multiple) * multiple;
            return Math.Max(rounded, multiple);
        }
    }
}

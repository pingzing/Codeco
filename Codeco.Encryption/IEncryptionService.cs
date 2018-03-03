using System;
using System.Collections.Generic;
using System.Text;

namespace Codeco.Encryption
{
    public interface IEncryptionService
    {
        (string encryptedData, string salt, string initializationVector) Encrypt(string data, string password);
        string Decrypt(string encryptedData, string password, string salt, string initializationVector);
    }
}

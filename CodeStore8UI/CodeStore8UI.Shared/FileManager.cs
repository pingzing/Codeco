using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeStore8UI
{
    //TODO: Add an async lock on these two methods. Otherwise race cases yaaaay
    public static class FileManager
    {
        private const string MASTER_FILE_KEY = "MasterFile.txt";

        private static readonly StorageFolder _localFolder = ApplicationData.Current.RoamingFolder;        

        /// <summary>
        /// Encrypts, and adds the contents to the end of the master file.
        /// </summary>
        /// <param name="encryptedContents"></param>
        /// <returns></returns>
        public static async Task AppendToEncryptedFile(string contents, string password)
        {
            StorageFile file = await _localFolder.CreateFileAsync(MASTER_FILE_KEY, CreationCollisionOption.OpenIfExists);
            string encryptedContents = EncryptionManager.Encrypt(contents, password);
            await FileIO.AppendTextAsync(file, encryptedContents);
        }

        /// <summary>
        /// Decrypts and returns the contents of the master file. Returns null if the file is empty, or full only of whitespace.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<string> RetrieveEncryptedFile(string password)
        {
            StorageFile file = await _localFolder.CreateFileAsync(MASTER_FILE_KEY, CreationCollisionOption.OpenIfExists);
            string encryptedContents = await FileIO.ReadTextAsync(file);
            if(String.IsNullOrWhiteSpace(encryptedContents))
            {
                return null;
            }
            string contents = EncryptionManager.Decrypt(encryptedContents, password);
            return contents;
        }
        
        /// <summary>
        /// Erases the contents of the master file, and replaces them with an empty string.
        /// </summary>
        /// <returns></returns>
        public static async Task ClearFile()
        {
            StorageFile file = await _localFolder.CreateFileAsync(MASTER_FILE_KEY, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, "");
        }        
    }
}

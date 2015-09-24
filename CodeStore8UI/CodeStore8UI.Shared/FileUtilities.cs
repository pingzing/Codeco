using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Linq;
using CodeStore8UI.Model;
using CodeStore8UI.Common;
using System.Xml.Serialization;
using System.IO;

namespace CodeStore8UI
{
    //TODO: Add an async lock on these methods. Otherwise race cases yaaaay
    public static class FileUtilities
    {      
        private static readonly StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        private static readonly StorageFolder _roamingFolder = ApplicationData.Current.RoamingFolder;  

        /// <summary>
        /// Attempts to save the given contents with the given filename, and encrypt it with the given password.
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="fileName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<string> SaveAndEncryptFileAsync(string contents, string fileName, string password, string salt)
        {
            if (contents.Length > 0)
            {
                string encryptedContents = EncryptionManager.Encrypt(contents, password, salt);
                var result = await _localFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                await FileIO.WriteTextAsync(result, encryptedContents);
                return result.Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the given encrypted StorageFile.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<StorageFile> GetEncryptedFileAsync(string fileName)
        {
            return await _localFolder.GetFileAsync(fileName);
        }

        /// <summary>
        /// Encrypts, and adds the contents to the end of the file.
        /// </summary>
        /// <param name="encryptedContents"></param>
        /// <returns></returns>
        public static async Task AppendToEncryptedFileAsync(string fileName, string contents, string password, string salt)
        {
            StorageFile file = await _localFolder.GetFileAsync(fileName);
            string encryptedContents = EncryptionManager.Encrypt(contents, password, salt);
            await FileIO.AppendTextAsync(file, encryptedContents);
        }        

        //TODO: Consider directly returning a List<string>?
        /// <summary>
        /// Decrypts and returns the contents of the file. Returns null if the file is empty, or full only of whitespace.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>A string of the files contents.</returns>
        public static async Task<string> RetrieveFileContentsAsync(string fileName, string password, string salt)
        {
            StorageFile file = await _localFolder.GetFileAsync(fileName);
            string encryptedContents = await FileIO.ReadTextAsync(file);
            if(String.IsNullOrWhiteSpace(encryptedContents))
            {
                return null;
            }
            string contents = EncryptionManager.Decrypt(encryptedContents, password, salt);
            return contents;
        }

        public static async Task MoveFileToRoamingAsync(StorageFile backingFile)
        {
            await backingFile.MoveAsync(_roamingFolder, backingFile.Name, NameCollisionOption.GenerateUniqueName);
            
        }

        public static async Task MoveFileToLocalAsync(StorageFile backingFile)
        {
            await backingFile.MoveAsync(_localFolder, backingFile.Name, NameCollisionOption.GenerateUniqueName);
        }

        /// <summary>
        /// Overwrites the contents of the given file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task ClearFileAsync(string fileName)
        {
            StorageFile file = await _localFolder.GetFileAsync(fileName);
            await FileIO.WriteTextAsync(file, "");
        }

        public static async Task<bool> DeleteFileAsync(string fileName)
        {
            StorageFile file = await _localFolder.GetFileAsync(fileName);
            return await DeleteFileAsync(file);
        }

        public static async Task<bool> DeleteFileAsync(StorageFile file)
        {
            try
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to delete file: " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Ensures that the given file conforms to the formatting constraints (two-column csv)
        /// </summary>
        /// <param name="file">The file to investigate.</param>
        /// <returns>True if formatted properly, false otherwise.</returns>
        public static async Task<bool> ValidateFileAsync(StorageFile file)
        {
            IList<string> lines = (await FileIO.ReadLinesAsync(file))
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .ToList();
            if(lines.Count == 0)
            {
                return false;
            }
            foreach(string line in lines)
            {
                string[] splitString = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if(splitString.Length != 2)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get all the saved files.
        /// </summary>
        /// <returns></returns>
        public static async Task<SavedFiles> GetFilesAsync()
        {
            SavedFiles allFiles = new SavedFiles
            {
                //Ignore files beginning with underscores. This allows us to use those as config (etc) files.
                LocalFiles = (await _localFolder.GetFilesAsync()).Where(x => x.Name[0] != '_'),
                RoamingFiles = (await _roamingFolder.GetFilesAsync()).Where(x => x.Name[0] != '_')
            };
            return allFiles;
        }

        public static async Task RenameFileAsync(StorageFile backingFile, string newName)
        {
            await backingFile.RenameAsync(newName, NameCollisionOption.GenerateUniqueName);
        }

        public static async Task<Dictionary<string, string>> GetSaltFile()
        {
            var saltFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(Constants.SALT_FILE_NAME, CreationCollisionOption.OpenIfExists);
            string json = await FileIO.ReadTextAsync(saltFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static async Task SaveSaltFile(Dictionary<string, string> saltDict)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(saltDict);
            var saltFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(Constants.SALT_FILE_NAME, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(saltFile, json);
        }

        public static async Task<ulong> GetSaltFileSize()
        {
            StorageFile file = await ApplicationData.Current.RoamingFolder.GetFileAsync(Constants.SALT_FILE_NAME);
            var props = await file.GetBasicPropertiesAsync();
            return props.Size;
        }
    }
}

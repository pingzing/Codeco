using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Linq;
using Codeco.Model;
using Codeco.Common;
using System.Xml.Serialization;
using System.IO;

namespace Codeco
{    
    public static class FileUtilities
    {      
        private static readonly StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        private static readonly StorageFolder _roamingFolder = ApplicationData.Current.RoamingFolder;          

        /// <summary>
        /// Returns the given encrypted StorageFile.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<StorageFile> GetEncryptedFileAsync(string fileName)
        {
            return await _localFolder.GetFileAsync(fileName);
        }         

        public static async Task MoveFileToRoamingAsync(StorageFile backingFile)
        {
            await backingFile.MoveAsync(_roamingFolder, backingFile.Name, NameCollisionOption.GenerateUniqueName);
            
        }

        public static async Task MoveFileToLocalAsync(StorageFile backingFile)
        {
            await backingFile.MoveAsync(_localFolder, backingFile.Name, NameCollisionOption.GenerateUniqueName);
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

        public static async Task<Dictionary<string, string>> GetIVFile()
        {
            var ivFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            string json = await FileIO.ReadTextAsync(ivFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        internal static IStorageFolder GetLocalFolder()
        {
            return _localFolder;
        }

        public static async Task SaveIVFile(Dictionary<string, string> saltDict)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(saltDict);
            var saltFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(saltFile, json);
        }

        public static async Task<ulong> GetIVFileSize()
        {
            StorageFile file = await ApplicationData.Current.RoamingFolder.GetFileAsync(Constants.IV_FILE_NAME);
            var props = await file.GetBasicPropertiesAsync();
            return props.Size;
        }
    }
}

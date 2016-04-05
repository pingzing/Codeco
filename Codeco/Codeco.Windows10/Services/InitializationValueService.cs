using Codeco.Windows10.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Codeco.Windows10.Services
{
    public class InitializationValueService : IInitializationValueService
    {
        bool loaded = false;

        private Dictionary<string, string> _localIvDict = new Dictionary<string, string>();
        private Dictionary<string, string> _roamingIvDict = new Dictionary<string, string>();

        public async Task Add(string fileName, string iv, FileService.FileLocation location)
        {
            if (!loaded)
            {
                await LoadFromStorage();
            }
            if (location == FileService.FileLocation.Local)
            {
                _localIvDict.Add(fileName, iv);
            }
            else
            {
                _roamingIvDict.Add(fileName, iv);
            }
            await SaveToStorage(location);
        }

        public async Task Remove(string key, FileService.FileLocation location)
        {
            if (!loaded)
            {
                await LoadFromStorage();
            }
            if (location == FileService.FileLocation.Local)
            {
                _localIvDict.Remove(key);
            }
            else
            {
                _roamingIvDict.Remove(key);
            }
            await SaveToStorage(location);
        }

        public async Task<bool> ContainsKey(string key, FileService.FileLocation location)
        {
            if(!loaded)
            {
                await LoadFromStorage();
            }
            if(location == FileService.FileLocation.Local)
            {
                return _localIvDict.ContainsKey(key);
            }
            else
            {
                return _roamingIvDict.ContainsKey(key);
            }
        }

        public async Task<string> GetValue(string key, FileService.FileLocation location)
        {
            if (!loaded)
            {
                await LoadFromStorage();
            }
            if (location == FileService.FileLocation.Local)
            {
                return _localIvDict[key];
            }
            else
            {
                return _roamingIvDict[key];
            }
        }

        public async Task LoadFromStorage()
        {
            var roamingFolder = ApplicationData.Current.RoamingFolder;
            var localFolder = ApplicationData.Current.LocalFolder;

            var localFile = await localFolder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            string localJson = await FileIO.ReadTextAsync(localFile);
            var localIvs = JsonConvert.DeserializeObject<Dictionary<string, string>>(localJson);
            _localIvDict = localIvs ?? _localIvDict;

            var roamingFile = await roamingFolder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            string roamingJson = await FileIO.ReadTextAsync(roamingFile);
            var roamingIvs = JsonConvert.DeserializeObject<Dictionary<string, string>>(roamingJson);
            _roamingIvDict = roamingIvs ?? _roamingIvDict;

            loaded = true;
        }

        /// <summary>
        /// Gets the IV file's current size in bytes.
        /// </summary>
        /// <returns></returns>
        public async Task<ulong> GetIVFileSize(FileService.FileLocation location)
        {
            var folder = location == FileService.FileLocation.Local
                ? ApplicationData.Current.LocalFolder
                : ApplicationData.Current.RoamingFolder;

            StorageFile file = await folder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            var props = await file.GetBasicPropertiesAsync();
            return props.Size;
        }

        public async Task SaveToStorage(FileService.FileLocation location)
        {
            var folder = location == FileService.FileLocation.Local
                ? ApplicationData.Current.LocalFolder
                : ApplicationData.Current.RoamingFolder;

            string json = location == FileService.FileLocation.Local
                ? JsonConvert.SerializeObject(_localIvDict)
                : JsonConvert.SerializeObject(_roamingIvDict);
            var ivFile = await folder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(ivFile, json);
        }
    }
}

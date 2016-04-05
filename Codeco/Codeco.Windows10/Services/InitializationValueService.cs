using Codeco.Windows10.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Codeco.Windows10.Services
{
    public class InitializationValueService : IInitializationValueService
    {
        bool loaded = false;

        private Dictionary<string, string> _fileNameIVDict = new Dictionary<string, string>();        

        public async Task AddKeyPair(string fileName, string iv)
        {
            if (!loaded)
            {
                await LoadFromStorage();
            }
            _fileNameIVDict.Add(fileName, iv);
            await SaveToStorage();
        }

        public async Task RemoveKeyPair(string key)
        {
            if(!loaded)
            {
                await LoadFromStorage();
            }
            _fileNameIVDict.Remove(key);
            await SaveToStorage();
        }

        public async Task<string> GetValue(string key)
        {
            if(!loaded)
            {
                await LoadFromStorage();
            }
            return _fileNameIVDict[key];
        }

        public async Task LoadFromStorage()
        {            
            var ivFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            string json = await FileIO.ReadTextAsync(ivFile);
            var ivs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            _fileNameIVDict = ivs ?? _fileNameIVDict;
            loaded = true;
        }        

        /// <summary>
        /// Gets the IV file's current size in bytes.
        /// </summary>
        /// <returns></returns>
        public async Task<ulong> GetIVFileSize()
        {
            StorageFile file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            var props = await file.GetBasicPropertiesAsync();
            return props.Size;
        }              

        public async Task SaveToStorage()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(_fileNameIVDict);
            var saltFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(Constants.IV_FILE_NAME, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(saltFile, json);
        }
    }
}

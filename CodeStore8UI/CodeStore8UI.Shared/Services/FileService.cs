using CodeStore8UI.Common;
using CodeStore8UI.Model;
using CodeStore8UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace CodeStore8UI
{
    public class FileService : ServiceBase
    {
        private bool _initialized = false;
        private AsyncLock f_lock = new AsyncLock();
        private List<IBindableStorageFile> _localFiles;
        private List<IBindableStorageFile> _roamedFiles;

        public enum FileLocation { Local, Roamed };                

        public async Task StopRoamingFile(IBindableStorageFile file)
        {
            using (await f_lock.Acquire())
            {
                if (!_localFiles.Contains(file))
                {
                    //UI                
                    file.IsRoamed = false;                    

                    //Backing values
                    await FileUtilities.MoveFileToRoamingAsync((StorageFile)file.BackingFile);

                    _roamedFiles.Remove(file);
                    _localFiles.Add(file);
                }
            }
        }

        public async Task RoamFile(IBindableStorageFile file)
        {
            using (await f_lock.Acquire())
            {
                if (!_roamedFiles.Contains(file))
                {
                    //UI
                    file.IsRoamed = true;

                    //Backing values
                    await FileUtilities.MoveFileToLocalAsync((StorageFile)file.BackingFile);

                    _localFiles.Remove(file);
                    _roamedFiles.Add(file);
                }
            }
        }

        protected async override Task CreateAsync()
        {
            if(_initialized)
            {
                return;
            }
            ApplicationData.Current.DataChanged += OnRoamingDataChanged;
            _localFiles = new List<IBindableStorageFile>();
            _roamedFiles = new List<IBindableStorageFile>();

            var allFiles = await FileUtilities.GetFilesAsync();
            foreach(var local in allFiles.LocalFiles)
            {
                var bsf = await BindableStorageFile.Create(local);
                _localFiles.Add(bsf);
            }
            foreach(var roamed in allFiles.RoamingFiles)
            {
                if (roamed.Name == Constants.SALT_FILE_NAME) continue;
                var bsf = await BindableStorageFile.Create(roamed);
                _roamedFiles.Add(bsf);
            }
                        
            _initialized = true;
        }        

        public IReadOnlyList<IBindableStorageFile> GetLocalFiles()
        {
            return _localFiles;
        }

        public IReadOnlyList<IBindableStorageFile> GetRoamedFiles()
        {
            return _roamedFiles;
        }

        /// <summary>
        /// Saves and encrypts the contents into a StorageFile with the given name, and the given password.
        /// </summary>
        /// <param name="contents">The contents to encrypt.</param>
        /// <param name="fileName">The filename with which to save the file.</param>
        /// <param name="password">The password that will be used to generate the encryption key.</param>
        /// <returns>If successful, returns the created StorageFile.</returns>
        public async Task<BindableStorageFile> SaveAndEncryptFileAsync(string contents, string fileName, string password)
        {
            using (await f_lock.Acquire())
            {
                if (!_initialized)
                {
                    throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
                }

                string salt = Guid.NewGuid().ToString("N");

                string savedFileName = await FileUtilities.SaveAndEncryptFileAsync(contents, fileName, password, salt);
                StorageFile savedFile = await FileUtilities.GetEncryptedFileAsync(savedFileName);
                BindableStorageFile bsf = await BindableStorageFile.Create(savedFile);                

                SaltStorage salts = new SaltStorage();
                await salts.LoadFromStorage();
                salts.FileNameSaltDict.Add(bsf.BackingFile.Name, salt);
                await salts.SaveToStorage();

                return bsf;
            }
        }        

        internal async Task<string> RetrieveFileContentsAsync(string fileName, string password, FileLocation location)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                var collectionToSearch = location == FileLocation.Local ? _localFiles : _roamedFiles;
                string encryptedContents = await FileIO.ReadTextAsync(collectionToSearch.Single(x => x.Name == fileName).BackingFile);
                if (String.IsNullOrWhiteSpace(encryptedContents))
                {
                    return null;
                }
                SaltStorage salts = new SaltStorage();
                await salts.LoadFromStorage();
                string salt = salts.FileNameSaltDict[fileName];
                return EncryptionManager.Decrypt(encryptedContents, password, salt);
            }
        }

        internal async Task ClearFileAsync(string name, FileLocation location)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                var collectionToSearch = location == FileLocation.Local ? _localFiles : _roamedFiles;
                var file = collectionToSearch.Single(x => x.Name == name);
                await FileIO.WriteTextAsync(file.BackingFile, string.Empty);
            }
        }

        internal async Task DeleteFileAsync(StorageFile backingFile, FileLocation location)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                string fileName = backingFile.Name;
                var collectionToSearch = location == FileLocation.Local ? _localFiles : _roamedFiles;
                collectionToSearch.Remove(collectionToSearch.Single(x => x.BackingFile == backingFile));
                await FileUtilities.DeleteFileAsync(backingFile);

                SaltStorage salts = new SaltStorage();
                await salts.LoadFromStorage();
                salts.FileNameSaltDict.Remove(fileName);
                await salts.SaveToStorage();
            }
        }

        internal async Task RenameFileAsync(IBindableStorageFile file, string newName)
        {
            if(!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                string oldName = file.BackingFile.Name;
                await FileUtilities.RenameFileAsync((StorageFile)file.BackingFile, newName);
                file.NameChanged();

                SaltStorage salts = new SaltStorage();
                await salts.LoadFromStorage();
                string salt = salts.FileNameSaltDict[oldName];
                salts.FileNameSaltDict.Remove(oldName);
                salts.FileNameSaltDict.Add(newName, salt);
                await salts.SaveToStorage();
            }
        }

        public async Task NukeFiles()
        {
            if(!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                for (int i = _localFiles.Count - 1; i >= 0; i--)
                {
                    await DeleteFileAsync((StorageFile)_localFiles[i].BackingFile, FileLocation.Local);
                }
                for (int i = _localFiles.Count - 1; i >= 0; i--)
                {
                    await DeleteFileAsync((StorageFile)_roamedFiles[i].BackingFile, FileLocation.Roamed);
                }

                SaltStorage salts = new SaltStorage();
                await salts.LoadFromStorage();
                salts.FileNameSaltDict.Clear();
                await salts.SaveToStorage();
            }
        }

        private async void OnRoamingDataChanged(ApplicationData sender, object args)
        {
            using (await f_lock.Acquire())
            {
                foreach (var file in _localFiles)
                {
                    if (file.IsRoamed)
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () => _localFiles.Remove(file));
                    }
                }

                var roamingFiles = await sender.RoamingFolder.GetFilesAsync();
                foreach (var file in roamingFiles.Where(f => f.Name != Constants.SALT_FILE_NAME))
                {
                    BindableStorageFile bsf = await BindableStorageFile.Create(file);
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () => _localFiles.Add(bsf));
                }
            }
        }                

        internal FileLocation GetFileLocation(BindableStorageFile file)
        {            
            return file.IsRoamed ? FileLocation.Roamed : FileLocation.Local;
        }
    }
}

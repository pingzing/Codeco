using Codeco.Windows10.Common;
using Codeco.Windows10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace Codeco.Windows10.Services
{
    public class FileService : IFileService
    {
        private bool _initialized = false;
        private AsyncLock f_lock = new AsyncLock();
        private List<IBindableStorageFile> _localFiles;
        private List<IBindableStorageFile> _roamedFiles;

        public enum FileLocation { Local, Roamed };

        public TaskCompletionSource<bool> IsInitialized { get; } = new TaskCompletionSource<bool>();

        public FileService()
        {
            CreateAsync();
        }

        public async Task StopRoamingFile(IBindableStorageFile file)
        {
            using (await f_lock.Acquire())
            {
                if (!_localFiles.Contains(file))
                {                                      
                    //Backing values
                    await FileUtilities.MoveFileToLocalAsync((StorageFile)file.BackingFile);

                    _roamedFiles.Remove(file);

                    //UI                
                    file.IsRoamed = false;

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
                    //Backing values
                    await FileUtilities.MoveFileToRoamingAsync((StorageFile)file.BackingFile);

                    _localFiles.Remove(file);

                    //UI
                    file.IsRoamed = true;

                    _roamedFiles.Add(file);                    
                }
            }
        }

        private async Task CreateAsync()
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

                if (roamed.Name == Constants.IV_FILE_NAME) continue;
                var bsf = await BindableStorageFile.Create(roamed);
                bsf.IsRoamed = true;                          
                _roamedFiles.Add(bsf);
            }
                        
            _initialized = true;
            IsInitialized.SetResult(true);
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
                if (_initialized)
                {
                    throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
                }
                if(contents == null)
                {
                    throw new ArgumentException("File contents cannot be null.");
                }

                string iv = Guid.NewGuid().ToString("N");
                
                string encryptedContents = EncryptionManager.Encrypt(contents, password, iv);
                var savedFile = await FileUtilities.GetLocalFolder().CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                await FileIO.WriteTextAsync(savedFile, encryptedContents);
                string savedFileName = savedFile.Name;
                
                BindableStorageFile bsf = await BindableStorageFile.Create(savedFile);                
                _localFiles.Add(bsf);

                IVStorage ivs = new IVStorage();
                await ivs.LoadFromStorage();
                ivs.FileNameIVDict.Add(bsf.BackingFile.Name, iv);
                await ivs.SaveToStorage();

                return bsf;
            }
        }        

        public async Task<string> RetrieveFileContentsAsync(string fileName, string password, FileLocation location)
        {
            if (_initialized)
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

                IVStorage ivs = new IVStorage();
                await ivs.LoadFromStorage();
                string iv = ivs.FileNameIVDict[fileName];
                try
                {
                    return EncryptionManager.Decrypt(encryptedContents, password, iv);
                }
                catch(Exception ex)
                {
                    //Just explicitly noting that .Decrypt() can and WILL throw.
                    throw;
                }
            }

        }

        public async Task ClearFileAsync(string name, FileLocation location)
        {
            if (_initialized)
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

        public async Task DeleteFileAsync(StorageFile backingFile, FileLocation location)
        {
            if (_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                string fileName = backingFile.Name;
                var collectionToSearch = location == FileLocation.Local ? _localFiles : _roamedFiles;
                collectionToSearch.Remove(collectionToSearch.Single(x => x.BackingFile == backingFile));
                await FileUtilities.DeleteFileAsync(backingFile);

                IVStorage ivs = new IVStorage();
                await ivs.LoadFromStorage();
                ivs.FileNameIVDict.Remove(fileName);
                await ivs.SaveToStorage();
            }
        }

        public async Task RenameFileAsync(IBindableStorageFile file, string newName)
        {
            if(_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                string oldName = file.BackingFile.Name;
                await FileUtilities.RenameFileAsync((StorageFile)file.BackingFile, newName);
                file.NameChanged();

                IVStorage ivs = new IVStorage();
                await ivs.LoadFromStorage();
                string iv = ivs.FileNameIVDict[oldName];
                ivs.FileNameIVDict.Remove(oldName);
                ivs.FileNameIVDict.Add(newName, iv);
                await ivs.SaveToStorage();
            }
        }

        public async Task NukeFiles()
        {
            if(_initialized)
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


            IVStorage ivs = new IVStorage();
            await ivs.LoadFromStorage();
            ivs.FileNameIVDict.Clear();
            await ivs.SaveToStorage();
            }
        }

        //TODO: Fix this to actually update lists in the various views somehow. Maybe each view should be responsible for subscribing to it?
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
                foreach (var file in roamingFiles.Where(f => f.Name != Constants.IV_FILE_NAME))
                {
                    BindableStorageFile bsf = await BindableStorageFile.Create(file);
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () => _localFiles.Add(bsf));
                }
            }
        }                

        public FileLocation GetFileLocation(BindableStorageFile file)
        {            
            return file.IsRoamed ? FileLocation.Roamed : FileLocation.Local;
        }
    }
}

using CodeStore8UI.Common;
using CodeStore8UI.Model;
using CodeStore8UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CodeStore8UI
{
    public class FileService : ServiceBase
    {
        private bool _initialized = false;

        public enum FileLocation { Local, Roamed };

        private ObservableCollection<IBindableStorageFile> _localFiles;        
        public ObservableCollection<IBindableStorageFile> LocalFiles
        {
            get
            {
                if (_initialized)
                {
                    return _localFiles;
                }
                else
                {
                    throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
                }
            }
            private set
            {
                if(_localFiles == value)
                {
                    return;
                }
                if(!_initialized)
                {
                    throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
                }

                _localFiles = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IBindableStorageFile> _roamedFiles;
        public ObservableCollection<IBindableStorageFile> RoamedFiles
        {
            get
            {
                if(_initialized)
                {
                    return _roamedFiles;
                }
                else
                {
                    throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
                }                
            }
            private set
            {
                if(_roamedFiles == value)
                {
                    return;
                }
                if(!_initialized)
                {
                    throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
                }

                _roamedFiles = value;
                RaisePropertyChanged();
            }
        }

        public async Task StopRoamingFile(BindableStorageFile file)
        {           
            //UI
            LocalFiles.Add(file);
            file.IsRoamed = false;
            RoamedFiles.Remove(file);

            //Backing values
            await FileUtilities.MoveFileToRoamingAsync((StorageFile)file.BackingFile);            
        }

        public async Task RoamFile(BindableStorageFile file)
        {            
            //UI
            RoamedFiles.Add(file);
            file.IsRoamed = true;
            LocalFiles.Remove(file);

            //Backing values
            await FileUtilities.MoveFileToLocalAsync((StorageFile)file.BackingFile);            
        }

        protected async override Task CreateAsync()
        {
            if(_initialized)
            {
                return;
            }
            ApplicationData.Current.DataChanged += OnRoamingDataChanged;

            var files = await FileUtilities.GetFilesAsync();            
            List<BindableStorageFile> localFiles = new List<BindableStorageFile>();
            foreach(var file in files.LocalFiles)
            {
                BindableStorageFile localFile = await BindableStorageFile.Create(file);
                localFiles.Add(localFile);
            }

            List<BindableStorageFile> roamedFiles = new List<BindableStorageFile>();
            foreach (var file in files.RoamingFiles)
            {
                if (file.Name == Constants.IV_FILE_NAME) continue;
                BindableStorageFile roamedFile = await BindableStorageFile.Create(file);
                roamedFile.IsRoamed = true;
                roamedFiles.Add(roamedFile);
            }
            
            _localFiles = new ObservableCollection<IBindableStorageFile>(localFiles);
            RaisePropertyChanged(nameof(LocalFiles));
            _roamedFiles = new ObservableCollection<IBindableStorageFile>(roamedFiles);
            RaisePropertyChanged(nameof(RoamedFiles));
            _initialized = true;
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
            if(!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            string iv = Guid.NewGuid().ToString("N");

            string savedFileName = await FileUtilities.SaveAndEncryptFileAsync(contents, fileName, password, iv);
            StorageFile savedFile = await FileUtilities.GetEncryptedFileAsync(savedFileName);
            BindableStorageFile bsf = await BindableStorageFile.Create(savedFile);
            LocalFiles.Add(bsf);

            IVStorage ivs = new IVStorage();
            await ivs.LoadFromStorage();            
            ivs.FileNameIVDict.Add(bsf.BackingFile.Name, iv);
            await ivs.SaveToStorage();

            return bsf;
        }

        public IBindableStorageFile GetLoadedFile(string savedFileName)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            return LocalFiles.Where(x => x.Name == savedFileName).Single();
        }

        internal async Task<string> RetrieveFileContentsAsync(string fileName, string password, FileLocation location)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            var collectionToSearch = location == FileLocation.Local ? LocalFiles : RoamedFiles;
            string encryptedContents = await FileIO.ReadTextAsync(collectionToSearch.Single(x => x.Name == fileName).BackingFile);
            if(String.IsNullOrWhiteSpace(encryptedContents))
            {
                return null;
            }
            IVStorage ivs = new IVStorage();
            await ivs.LoadFromStorage();
            string iv = ivs.FileNameIVDict[fileName];            
            return EncryptionManager.Decrypt(encryptedContents, password, iv);
        }

        internal async Task ClearFileAsync(string name, FileLocation location)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            var collectionToSearch = location == FileLocation.Local ? LocalFiles : RoamedFiles;
            var file = collectionToSearch.Single(x => x.Name == name);
            await FileIO.WriteTextAsync(file.BackingFile, string.Empty);
        }

        internal async Task DeleteFileAsync(StorageFile backingFile, FileLocation location)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            string fileName = backingFile.Name;
            var collectionToSearch = location == FileLocation.Local ? LocalFiles : RoamedFiles;
            collectionToSearch.Remove(collectionToSearch.Single(x => x.BackingFile == backingFile)); 
            await FileUtilities.DeleteFileAsync(backingFile);

            IVStorage ivs = new IVStorage();
            await ivs.LoadFromStorage();
            ivs.FileNameIVDict.Remove(fileName);
            await ivs.SaveToStorage();
        }

        internal async Task RenameFileAsync(IBindableStorageFile file, string newName)
        {
            if(!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
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

        public async Task NukeFiles()
        {
            if(!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            for(int i = LocalFiles.Count - 1; i >= 0; i--)
            {
                await DeleteFileAsync((StorageFile)LocalFiles[i].BackingFile, FileLocation.Local);
            }
            for (int i = LocalFiles.Count - 1; i >= 0; i--)
            {
                await DeleteFileAsync((StorageFile)RoamedFiles[i].BackingFile, FileLocation.Roamed);
            }

            IVStorage ivs = new IVStorage();
            await ivs.LoadFromStorage();
            ivs.FileNameIVDict.Clear();
            await ivs.SaveToStorage();
        }

        private async void OnRoamingDataChanged(ApplicationData sender, object args)
        {
            foreach(var file in LocalFiles)
            {
                if(file.IsRoamed)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, 
                        () => LocalFiles.Remove(file));
                }
            }

            var roamingFiles = await sender.RoamingFolder.GetFilesAsync();
            foreach(var file in roamingFiles.Where(f => f.Name != Constants.IV_FILE_NAME))
            {
                BindableStorageFile bsf = await BindableStorageFile.Create(file);
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () => LocalFiles.Add(bsf));
            }
        }                

        internal FileLocation GetFileLocation(BindableStorageFile file)
        {            
            return file.IsRoamed ? FileLocation.Roamed : FileLocation.Local;
        }
    }
}

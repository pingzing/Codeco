using CodeStore8UI.Model;
using CodeStore8UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeStore8UI
{
    public class FileService : ServiceBase
    {
        private ObservableCollection<BindableStorageFile> _loadedFiles;        

        public ObservableCollection<BindableStorageFile> LoadedFiles
        {
            get
            {
                if (_initialized)
                {
                    return _loadedFiles;
                }
                else
                {
                    throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
                }
            }
            set
            {
                if(_loadedFiles == value)
                {
                    return;
                }
                if(!_initialized)
                {
                    throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
                }

                _loadedFiles = value;
                RaisePropertyChanged();
            }
        }
        private bool _initialized = false;

        protected async override Task CreateAsync()
        {
            if(_initialized)
            {
                return;
            }
            ApplicationData.Current.DataChanged += OnRoamingDataChanged;

            var files = await FileUtilities.GetFilesAsync();            
            List<BindableStorageFile> allFiles = new List<BindableStorageFile>();
            foreach(var file in files.LocalFiles)
            {
                BindableStorageFile localFile = await BindableStorageFile.Create(file);
                allFiles.Add(localFile);
            }
            foreach (var file in files.RoamingFiles)
            {
                BindableStorageFile localFile = await BindableStorageFile.Create(file);
                localFile.IsRoamed = true;
                allFiles.Add(localFile);
            }
            
            _loadedFiles = new ObservableCollection<BindableStorageFile>(allFiles);
            RaisePropertyChanged(nameof(LoadedFiles));
            _initialized = true;
        }        

        /// <summary>
        /// Saves and encrypts the contents into a StorageFile with the given name, and the given password.
        /// </summary>
        /// <param name="contents">The contents to encrypt.</param>
        /// <param name="fileName">The filename with which to save the file.</param>
        /// <param name="password">The password that will be used to generate the encryption key.</param>
        /// <returns>If successful, returns the created StorageFile.</returns>
        public async Task<StorageFile> SaveAndEncryptFileAsync(string contents, string fileName, string password)
        {
            if(!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            string savedFileName = await FileUtilities.SaveAndEncryptFileAsync(contents, fileName, password);
            StorageFile savedFile = await FileUtilities.GetEncryptedFileAsync(savedFileName);
            BindableStorageFile bsf = await BindableStorageFile.Create(savedFile);
            LoadedFiles.Add(bsf);
            return savedFile;
        }

        public BindableStorageFile GetLoadedFile(string savedFileName)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            return LoadedFiles.Where(x => x.Name == savedFileName).Single();
        }

        internal async Task<string> RetrieveFileContentsAsync(string name, string password)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            string encryptedContents = await FileIO.ReadTextAsync(LoadedFiles.Single(x => x.Name == name).BackingFile);
            if(String.IsNullOrWhiteSpace(encryptedContents))
            {
                return null;
            }
            return EncryptionManager.Decrypt(encryptedContents, password);
        }

        internal async Task ClearFileAsync(string name)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            var file = LoadedFiles.Single(x => x.Name == name);
            await FileIO.WriteTextAsync(file.BackingFile, string.Empty);
        }

        internal async Task DeleteFileAsync(StorageFile backingFile)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }
            LoadedFiles.Remove(LoadedFiles.Single(x => x.BackingFile == backingFile));
            await FileUtilities.DeleteFileAsync(backingFile);
        }

        private async void OnRoamingDataChanged(ApplicationData sender, object args)
        {
            foreach(var file in LoadedFiles)
            {
                if(file.IsRoamed)
                {
                    LoadedFiles.Remove(file);
                }
            }

            var roamingFiles = await sender.RoamingFolder.GetFilesAsync();
            foreach(var file in roamingFiles)
            {
                BindableStorageFile bsf = await BindableStorageFile.Create(file);                
                LoadedFiles.Add(bsf);
            }
        }
    }
}

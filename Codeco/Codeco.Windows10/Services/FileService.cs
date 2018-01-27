using Codeco.Windows10.Common;
using Codeco.Windows10.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace Codeco.Windows10.Services
{
    public class FileService : IFileService
    {
        private readonly IInitializationValueService _ivService;

        private bool _initialized = false;
        private AsyncLock f_lock = new AsyncLock();        
        private static readonly StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        private static readonly StorageFolder _roamingFolder = ApplicationData.Current.RoamingFolder;

        public ObservableCollection<IBindableStorageFile> LocalFiles { get; private set; }
        public ObservableCollection<IBindableStorageFile> RoamedFiles { get; private set; }        

        public enum FileLocation { Local, Roamed };

        public TaskCompletionSource<bool> IsInitialized { get; } = new TaskCompletionSource<bool>();

        public FileService(IInitializationValueService ivService)
        {
            _ivService = ivService;
            CreateAsync();
        }

        public async Task StopRoamingFile(IBindableStorageFile file)
        {
            using (await f_lock.Acquire())
            {
                if (!LocalFiles.Contains(file))
                {                    
                    //Backing values
                    string oldPath = GetParentFolder(file.BackingFile);
                    string value = await _ivService.GetValue(oldPath, FileLocation.Roamed);
                    await _ivService.Remove(oldPath, FileLocation.Roamed);

                    await file.BackingFile.MoveAsync(_localFolder, file.BackingFile.Name, NameCollisionOption.GenerateUniqueName);

                    await _ivService.Add(GetParentFolder(file.BackingFile), value, FileLocation.Local);
                    RoamedFiles.Remove(file);

                    //UI                
                    file.IsRoamed = false;
                    LocalFiles.Add(file);                    
                }
            }
        }

        public async Task RoamFile(IBindableStorageFile file)
        {
            using (await f_lock.Acquire())
            {
                if (!RoamedFiles.Contains(file))
                {
                    //Backing values
                    string oldPath = GetParentFolder(file.BackingFile);
                    string value = await _ivService.GetValue(oldPath, FileLocation.Local);
                    await _ivService.Remove(oldPath, FileLocation.Local);

                    await file.BackingFile.MoveAsync(_roamingFolder, file.BackingFile.Name, NameCollisionOption.GenerateUniqueName);

                    await _ivService.Add(GetParentFolder(file.BackingFile), value, FileLocation.Roamed);
                    LocalFiles.Remove(file);

                    //UI
                    file.IsRoamed = true;

                    RoamedFiles.Add(file);                    
                }
            }
        }

        private async Task CreateAsync()
        {
            if (_initialized)
            {
                return;
            }

            ApplicationData.Current.DataChanged += OnRoamingDataChanged;
            LocalFiles = new ObservableCollection<IBindableStorageFile>();
            RoamedFiles = new ObservableCollection<IBindableStorageFile>();

            var allFiles = await GetFilesAsync();
            foreach (var local in allFiles.LocalFiles)
            {
                var bsf = await BindableStorageFile.Create(local);
                LocalFiles.Add(bsf);
            }
            foreach (var roamed in allFiles.RoamingFiles)
            {
                if (roamed.Name == Constants.IV_FILE_NAME) continue;
                var bsf = await BindableStorageFile.Create(roamed);
                bsf.IsRoamed = true;
                RoamedFiles.Add(bsf);
            }

            _initialized = true;
            IsInitialized.SetResult(true);
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
                if (contents == null)
                {
                    throw new ArgumentException("File contents cannot be null.");
                }

                //string iv = Guid.NewGuid().ToString("N"); // OLD IV code.

                (string encryptedContents, string iv) = EncryptionManager.Encrypt(contents, password);
                var savedFile = await _localFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                await FileIO.WriteTextAsync(savedFile, encryptedContents);
                string savedFileName = savedFile.Name;

                BindableStorageFile bsf = await BindableStorageFile.Create(savedFile);
                LocalFiles.Add(bsf);
                                
                await _ivService.Add(GetParentFolder(bsf.BackingFile), iv, FileLocation.Local);                

                return bsf;
            }
        }

        public async Task<string> RetrieveFileContentsAsync(string fileName, string password, FileLocation location)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                var collectionToSearch = location == FileLocation.Local ? LocalFiles : RoamedFiles;
                var file = collectionToSearch.Single(x => x.Name == fileName);
                string encryptedContents = await FileIO.ReadTextAsync(file.BackingFile);
                if (String.IsNullOrWhiteSpace(encryptedContents))
                {
                    return null;
                }
                                
                string iv = await _ivService.GetValue(GetParentFolder(file.BackingFile), location);
                try
                {
                    return EncryptionManager.Decrypt(encryptedContents, password, iv);
                }
                catch (Exception ex)
                {
                    //Just explicitly noting that .Decrypt() can and WILL throw.
                    throw;
                }
            }
        }

        public async Task DeleteFileAsync(IStorageFile backingFile, FileLocation location)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                string fileName = backingFile.Name;                
                var collectionToSearch = location == FileLocation.Local ? LocalFiles : RoamedFiles;
                collectionToSearch.Remove(collectionToSearch.Single(x => x.BackingFile == backingFile));
                try
                {
                    await backingFile.DeleteAsync(StorageDeleteOption.PermanentDelete);                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to delete file: " + ex);                    
                }

                await _ivService.Remove(GetParentFolder(backingFile), location);                
            }
        }

        public async Task RenameFileAsync(IBindableStorageFile file, string newName)
        {
            if (!_initialized)
            {
                throw new ServiceNotInitializedException($"{nameof(FileService)} was not initialized before access was attempted.");
            }

            using (await f_lock.Acquire())
            {
                string oldName = file.BackingFile.Name;
                string oldPath = GetParentFolder(file.BackingFile);
                await file.BackingFile.RenameAsync(newName, NameCollisionOption.GenerateUniqueName);
                file.NameChanged();

                FileLocation location = file.IsRoamed ? FileLocation.Roamed : FileLocation.Local;
                                
                string iv = await _ivService.GetValue(oldPath, location);
                await _ivService.Remove(oldPath, location);
                await _ivService.Add(GetParentFolder(file.BackingFile), iv, location);
            }
        }       
           
        /// <summary>
        /// Ensures that the given file conforms to the formatting constraints (two-column csv)
        /// </summary>
        /// <param name="file">The file to investigate.</param>
        /// <returns>True if formatted properly, false otherwise.</returns>
        public async Task<bool> ValidateFileAsync(StorageFile file)
        {
            IList<string> lines = (await FileIO.ReadLinesAsync(file))
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .ToList();
            if (lines.Count == 0)
            {
                return false;
            }
            return lines.Select(line => line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .All(splitString => splitString.Length == 2);
        }

        public FileLocation GetFileLocation(BindableStorageFile file)
        {
            return file.IsRoamed ? FileLocation.Roamed : FileLocation.Local;
        }

        /// <summary>
        /// Get all the saved files.
        /// </summary>
        /// <returns></returns>
        private static async Task<SavedFiles> GetFilesAsync()
        {
            SavedFiles allFiles = new SavedFiles
            {
                //Ignore files beginning with underscores. This allows us to use those as config (etc) files.
                LocalFiles = (await _localFolder.GetFilesAsync()).Where(x => x.Name[0] != '_'),
                RoamingFiles = (await _roamingFolder.GetFilesAsync()).Where(x => x.Name[0] != '_')
            };
            return allFiles;
        }
        
        private async void OnRoamingDataChanged(ApplicationData sender, object args)
        {
            using (await f_lock.Acquire())
            {                
                var roamingFiles = await Task.WhenAll((await sender.RoamingFolder.GetFilesAsync())
                    .Where(f => f.Name != Constants.IV_FILE_NAME)
                    .Select(async x => await BindableStorageFile.Create(x)));

                var newDictionary = (await sender.RoamingFolder.GetFilesAsync()).First(f => f.Name == Constants.IV_FILE_NAME);
                await newDictionary.CopyAsync(_roamingFolder, Constants.IV_FILE_NAME, NameCollisionOption.ReplaceExisting);

                //Remove no-longer-synced-files              
                foreach(var file in RoamedFiles)
                {
                    if(! await ContainsBindableStorageFile(file, roamingFiles))
                    {
                        await DeleteFileAsync(file.BackingFile, FileLocation.Roamed);
                    }
                }

                foreach (var file in roamingFiles.Where(f => f.Name != Constants.IV_FILE_NAME))
                {                    
                    if (!(await ContainsBindableStorageFile(file, RoamedFiles)))
                    {
                        file.IsRoamed = true;                        
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () => RoamedFiles.Add(file));
                    }
                }                
            }
        }       

        private async Task<bool> ContainsBindableStorageFile(IBindableStorageFile element, IEnumerable<IBindableStorageFile> collection)
        {
            foreach(var item in collection)
            {
                if(await element.CompareAsync(item))
                {
                    return true;
                }
            }
            return false;
        }

        private string GetParentFolder(IStorageFile backingFile)
        {
            return Directory.GetParent(backingFile.Path).Name + "/" + backingFile.Name;
        }

        public async Task ClearAllData()
        {            
            foreach(var item in await _localFolder.GetFilesAsync())
            {
                await item.DeleteAsync();
            }
            foreach (var item in await _roamingFolder.GetFilesAsync())
            {
                await item.DeleteAsync();
            }            
        }
    }
}

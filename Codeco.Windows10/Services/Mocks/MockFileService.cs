﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;
using Codeco.Windows10.Models;
using System.Collections.ObjectModel;

namespace Codeco.Windows10.Services.Mocks
{
    class MockFileService : IFileService
    {
        public ObservableCollection<IBindableStorageFile> LocalFiles { get; }
        public ObservableCollection<IBindableStorageFile> RoamedFiles { get; }

        public TaskCompletionSource<bool> IsInitialized
        {
            get
            {
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetResult(true);
                return tcs;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Task ClearFileAsync(string name, FileService.FileLocation location)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(StorageFile backingFile, FileService.FileLocation location)
        {
            throw new NotImplementedException();
        }

        public FileService.FileLocation GetFileLocation(BindableStorageFile file)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IBindableStorageFile> GetLocalFiles()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IBindableStorageFile> GetRoamedFiles()
        {
            throw new NotImplementedException();
        }

        public Task NukeFiles()
        {
            throw new NotImplementedException();
        }

        public Task RenameFileAsync(IBindableStorageFile file, string newName)
        {
            throw new NotImplementedException();
        }

        public Task<string> RetrieveFileContentsAsync(string fileName, string password, FileService.FileLocation location)
        {
            throw new NotImplementedException();
        }

        public Task RoamFile(IBindableStorageFile file)
        {
            throw new NotImplementedException();
        }

        public Task<BindableStorageFile> SaveAndEncryptFileAsync(string contents, string fileName, string password)
        {
            throw new NotImplementedException();
        }

        public Task StopRoamingFile(IBindableStorageFile file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateFileAsync(StorageFile file)
        {
            throw new NotImplementedException();
        }

        public Task ClearAllData()
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(IStorageFile backingFile, FileService.FileLocation location)
        {
            throw new NotImplementedException();
        }
    }
}

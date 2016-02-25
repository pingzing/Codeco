using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Codeco.Model;

namespace Codeco.Services.Mocks
{
    class MockFileService : IFileService
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Task<IService> InitializeAsync()
        {
            return null;
        }

        public IService InitializeSync()
        {
            throw new NotImplementedException();
        }

        public bool Initialized { get; set; }

        public void RaisePropertyChanged(string property)
        {
            throw new NotImplementedException();
        }

        public Task StopRoamingFile(IBindableStorageFile file)
        {
            throw new NotImplementedException();
        }

        public Task RoamFile(IBindableStorageFile file)
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

        public Task<BindableStorageFile> SaveAndEncryptFileAsync(string contents, string fileName, string password)
        {
            throw new NotImplementedException();
        }

        public Task<string> RetrieveFileContentsAsync(string fileName, string password, FileService.FileLocation location)
        {
            throw new NotImplementedException();
        }

        public Task ClearFileAsync(string name, FileService.FileLocation location)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(StorageFile backingFile, FileService.FileLocation location)
        {
            throw new NotImplementedException();
        }

        public Task RenameFileAsync(IBindableStorageFile file, string newName)
        {
            throw new NotImplementedException();
        }

        public Task NukeFiles()
        {
            throw new NotImplementedException();
        }

        public FileService.FileLocation GetFileLocation(BindableStorageFile file)
        {
            throw new NotImplementedException();
        }
    }
}

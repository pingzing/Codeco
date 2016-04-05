using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Codeco.Windows10.Models;
using System.Collections.ObjectModel;

namespace Codeco.Windows10.Services
{
    public interface IFileService
    {
        TaskCompletionSource<bool> IsInitialized { get; }

        ObservableCollection<IBindableStorageFile> LocalFiles { get; }
        ObservableCollection<IBindableStorageFile> RoamedFiles { get; }

        Task StopRoamingFile(IBindableStorageFile file);
        Task RoamFile(IBindableStorageFile file);        

        /// <summary>
        /// Saves and encrypts the contents into a StorageFile with the given name, and the given password.
        /// </summary>
        /// <param name="contents">The contents to encrypt.</param>
        /// <param name="fileName">The filename with which to save the file.</param>
        /// <param name="password">The password that will be used to generate the encryption key.</param>
        /// <returns>If successful, returns the created StorageFile.</returns>
        Task<BindableStorageFile> SaveAndEncryptFileAsync(string contents, string fileName, string password);
        Task<string> RetrieveFileContentsAsync(string fileName, string password, FileService.FileLocation location);        
        Task DeleteFileAsync(IStorageFile backingFile, FileService.FileLocation location);
        Task RenameFileAsync(IBindableStorageFile file, string newName);        
        FileService.FileLocation GetFileLocation(BindableStorageFile file);
        Task<bool> ValidateFileAsync(StorageFile file);

        Task ClearAllData();
    }
}

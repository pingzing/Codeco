using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Codeco.Windows10.Models;

namespace Codeco.Windows10.Services
{
    public interface IFileService
    {
        TaskCompletionSource<bool> IsInitialized { get; }

        Task StopRoamingFile(IBindableStorageFile file);
        Task RoamFile(IBindableStorageFile file);
        IReadOnlyList<IBindableStorageFile> GetLocalFiles();
        IReadOnlyList<IBindableStorageFile> GetRoamedFiles();

        /// <summary>
        /// Saves and encrypts the contents into a StorageFile with the given name, and the given password.
        /// </summary>
        /// <param name="contents">The contents to encrypt.</param>
        /// <param name="fileName">The filename with which to save the file.</param>
        /// <param name="password">The password that will be used to generate the encryption key.</param>
        /// <returns>If successful, returns the created StorageFile.</returns>
        Task<BindableStorageFile> SaveAndEncryptFileAsync(string contents, string fileName, string password);
        Task<string> RetrieveFileContentsAsync(string fileName, string password, FileService.FileLocation location);
        Task ClearFileAsync(string name, FileService.FileLocation location);
        Task DeleteFileAsync(StorageFile backingFile, FileService.FileLocation location);
        Task RenameFileAsync(IBindableStorageFile file, string newName);
        Task NukeFiles();
        FileService.FileLocation GetFileLocation(BindableStorageFile file);
    }
}

using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.ViewModels;
using DynamicData;
using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IUserFileService
    {
        IObservableList<SimpleFileInfoViewModel> FilesList { get; }

        Task CreateUserFileAsync(string fileName, FileLocation fileLocation);
        Task<string> CreateUserFileAsync(string fileName, FileLocation fileLocation, byte[] data);
        Task<string> CreateUserFileAsync(string fileName, FileLocation fileLocation, string data);
        Task<DirectoryInfo> CreateUserFolderAsync(string relativeFolderPath);
        Task DeleteUserFileAsync(string fileName, FileLocation fileLocation);
        Task<bool> ValidateFileAsync(byte[] dataArray);
        Task RenameUserFile(string fileName, FileLocation fileLocation, string newName);
    }
}
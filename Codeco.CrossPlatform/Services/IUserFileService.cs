using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.ViewModels;
using DynamicData;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IUserFileService
    {
        IObservableList<SimpleFileInfoViewModel> FilesList { get; }

        Task CreateUserFileAsync(string fileName, FileLocation fileLocation);
        Task<string> CreateUserFileAsync(string fileName, FileLocation fileLocation, string password, string pickedFileData);
        DirectoryInfo CreateUserFolderAsync(string relativeFolderPath);
        Task DeleteUserFileAsync(string fileName, FileLocation fileLocation);
        Task<bool> ValidateFileAsync(byte[] dataArray);
        Task RenameUserFile(string fileName, FileLocation fileLocation, string newName);
        Task ChangeUserFileLocationAsync(string fileName, FileLocation sourceLocation, FileLocation destinationLocation);
        Task<Dictionary<string, string>> GetUserFileContentsAsync(string name, FileLocation fileLocation, string password);
        string GetRelativeFilePath(string fileName, FileLocation location);
    }
}
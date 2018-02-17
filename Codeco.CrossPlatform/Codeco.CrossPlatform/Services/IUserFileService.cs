using Codeco.CrossPlatform.Models;
using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IUserFileService
    {
        Task CreateUserFileAsync(string fileName, FileLocation fileLocation);
        DirectoryInfo CreateUserFolder(string relativeFolderPath);
        Task CreateUserFileAsync(string fileName, FileLocation fileLocation, byte[] data);
        Task CreateUserFileAsync(string fileName, FileLocation fileLocation, string data);
        Task<bool> ValidateFileAsync(byte[] dataArray);
    }
}
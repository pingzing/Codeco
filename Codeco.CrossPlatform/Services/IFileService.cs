using Codeco.CrossPlatform.Models.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IFileService
    {
        Task<FileStream> OpenOrCreateFileAsync(string relativeFilePath);
        Task<CreateFileResult> CreateFileAsync(string relativeFilePath);
        DirectoryInfo CreateFolder(string absoluteFolderPath);
        Task WriteBytesAsync(string relativeFilePath, byte[] data);
        Task<List<string>> GetFilesInFolder(string relativeFolderPath);
        Task DeleteFileAsync(string relativeFilePath);
        Task<string> RenameFileAsync(string relativeFilePath, string newName);
        Task<string> MoveFileAsync(string sourceRelativeFilePath, string destinationRelativeFlePath);
        Task<string> GetFileContentsAsync(string relativeFilePath);
    }
}
using Codeco.CrossPlatform.Models.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IFileService
    {
        Task OpenOrCreateFileAsync(string fileName);
        Task<CreateFileResult> CreateFileAsync(string fileName);
        DirectoryInfo CreateFolder(string absoluteFolderPath);
        Task WriteBytesAsync(string relativeFileName, byte[] data);
        Task<List<string>> GetFilesInFolder(string relativeFolderPath);
    }
}
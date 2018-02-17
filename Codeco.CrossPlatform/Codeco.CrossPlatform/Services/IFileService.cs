using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IFileService
    {
        Task OpenOrCreateFileAsync(string fileName);
        Task CreateFileAsync(string fileName);
        DirectoryInfo CreateFolder(string absoluteFolderPath);
        Task WriteBytesAsync(string relativeFileName, byte[] data);
    }
}
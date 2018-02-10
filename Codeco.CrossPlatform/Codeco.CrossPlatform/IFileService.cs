using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IFileService
    {
        Task CreateFileAsync(string fileName);
        DirectoryInfo CreateFolder(string absoluteFolderPath);
    }
}
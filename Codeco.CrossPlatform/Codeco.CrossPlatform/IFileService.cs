using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IFileService
    {
        Task CreateFileAsync(string fileName);
    }
}
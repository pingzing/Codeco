using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IUserFileService
    {
        Task CreateUserFileAsync(string fileName);
    }
}
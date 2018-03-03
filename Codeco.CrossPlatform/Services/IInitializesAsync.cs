using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public interface IInitializesAsync
    {
        Task Initialization { get; }
    }
}

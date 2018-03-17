using Codeco.CrossPlatform.Models.DependencyServices;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services.DependencyInterfaces
{
    public interface IConnectedDeviceService
    {
        Task<GetDeviceResult> GetDeviceWatcher();
        Task<bool> OpenRemoteConnection(RemoteSystem remoteSystem);
    }
}

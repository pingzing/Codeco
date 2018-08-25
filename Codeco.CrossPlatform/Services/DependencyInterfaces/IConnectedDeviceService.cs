using Codeco.CrossPlatform.Models.DependencyServices;
using System;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services.DependencyInterfaces
{
    public interface IConnectedDeviceService
    {
        Task<GetDeviceResult> GetDeviceWatcher();
        Task<bool> OpenRemoteConnection(RemoteSystem remoteSystem);
        event Action<string> GotOAuthUrl;
        Task<bool> InitializeAsync();
    }
}

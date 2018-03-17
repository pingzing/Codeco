using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CrossPlat = Codeco.CrossPlatform.Models.DependencyServices;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Microsoft.ConnectedDevices;
using CDPlatform = Microsoft.ConnectedDevices.Platform;

namespace Codeco.CrossPlatform.Droid.DependencyServices
{
    public class ConnectedDeviceService : IConnectedDeviceService
    {
        private Context _context;
        private AppServiceConnection _remoteConnection = null;

        public ConnectedDeviceService(Context context)
        {
            _context = context;
        }

        public async Task<CrossPlat.GetDeviceResult> GetDeviceWatcher()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> OpenRemoteConnection(CrossPlat.RemoteSystem remoteSystem)
        {
            RemoteSystem locallyTrackedSystem = null;
            RemoteSystemConnectionRequest request = new RemoteSystemConnectionRequest(locallyTrackedSystem);

            _remoteConnection = new AppServiceConnection
            (
                "ReceiveSyncDataService",
                "10707NeilApps.Codeco.Test_1zcj54t5p2twp", //this might need to be the Live ID
                request
            );
            AppServiceConnectionStatus result = await _remoteConnection.OpenRemoteAsync();
            if (result != AppServiceConnectionStatus.Success)
            {
                // TODO: return better error
                return false;
            }

            return true;
        }
    }
}
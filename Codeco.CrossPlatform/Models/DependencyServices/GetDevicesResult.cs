using System;
using System.Collections.Generic;
using System.Text;

namespace Codeco.CrossPlatform.Models.DependencyServices
{
    public struct GetDeviceResult
    {
        public ConnectedDeviceAccessStatus AccessStatus { get; set; }
        public IObservable<RemoteSystemEvent> RemoteDevices { get; set; }
    }

    public enum ConnectedDeviceAccessStatus
    {
        Denied,
        Allowed,
    }
}

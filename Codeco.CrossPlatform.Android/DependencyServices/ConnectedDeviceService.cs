using System;
using System.Threading.Tasks;
using Android.Content;
using CrossPlat = Codeco.CrossPlatform.Models.DependencyServices;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Microsoft.ConnectedDevices;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using static Microsoft.ConnectedDevices.RemoteSystemWatcher;

namespace Codeco.CrossPlatform.Droid.DependencyServices
{
    public class ConnectedDeviceService : IConnectedDeviceService
    {
        private Context _context;

        private RemoteSystemWatcher _watcher;
        private IObservable<CrossPlat.RemoteSystemEvent> _remoteSystemWatcher;
        private ConcurrentDictionary<string, RemoteSystem> _foundSystems = new ConcurrentDictionary<string, RemoteSystem>();
        private AppServiceConnection _remoteConnection = null;

        public event Action<string> GotOAuthUrl
        {
            add { Platform.FetchAuthCode += value; }
            remove { Platform.FetchAuthCode -= value; }
        }

        public ConnectedDeviceService(Context context)
        {
            _context = context;
        }

        public async Task<bool> InitializeAsync()
        {
            return await Platform.InitializeAsync(_context, "000000004C16315A");
        }

        public async Task<CrossPlat.GetDeviceResult> GetDeviceWatcher()
        {
            RemoteSystem.RequestAccessAsync();
            _watcher = RemoteSystem.CreateWatcher();
            _remoteSystemWatcher = CreateRemoteSystemWatcherSources(_watcher).Merge();

            SetupLocalHandlers(_watcher);

            return new CrossPlat.GetDeviceResult
            {
                AccessStatus = CrossPlat.ConnectedDeviceAccessStatus.Allowed,
                RemoteDevices = _remoteSystemWatcher
        };
        }

        public async Task<bool> OpenRemoteConnection(CrossPlat.RemoteSystem remoteSystem)
        {
            RemoteSystem locallyTrackedSystem = null;
            RemoteSystemConnectionRequest request = new RemoteSystemConnectionRequest(locallyTrackedSystem);

            _remoteConnection = new AppServiceConnection
            (
                "ReceiveSyncDataService",
                "000000004C16315A", //this might need to be the Live ID
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

        private void SetupLocalHandlers(RemoteSystemWatcher watcher)
        {
            void Watcher_RemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
            {
                _foundSystems.AddOrUpdate(args.P0.Id, args.P0, (_id, _system) => args.P0);
            }

            watcher.RemoteSystemAdded -= Watcher_RemoteSystemAdded;
            watcher.RemoteSystemAdded += Watcher_RemoteSystemAdded;

            void Watcher_RemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
            {
                _foundSystems.TryRemove(args.P0, out RemoteSystem _);
            }

            watcher.RemoteSystemRemoved -= Watcher_RemoteSystemRemoved;
            watcher.RemoteSystemRemoved += Watcher_RemoteSystemRemoved;

            void Watcher_RemoteSystemUpdated(RemoteSystemWatcher sender, RemoteSystemUpdatedEventArgs args)
            {
                _foundSystems.AddOrUpdate(args.P0.Id, args.P0, (_id, _system) => args.P0);
            }

            watcher.RemoteSystemUpdated -= Watcher_RemoteSystemUpdated;
            watcher.RemoteSystemUpdated += Watcher_RemoteSystemUpdated;
        }

        private IObservable<CrossPlat.RemoteSystemEvent>[] CreateRemoteSystemWatcherSources(RemoteSystemWatcher watcher)
        {
            return new[] {
                Observable.FromEvent<OnRemoteSystemAdded, RemoteSystemAddedEventArgs>(
                    handler => (sdr, addedArgs) => handler(addedArgs),
                    h => watcher.RemoteSystemAdded += h,
                    h => watcher.RemoteSystemAdded -= h)
                    .Select(x => new CrossPlat.RemoteSystemEvent
                    {
                        Id = x.P0.Id,
                        EventKind = CrossPlat.RemoteEventKind.Added,
                        System = x.P0.ToCrossPlatformRemoteSystem()
                    }),
                Observable.FromEvent<OnRemoteSystemRemoved, RemoteSystemRemovedEventArgs>(
                    handler => (sdr, removedArgs) => handler(removedArgs),
                    h => watcher.RemoteSystemRemoved += h,
                    h => watcher.RemoteSystemRemoved -= h)
                    .Select(x => new CrossPlat.RemoteSystemEvent
                    {
                        Id = x.P0,
                        EventKind = CrossPlat.RemoteEventKind.Removed,
                        System = null
                    }),
                Observable.FromEvent<OnRemoteSystemUpdated, RemoteSystemUpdatedEventArgs>(
                    handler => (sr, updatedArgs) => handler(updatedArgs),
                    h => watcher.RemoteSystemUpdated += h,
                    h => watcher.RemoteSystemUpdated -= h)
                    .Select(x => new CrossPlat.RemoteSystemEvent
                    {
                        Id = x.P0.Id,
                        EventKind = CrossPlat.RemoteEventKind.Updated,
                        System = x.P0.ToCrossPlatformRemoteSystem()
                    })
            };
        }
    }

    public static class RemoteSystemExtensions
    {
        public static CrossPlat.RemoteSystem ToCrossPlatformRemoteSystem(this RemoteSystem remoteSystem)
        {
            CrossPlat.RemoteSystemKind remoteKind;
            if (remoteSystem.Kind.Value == RemoteSystemKinds.Desktop.Value)
            {                
                remoteKind = CrossPlat.RemoteSystemKind.Desktop;
            }
            else if (remoteSystem.Kind.Value == RemoteSystemKinds.Holographic.Value)
            {
                remoteKind = CrossPlat.RemoteSystemKind.Holographic;
            }
            else if (remoteSystem.Kind.Value == RemoteSystemKinds.Hub.Value)
            {
                remoteKind = CrossPlat.RemoteSystemKind.Hub;
            }
            else if (remoteSystem.Kind.Value == RemoteSystemKinds.Iot.Value)
            {
                remoteKind = CrossPlat.RemoteSystemKind.Iot;
            }
            else if (remoteSystem.Kind.Value == RemoteSystemKinds.Laptop.Value)
            {
                remoteKind = CrossPlat.RemoteSystemKind.Laptop;
            }
            else if (remoteSystem.Kind.Value == RemoteSystemKinds.Phone.Value)
            {
                remoteKind = CrossPlat.RemoteSystemKind.Phone;
            }
            else if (remoteSystem.Kind.Value == RemoteSystemKinds.Tablet.Value)
            {
                remoteKind = CrossPlat.RemoteSystemKind.Tablet;
            }
            else if (remoteSystem.Kind.Value == RemoteSystemKinds.Xbox.Value)
            {
                remoteKind = CrossPlat.RemoteSystemKind.Xbox;
            }
            else
            {
                remoteKind = CrossPlat.RemoteSystemKind.Unknown;
            }

            CrossPlat.RemoteSystemStatus crossPlatStatus;
            if (remoteSystem.Status.Value == RemoteSystemStatus.Available.Value)
            {
                crossPlatStatus = CrossPlat.RemoteSystemStatus.Available;
            }
            else if (remoteSystem.Status.Value == RemoteSystemStatus.DiscoveringAvailability.Value)
            {
                crossPlatStatus = CrossPlat.RemoteSystemStatus.DiscoveringAvailability;
            }
            else if (remoteSystem.Status.Value == RemoteSystemStatus.Unavailable.Value)
            {
                crossPlatStatus = CrossPlat.RemoteSystemStatus.Unavailable;
            }
            else
            {
                crossPlatStatus = CrossPlat.RemoteSystemStatus.Unknown;
            }

            return new CrossPlat.RemoteSystem
            {
                DisplayName = remoteSystem.DisplayName,
                Id = remoteSystem.Id,
                IsAvailableByProximity = remoteSystem.IsAvailableByProximity,
                Kind = remoteKind,
                Status = crossPlatStatus
            };
        }
    }
}
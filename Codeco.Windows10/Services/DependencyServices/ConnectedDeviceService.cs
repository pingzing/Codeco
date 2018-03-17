using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.Windows10.Common;
using Codeco.Windows10.Services.DependencyServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.RemoteSystems;
using CrossPlat = Codeco.CrossPlatform.Models.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(ConnectedDeviceService))]
namespace Codeco.Windows10.Services.DependencyServices
{    
    public class ConnectedDeviceService : IConnectedDeviceService
    {
        private RemoteSystemWatcher _watcher;
        private IObservable<CrossPlat.RemoteSystemEvent> _remoteSystemWatcher;
        private ConcurrentDictionary<string, RemoteSystem> _foundSystems = new ConcurrentDictionary<string, RemoteSystem>();
        private AppServiceConnection _remoteConnection = new AppServiceConnection
        {
            AppServiceName = "ReceiveSyncDataService",
            PackageFamilyName = "10707NeilApps.Codeco.Test_1zcj54t5p2twp",
        };

        public async Task<CrossPlat.GetDeviceResult> GetDeviceWatcher()
        {
            RemoteSystemAccessStatus accessStatus = await RemoteSystem.RequestAccessAsync();
            if (accessStatus == RemoteSystemAccessStatus.Allowed)
            {
                _watcher = RemoteSystem.CreateWatcher();
                _remoteSystemWatcher = CreateRemoteSystemWatcherSources(_watcher)
                        .Merge();

                SetupLocalHandlers(_watcher);

                return new CrossPlat.GetDeviceResult
                {
                    AccessStatus = CrossPlat.ConnectedDeviceAccessStatus.Allowed,
                    RemoteDevices = _remoteSystemWatcher
                };                
            }
            else
            {
                return new CrossPlat.GetDeviceResult
                {
                    AccessStatus = CrossPlat.ConnectedDeviceAccessStatus.Denied,
                    RemoteDevices = null
                };
            }
        }

        public async Task<bool> OpenRemoteConnection(CrossPlat.RemoteSystem remoteSystem)
        {            
            if (_foundSystems.TryGetValue(remoteSystem.Id, out RemoteSystem locallyTrackedSystem) == false)
            {
                // immediate failure--somehow, the crossplatform app wants us to connect to a system that the
                // local platform-project no longer knows about
                return false;
            }

            RemoteSystemConnectionRequest connectionRequest = new RemoteSystemConnectionRequest(locallyTrackedSystem);

            var status = await _remoteConnection.OpenRemoteAsync(connectionRequest);
            if (status != AppServiceConnectionStatus.Success)
            {
                // TODO: return better error
                return false;
            }

            return true;
        }

        public async Task SendRemoteMessage(string message)
        {
            ValueSet values = new ValueSet();
            values.Add("Message", message);

            var response = await _remoteConnection.SendMessageAsync(values);
            response.Status == AppServiceResponseStatus.
        }

        private void SetupLocalHandlers(RemoteSystemWatcher watcher)
        {
            void Watcher_RemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
            {
                _foundSystems.AddOrUpdate(args.RemoteSystem.Id, args.RemoteSystem, (_id, _system) => args.RemoteSystem);
            }

            watcher.RemoteSystemAdded -= Watcher_RemoteSystemAdded;
            watcher.RemoteSystemAdded += Watcher_RemoteSystemAdded;

            void Watcher_RemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
            {
                _foundSystems.TryRemove(args.RemoteSystemId, out RemoteSystem _);
            }

            watcher.RemoteSystemRemoved -= Watcher_RemoteSystemRemoved;
            watcher.RemoteSystemRemoved += Watcher_RemoteSystemRemoved;

            void Watcher_RemoteSystemUpdated(RemoteSystemWatcher sender, RemoteSystemUpdatedEventArgs args)
            {
                _foundSystems.AddOrUpdate(args.RemoteSystem.Id, args.RemoteSystem, (_id, _system) => args.RemoteSystem);
            }

            watcher.RemoteSystemUpdated -= Watcher_RemoteSystemUpdated;
            watcher.RemoteSystemUpdated += Watcher_RemoteSystemUpdated;
        }        

        private IObservable<CrossPlat.RemoteSystemEvent>[] CreateRemoteSystemWatcherSources(RemoteSystemWatcher remoteWatcher)
        {
            return new[]
            {
                Observable.FromEventPattern
                <TypedEventHandler<RemoteSystemWatcher, RemoteSystemAddedEventArgs>,
                RemoteSystemWatcher,
                RemoteSystemAddedEventArgs>(
                    x => remoteWatcher.RemoteSystemAdded += x,
                    x => remoteWatcher.RemoteSystemAdded -= x)
                    .Select(ev => new CrossPlat.RemoteSystemEvent
                    {
                        Id = ev.EventArgs.RemoteSystem.Id,
                        System = ev.EventArgs.RemoteSystem.ToCrossPlatformRemoteSystem(),
                        EventKind = CrossPlat.RemoteEventKind.Added
                    }),

                Observable.FromEventPattern<
                    TypedEventHandler<RemoteSystemWatcher, RemoteSystemRemovedEventArgs>,
                    RemoteSystemWatcher,
                    RemoteSystemRemovedEventArgs>(
                    x => remoteWatcher.RemoteSystemRemoved += x,
                    x => remoteWatcher.RemoteSystemRemoved -= x)
                    .Select(ev => new CrossPlat.RemoteSystemEvent
                    {
                        Id = ev.EventArgs.RemoteSystemId,
                        System = null,
                        EventKind = CrossPlat.RemoteEventKind.Removed
                    }),

                Observable.FromEventPattern<
                    TypedEventHandler<RemoteSystemWatcher, RemoteSystemUpdatedEventArgs>,
                    RemoteSystemWatcher,
                    RemoteSystemUpdatedEventArgs>(
                    x => remoteWatcher.RemoteSystemUpdated += x,
                    x => remoteWatcher.RemoteSystemUpdated -= x)
                    .Select(ev => new CrossPlat.RemoteSystemEvent
                    {
                        Id = ev.EventArgs.RemoteSystem.Id,
                        System = ev.EventArgs.RemoteSystem.ToCrossPlatformRemoteSystem(),
                        EventKind = CrossPlat.RemoteEventKind.Updated
                    })
            };
        }
    }

    public static class RemoteSystemExtensions
    {
        public static CrossPlat.RemoteSystem ToCrossPlatformRemoteSystem(this RemoteSystem remote)
        {
            bool parsedKind = Enum.TryParse(remote.Kind, out CrossPlat.RemoteSystemKind kind);
            return new CrossPlat.RemoteSystem
            {
                DisplayName = remote.DisplayName,
                Id = remote.Id,
                IsAvailableByProximity = remote.IsAvailableByProximity,
                Kind = parsedKind ? kind : CrossPlat.RemoteSystemKind.Unknown,
                Status = (CrossPlat.RemoteSystemStatus)remote.Status
            };
        }
    }
}

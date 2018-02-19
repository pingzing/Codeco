using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Subjects;
using Android.OS;
using Android.Runtime;
using Codeco.CrossPlatform.Droid.DependencyServices;
using Codeco.CrossPlatform.Models.FileSystem;
using Codeco.CrossPlatform.Services.DependencyInterfaces;

[assembly: Xamarin.Forms.Dependency(typeof(FileSystemWatcherService))]
namespace Codeco.CrossPlatform.Droid.DependencyServices
{
    public class FileSystemWatcherService : IFileSystemWatcherService
    {
        private static Dictionary<string, Subject<FileChangedEvent>> _storedObservables = new Dictionary<string, Subject<FileChangedEvent>>();
        private static List<CrossPlatFileObserver> _storedObservers = new List<CrossPlatFileObserver>();

        public IObservable<FileChangedEvent> ObserveFolderChanges(string absoluteFolderPath)
        {
            var fileObservable = new Subject<FileChangedEvent>();

            CrossPlatFileObserver observer = new CrossPlatFileObserver(absoluteFolderPath);
            observer.StartWatching();
            _storedObservers.Add(observer);

            _storedObservables.Add(absoluteFolderPath, fileObservable);
            return fileObservable;
        }

        private class CrossPlatFileObserver : FileObserver
        {
            private static FileObserverEvents _events = FileObserverEvents.AllEvents;

            private string _watchedPath;
            private RenameFlag _renamedFrom = null;

            public CrossPlatFileObserver(string path) : base(path, _events)
            {
                _watchedPath = path;
            }

            public CrossPlatFileObserver(string path, FileObserverEvents events) : base(path, events)
            {
                _watchedPath = path;
            }

            public override void OnEvent([GeneratedEnum] FileObserverEvents e, string pathRelativeToWatcher)
            {
                // TODO: Our fancy rename strategy doesn't work if things are going from one folder to another.
                // Reconsider how we track going from Local -> Roamed and vice-versa.
                // Maybe a static dictionary that tracks movement via a "consumable" RenameFlag.
                System.Diagnostics.Debug.WriteLine($"ANDROID FILEOBSERVER: {e}: {pathRelativeToWatcher}");

                bool found = _storedObservables.TryGetValue(_watchedPath, out Subject<FileChangedEvent> observable);

                string name = Path.GetFileNameWithoutExtension(pathRelativeToWatcher);
                string fullPath = Path.Combine(_watchedPath, pathRelativeToWatcher);

                switch (e)
                {
                    case FileObserverEvents.Create:
                        _renamedFrom = null;
                        observable.OnNext(new FileChangedEvent(fullPath, name, WatcherChangeTypes.Created));
                        break;
                    case FileObserverEvents.Delete:
                        _renamedFrom = null;
                        observable.OnNext(new FileChangedEvent(fullPath, name, WatcherChangeTypes.Deleted));
                        break;
                    case FileObserverEvents.Modify:
                        _renamedFrom = null;
                        observable.OnNext(new FileChangedEvent(fullPath, name, WatcherChangeTypes.Changed));
                        break;
                    case FileObserverEvents.MovedFrom:
                        _renamedFrom = new RenameFlag { MovedFromPath = fullPath };
                        break;
                    case FileObserverEvents.MovedTo:
                        if (_renamedFrom != null)
                        {
                            observable.OnNext(new FileChangedEvent
                            (
                                fullPath, 
                                name, 
                                WatcherChangeTypes.Renamed, 
                                _renamedFrom.MovedFromPath, 
                                Path.GetFileName(_renamedFrom.MovedFromPath))
                            );
                            _renamedFrom = null;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private class RenameFlag
        {
            public string MovedFromPath { get; set; }
        }
    }
}
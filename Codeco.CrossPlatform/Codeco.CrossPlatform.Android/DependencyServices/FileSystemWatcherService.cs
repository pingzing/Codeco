using System;
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
        private static Subject<FileChangedEvent> _fileObservable = null;        

        public IObservable<FileChangedEvent> ObserveFolderChanges(string absoluteFolderPath)
        {
            _fileObservable = new Subject<FileChangedEvent>();

            CrossPlatFileObserver observer = new CrossPlatFileObserver(absoluteFolderPath);
            observer.StartWatching();

            return _fileObservable;
        }

        private class CrossPlatFileObserver : FileObserver
        {
            private static FileObserverEvents _events = FileObserverEvents.AllEvents;

            public CrossPlatFileObserver(string path) : base(path, _events) { }

            public CrossPlatFileObserver(string path, FileObserverEvents events) : base(path, events) { }

            public override void OnEvent([GeneratedEnum] FileObserverEvents e, string path)
            {
                System.Diagnostics.Debug.WriteLine($"ANDROID FILEOBSERVER: Received {e} from {path}");

                string name = Path.GetFileNameWithoutExtension(path);

                switch (e)
                {
                    case FileObserverEvents.Create:
                        _fileObservable.OnNext(new FileChangedEvent(path, name, WatcherChangeTypes.Created));
                        break;
                    case FileObserverEvents.Delete:
                        _fileObservable.OnNext(new FileChangedEvent(path, name, WatcherChangeTypes.Deleted));
                        break;
                    case FileObserverEvents.Modify:
                        _fileObservable.OnNext(new FileChangedEvent(path, name, WatcherChangeTypes.Changed));
                        break;
                    default:
                        break;
                }
            }
        }
    }    
}
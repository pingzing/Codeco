using Codeco.CrossPlatform.Models.FileSystem;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Codeco.CrossPlatform.Extensions.Reactive;
using Codeco.Windows10.Services.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(FileSystemWatcherService))]
namespace Codeco.Windows10.Services.DependencyServices
{
    public class FileSystemWatcherService : IFileSystemWatcherService
    {
        public IObservable<FileChangedEvent> ObserveFolderChanges(string folder)
        {
            return Observable.Using(
                () => new FileSystemWatcher(folder) { EnableRaisingEvents = true },
                fsWatcher => CreateFileSystemWatcherSources(fsWatcher)
                    .Merge()
                    .GroupBy(ev => new { ev.FullPath, ev.ChangeType })
                    .SelectMany(fileEvents => fileEvents));
        }

        private IObservable<FileChangedEvent>[] CreateFileSystemWatcherSources(FileSystemWatcher fileWatcher)
        {
            return new[] {
                Observable.FromEventPattern<FileSystemEventArgs>(fileWatcher, nameof(FileSystemWatcher.Changed))
                    .Select(ev => new FileChangedEvent(ev.EventArgs))
                    // FileChanges seem to fire multiple events. Ignore them from the same file for 30ms after the first change. That seems to be good enough to block duplicates.
                    .DistinctUntilTimeout(TimeSpan.FromMilliseconds(30), new FileChangedEqualityComparer()),

                Observable.FromEventPattern<FileSystemEventArgs>(fileWatcher, nameof(FileSystemWatcher.Created))
                    .Select(ev => new FileChangedEvent(ev.EventArgs)),

                Observable.FromEventPattern<FileSystemEventArgs>(fileWatcher, nameof(FileSystemWatcher.Deleted))
                    .Select(ev => new FileChangedEvent(ev.EventArgs)),

                Observable.FromEventPattern<RenamedEventArgs>(fileWatcher, nameof(FileSystemWatcher.Renamed))
                    .Select(ev => new FileChangedEvent(ev.EventArgs))
            };
        }

        private class FileChangedEqualityComparer : IEqualityComparer<FileChangedEvent>
        {
            public bool Equals(FileChangedEvent x, FileChangedEvent y)
            {
                return x.ChangeType == y.ChangeType && x.FullPath == y.FullPath;
            }

            public int GetHashCode(FileChangedEvent obj)
            {
                return obj.ChangeType.GetHashCode() ^ obj.FullPath?.GetHashCode() ?? 7 * 7;
            }
        }
    }
}

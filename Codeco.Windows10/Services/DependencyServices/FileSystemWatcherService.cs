using Codeco.CrossPlatform.Models.FileSystem;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Codeco.CrossPlatform.Extensions.Reactive;

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

        private IObservable<FileChangedEvent>[] CreateFileSystemWatcherSources(FileSystemWatcher watcher)
        {
            return new[] {
                Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    h => watcher.Changed += h,
                    h => watcher.Changed -= h
                    ).Select(ev => new FileChangedEvent(ev.EventArgs))
                        // FileChanges seem to fire multiple events. Ignore them from the same file for 30ms after the first change. That seems to be good enough to block duplicates.
                        .DistinctUntilTimeout(TimeSpan.FromMilliseconds(30), new FileChangedEqualityComparer()),

                 Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    h => watcher.Created += h,
                    h => watcher.Created -= h
                    ).Select(ev => new FileChangedEvent(ev.EventArgs)),

                 Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    h => watcher.Deleted += h,
                    h => watcher.Deleted -= h
                    ).Select(ev => new FileChangedEvent(ev.EventArgs)),

                Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
                    h => watcher.Renamed += h,
                    h => watcher.Renamed -= h
                    ).Select(ev => new FileChangedEvent(ev.EventArgs)),
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

using Codeco.CrossPlatform.Models.FileSystem;
using System;

namespace Codeco.CrossPlatform.Services.DependencyInterfaces
{
    public interface IFileSystemWatcherService
    {
        IObservable<FileChangedEvent> ObserveFolderChanges(string absoluteFolderPath);
    }
}

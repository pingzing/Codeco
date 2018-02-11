using Codeco.CrossPlatform.Extensions.Reactive;
using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.Models.FileSystem;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public class UserFileService : IUserFileService
    {
        private const string UserFilesFolderName = "CodecoFiles";        
        private readonly string _fullUserFilesFolderPath;

        private readonly IAppFolderService _appFolderService;
        private readonly IFileService _fileService;

        public UserFileService(IAppFolderService appFolderService, IFileService fileService)
        {
            _appFolderService = appFolderService;
            _fileService = fileService;

            _fullUserFilesFolderPath = Path.Combine(_appFolderService.GetAppFolderPath(), UserFilesFolderName);

            string localFolderName = FileLocation.Local.FolderName();
            string roamedFolderName = FileLocation.Roamed.FolderName();

            CreateUserFolder(localFolderName);
            CreateUserFolder(roamedFolderName);


            var localFolderWatcher = ObserveFolderChanges(Path.Combine(_fullUserFilesFolderPath, localFolderName));
            localFolderWatcher.Subscribe(x => Debug.WriteLine($"LocalFolder Observed a change to: {x.Name}. ChangeType: {x.ChangeType}"));

            var roamedFolderWatcher = ObserveFolderChanges(Path.Combine(_fullUserFilesFolderPath, roamedFolderName));
            roamedFolderWatcher.Subscribe(x => Debug.WriteLine($"RoamedFolder Observed a change to: {x.Name}. ChangeType: {x.ChangeType}"));
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">A file name only, not a path.</param>
        /// <returns></returns>
        public Task CreateUserFileAsync(string fileName, FileLocation fileLocation)
        {
            string absoluteFilePath = Path.Combine(UserFilesFolderName, fileLocation.FolderName(), fileName);
            return _fileService.CreateFileAsync(absoluteFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">A file name only, not a path.</param>
        /// <returns></returns>
        public Task CreateUserFileAsync(string fileName, FileLocation fileLocation, byte[] data)
        {
            string absoluteFilePath = Path.Combine(UserFilesFolderName, fileLocation.FolderName(), fileName);
            return _fileService.CreateFileAsync(absoluteFilePath);
        }

        /// <summary>
        /// Creates a folder for user files under the CodecoFiles folder 
        /// (which is itself at the application root).
        /// </summary>
        /// <param name="relativeFolderPath">Path of the folder to create, relative to 
        /// the AppRoot/CoedcoFiles/ folder.</param>
        /// <returns>The <see cref="DirectoryInfo"/> of the created folder, or null.</returns>
        public DirectoryInfo CreateUserFolder(string relativeFolderPath)
        {
            string absoluteFolderPath = Path.Combine(_fullUserFilesFolderPath, relativeFolderPath);
            return _fileService.CreateFolder(absoluteFolderPath);
        }

        public async Task<bool> ValidateFileAsync(byte[] dataArray)
        {
            using (var stream = new StreamReader(new MemoryStream(dataArray)))
            {
                IList<string> lines = (await stream.ReadToEndAsync())
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries) // split into lines
                    .Where(x => !String.IsNullOrWhiteSpace(x)) // filter out empty lines                    
                    .ToList();

                if (lines.Count == 0)
                {
                    return false;
                }

                // Split by separator, and ensure only 2 columns
                return lines.Select(x => x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    .All(splitStrings => splitStrings.Length == 2);
            }
        }

        private IObservable<FileChangedEvent> ObserveFolderChanges(string folder)
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

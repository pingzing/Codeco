using Codeco.CrossPlatform.Extensions;
using Codeco.CrossPlatform.Extensions.Reactive;
using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.Models.FileSystem;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.CrossPlatform.ViewModels;
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
        private readonly string _localFolderPath = Path.Combine(UserFilesFolderName, FileLocation.Local.FolderName());
        private readonly string _roamedFolderPath = Path.Combine(UserFilesFolderName, FileLocation.Roamed.FolderName());

        private readonly IAppFolderService _appFolderService;
        private readonly IFileService _fileService;
        private readonly IFileSystemWatcherService _fileSystemWatcherService;

        private SourceList<SimpleFileInfoViewModel> _filesList = new SourceList<SimpleFileInfoViewModel>();        
        public IObservableList<SimpleFileInfoViewModel> FilesList { get; private set; }

        public UserFileService(IAppFolderService appFolderService,
                               IFileService fileService,
                               IFileSystemWatcherService fileSystemWatcherService)
        {
            _appFolderService = appFolderService;
            _fileService = fileService;
            _fileSystemWatcherService = fileSystemWatcherService;

            _fullUserFilesFolderPath = Path.Combine(_appFolderService.GetAppFolderPath(), UserFilesFolderName);

            string localFolderName = FileLocation.Local.FolderName();
            string roamedFolderName = FileLocation.Roamed.FolderName();

            CreateUserFolder(localFolderName);
            CreateUserFolder(roamedFolderName);

            FilesList = _filesList.AsObservableList();

            InitializeFileList(localFolderName, roamedFolderName);
        }

        private async Task InitializeFileList(string localFolderName, string roamedFolderName)
        {
            var localFiles = (await _fileService.GetFilesInFolder(_localFolderPath))
                .Select(x => new SimpleFileInfoViewModel
                {
                    Name = Path.GetFileName(x),
                    Path = x,
                    FileLocation = FileLocation.Local
                });

            var roamedFiles = (await _fileService.GetFilesInFolder(_roamedFolderPath))
                .Select(x => new SimpleFileInfoViewModel
                {
                    Name = Path.GetFileName(x),
                    Path = x,
                    FileLocation = FileLocation.Roamed
                });

            _filesList.AddRange(localFiles);
            _filesList.AddRange(roamedFiles);

            var localFolderWatcher = _fileSystemWatcherService.ObserveFolderChanges(Path.Combine(_fullUserFilesFolderPath, localFolderName));
            var roamedFolderWatcher = _fileSystemWatcherService.ObserveFolderChanges(Path.Combine(_fullUserFilesFolderPath, roamedFolderName));

            var uiThreadContext = await SynchronizationContextExtensions.GetUIThreadAsync();
            Observable.Merge(localFolderWatcher, roamedFolderWatcher)
                .ObserveOn(uiThreadContext)
                .Subscribe(changeEvent =>
                {
                    switch (changeEvent.ChangeType)
                    {
                        case WatcherChangeTypes.Created:
                            _filesList.Add(new SimpleFileInfoViewModel
                            {
                                Name = changeEvent.Name,
                                Path = changeEvent.FullPath,
                                FileLocation = changeEvent.FullPath.Contains(_roamedFolderPath) ? FileLocation.Roamed  : FileLocation.Local
                            });
                            break;
                        case WatcherChangeTypes.Changed:
                            // TODO: Update item size display
                            break;
                        case WatcherChangeTypes.Deleted:
                            var itemToRemove = _filesList.Items.FirstOrDefault(x => x.Path == changeEvent.FullPath);
                            if (itemToRemove != null)
                            {
                                _filesList.Remove(itemToRemove);
                            }
                            break;
                        case WatcherChangeTypes.Renamed:                            
                            var itemToRename = _filesList.Items.FirstOrDefault(x => x.Path == changeEvent.RenamedOldPath);
                            if (itemToRename != null)
                            {
                                // Trigger INotifyPropertyChanged events by updating properties
                                itemToRename.Path = changeEvent.FullPath;
                                itemToRename.Name = changeEvent.Name;
                            }
                            break;
                    }
                });
        }

        /// <summary>
        /// Creates a file with the given name.
        /// </summary>
        /// <param name="fileName">File name only, not a path.</param>
        /// <param name="fileLocation">Whether the file should be stored only on the device, or synced between devices.</param>
        /// <returns></returns>
        public Task CreateUserFileAsync(string fileName, FileLocation fileLocation)
        {
            string absoluteFilePath = Path.Combine(UserFilesFolderName, fileLocation.FolderName(), fileName);
            return _fileService.CreateFileAsync(absoluteFilePath);
        }

        /// <summary>
        /// Creates a file with the given name, and returns its name and a FileStream pointed to it.
        /// </summary>
        /// <param name="fileName">File name only, not a path.</param>
        /// <param name="fileLocation">Whether the file should be stored only on the device, or synced between devices.</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> CreateUserFileAsync(string fileName, FileLocation fileLocation, byte[] data)
        {
            string relativeFilePath = Path.Combine(UserFilesFolderName, fileLocation.FolderName(), fileName);
            var createdFile = await _fileService.CreateFileAsync(relativeFilePath);
            using (createdFile.Stream)
            {
                await createdFile.Stream.WriteAsync(data, 0, data.Length);
            }
            return createdFile.FileName;
        }

        /// <summary>
        /// Creates a file with the given name, and returns its name and a FileStream pointed to it.
        /// </summary>
        /// <param name="fileName">File name only, not a path.</param>
        /// <param name="fileLocation">Whether the file should be stored only on the device, or synced between devices.</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> CreateUserFileAsync(string fileName, FileLocation fileLocation, string data)
        {
            string relativeFilePath = Path.Combine(UserFilesFolderName, fileLocation.FolderName(), fileName);
            var createdFile = await _fileService.CreateFileAsync(relativeFilePath);
            using (createdFile.Stream)
            {
                using (var streamWriter = new StreamWriter(createdFile.Stream))
                {
                    await streamWriter.WriteAsync(data);
                }
            }
            return createdFile.FileName;
        }
        
        public async Task DeleteUserFileAsync(string fileName, FileLocation fileLocation)
        {
            string relativeFilePath = Path.Combine(_localFolderPath, fileName);
            await _fileService.DeleteFileAsync(relativeFilePath);
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

        private string GetRelativeFilePath(string fileName, FileLocation location)
        {
            switch (location)
            {
                case FileLocation.Roamed:
                    return Path.Combine(_roamedFolderPath, fileName);
                default:
                case FileLocation.Local:
                    return Path.Combine(_localFolderPath, fileName);
            }
        }        
    }
}

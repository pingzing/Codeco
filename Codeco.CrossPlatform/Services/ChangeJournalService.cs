using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Codeco.CrossPlatform.Services
{
    public class ChangeJournalService : IChangeJournalService, IInitializesAsync
    {
        private readonly IAppFolderService _appFolderService;
        private readonly IFileService _fileService;
        private readonly IUserFileService _userFileService;
        private readonly IFileSystemWatcherService _fileSystemWatcherService;

        private readonly string _appFolderPath;
        private const string JournalFileName = "ChangeJournal.json";
        private readonly string _localFolderPath = Path.Combine(UserFileService.UserFilesFolderName, FileLocation.Local.FolderName());
        private readonly string _roamedFolderPath = Path.Combine(UserFileService.UserFilesFolderName, FileLocation.Roamed.FolderName());        

        public Task Initialization { get; private set; }

        public ChangeJournalService(IAppFolderService appFolderService,
                                    IFileService fileService,
                                    IUserFileService userFileService,
                                    IFileSystemWatcherService fileSystemWatcherService)
        {
            _appFolderService = appFolderService;
            _fileSystemWatcherService = fileSystemWatcherService;
            _fileService = fileService;
            _userFileService = userFileService;
            _appFolderPath = _appFolderService.GetAppFolderPath();

            Initialization = InitializeWatchersAsync();
        }

        private async Task InitializeWatchersAsync()
        {
            // Await the UserFileService's initialization, because it's responsible for creating 
            // the two folders we'll be watching.
            await (_userFileService as IInitializesAsync).Initialization;

            var localFolderWatcher = _fileSystemWatcherService.ObserveFolderChanges(Path.Combine
            (
                _appFolderPath,
                _localFolderPath
            ));

            var roamedFolderWatcher = _fileSystemWatcherService.ObserveFolderChanges(Path.Combine
            (
                _appFolderPath,
                _roamedFolderPath
            ));                                   

            Observable.Merge(localFolderWatcher, roamedFolderWatcher)
                .Buffer(TimeSpan.FromSeconds(10))
                .Subscribe(async changeEvents =>
                {
                    var eventBuffer = new BlockingCollection<ChangeJournalEntry>();
                    var processingTask = ProcessChangeLoop(eventBuffer);
                    foreach (var changeEvent in changeEvents)
                    {
                        switch (changeEvent.ChangeType)
                        {
                            case WatcherChangeTypes.Created:
                                string createRelativePath = _userFileService.GetRelativeFilePath(changeEvent.Name, GetFileLocation(changeEvent.FullPath));
                                string fileJson = await _fileService.GetFileContentsAsync(createRelativePath);
                                byte[] fileBytes = Encoding.UTF8.GetBytes(fileJson);
                                eventBuffer.Add(new CreationJournalEntry
                                {
                                    ChangeType = WatcherChangeTypes.Created,
                                    FileContents = fileBytes,
                                    RelativeFilePath = createRelativePath,
                                    Id = Guid.NewGuid(),
                                    Timestamp = DateTimeOffset.UtcNow
                                });
                                break;
                            case WatcherChangeTypes.Deleted:
                                string deleteRelativePath = _userFileService.GetRelativeFilePath(changeEvent.Name, GetFileLocation(changeEvent.FullPath));
                                eventBuffer.Add(new DeletionJournalEntry
                                {
                                    ChangeType = WatcherChangeTypes.Deleted,
                                    Id = Guid.NewGuid(),
                                    RelativeFilePath = deleteRelativePath,
                                    Timestamp = DateTimeOffset.UtcNow
                                });
                                break;
                            case WatcherChangeTypes.Renamed:
                                string oldRelativePath = _userFileService.GetRelativeFilePath(changeEvent.RenamedOldName, GetFileLocation(changeEvent.RenamedOldPath));
                                string newRelativePath = _userFileService.GetRelativeFilePath(changeEvent.Name, GetFileLocation(changeEvent.FullPath));
                                eventBuffer.Add(new RenameJournalEntry
                                {
                                    ChangeType = WatcherChangeTypes.Renamed,
                                    Id = Guid.NewGuid(),
                                    OldRelativePath = oldRelativePath,
                                    RelativeFilePath = newRelativePath,
                                    Timestamp = DateTimeOffset.UtcNow
                                });
                                break;
                        }
                    }
                    eventBuffer.CompleteAdding();
                    await processingTask;
                });
        }

        private async Task ProcessChangeLoop(BlockingCollection<ChangeJournalEntry> buffer)
        {                        
            using (var fileStream = await _fileService.OpenOrCreateFileAsync(JournalFileName))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    foreach (var changeEvent in buffer.GetConsumingEnumerable())
                    {                        
                        Debug.WriteLine($"Processing changeEvent: {changeEvent.ChangeType} for file {changeEvent.RelativeFilePath}.");
                        string serializedChangedEvent = JsonConvert.SerializeObject(changeEvent);
                        writer.WriteLine(serializedChangedEvent);

                    }
                }
            }            
        }

        private FileLocation GetFileLocation(string filePath)
        {
            if (filePath.Contains(_localFolderPath))
            {
                return FileLocation.Local;
            }
            else if (filePath.Contains(_roamedFolderPath))
            {
                return FileLocation.Roamed;
            }
            else
            {
                throw new ArgumentException($"{nameof(GetFileLocation)} received a filePath for a file in neither the Roamed nor the Local folders.");
            }

        }
    }
}

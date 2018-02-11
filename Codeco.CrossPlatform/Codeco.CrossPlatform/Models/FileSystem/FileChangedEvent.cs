using System.IO;

namespace Codeco.CrossPlatform.Models.FileSystem
{
    public class FileChangedEvent
    {
        public string FullPath { get; set; }
        public WatcherChangeTypes ChangeType { get; set; }
        public string Name { get; set; }
        public bool IsRenamed => ChangeType == WatcherChangeTypes.Renamed;

        public string RenamedOldPath { get; set; }
        public string RenamedOldName { get; set; }

        public FileChangedEvent(FileSystemEventArgs eventArgs)
        {
            FullPath = eventArgs.FullPath;
            ChangeType = eventArgs.ChangeType;
            Name = eventArgs.Name;
        }

        public FileChangedEvent(RenamedEventArgs renamedEvent) 
            : this((FileSystemEventArgs)renamedEvent)
        {
            RenamedOldPath = renamedEvent.OldFullPath;
            RenamedOldName = renamedEvent.OldName;
        }
    }
}

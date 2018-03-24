using System;
using System.IO;

namespace Codeco.CrossPlatform.Models
{
    public class ChangeJournalEntry
    {
        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string RelativeFilePath { get; set; }
        public WatcherChangeTypes ChangeType { get; set; }
    }

    public class CreationJournalEntry : ChangeJournalEntry
    {
        public byte[] FileContents { get; set; }        
    }

    public class DeletionJournalEntry : ChangeJournalEntry
    {
        // maybe don't even need this?
    }

    public class RenameJournalEntry : ChangeJournalEntry
    {
        public string OldRelativePath { get; set; }
    }
}

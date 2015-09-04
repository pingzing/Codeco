using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace CodeStore8UI.Model
{
    public class SavedFiles
    {
        public IEnumerable<StorageFile> LocalFiles { get; set; }
        public IEnumerable<StorageFile> RoamingFiles { get; set; }
    }
}

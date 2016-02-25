using System.Collections.Generic;
using Windows.Storage;

namespace Codeco.Windows10.Models
{
    public class SavedFiles
    {
        public IEnumerable<StorageFile> LocalFiles { get; set; }
        public IEnumerable<StorageFile> RoamingFiles { get; set; }
    }
}

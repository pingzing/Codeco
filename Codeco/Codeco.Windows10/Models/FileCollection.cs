using Codeco.Windows10.Services;
using System.Collections.ObjectModel;

namespace Codeco.Windows10.Models
{
    public class FileCollection
    {
        public string Title { get; set; }
        public ObservableCollection<IBindableStorageFile> Files {get; set;}
        public FileService.FileLocation Location { get; set; }

        public FileCollection(string title, FileService.FileLocation location)
        {
            Title = title;
            Files = new ObservableCollection<IBindableStorageFile>();
            Location = location;
        }

        public FileCollection(string title, ObservableCollection<IBindableStorageFile> files, FileService.FileLocation location)
        {
            Title = title;
            Files = files;
            Location = location;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}

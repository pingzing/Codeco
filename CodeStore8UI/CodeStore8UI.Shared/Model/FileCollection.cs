using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CodeStore8UI.Model
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

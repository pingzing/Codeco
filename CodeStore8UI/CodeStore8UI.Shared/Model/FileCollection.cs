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

        public FileCollection(string title)
        {
            Title = title;
            Files = new ObservableCollection<IBindableStorageFile>();
        }

        public FileCollection(string title, ObservableCollection<IBindableStorageFile> files)
        {
            Title = title;
            Files = files;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CodeStore8UI.Model
{
    public class FileCollection
    {
        public string Title { get; set; }
        public ObservableCollection<BindableStorageFile> Files {get; set;}

        public FileCollection(string title)
        {
            Title = title;
            Files = new ObservableCollection<BindableStorageFile>();
        }

        public FileCollection(string title, ObservableCollection<BindableStorageFile> files)
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

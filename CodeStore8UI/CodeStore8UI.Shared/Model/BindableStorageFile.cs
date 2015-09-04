using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeStore8UI.Model
{
    public class BindableStorageFile : INotifyPropertyChanged
    {
        private const uint BYTES_PER_KB = 1024;
        private const uint BYTES_PER_MB = BYTES_PER_KB * 1024;
        private const uint BYTES_PER_GB = BYTES_PER_MB * 1024;        

        private BindableStorageFile() { }

        public static async Task<BindableStorageFile> Create(StorageFile file)
        {
            BindableStorageFile bsf = new BindableStorageFile();
            bsf._backingFile = file;

            var props = await bsf._backingFile.GetBasicPropertiesAsync();            
            bsf._fileSize = GetHumanReadableSize(props.Size);
           
            return bsf;
        }
        
        public string Name { get { return _backingFile.Name; }}
        public DateTime CreateDate { get { return _backingFile.DateCreated.DateTime; } }
        public bool IsRoamed { get; set; }

        private StorageFile _backingFile;
        public StorageFile BackingFile
        {
            get
            {
                return _backingFile;
            }
            set
            {
                if(value == _backingFile)
                {
                    return;
                }
                _backingFile = value;
                UpdateBoundSize(_backingFile);  
                RaisePropertyChanged();
            }
        }        

        private string _fileSize = "0 b";
        public string FileSize
        {
            get
            {
                return _fileSize;
            }            
            private set
            {
                if(value == _fileSize)
                {
                    return;
                }
                _fileSize = value;
                RaisePropertyChanged();
            }
        }

        private async void UpdateBoundSize(StorageFile _backingFile)
        {
            var props = await _backingFile.GetBasicPropertiesAsync();
            FileSize = GetHumanReadableSize(props.Size);
        }

        private static string GetHumanReadableSize(ulong size)
        {
            if (size < 1024)
            {
                return $"{size} B"; //bytes
            }
            else if (size > BYTES_PER_KB && size < BYTES_PER_MB)
            {
                return $"{ (double)(size / BYTES_PER_KB)} kB"; //kilobytes
            }
            else if (size > BYTES_PER_MB && size < BYTES_PER_GB)
            {
                return $"{(double)(size / BYTES_PER_MB)} mB"; //megabytes
            }
            else //size > BYTES_PER_GB
            {
                return $"{(double)(size / BYTES_PER_GB)} gB"; //gigabytes
            }
        }        

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}

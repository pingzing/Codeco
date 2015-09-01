using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeStore8UI.Model
{
    public class BindableStorageFile
    {
        private const uint BYTES_PER_KB = 1024;
        private const uint BYTES_PER_MB = BYTES_PER_KB * 1024;
        private const uint BYTES_PER_GB = BYTES_PER_MB * 1024;

        private StorageFile _backingFile;

        private BindableStorageFile() { }

        public static async Task<BindableStorageFile> Create(StorageFile file)
        {
            BindableStorageFile bsf = new BindableStorageFile();
            bsf._backingFile = file;

            var props = await bsf._backingFile.GetBasicPropertiesAsync();
            ulong size = props.Size;
            if (size < 1024)
            {
                bsf._fileSize = $"{size} B"; //bytes
            }
            else if (size > BYTES_PER_KB && size < BYTES_PER_MB)
            {
                bsf._fileSize = $"{ (double)(size / BYTES_PER_KB)} kB"; //kilobytes
            }
            else if (size > BYTES_PER_MB && size < BYTES_PER_GB)
            {
                bsf._fileSize = $"{(double)(size / BYTES_PER_MB)} mB"; //megabytes
            }
            else //size > BYTES_PER_GB
            {
                bsf._fileSize = $"{(double)(size / BYTES_PER_GB)} gB"; //gigabytes
            }
            return bsf;
        }

        public StorageFile BackingFile { get{ return _backingFile; } }
        public string Name { get { return _backingFile.Name; }}
        public DateTime CreateDate { get { return _backingFile.DateCreated.DateTime; } }

        private string _fileSize = "0 b";
        public string FileSize
        {
            get
            {
                return _fileSize;
            }            
        }
    }
}

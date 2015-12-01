using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Codeco.Model
{
    public interface IBindableStorageFile
    {
        string Name { get; }
        DateTime CreateDate { get; }
        bool IsRoamed { get; set; }
        IStorageFile BackingFile { get; set; }
        string FileSize { get; }

        Task<ulong> GetFileSizeInBytes();
        void NameChanged();  
    }
}

using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Codeco.Windows10.Models
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

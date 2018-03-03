using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.Windows10.Services
{
    public interface IInitializationValueService
    {
        Task Add(string fileName, string iv, FileService.FileLocation location);
        Task Remove(string key, FileService.FileLocation location);
        Task<string> GetValue(string key, FileService.FileLocation location);
        Task<bool> ContainsKey(string key, FileService.FileLocation location);
        Task LoadFromStorage();
        Task<ulong> GetIVFileSize(FileService.FileLocation location);
        Task SaveToStorage(FileService.FileLocation location);
    }
}

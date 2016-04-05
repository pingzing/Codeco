using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.Windows10.Services
{
    public interface IInitializationValueService
    {
        Task AddKeyPair(string fileName, string iv);
        Task RemoveKeyPair(string key);
        Task<string> GetValue(string key);
        Task LoadFromStorage();
        Task<ulong> GetIVFileSize();
        Task SaveToStorage();
    }
}

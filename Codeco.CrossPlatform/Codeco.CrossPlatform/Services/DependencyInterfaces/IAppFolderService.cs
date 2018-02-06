using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services.DependencyInterfaces
{
    public interface IAppFolderService
    {
        string GetAppFolderPath();

        Task<SafeFileHandle> UWPOpenOrCreateSafeFileHandle(string path);
    }
}

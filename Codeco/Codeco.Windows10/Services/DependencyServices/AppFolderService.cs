using System;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.Windows10.Services.DependencyServices;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32.SafeHandles;

[assembly: Xamarin.Forms.Dependency(typeof(AppFolderService))]
namespace Codeco.Windows10.Services.DependencyServices
{
    public class AppFolderService : IAppFolderService
    {
        public string GetAppFolderPath()
        {
            return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        }

        public async Task<SafeFileHandle> UWPOpenOrCreateSafeFileHandle(string fileName)
        {
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(fileName);
            return file.CreateSafeFileHandle();
        }
    }
}

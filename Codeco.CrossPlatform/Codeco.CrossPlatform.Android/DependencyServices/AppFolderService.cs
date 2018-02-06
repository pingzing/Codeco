using Codeco.CrossPlatform.Android.DependencyServices;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Microsoft.Win32.SafeHandles;
using System;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AppFolderService))]
namespace Codeco.CrossPlatform.Android.DependencyServices
{
    public class AppFolderService : IAppFolderService
    {
        public string GetAppFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public Task<SafeFileHandle> UWPOpenOrCreateSafeFileHandle(string path)
        {
            throw new NotImplementedException($"This method is only implemented on UWP. Use {nameof(GetAppFolderPath)} instead, and use that path to construct a Stream.");
        }
    }
}

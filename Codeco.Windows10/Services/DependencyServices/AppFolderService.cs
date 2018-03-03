using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.Windows10.Services.DependencyServices;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(AppFolderService))]
namespace Codeco.Windows10.Services.DependencyServices
{
    public class AppFolderService : IAppFolderService
    {
        public string GetAppFolderPath()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }
    }
}

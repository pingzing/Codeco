using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Windows.Storage;

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

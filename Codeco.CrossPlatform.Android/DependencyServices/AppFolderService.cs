using Codeco.CrossPlatform.Android.DependencyServices;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(AppFolderService))]
namespace Codeco.CrossPlatform.Android.DependencyServices
{
    public class AppFolderService : IAppFolderService
    {
        public string GetAppFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }       
    }
}

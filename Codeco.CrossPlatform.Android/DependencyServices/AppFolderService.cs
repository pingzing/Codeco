using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System;

namespace Codeco.CrossPlatform.Droid.DependencyServices
{
    public class AppFolderService : IAppFolderService
    {
        public string GetAppFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }       
    }
}

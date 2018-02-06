using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Services
{
    public class FileService : IFileService
    {
        private readonly IAppFolderService _appFolderService;

        public FileService(IAppFolderService appFolderService)
        {
            _appFolderService = appFolderService;

            FileSystemWatcher watcher = new FileSystemWatcher(_appFolderService.GetAppFolderPath());
            watcher.Changed += Changed;
            watcher.Created += Created;
            watcher.Deleted += Deleted;
            watcher.Renamed += Renamed;

            watcher.EnableRaisingEvents = true;
        }

        public async Task CreateFileAsync(string fileName)
        {
            FileStream fileStream = null;
            if (Device.RuntimePlatform == Device.UWP)
            {
                var fileHandle = await _appFolderService.UWPOpenOrCreateSafeFileHandle(fileName);
                fileStream = new FileStream(fileHandle, FileAccess.ReadWrite, 4096, false);
            }
            else
            {
                fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, true);
            }

            using (fileStream)
            using (var streamWriter = new StreamWriter(fileStream))
            {
                await streamWriter.WriteAsync("test value");
            }
        }

        private void Changed(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"CHANGED: ChangeType: {e.ChangeType}, Name: {e.Name}");
        }

        private void Created(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"CREATED: ChangeType: {e.ChangeType}, {e.Name}");
        }

        private void Deleted(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"DELETED: ChangeType: {e.ChangeType}, {e.Name}");
        }

        private void Renamed(object sender, RenamedEventArgs e)
        {
            Debug.WriteLine($"RENAMED: ChangeType: {e.ChangeType}, {e.Name}");
        }

    }
}

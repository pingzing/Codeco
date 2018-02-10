using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public class FileService : IFileService
    {
        private readonly IAppFolderService _appFolderService;

        public FileService(IAppFolderService appFolderService)
        {
            _appFolderService = appFolderService;
        }

        public async Task CreateFileAsync(string fileName)
        {
            FileStream fileStream = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Since we're in a Xamarin Forms project, we can infer that this is UWP
            {
                var fileHandle = await _appFolderService.UWPOpenOrCreateSafeFileHandle(fileName);
                using (fileStream = new FileStream(fileHandle, FileAccess.ReadWrite, 4096, false))
                {
                    // Don't do anything with the filestream--it's already created the file.
                }
            }
            else
            {
                using (fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, true))
                {
                    // Don't do anything with the filestream--it's already created the file.
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="absoluteFolderPath"></param>
        /// <returns></returns>
        public DirectoryInfo CreateFolder(string absoluteFolderPath)
        {
            try
            {
                DirectoryInfo createdDir = Directory.CreateDirectory(absoluteFolderPath);
                return createdDir;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to create folder at {absoluteFolderPath}. Reason: {ex.Message}");
                return null;
            }
        }

    }
}

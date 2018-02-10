using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public class UserFileService : IUserFileService
    {
        private const string UserFilesFolderName = "CodecoFiles";
        private const string LocalFilesFolderName = "Local";
        private const string RoamedFilesFolderNamed = "Roamed";
        private readonly string _fullUserFilesFolderPath;

        private readonly IAppFolderService _appFolderService;
        private readonly IFileService _fileService;

        public UserFileService(IAppFolderService appFolderService, IFileService fileService)
        {
            _appFolderService = appFolderService;
            _fileService = fileService;

            _fullUserFilesFolderPath = Path.Combine(_appFolderService.GetAppFolderPath(), UserFilesFolderName);

            CreateUserFolder(LocalFilesFolderName);
            CreateUserFolder(RoamedFilesFolderNamed);

            FileSystemWatcher watcher = new FileSystemWatcher(_fullUserFilesFolderPath);
            watcher.Changed += Changed;
            watcher.Created += Created;
            watcher.Deleted += Deleted;
            watcher.Renamed += Renamed;

            watcher.EnableRaisingEvents = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">A file name only, not a path.</param>
        /// <returns></returns>
        public Task CreateUserFileAsync(string fileName)
        {
            return _fileService.CreateFileAsync(fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeFolderPath">Path of the folder to create, relative to the app root.</param>
        /// <returns></returns>
        public void CreateUserFolder(string relativeFolderPath)
        {
            string absoluteFolderPath = Path.Combine(_fullUserFilesFolderPath, relativeFolderPath);
            _fileService.CreateFolder(absoluteFolderPath);
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codeco.CrossPlatform.Droid.DependencyServices;
using Codeco.CrossPlatform.Models.FileSystem;
using Codeco.CrossPlatform.Services.DependencyInterfaces;

[assembly: Xamarin.Forms.Dependency(typeof(NativeFileServiceFacade))]
namespace Codeco.CrossPlatform.Droid.DependencyServices
{
    public class NativeFileServiceFacade : INativeFileServiceFacade
    {
        public static readonly string AppDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public Task<List<string>> GetFilesAsync(string relativeFolderPath)
        {
            string absoluteFolderPath = Path.Combine(AppDataRoot, relativeFolderPath);
            return Task.FromResult(Directory.GetFiles(absoluteFolderPath).ToList());
        }

        public Task<CreateFileResult> CreateFileAsync(string relativeFilePath)
        {
            string absoluteFilePath = Path.Combine(AppDataRoot, relativeFilePath);
            if (File.Exists(absoluteFilePath))
            {
                absoluteFilePath = GenerateUniqueFilePath(absoluteFilePath);
            }
            var fileStream = new FileStream(absoluteFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, true);
            return Task.FromResult(new CreateFileResult
            {
                FileName = absoluteFilePath,
                Stream = fileStream
            });
        }

        public Task<FileStream> OpenOrCreateFileAsync(string relativeFilePath)
        {
            string absoluteFilePath = Path.Combine(AppDataRoot, relativeFilePath);
            var fileStream = new FileStream(absoluteFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, true);
            if (File.Exists(absoluteFilePath))
            {
                absoluteFilePath = GenerateUniqueFilePath(absoluteFilePath);
            }
            return Task.FromResult(fileStream);
        }

        public Task<string> RenameFileAsync(string relativeFilePath, string newName)
        {

            string absoluteFilePath = Path.Combine(AppDataRoot, relativeFilePath);
            string absoluteDestPath = Path.Combine(Path.GetDirectoryName(absoluteFilePath), newName);
            if (File.Exists(absoluteDestPath))
            {
                absoluteDestPath = GenerateUniqueFilePath(absoluteDestPath);
            }
            File.Move(absoluteFilePath, absoluteDestPath);
            return Task.FromResult(absoluteDestPath);
        }

        public Task DeleteFileAsync(string relativeFilePath)
        {
            string absoluteFilePath = Path.Combine(AppDataRoot, relativeFilePath);
            File.Delete(absoluteFilePath);
            return Task.CompletedTask;
        }

        private string GenerateUniqueFilePath(string absoluteFilePath)
        {
            int count = 1;

            string filenameOnly = Path.GetFileNameWithoutExtension(absoluteFilePath);
            string extension = Path.GetExtension(absoluteFilePath);
            string pathWithoutName = Path.GetDirectoryName(absoluteFilePath);

            string newFullPath = absoluteFilePath;

            // using a do-loop, because if we're in here, we already need at least one iteration, so we don't need to check 
            // the sentinel expression on the first iteration
            do
            {
                string tempFileName = $"{filenameOnly}({count})";
                newFullPath = Path.Combine(pathWithoutName, $"{tempFileName}{extension}");
                count += 1;
            } while (File.Exists(newFullPath));

            return newFullPath;
        }
    }
}
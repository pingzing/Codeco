using Codeco.CrossPlatform.Models.FileSystem;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services
{
    public class FileService : IFileService
    {
        private readonly INativeFileServiceFacade _nativeFileService;

        public FileService(INativeFileServiceFacade nativeFileService)
        {
            _nativeFileService = nativeFileService;
        }

        public async Task OpenOrCreateFileAsync(string relativeFilePath)
        {
            using (var fileStream = await _nativeFileService.OpenOrCreateFileAsync(relativeFilePath))
            {
                // Just let it close, it's already been created.
            }
        }

        public async Task<CreateFileResult> CreateFileAsync(string relativeFilePath)
        {
            var createdFile = await _nativeFileService.CreateFileAsync(relativeFilePath);
            if (createdFile == null || createdFile.Stream == null)
            {
                return null;
            }

            return createdFile;
        }

        public async Task WriteBytesAsync(string relativeFilePath, byte[] data)
        {
            using (var fileStream = await _nativeFileService.OpenOrCreateFileAsync(relativeFilePath))
            {
                await fileStream.WriteAsync(data, 0, data.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeFolderPath"></param>
        /// <returns></returns>
        public DirectoryInfo CreateFolder(string relativeFolderPath)
        {
            try
            {
                DirectoryInfo createdDir = Directory.CreateDirectory(relativeFolderPath);
                return createdDir;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to create folder at {relativeFolderPath}. Reason: {ex.Message}");
                return null;
            }
        }

        public async Task<string> RenameFileAsync(string relativeFilePath, string newName)
        {
            return await _nativeFileService.RenameFileAsync(relativeFilePath, newName);
        }

        public Task<string> MoveFileAsync(string sourceRelativeFilePath, string destinationRelativeFlePath)
        {
            return _nativeFileService.MoveFileAsync(sourceRelativeFilePath, destinationRelativeFlePath);
        }

        public Task DeleteFileAsync(string relativeFilePath)
        {
            return _nativeFileService.DeleteFileAsync(relativeFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeFolderPath"></param>
        /// <returns></returns>
        public Task<List<string>> GetFilesInFolder(string relativeFolderPath)
        {
            return _nativeFileService.GetFilesAsync(relativeFolderPath);
        }       
    }
}

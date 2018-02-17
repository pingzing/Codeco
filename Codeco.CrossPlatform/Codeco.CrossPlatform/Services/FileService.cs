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

        public async Task OpenOrCreateFileAsync(string relativeFileName)
        {
            using (var fileStream = await _nativeFileService.OpenOrCreateFileAsync(relativeFileName))
            {
                // Just let it close, it's already been created.
            }
        }

        public async Task CreateFileAsync(string relativeFileName)
        {
            using (var fileStream = await _nativeFileService.CreateFileAsync(relativeFileName))
            {

            }
        }

        public async Task WriteBytesAsync(string relativeFileName, byte[] data)
        {
            using (var fileStream = await _nativeFileService.OpenOrCreateFileAsync(relativeFileName))
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

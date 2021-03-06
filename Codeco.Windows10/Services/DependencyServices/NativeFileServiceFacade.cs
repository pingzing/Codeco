﻿using Codeco.CrossPlatform.Models.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Codeco.Windows10.Services.DependencyServices
{
    public class NativeFileServiceFacade : CrossPlatform.Services.DependencyInterfaces.INativeFileServiceFacade
    {
        public static readonly StorageFolder AppDataRoot = ApplicationData.Current.LocalFolder;

        public async Task<List<string>> GetFilesAsync(string relativeFolderPath)
        {
            var folder = await AppDataRoot.CreateFolderAsync(relativeFolderPath, CreationCollisionOption.OpenIfExists);
            var foundFilePaths = (await folder.GetFilesAsync())?.Select(x => x.Path).ToList();
            return foundFilePaths ?? new List<string>();
        }

        public async Task<CreateFileResult> CreateFileAsync(string relativeFilePath)
        {
            var file = await AppDataRoot.CreateFileAsync(relativeFilePath, CreationCollisionOption.GenerateUniqueName);
            var fileHandle = file.CreateSafeFileHandle();
            FileStream fileStream = new FileStream(fileHandle, FileAccess.ReadWrite, 4096, false);
            return new CreateFileResult
            {
                FileName = file.Name,
                Stream = fileStream
            };
        }

        public async Task<FileStream> OpenOrCreateFileAsync(string relativeFilePath)
        {
            var file = await AppDataRoot.CreateFileAsync(relativeFilePath, CreationCollisionOption.OpenIfExists);
            var fileHandle = file.CreateSafeFileHandle();
            FileStream fileStream = new FileStream(fileHandle, FileAccess.ReadWrite, 4096, false);
            return fileStream;
        }

        public async Task<string> RenameFileAsync(string relativeFilePath, string newName)
        {
            var file = await AppDataRoot.GetFileAsync(relativeFilePath);
            await file.RenameAsync(newName, NameCollisionOption.GenerateUniqueName);
            return file.Name;
        }

        public async Task<string> MoveFileAsync(string sourceRelativeFilePath, string destinationRelativeFlePath)
        {
            string destFolderPath = Path.GetDirectoryName(destinationRelativeFlePath);
            string destFileName = Path.GetFileName(destinationRelativeFlePath);

            var file = await AppDataRoot.GetFileAsync(sourceRelativeFilePath);            
            var destinationFolder = await AppDataRoot.GetFolderAsync(destFolderPath);
            await file.MoveAsync(destinationFolder, destFileName, NameCollisionOption.GenerateUniqueName);
            return file.Name;
        }

        public async Task DeleteFileAsync(string relativeFilePath)
        {
            var file = await AppDataRoot.GetFileAsync(relativeFilePath);
            await file.DeleteAsync();
        }

        public async Task<string> GetFileContentsAsync(string relativeFilePath)
        {
            var file = await AppDataRoot.GetFileAsync(relativeFilePath);
            string fileContents = await FileIO.ReadTextAsync(file);
            return fileContents;
        }
    }
}

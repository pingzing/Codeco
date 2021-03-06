﻿using Codeco.CrossPlatform.Models.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Services.DependencyInterfaces
{
    public interface INativeFileServiceFacade
    {
        /// <summary>
        /// Creates a file for the given path, relative to the application data root. In the event of a filename collision,
        /// a substitute name will be chosen.
        /// </summary>
        /// <param name="relativeFilePath">The filepath, relative to the application data root.</param>
        /// <returns>The name of the created file, and an open filestream pointed to it.</returns>
        Task<CreateFileResult> CreateFileAsync(string relativeFilePath);

        /// <summary>
        /// Creates (or opens, if exists) a FileStream for the file at the given path, relative to the application data root.
        /// </summary>
        /// <param name="relativeFilePath">The filepath, relative to the application data root.</param>
        /// <returns>A <see cref="FileStream"/> pointing to the opened-or-created file, or null.</returns>
        Task<FileStream> OpenOrCreateFileAsync(string relativeFilePath);      
        
        /// <summary>
        /// Gets the full, absolute paths of all files contained inside the given folder, or an empty list if there are none.
        /// </summary>
        /// <param name="relativeFolderPath">The path of the folder to search, relative to teh application data root.</param>
        /// <returns>A list of the fully-qualified paths of all files contained inside the given folder, or an empty list.</returns>
        Task<List<string>> GetFilesAsync(string relativeFolderPath);

        Task DeleteFileAsync(string relativeFilePath);
        Task<string> RenameFileAsync(string relativeFilePath, string newName);
        Task<string> MoveFileAsync(string sourceRelativeFilePath, string destinationRelativeFlePath);
        Task<string> GetFileContentsAsync(string relativeFilePath);
    }
}


using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codeco.CrossPlatform.Droid.DependencyServices;
using Codeco.CrossPlatform.Services.DependencyInterfaces;

[assembly: Xamarin.Forms.Dependency(typeof(NativeFileServiceFacade))]
namespace Codeco.CrossPlatform.Droid.DependencyServices
{
    public class NativeFileServiceFacade : INativeFileServiceFacade
    {
        public static readonly string AppDataRoot = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

        public Task<List<string>> GetFilesAsync(string relativeFolderPath)
        {
            string absoluteFolderPath = Path.Combine(AppDataRoot, relativeFolderPath);
            return new Task<List<string>>(() => Directory.GetFiles(absoluteFolderPath).ToList());
        }

        public Task<FileStream> OpenOrCreateFileAsync(string relativeFilePath)
        {
            string absoluteFilePath = Path.Combine(AppDataRoot, relativeFilePath);
            var fileStream = new FileStream(absoluteFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, true);
            return new Task<FileStream>(() => fileStream);
        }
    }
}
using System.IO;

namespace Codeco.CrossPlatform.Models.FileSystem
{
    public class CreateFileResult
    {
        public string FileName { get; set; }
        public FileStream Stream { get; set; }
    }
}

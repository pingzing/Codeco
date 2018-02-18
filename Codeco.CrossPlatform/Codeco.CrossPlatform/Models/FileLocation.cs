using System;

namespace Codeco.CrossPlatform.Models
{
    public enum FileLocation
    {
        Local,
        Roamed
    }

    public static class FileLocationExtensions
    {
        private const string LocalFilesFolderName = "Local";
        private const string RoamedFilesFolderName = "Roamed";

        public static string FolderName(this FileLocation location)
        {
            switch (location)
            {
                case FileLocation.Local: return LocalFilesFolderName;
                case FileLocation.Roamed: return RoamedFilesFolderName;
                default: throw new ArgumentException("The given location enum varaint" +
                    $" ({location}) does not have an assigned folder name. GO WRITE ONE, DUNCE");
            }
        }
    }
}
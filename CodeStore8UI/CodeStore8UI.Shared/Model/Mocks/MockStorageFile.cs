using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace CodeStore8UI.Model.Mocks
{
    public class MockStorageFile : IStorageFile
    {
        public FileAttributes Attributes
        {
            get
            {
                return FileAttributes.Normal;
            }
        }

        public string ContentType
        {
            get
            {
                return "txt";
            }
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return DateTime.Now;
            }
        }

        public string FileType
        {
            get
            {
                return ".txt";
            }
        }

        public string Name
        {
            get
            {
                return "dummy file";
            }
        }

        public string Path
        {
            get
            {
                return @"C:\Some Path\Blarg";
            }
        }

        public IAsyncAction CopyAndReplaceAsync(IStorageFile fileToReplace)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncAction DeleteAsync(StorageDeleteOption option)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<BasicProperties> GetBasicPropertiesAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsOfType(StorageItemTypes type)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAndReplaceAsync(IStorageFile fileToReplace)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IInputStream> OpenSequentialReadAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncAction RenameAsync(string desiredName)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction RenameAsync(string desiredName, NameCollisionOption option)
        {
            throw new NotImplementedException();
        }
    }
}

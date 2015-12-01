using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Codeco.Model.Mocks
{
    class MockBindableStorageFile : IBindableStorageFile
    {
        public IStorageFile BackingFile
        {
            get { return new MockStorageFile(); }
            set { throw new NotImplementedException(); }
        }

        public DateTime CreateDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        public string FileSize
        {
            get
            {
                return "50 kB";
            }
        }

        public bool IsRoamed
        {
            get
            {
                Random rand = new Random();
                return rand.Next() % 2 == 0;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                return "dummy file";
            }
        }

        public Task<ulong> GetFileSizeInBytes()
        {
            return Task.Run(() => (ulong)(50 * 1024));
        }

        public void NameChanged()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CodeStore8UI.Services.Mocks
{
    class MockFileService : IService
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Task<IService> InitializeAsync()
        {
            return null;
        }

        public void RaisePropertyChanged(string property)
        {
            throw new NotImplementedException();
        }
    }
}

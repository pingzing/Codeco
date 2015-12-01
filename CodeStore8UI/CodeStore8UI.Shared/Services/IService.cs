using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.Services
{
    public interface IService
    {
        event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string property);
        Task<IService> InitializeAsync();
    }
}

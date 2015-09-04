using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CodeStore8UI.Services
{
    public interface IService
    {
        event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string property);
        Task<IService> InitializeAsync();
    }
}

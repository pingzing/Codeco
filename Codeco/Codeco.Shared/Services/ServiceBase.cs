using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.Services
{
    public abstract class ServiceBase : IService
    {
        public bool Initialized { get; set; }

        protected ServiceBase()
        {
            InitializeSync();
        }

        public IService InitializeSync()
        {
            CreateAsync();
            return this;
        }

        public async Task<IService> InitializeAsync()
        {
            await CreateAsync();
            return this;
        }

        protected abstract Task CreateAsync();

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}

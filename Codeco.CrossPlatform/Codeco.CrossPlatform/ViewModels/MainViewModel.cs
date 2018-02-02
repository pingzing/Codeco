using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeco.CrossPlatform.ViewModels
{
    public class MainViewModel : NavigableViewModelBase
    {
        public MainViewModel(INavigationService navService) : base(navService)
        {
            
        }
    }
}

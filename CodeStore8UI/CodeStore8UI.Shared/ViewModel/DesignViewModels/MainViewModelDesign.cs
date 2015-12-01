using System;
using System.Collections.Generic;
using System.Text;
using Codeco.Services;
using GalaSoft.MvvmLight.Views;

namespace Codeco.ViewModel.DesignViewModels
{
    public class MainViewModelDesign : MainViewModel
    {
        public MainViewModelDesign(IService fileService, INavigationServiceEx navService) : base(fileService, navService)
        {
        }
    }
}

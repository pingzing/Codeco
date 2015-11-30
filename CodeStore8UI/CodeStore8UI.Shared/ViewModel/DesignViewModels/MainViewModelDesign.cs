using System;
using System.Collections.Generic;
using System.Text;
using CodeStore8UI.Services;
using GalaSoft.MvvmLight.Views;

namespace CodeStore8UI.ViewModel.DesignViewModels
{
    public class MainViewModelDesign : MainViewModel
    {
        public MainViewModelDesign(IService fileService, INavigationServiceEx navService) : base(fileService, navService)
        {
        }
    }
}

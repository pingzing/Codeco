using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Core;
using GalaSoft.MvvmLight;

namespace Codeco.ViewModel
{
    public class UniversalViewModelBase : ViewModelBase
    {
        private INavigationServiceEx _navigationService;

        public UniversalViewModelBase(INavigationServiceEx navService)
        {
            _navigationService = navService;
            SystemNavigationManager.GetForCurrentView().BackRequested += UniversalViewModelBase_BackRequested;
        }

        private void UniversalViewModelBase_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if(_navigationService	)
        }
    }
}

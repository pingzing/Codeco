using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using GalaSoft.MvvmLight;

namespace Codeco.Windows10.ViewModels
{
    public class UniversalBaseViewModel : ViewModelBase
    {
        private readonly SystemNavigationManager _systemNavManager;

        protected readonly INavigationServiceEx NavigationService;

        private bool _allowGoingBack = true;
        protected bool AllowGoingBack
        {
            get { return _allowGoingBack; }
            set
            {
                if (_allowGoingBack != value)
                {
                    _allowGoingBack = value;
                    _navigationService_CanGoBackChanged(null, new CanGoBackChangedHandlerArgs(NavigationService.CanGoBack));
                }                
            }
        }

        public UniversalBaseViewModel(INavigationServiceEx navService)
        {
            NavigationService = navService;
            NavigationService.CanGoBackChanged += _navigationService_CanGoBackChanged;

            _systemNavManager = SystemNavigationManager.GetForCurrentView();
            _systemNavManager.BackRequested += UniversalBaseViewModel_BackRequested;
        }

        private void _navigationService_CanGoBackChanged(object sender, CanGoBackChangedHandlerArgs args)
        {
            bool newCanGoBack = args.NewCanGoBack;
            if (newCanGoBack && AllowGoingBack)
            {
                _systemNavManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                _systemNavManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;                
            }
        }

        private void UniversalBaseViewModel_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if(NavigationService.BackStackDepth == 0)
            {
                //Let the system do whatever it's supposed to
                e.Handled = false;
                return;
            }
            if (NavigationService.CanGoBack && AllowGoingBack)
            {
                NavigationService.GoBack();
                e.Handled = true;
            }
            else
            {
                e.Handled = true; //Swallow the event and do nothing
            }
        }
    }
}

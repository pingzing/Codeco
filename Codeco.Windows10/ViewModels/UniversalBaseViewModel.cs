using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using Codeco.Windows10.Common;
using GalaSoft.MvvmLight;
using Codeco.Windows10.Services;

namespace Codeco.Windows10.ViewModels
{
    public class UniversalBaseViewModel : ViewModelBase, INavigable
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
                    NavigationService_CanGoBackChanged(null, new CanGoBackChangedHandlerArgs(NavigationService.CanGoBack));
                }                
            }
        }

        public UniversalBaseViewModel(INavigationServiceEx navService)
        {
            NavigationService = navService;
            NavigationService.CanGoBackChanged += NavigationService_CanGoBackChanged;

            _systemNavManager = SystemNavigationManager.GetForCurrentView();            
        }

        private void NavigationService_CanGoBackChanged(object sender, CanGoBackChangedHandlerArgs args)
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

        protected virtual void UniversalBaseViewModel_BackRequested(object sender, BackRequestedEventArgs e)
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

        public virtual void Activate(object parameter, NavigationMode navigationMode)
        {
            _systemNavManager.BackRequested += UniversalBaseViewModel_BackRequested;
        }

        public virtual void Deactivated(object parameter)
        {
            _systemNavManager.BackRequested -= UniversalBaseViewModel_BackRequested;
        }
    }
}

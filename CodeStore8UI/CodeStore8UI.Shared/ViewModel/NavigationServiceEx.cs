using System;
using System.Collections.Generic;
using System.Text;
using CodeStore8UI.Common;
#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif
using GalaSoft.MvvmLight.Views;

namespace CodeStore8UI.ViewModel
{
    public class NavigationServiceEx : NavigationService, INavigationServiceEx
    {
        public event EventHandler<UniversalBackPressedEventArgs> BackButtonPressed;

        public void OnBackButtonPressed(object sender, UniversalBackPressedEventArgs args)
        {
            BackButtonPressed?.Invoke(sender, args);
        }
    }
}

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
    public interface INavigationServiceEx : INavigationService
    {
#if WINDOWS_PHONE_APP
        event EventHandler<UniversalBackPressedEventArgs> BackButtonPressed;
        void OnBackButtonPressed(object sender, UniversalBackPressedEventArgs args);
#endif
    }
}

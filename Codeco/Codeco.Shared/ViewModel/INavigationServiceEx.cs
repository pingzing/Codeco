using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Navigation;
using Codeco.Common;
using GalaSoft.MvvmLight.Views;

namespace Codeco.ViewModel
{
    public interface INavigationServiceEx : INavigationService
    {
        event EventHandler<UniversalBackPressedEventArgs> BackButtonPressed;
        void OnBackButtonPressed(object sender, UniversalBackPressedEventArgs args);
        IList<PageStackEntry	> BackStack { get; }
    }
}

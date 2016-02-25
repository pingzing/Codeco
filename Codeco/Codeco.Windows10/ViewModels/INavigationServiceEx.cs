using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;
using Codeco.Windows10.Common;
using GalaSoft.MvvmLight.Views;

namespace Codeco.Windows10.ViewModels
{
    public interface INavigationServiceEx : INavigationService
    {
        event EventHandler<UniversalBackPressedEventArgs> BackButtonPressed;
        void OnBackButtonPressed(object sender, UniversalBackPressedEventArgs args);
        IList<PageStackEntry> BackStack { get; }
    }
}

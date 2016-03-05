using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Navigation;
using Codeco.Windows10.Common;
using GalaSoft.MvvmLight.Views;

namespace Codeco.Windows10.ViewModels
{
    public interface INavigationServiceEx : INavigationService
    {
        new void GoBack();

        int BackStackDepth { get; }
        bool CanGoBack { get; }
        void ClearBackStack();
        void BackStackRemoveAt(int index);
        void BackStackRemove(PageStackEntry entry);        
        PageStackEntry BackStackGet(PageStackEntry entry);
        PageStackEntry BackStackGetAt(int index);        

        event CanGoBackChangedHandler CanGoBackChanged;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Codeco.Common;
#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif
using GalaSoft.MvvmLight.Views;

namespace Codeco.ViewModel
{
    public class NavigationServiceEx : NavigationService, INavigationServiceEx
    {
        public event EventHandler<UniversalBackPressedEventArgs> BackButtonPressed;

        public void OnBackButtonPressed(object sender, UniversalBackPressedEventArgs args)
        {
            BackButtonPressed?.Invoke(sender, args);
        }

        public IList<PageStackEntry> BackStack
        {
            get
            {                           
                var frame = (Frame) Window.Current.Content;
                return frame.BackStack;
            }
        }

        public int BackStackDepth
        {
            get
            {
                var frame = (Frame) Window.Current.Content;
                return frame.BackStackDepth;
            }
        }

        public bool CanGoBack
        {
            get
            {
                var frame = (Frame) Window.Current.Content;
                return frame.CanGoBack;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeStore8UI.Common
{
    public class BindablePage : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navigableViewModel = this.DataContext as INavigable;
            navigableViewModel?.Activate(e.Parameter, e.NavigationMode);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var navigableViewModel = this.DataContext as INavigable;
            navigableViewModel?.Deactivated(e.Parameter);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            var navigable = this.DataContext as INavigable;
            if(navigable == null)
            {
                return;
            }

            navigable.Deactivating(e.Parameter);

            if (e.NavigationMode != NavigationMode.Back || navigable.AllowGoingBack)
            {
                return;
            }            
            e.Cancel = true;
            Application.Current.Exit();
        }
    }
}

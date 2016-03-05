using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Codeco.Windows10.Common
{
    public class BindablePage : Page
    {

        public BindablePage()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Application.Current.Resuming += Current_Resuming;
            }
        }

        private void Current_Resuming(object sender, object e)
        {
            if (Frame.Content == this)
            {
                var navigableViewModel = this.DataContext as INavigable;
                navigableViewModel?.Activate(e, NavigationMode.Refresh);
            }
        }

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
    }
}

using Codeco.Common;
using Codeco.Model;
using Codeco.Services;
using Codeco.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Codeco
{
    public sealed partial class MainPage : BindablePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;                    
        }      

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AppBar.IsOpen = true;
        }

        private void SavedFile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedFile = (sender as StackPanel)?.Tag as BindableStorageFile;
            if (tappedFile == null)
            {
                return;
            }

            var context = (DataContext as MainViewModel);
            context?.ChangeActiveFileCommand.Execute(tappedFile);
        }


        private void SavedFile_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            if (item != null)
            {
                FlyoutBase.ShowAttachedFlyout(item);
                e.Handled = true;
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await (DataContext as MainViewModel).FileService.NukeFiles();

        }        
    }
}

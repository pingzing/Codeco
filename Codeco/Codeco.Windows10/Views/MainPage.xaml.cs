using Codeco.Windows10.Common;
using Codeco.Windows10.Models;
using Codeco.Windows10.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Codeco.Windows10.Views
{
    public sealed partial class MainPage : BindablePage
    {
        public MainViewModel ViewModel { get; }

        public MainPage()
        {            
            this.InitializeComponent();
            ViewModel = DataContext as MainViewModel;
        }                               

        private void SavedFile_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            if(item != null)
            {
                MenuFlyout flyout = FlyoutBase.GetAttachedFlyout(item) as MenuFlyout;
                flyout?.ShowAt(this, e.GetPosition(this));
            }
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
    }
}

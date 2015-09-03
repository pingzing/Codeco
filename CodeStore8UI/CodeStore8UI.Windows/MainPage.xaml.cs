using CodeStore8UI.Common;
using CodeStore8UI.Model;
using CodeStore8UI.ViewModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CodeStore8UI
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

        private void SavedFilesControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                StorageFile file = (e.AddedItems[0] as BindableStorageFile).BackingFile;
                (this.DataContext as MainViewModel).ChangeActiveFileCommand.Execute(file);
            }
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
    }
}

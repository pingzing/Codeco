using CodeStore8UI.Common;
using CodeStore8UI.Controls;
using CodeStore8UI.Model;
using CodeStore8UI.ViewModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CodeStore8UI
{
    public sealed partial class MainPage : BindablePage, IFileOpenPickerContinuable
    {       
        public MainPage()
        {
            this.InitializeComponent();            
        }                       

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            await (DataContext as MainViewModel).AddFile_PhoneContinued(args);
        }       


        private void SavedFile_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            if(item != null)
            {
                FlyoutBase.ShowAttachedFlyout(item);
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

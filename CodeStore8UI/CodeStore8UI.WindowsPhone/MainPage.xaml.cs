using CodeStore8UI.Common;
using CodeStore8UI.Model;
using CodeStore8UI.ViewModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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

        private void SavedFilesControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {                               
                StorageFile file = ((BindableStorageFile)e.AddedItems[0]).BackingFile;
                (this.DataContext as MainViewModel).ChangeActiveFileCommand.Execute(file);                
            }
        }

        //Binding doesn't seem to work, so code-behind it is.
        private void BindablePage_Loaded(object sender, RoutedEventArgs e)
        {
            SavedFiles.ItemsSource = (this.DataContext as MainViewModel).FileGroups;
        }

        private void SavedFile_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            if(item != null)
            {
                FlyoutBase.ShowAttachedFlyout(item);
            }
        }
    }
}

using Codeco.Common;
using Codeco.Model;
using Codeco.ViewModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Codeco
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : BindablePage
    {
        private bool _pageLoaded = false;
        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {            
            _pageLoaded = true;
        }
        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if(!_pageLoaded)
            {
                return;
            }
            var tappedItem = (sender as ToggleSwitch)?.Tag as BindableStorageFile;
            if (tappedItem == null)
            {
                return;
            }

            //Cheap hack to reduce the likelihood of of Togglebuttons being "stuck" in the wrong state after switching groups.
            //It looks like it's an issue with virtualization and reusing the same datatemplate, but failing to update its state.
            //It only occurs if multiple items are swapped from "Roaming" to "Local" simultaneously.
            //Alternatively: disable virtualization? Not sure if UI/UX bug or performance loss is worse.
            await Task.Delay(250);
            
            var context = (DataContext as SettingsViewModel);
            if (!tappedItem.IsRoamed && !context.FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files.Contains(tappedItem))
            {
                context?.RemoveFileFromSyncCommand.Execute(tappedItem);
            }

            if(tappedItem.IsRoamed && !context.FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files.Contains(tappedItem))
            {
                context?.SyncFileCommand.Execute(tappedItem);
            }            
        }
    }
}

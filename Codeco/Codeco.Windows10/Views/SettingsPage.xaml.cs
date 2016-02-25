using Codeco.Windows10.Common;
using Codeco.Windows10.Models;
using Codeco.Windows10.Services;
using Codeco.Windows10.ViewModels;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Codeco.Windows10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : BindablePage
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {            
            var tappedItem = (sender as ToggleSwitch)?.Tag as BindableStorageFile;            
            if (tappedItem == null)
            {
                return;
            }

            var context = (DataContext as SettingsViewModel);
            if (!tappedItem.IsRoamed && !context.FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files.Contains(tappedItem))
            {
                context?.RemoveFileFromSyncCommand.Execute(tappedItem);
            }

            if (tappedItem.IsRoamed && !context.FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files.Contains(tappedItem))
            {
                context?.SyncFileCommand.Execute(tappedItem);
            }            
        }        
    }
}

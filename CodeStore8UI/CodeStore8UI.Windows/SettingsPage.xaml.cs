using CodeStore8UI.Common;
using CodeStore8UI.Model;
using CodeStore8UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CodeStore8UI
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
            FileListView.SelectedItem = null;
            _pageLoaded = true;
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {      
            if(!_pageLoaded)
            {
                return;
            }
            if(e.AddedItems.Count == 0)
            {
                return;
            }
            var selectedFile = e.AddedItems[0] as BindableStorageFile;
            if(selectedFile == null)
            {
                return;
            }

            var context = (DataContext as SettingsViewModel);
            if (selectedFile.IsRoamed)
            {                
                context?.RemoveFileFromSyncCommand.Execute(selectedFile);
            }
            else
            {
                context?.SyncFileCommand.Execute(selectedFile);
            }
        }        
    }
}

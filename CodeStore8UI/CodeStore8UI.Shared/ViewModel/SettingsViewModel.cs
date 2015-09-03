using CodeStore8UI.Common;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System.Collections.ObjectModel;

namespace CodeStore8UI.ViewModel
{
    public class SettingsViewModel : ViewModelBase, INavigable
    {
        private ulong _roamingSpaceUsed = 0;
        public ulong RoamingSpaceUsed
        {
            get { return _roamingSpaceUsed; }
            set
            {
                if(value == _roamingSpaceUsed)
                {
                    return;
                }
                _roamingSpaceUsed = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<StorageFile> _syncingFiles = new ObservableCollection<StorageFile>();
        public ObservableCollection<StorageFile> SyncingFiles
        {
            get { return _syncingFiles; }
            set
            {
                if(value == _syncingFiles)
                {
                    return;
                }
                _syncingFiles = value;
                RaisePropertyChanged();
            }
        }

        public bool AllowGoingBack { get; set; }

        public async void Activate(object parameter, NavigationMode navigationMode)
        {
            var files = await ApplicationData.Current.RoamingFolder.GetFilesAsync(CommonFileQuery.DefaultQuery);
            foreach(var file in files)
            {
                var props = await file.GetBasicPropertiesAsync();
                _roamingSpaceUsed += props.Size;
            }
        }

        public void Deactivate(object parameter)
        {
            
        }
    }
}

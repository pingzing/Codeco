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
using CodeStore8UI.Services;
using CodeStore8UI.Model;
using GalaSoft.MvvmLight.Command;

namespace CodeStore8UI.ViewModel
{
    public class SettingsViewModel : ViewModelBase, INavigable
    {
        private FileService _fileService;

        public bool AllowGoingBack { get; set; }

        private RelayCommand<BindableStorageFile> _syncFileCommand;
        public RelayCommand<BindableStorageFile> SyncFileCommand => 
            _syncFileCommand ?? (_syncFileCommand = new RelayCommand<BindableStorageFile>(SyncFile));

        private RelayCommand<BindableStorageFile> _removeFileFromSyncCommand;
        public RelayCommand<BindableStorageFile> RemoveFileFromSyncCommand => 
            _removeFileFromSyncCommand ?? (_removeFileFromSyncCommand = new RelayCommand<BindableStorageFile>(RemoveFileFromSync));        

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

        private ObservableCollection<FileCollection> _fileGroups = new ObservableCollection<FileCollection>();
        public ObservableCollection<FileCollection> FileGroups
        {
            get { return _fileGroups; }
            set
            {
                if (_fileGroups == value)
                {
                    return;
                }
                _fileGroups = value;
                RaisePropertyChanged();
            }
        }

        public SettingsViewModel(IService fileService)
        {
            _fileService = fileService as FileService;
        }

        private void SyncFile(BindableStorageFile file)
        {
            _fileService.RoamFile(file);
        }

        private void RemoveFileFromSync(BindableStorageFile file)
        {
            _fileService.StopRoamingFile(file);
        }

        public async void Activate(object parameter, NavigationMode navigationMode)
        {
            await _fileService.InitializeAsync();
            FileGroups.Add(new FileCollection("Synced", _fileService.RoamedFiles));
            FileGroups.Add(new FileCollection("Local", _fileService.LocalFiles));            
        }

        public void Deactivate(object parameter)
        {
            
        }
    }
}

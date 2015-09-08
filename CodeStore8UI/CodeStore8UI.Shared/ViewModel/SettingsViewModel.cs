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
using GalaSoft.MvvmLight.Views;

namespace CodeStore8UI.ViewModel
{
    public class SettingsViewModel : ViewModelBase, INavigable
    {
        private FileService _fileService;
        private NavigationService _navigationService;

        public bool AllowGoingBack { get; set; } = true;

        private RelayCommand<BindableStorageFile> _syncFileCommand;
        public RelayCommand<BindableStorageFile> SyncFileCommand => 
            _syncFileCommand ?? (_syncFileCommand = new RelayCommand<BindableStorageFile>(SyncFile));

        private RelayCommand<BindableStorageFile> _removeFileFromSyncCommand;
        public RelayCommand<BindableStorageFile> RemoveFileFromSyncCommand => 
            _removeFileFromSyncCommand ?? (_removeFileFromSyncCommand = new RelayCommand<BindableStorageFile>(RemoveFileFromSync));

        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));        

        private double _roamingSpaceUsed = 0;
        public double RoamingSpaceUsed
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

        public SettingsViewModel(IService fileService, INavigationService navService)
        {
            _fileService = fileService as FileService;
            _navigationService = navService as NavigationService;
        }

        private async void SyncFile(BindableStorageFile file)
        {
            await _fileService.RoamFile(file);
            ulong space = 0;
            foreach(var f in  FileGroups.First(x => x.Title == "Synced").Files)
            {
                space += await f.GetFileSizeInBytes();
            }
            RoamingSpaceUsed = (double)space / 1024;
        }

        private async void RemoveFileFromSync(BindableStorageFile file)
        {
            await _fileService.StopRoamingFile(file);
            ulong space = 0;
            foreach (var f in FileGroups.First(x => x.Title == "Synced").Files)
            {
                space += await f.GetFileSizeInBytes();
            }
            RoamingSpaceUsed = (double)space / 1024;
        }

        private void GoBack()
        {
            _navigationService.GoBack();            
        }

        public void Activate(object parameter, NavigationMode navigationMode)
        {            
            if (navigationMode == NavigationMode.New && FileGroups.Count == 0)
            {                
                FileGroups.Add(new FileCollection(Constants.ROAMED_FILES_TITLE, _fileService.RoamedFiles));
                FileGroups.Add(new FileCollection(Constants.LOCAL_FILES_TITLE, _fileService.LocalFiles));
            }
        }

        public void Deactivate(object parameter)
        {
            
        }
    }
}

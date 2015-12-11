using Codeco.Common;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System.Collections.ObjectModel;
using Codeco.Services;
using Codeco.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Threading.Tasks;

namespace Codeco.ViewModel
{
    public class SettingsViewModel : ViewModelBase, INavigable
    {
        private readonly FileService _fileService;
        private readonly NavigationServiceEx _navigationService;

        public bool AllowGoingBack { get; set; } = true;
        private static readonly AsyncLock s_lock = new AsyncLock();

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

        private double _roamingSpaceFree = 100;
        public double RoamingSpaceFree
        {
            get { return _roamingSpaceFree; }
            set
            {
                if (value == _roamingSpaceFree)
                {
                    return;
                }
                _roamingSpaceFree = value;
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

        public SettingsViewModel(IService fileService, INavigationServiceEx navService)
        {
            _fileService = fileService as FileService;
            _navigationService = navService as NavigationServiceEx;            
        }        

        private async void SyncFile(BindableStorageFile file)
        {            
            FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files.Remove(file);
            FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files.Add(file);            
            await UpdateAvailableRoamingSpace();            
        }

        private async void RemoveFileFromSync(BindableStorageFile file)
        {
            FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files.Remove(file);
            FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files.Add(file);            
            await UpdateAvailableRoamingSpace();            
        }

        private async Task UpdateAvailableRoamingSpace()
        {
            using (await s_lock.Acquire())
            {

                ulong space = 0;
                if (FileGroups.All(x => x.Location != FileService.FileLocation.Roamed))
                {
                    return;
                }
                var syncedFiles = FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files;
                for (int i = syncedFiles.Count - 1; i >= 0; i--)
                {
                    if (syncedFiles[i].Name == Constants.IV_FILE_NAME) continue; //want to hide this from the user...
                    space += await syncedFiles[i].GetFileSizeInBytes();                    
                }
                ulong ivFileSize = await FileUtilities.GetIVFileSize();
                space += ivFileSize;
                RoamingSpaceUsed = (double)space / 1024;
                RoamingSpaceFree = (100 - ((double)ivFileSize / 1024));
            }            
        }                

        private async Task BeforeGoingBack()
        {            
            foreach (var local in FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files)
            {
                await _fileService.StopRoamingFile(local);
            }
            foreach (var roamed in FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files)
            {
                await _fileService.RoamFile(roamed);
            }
        }

        private async void GoBack()
        {
            await BeforeGoingBack();            
            _navigationService.GoBack();                        
        }

        //This is seperate from GoBack() because the MVVM Commanding model requires async void, and the
        //event handler style here for the hardware back button requires async Task<T> or void. So we need both!
        private void OnBackPressed(object sender, UniversalBackPressedEventArgs args)
        {
            GoBack();
        }

        public async void Activate(object parameter, NavigationMode navigationMode)
        {
            _navigationService.BackButtonPressed += OnBackPressed;

            //TODO: Rework this to not clear + add, but instead just check somehow for a changed list and add only what's changed
            FileGroups.Clear();
            FileGroups.Add(new FileCollection(Constants.ROAMED_FILES_TITLE,
                new ObservableCollection<IBindableStorageFile>(_fileService.GetRoamedFiles()), FileService.FileLocation.Roamed));
            FileGroups.Add(new FileCollection(Constants.LOCAL_FILES_TITLE,
                new ObservableCollection<IBindableStorageFile>(_fileService.GetLocalFiles()), FileService.FileLocation.Local));

            await UpdateAvailableRoamingSpace();
        }

        public void Deactivating(object parameter)
        {
            
        }

        public void Deactivated(object parameter)
        {
            _navigationService.BackButtonPressed -= OnBackPressed;
        }
    }
}

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
        private FileService _fileService;
        private NavigationServiceEx _navigationService;

        public bool AllowGoingBack { get; set; } = true;
        private static AsyncLock s_lock = new AsyncLock();

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

        //I hate this solution.
        //For some reason, whenever two elements are modified simultaneously on the phone, the app crashes.
        //No exception. No message. No clue whatsoever. It's got to be some kind of race case, but I don't even
        //know where to begin. So you know what? No changing multiple elements simultaneously on the phone.
        //This property controls the List's IsEnabled.
        private bool _isListReady = true;
        public bool IsListReady
        {
            get { return _isListReady;}
            set
            {
                if (_isListReady == value)
                {
                    return;
                }
                _isListReady = value;
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
            IsListReady = false;

            FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files.Remove(file);
            FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files.Add(file);            
            await UpdateRoamingSpaceUsed();

            await Task.Delay(250); //Hacky make-phone-not-crash.
            IsListReady = true;
        }

        private async void RemoveFileFromSync(BindableStorageFile file)
        {
            IsListReady = false;

            FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files.Remove(file);
            FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files.Add(file);               
            await UpdateRoamingSpaceUsed();

            await Task.Delay(250); //Hacky make-phone-not-crash.
            IsListReady = true;
        }

        private async Task UpdateRoamingSpaceUsed()
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
                    space += await syncedFiles[i].GetFileSizeInBytes();                    
                }
                space += await FileUtilities.GetIVFileSize();
                RoamingSpaceUsed = (double)space / 1024;
            }            
        }        

        //This is seperate from GoBack() because the MVVM Commanding model requires async void, and the
        //event handler style here for the hardware back button requires async Task<T> or void. So we need both!
        private void OnBackPressed(object sender, UniversalBackPressedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("SettingsVM BackPressed!");
            GoBack();
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

        public async void Activate(object parameter, NavigationMode navigationMode)
        {
            _navigationService.BackButtonPressed += OnBackPressed;

            //TODO: Rework this to not clear + add, but instead just check somehow for a changed list and add only what's changed
            FileGroups.Clear();
            FileGroups.Add(new FileCollection(Constants.ROAMED_FILES_TITLE,
                new ObservableCollection<IBindableStorageFile>(_fileService.GetRoamedFiles()), FileService.FileLocation.Roamed));
            FileGroups.Add(new FileCollection(Constants.LOCAL_FILES_TITLE,
                new ObservableCollection<IBindableStorageFile>(_fileService.GetLocalFiles()), FileService.FileLocation.Local));

            await UpdateRoamingSpaceUsed();
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

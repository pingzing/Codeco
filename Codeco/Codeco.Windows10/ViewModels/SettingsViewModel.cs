using GalaSoft.MvvmLight;
using System;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using Windows.UI.Core;
using Codeco.Windows10.Common;
using Codeco.Windows10.Services;
using Codeco.Windows10.Models;

namespace Codeco.Windows10.ViewModels
{
    public class SettingsViewModel : UniversalBaseViewModel, INavigable
    {
        private readonly IFileService _fileService;        
        
        private static readonly AsyncLock s_lock = new AsyncLock();

        private RelayCommand<BindableStorageFile> _syncFileCommand;
        public RelayCommand<BindableStorageFile> SyncFileCommand => 
            _syncFileCommand ?? (_syncFileCommand = new RelayCommand<BindableStorageFile>(SyncFile));

        private RelayCommand<BindableStorageFile> _removeFileFromSyncCommand;
        public RelayCommand<BindableStorageFile> RemoveFileFromSyncCommand => 
            _removeFileFromSyncCommand ?? (_removeFileFromSyncCommand = new RelayCommand<BindableStorageFile>(RemoveFileFromSync));        

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

        public SettingsViewModel(IFileService fileService, INavigationServiceEx navService) : base(navService)
        {
            _fileService = fileService;
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
                    if (syncedFiles[i].Name == Constants.IV_FILE_NAME)
                    {
                        continue;
                    }
                    space += await syncedFiles[i].GetFileSizeInBytes();                    
                }
                ulong ivFileSize = await FileUtilities.GetIVFileSize();
                space += ivFileSize;
                RoamingSpaceUsed = (double)space / 1024;
                RoamingSpaceFree = (100 - ((double)ivFileSize / 1024));
            }            
        }                

        public override async void Activate(object parameter, NavigationMode navigationMode)
        {
            base.Activate(parameter, navigationMode);

            //TODO: Rework this to not clear + add, but instead just check somehow for a changed list and add only what's changed
            FileGroups.Clear();
            FileGroups.Add(new FileCollection(Constants.ROAMED_FILES_TITLE,
                new ObservableCollection<IBindableStorageFile>(_fileService.GetRoamedFiles()), FileService.FileLocation.Roamed));
            FileGroups.Add(new FileCollection(Constants.LOCAL_FILES_TITLE,
                new ObservableCollection<IBindableStorageFile>(_fileService.GetLocalFiles()), FileService.FileLocation.Local));

            await UpdateAvailableRoamingSpace();            
        }

        protected override async void UniversalBaseViewModel_BackRequested(object sender, BackRequestedEventArgs e)
        {
            foreach (var local in FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files)
            {
                await _fileService.StopRoamingFile(local);
            }
            foreach (var roamed in FileGroups.First(x => x.Location == FileService.FileLocation.Roamed).Files)
            {
                await _fileService.RoamFile(roamed);
            }

            base.UniversalBaseViewModel_BackRequested(sender, e);
        }   
    }
}

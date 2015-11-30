using System;
using System.Collections.Generic;
using System.Text;
using CodeStore8UI.Services;
using GalaSoft.MvvmLight.Views;
using CodeStore8UI.Model;
using System.Collections.ObjectModel;
using CodeStore8UI.Model.Mocks;
using CodeStore8UI.Common;

namespace CodeStore8UI.ViewModel.DesignViewModels
{
    public class SettingsViewModelDesign : SettingsViewModel
    {
        public SettingsViewModelDesign(IService fileService, INavigationServiceEx navService) : base(fileService, navService)
        {
            ObservableCollection<IBindableStorageFile> localFiles = new ObservableCollection<IBindableStorageFile>
            {
                new MockBindableStorageFile(),                
                new MockBindableStorageFile()
            };
            ObservableCollection<IBindableStorageFile> roamedFiles = new ObservableCollection<IBindableStorageFile>
            {
                new MockBindableStorageFile(),                
                new MockBindableStorageFile()
            };
            FileGroups = new ObservableCollection<FileCollection>
            {
                new FileCollection(Constants.LOCAL_FILES_TITLE, localFiles, FileService.FileLocation.Local),
                new FileCollection(Constants.ROAMED_FILES_TITLE, roamedFiles, FileService.FileLocation.Roamed)
            };
            this.RoamingSpaceUsed = 50.23;
        }
    }
}

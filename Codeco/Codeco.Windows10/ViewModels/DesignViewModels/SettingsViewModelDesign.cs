using System.Collections.ObjectModel;
using Codeco.Windows10.Services;
using Codeco.Windows10.Models;
using Codeco.Windows10.Models.Mocks;
using Codeco.Windows10.Common;

namespace Codeco.Windows10.ViewModels.DesignViewModels
{
    public class SettingsViewModelDesign : SettingsViewModel
    {
        public SettingsViewModelDesign(IFileService fileService, INavigationServiceEx navService, IInitializationValueService ivService) : base(fileService, navService, ivService)
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

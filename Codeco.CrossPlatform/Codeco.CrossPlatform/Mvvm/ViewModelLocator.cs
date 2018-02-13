using Codeco.CrossPlatform.Services;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.CrossPlatform.ViewModels;
using Codeco.CrossPlatform.Views;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Contracts;
using Plugin.FilePicker.Abstractions;

namespace Codeco.CrossPlatform.Mvvm
{

    /*
      In the View:
      BindingContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"  
    */
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            //Register (and initialize, if necessary) your services here
            SimpleIoc.Default.Register<IAppFolderService>(() => DependencyService.Get<IAppFolderService>());
            SimpleIoc.Default.Register<INativeFileServiceFacade>(() => DependencyService.Get<INativeFileServiceFacade>());
            SimpleIoc.Default.Register<IPopupNavigation>(() => PopupNavigation.Instance);
            SimpleIoc.Default.Register<INavigationService>(InitializeNavigationService);
            SimpleIoc.Default.Register<ILocalizeService>(() => DependencyService.Get<ILocalizeService>());
            SimpleIoc.Default.Register<IUserDialogs>(() => UserDialogs.Instance);
            SimpleIoc.Default.Register<IFileService, FileService>();
            SimpleIoc.Default.Register<IFileSystemWatcherService>(() => DependencyService.Get<IFileSystemWatcherService>());
            SimpleIoc.Default.Register<IUserFileService, UserFileService>();
            SimpleIoc.Default.Register<IFilePicker>(() => Plugin.FilePicker.CrossFilePicker.Current);

            //Register your ViewModels here
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainPage => SimpleIoc.Default.GetInstance<MainViewModel>();

        private INavigationService InitializeNavigationService()
        {
            NavigationService navService = new NavigationService(((App)Application.Current).MainNavigationHost)
                .Configure(typeof(MainViewModel), typeof(MainPage));

            return navService;
        }
    }
}

using Codeco.CrossPlatform.Services;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.CrossPlatform.ViewModels;
using Codeco.CrossPlatform.Views;
using Acr.UserDialogs;

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
            IAppFolderService appFolderService = DependencyService.Get<IAppFolderService>();
            SimpleIoc.Default.Register(() => appFolderService);

            INativeFileServiceFacade crossplatFileService = DependencyService.Get<INativeFileServiceFacade>();
            SimpleIoc.Default.Register(() => crossplatFileService);

            SimpleIoc.Default.Register(InitializeNavigationService);
            SimpleIoc.Default.Register(() => DependencyService.Get<ILocalizeService>());
            SimpleIoc.Default.Register(() => UserDialogs.Instance);

            IFileService fileService = new FileService(crossplatFileService);
            SimpleIoc.Default.Register(() => fileService);

            SimpleIoc.Default.Register<IUserFileService>(() => new UserFileService(appFolderService, fileService));

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

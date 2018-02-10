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
            SimpleIoc.Default.Register<IAppFolderService>(() => appFolderService);

            SimpleIoc.Default.Register<INavigationService>(InitializeNavigationService);
            SimpleIoc.Default.Register<ILocalizeService>(() => DependencyService.Get<ILocalizeService>());
            SimpleIoc.Default.Register<IUserDialogs>(() => UserDialogs.Instance);

            IFileService fileService = new FileService(appFolderService);
            SimpleIoc.Default.Register<IFileService>(() => fileService);

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

using Codeco.CrossPlatform.Services;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.CrossPlatform.ViewModels;
using Codeco.CrossPlatform.Views;

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
            SimpleIoc.Default.Register<INavigationService>(InitializeNavigationService);
            SimpleIoc.Default.Register<ILocalizeService>(() => DependencyService.Get<ILocalizeService>());

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

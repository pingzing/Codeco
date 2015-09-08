/*
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"  
*/

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using CodeStore8UI.Services;
using System.Threading.Tasks;
using CodeStore8UI.ViewModel.DesignViewModels;
using CodeStore8UI.Services.Mocks;
#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif

namespace CodeStore8UI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                //SimpleIoc.Default.Register<IDataService, DesignDataService>();
                //SimpleIoc.Default.Register<IService, FileService>();                                 
                SimpleIoc.Default.Register<IService, MockFileService>();
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
                SimpleIoc.Default.Register<SettingsViewModelDesign>();
                SimpleIoc.Default.Register<MainViewModelDesign>();
            }
            else
            {
                // Create run time view services and models
                //SimpleIoc.Default.Register<IDataService, DataService>();                
                NavigationService navService = InitializeNavigationService();
                SimpleIoc.Default.Register<INavigationService>(() => navService);

                IService fileService = InitializeFileService().Result;
                SimpleIoc.Default.Register(() => fileService);

                SimpleIoc.Default.Register<MainViewModel>();
                SimpleIoc.Default.Register<SettingsViewModel>();
            }            
        }

        private async Task<IService> InitializeFileService()
        {
            FileService service = new FileService();
            return await service.InitializeAsync();
        }

        private NavigationService InitializeNavigationService()
        {
            NavigationService navService = new NavigationService();
            navService.Configure(nameof(MainPage), typeof(MainPage));
            navService.Configure(nameof(SettingsPage), typeof(SettingsPage));
#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed += (s, e) =>
            {                
                //TODO: Extend the navService so it has access to the back stack so we can be smarter here
                navService.GoBack();
                e.Handled = true;
            };
#endif
            return navService;
        }

        public MainViewModel Main => ViewModelBase.IsInDesignModeStatic
            ? ServiceLocator.Current.GetInstance<MainViewModelDesign>()
            : ServiceLocator.Current.GetInstance<MainViewModel>();

        public SettingsViewModel Settings => ViewModelBase.IsInDesignModeStatic 
            ? ServiceLocator.Current.GetInstance<SettingsViewModelDesign>() 
            : ServiceLocator.Current.GetInstance<SettingsViewModel>();
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
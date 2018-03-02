/*
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"  
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Codeco.Windows10.Services.Mocks;
using Codeco.Windows10.ViewModels.DesignViewModels;
using Codeco.Windows10.Services;
using Codeco.Windows10.Views;

namespace Codeco.Windows10.ViewModels
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
                SimpleIoc.Default.Register<IFileService, MockFileService>();
                SimpleIoc.Default.Register<INavigationServiceEx, NavigationServiceEx>();
                SimpleIoc.Default.Register<IInitializationValueService, InitializationValueService>();
                SimpleIoc.Default.Register<SettingsViewModelDesign>();
                SimpleIoc.Default.Register<MainViewModelDesign>();
            }
            else
            {
                // Create run time view services and models                          
                NavigationServiceEx navService = InitializeNavigationService();
                SimpleIoc.Default.Register<INavigationServiceEx>(() => navService);

                SimpleIoc.Default.Register<IFileService, FileService>();
                SimpleIoc.Default.Register<IInitializationValueService, InitializationValueService>();
                SimpleIoc.Default.Register<MainViewModel>();
                SimpleIoc.Default.Register<SettingsViewModel>();
            }            
        }

        private NavigationServiceEx InitializeNavigationService()
        {
            NavigationServiceEx navService = new NavigationServiceEx();
            navService.Configure(nameof(MainPage), typeof(MainPage));
            navService.Configure(nameof(SettingsPage), typeof(SettingsPage));                        
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
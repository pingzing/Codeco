using Codeco.CrossPlatform.Services;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.CrossPlatform.ViewModels;
using Codeco.CrossPlatform.Views;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Contracts;
using Codeco.CrossPlatform.Popups;
using Codeco.Encryption;
using System;
using Plugin.Clipboard;
using Plugin.Clipboard.Abstractions;

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
            //SimpleIoc.Default.Register<IAppFolderService>(() => DependencyService.Get<IAppFolderService>());
            //SimpleIoc.Default.Register<INativeFileServiceFacade>(() => DependencyService.Get<INativeFileServiceFacade>());
            SimpleIoc.Default.Register<IPopupNavigation>(() => PopupNavigation.Instance);
            SimpleIoc.Default.Register<INavigationService>(InitializeNavigationService);
            SimpleIoc.Default.Register<ILocalizeService>(() => DependencyService.Get<ILocalizeService>());
            SimpleIoc.Default.Register<IUserDialogs>(() => UserDialogs.Instance);
            SimpleIoc.Default.Register<IFileService, FileService>();
            SimpleIoc.Default.Register<IUserFileService, UserFileService>();
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            SimpleIoc.Default.Register<IClipboard>(() => CrossClipboard.Current);

            //Register your ViewModels here
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AddFileViewModel>();
        }

        // Page ViewModel properties, for XAML-y access
        public MainViewModel MainPage => SimpleIoc.Default.GetInstance<MainViewModel>();

        // Popup ViewModel properties, for more XAML-y access
        public AddFileViewModel AddFilePopup => SimpleIoc.Default.GetInstance<AddFileViewModel>(Guid.NewGuid().ToString());

        private INavigationService InitializeNavigationService()
        {
            var popupHost = new PopupHost(SimpleIoc.Default.GetInstance<IPopupNavigation>());

            NavigationService navService = new NavigationService(((App)Application.Current).MainNavigationHost,
                popupHost)
                .Configure(typeof(MainViewModel), typeof(MainPage))
                .Configure(typeof(AddFileViewModel), typeof(AddFileView));

            return navService;
        }
    }
}

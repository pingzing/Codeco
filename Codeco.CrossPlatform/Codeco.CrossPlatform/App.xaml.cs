using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.CrossPlatform.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

#if !DEBUG
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
#endif
namespace Codeco.CrossPlatform
{
    public partial class App : Application
    {
        // Guard against resuming reinitializing on Android.
        private static bool _initialized = false;

        public ViewModelLocator Locator { get; }
        public NavigationHost MainNavigationHost { get; set; }

        public App ()
        {            
            if (_initialized)
            {
                // TODO: Maybe call OnResume in Android doesn't do it for us.
                return;
            }

            _initialized = true;

            InitializeComponent();
            MainNavigationHost = new NavigationHost();
            Locator = (ViewModelLocator)Current.Resources["Locator"];

            MainPage = MainNavigationHost;            

            MainNavigationHost.NavigateToAsync(new MainPage(), false);

            SetupPlatformThemeColorMonitoring();
        }

        // Sets up the platform-specific DynamicResource colors
        private void SetupPlatformThemeColorMonitoring()
        {
            var colorService = DependencyService.Get<IPlatformColorService>();
            colorService.PlatformAccentColor.Subscribe(newColor => 
            {
                Device.BeginInvokeOnMainThread(() => Resources["AccentColor"] = newColor);
            });

            colorService.PlatformForegroundColor.Subscribe(newColor =>
            {
                Device.BeginInvokeOnMainThread(() => Resources["ForegroundColor"] = newColor);
            });

            colorService.PlatformBackgroundColor.Subscribe(newColor =>
            {
                Device.BeginInvokeOnMainThread(() => Resources["BackgroundColor"] = newColor);
            });
        }

        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}

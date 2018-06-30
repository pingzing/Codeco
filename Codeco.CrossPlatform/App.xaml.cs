using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.CrossPlatform.Views;
using AC = Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms.Internals;

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

        public App()
        {            
            if (_initialized)
            {
                // TODO: Maybe call OnResume if Android doesn't do it for us.
                return;
            }

            _initialized = true;
#if DEBUG
            Log.Listeners.Add(new DelegateLogListener((_, arg2) => Debug.WriteLine(arg2)));
            LiveReload.Init();
#endif


            InitializeComponent();
            MainNavigationHost = new NavigationHost();
            Locator = (ViewModelLocator)Current.Resources["Locator"];

            MainPage = MainNavigationHost;
        }

        // Sets up the platform-specific DynamicResource colors
        bool _colorsInitialized = false;
        private void SetupPlatformThemeColorMonitoring()
        {
            if (_colorsInitialized)
            {
                return;
            }
            _colorsInitialized = true;

            var colorService = SimpleIoc.Default.GetInstance<IPlatformColorService>();
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
            AC.AppCenter.Start($"uwp={AppCenterConfig.UwpKey};android={AppCenterConfig.AndroidKey}", typeof(Analytics), typeof(Crashes));
            SetupPlatformThemeColorMonitoring();            

            MainNavigationHost.NavigateToAsync(new MainPage(), false);
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

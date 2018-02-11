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
        public ViewModelLocator Locator { get; }
        public NavigationHost MainNavigationHost { get; set; }

        public App ()
        {
            InitializeComponent();
            MainNavigationHost = new NavigationHost();
            Locator = (ViewModelLocator)Current.Resources["Locator"];

            MainPage = MainNavigationHost;            

            MainNavigationHost.NavigateToAsync(new MainPage(), false);

            SetupAccentColorMonitoring();
        }

        private void SetupAccentColorMonitoring()
        {
            var accentColorervice = DependencyService.Get<IAccentColorService>();
            accentColorervice.PlatformAccentColor.Subscribe(
                newColor => 
                {
                    Device.BeginInvokeOnMainThread(
                        () => Resources["AccentColor"] = newColor
                    );
                }
            );            
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

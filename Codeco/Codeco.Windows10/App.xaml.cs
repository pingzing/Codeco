using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Codeco.Windows10.Common;
using Codeco.Windows10.Views;
using Codeco.Windows10.ViewModels;
using GalaSoft.MvvmLight.Threading;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Foundation.Metadata;
using Microsoft.Practices.ServiceLocation;
using Codeco.Windows10.Extensions;
using Codeco.Windows10.Models;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace Codeco.Windows10
{    
    public sealed partial class App : Application
    {
        private bool _isUIOpen = false;

        public App()
        {            
            this.InitializeComponent();            
            this.Suspending += this.OnSuspending;            
            this.UnhandledException += App_UnhandledException;                         
        }
         
        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debugger.Break();            
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {            
            // Don't init the window if we're running from the command line.
            if (ApiInformation.IsEnumNamedValuePresent(typeof(ActivationKind).FullName, "CommandLineLaunch"))
            {
                if (e.Kind == ActivationKind.CommandLineLaunch)
                {
                    return;
                }
            }

            Frame rootFrame = Window.Current.Content as Frame;
            SetupRuntimeResources();            
            if (rootFrame == null)
            {              
                rootFrame = new Frame();
                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {                    
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        Debug.WriteLine("Failed to restore suspension state.");
                    }
                }                 

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                _isUIOpen = true;
            }

            if (rootFrame.Content == null)
            {                
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 450));            
            
            Window.Current.Activate();
            DispatcherHelper.Initialize();            
        }

        private void SetupRuntimeResources()
        {
            Color accCol = (Color)Current.Resources["SystemAccentColor"];
            accCol.B = (byte)(accCol.B * 0.6);
            accCol.R = (byte)(accCol.R * 0.6);
            accCol.G = (byte)(accCol.G * 0.6);
            SolidColorBrush darkAccentColor = new SolidColorBrush(accCol);
            Current.Resources["SystemAccentColorLow"] = darkAccentColor;
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {            
            base.OnActivated(args);

            if (ApiInformation.IsEnumNamedValuePresent(typeof(ActivationKind).FullName, "CommandLineLaunch"))
            {
                if (args.Kind == ActivationKind.CommandLineLaunch)
                {
                    var cmdArgs = args as CommandLineActivatedEventArgs;
                    var cmdOperation = cmdArgs.Operation;
                    cmdOperation.ExitCode = 1; // set to failure by default, only 0 on success

                    using (Deferral deferral = cmdArgs.Operation.GetDeferral())
                    {
                        var (title, password, inputCode, _) = cmdOperation.Arguments
                            .Substring(0, cmdOperation.Arguments.Length - 1) // .Arguments seems to add a trailing space for no good reason
                            .Split(new[] { ',' });

                        if (String.IsNullOrWhiteSpace(title) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(inputCode))
                        {
                            return;
                        }
                        
                        if (await FindAndCopy(title, password, inputCode))
                        {
                            cmdOperation.ExitCode = 0;
                        }
                    }

                    Current.Exit();
                }
            }
        }

        private async Task<bool> FindAndCopy(string title, string password, string inputCode)
        {
            var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
            await mainVm.ActivateAsync(null, NavigationMode.New);

            foreach (var group in mainVm.FileGroups)
            {
                foreach (var file in group.Files)
                {
                    if (file.Name == title)
                    {
                        mainVm.ActiveFile = (BindableStorageFile)file;
                        var codes = await mainVm.GetCodes(password);
                        if (codes.TryGetValue(inputCode, out string code))
                        {
                            DataPackage package = new DataPackage();
                            package.RequestedOperation = DataPackageOperation.Copy;
                            package.SetText(code);
                            Clipboard.SetContent(package);
                            Clipboard.Flush();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    
    }
}
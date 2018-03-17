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
using Codeco.Windows10.Services.DependencyServices;
using System.Reflection;
using System.Collections.Generic;
using Rg.Plugins.Popup.Windows;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace Codeco.Windows10
{    
    public sealed partial class App : Application
    {                
        public App()
        {            
            this.InitializeComponent();            
            this.Suspending += this.OnSuspending;            
            this.UnhandledException += App_UnhandledException;                         
        }
         
        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Debugger.Break();            
        }
        
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {            
            Frame rootFrame = Window.Current.Content as Frame;

            SetupRuntimeResources();
            
            if (rootFrame == null)
            {                
                rootFrame = new Frame();

                //Associate the frame with a SuspensionManager key
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                Rg.Plugins.Popup.Popup.Init();
                List<Assembly> assembliesToLoad = new List<Assembly>();
                assembliesToLoad.AddRange(Rg.Plugins.Popup.Popup.GetExtraAssemblies());

                Xamarin.Forms.Forms.Init(e, assembliesToLoad);

                //Dependency services
                Xamarin.Forms.DependencyService.Register<AppFolderService>();
                Xamarin.Forms.DependencyService.Register<NativeFileServiceFacade>();
                Xamarin.Forms.DependencyService.Register<PlatformColorService>();
                Xamarin.Forms.DependencyService.Register<FileSystemWatcherService>();
                Xamarin.Forms.DependencyService.Register<ConnectedDeviceService>();
                
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue
                    }
                }                 

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;                
            }

            if (rootFrame.Content == null)
            {                                
                if (!rootFrame.Navigate(typeof(MainXamarinPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 450));            

            // Ensure the current window is active
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

        protected override void OnActivated(IActivatedEventArgs args)
        {            
            base.OnActivated(args);          
        }

        private AppServiceConnection _appServiceConnection;
        private BackgroundTaskDeferral _appServiceDeferral;
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            IBackgroundTaskInstance taskInstance = args.TaskInstance;
            AppServiceTriggerDetails appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _appServiceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnAppServiceCanceled;
            _appServiceConnection = appService.AppServiceConnection;
            _appServiceConnection.RequestReceived += OnAppServiceRequestReceived;
            _appServiceConnection.ServiceClosed += OnAppServiceClosed;
        }

        private void OnAppServiceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _appServiceDeferral?.Complete();
        }

        private void OnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral messageDeferral = args.GetDeferral();
            // do syncing stuff
            messageDeferral.Complete();
        }

        private void OnAppServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            _appServiceDeferral?.Complete();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    
    }
}
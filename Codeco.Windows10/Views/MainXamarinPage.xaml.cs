using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.Windows10.Services.DependencyServices;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms.Platform.UWP;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Codeco.Windows10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainXamarinPage : WindowsPage
    {
        public MainXamarinPage()
        {
            this.InitializeComponent();
            var app = new Codeco.CrossPlatform.App();
            RegisterXamarinDependencies();
            LoadApplication(app);
        }

        public void RegisterXamarinDependencies()
        {
            //Dependency services            
            SimpleIoc.Default.Register<IAppFolderService, AppFolderService>();
            SimpleIoc.Default.Register<INativeFileServiceFacade, NativeFileServiceFacade>();
            SimpleIoc.Default.Register<IPlatformColorService, PlatformColorService>();
            SimpleIoc.Default.Register<IFileSystemWatcherService, FileSystemWatcherService>();
        }
    }
}

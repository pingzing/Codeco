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
            LoadApplication(app);
        }
    }
}

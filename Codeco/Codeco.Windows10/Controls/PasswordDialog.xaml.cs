using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Codeco.Windows10.Controls
{
    public sealed partial class PasswordDialog : ContentDialog
    {
        public string Result { get; set; }

        public PasswordDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = PasswordBox.Password;
            this.Hide();
        }        

        private void PasswordBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                ContentDialog_PrimaryButtonClick(this, null);
                e.Handled = true;
            }
        }            
    }
}

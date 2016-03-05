using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Codeco.Windows10.Controls
{
    public sealed partial class RenameFileDialog : ContentDialog
    {
        public string Result { get; set; }

        public RenameFileDialog() : this(null)
        {
            
        }

        public RenameFileDialog(string currentName)
        {
            this.InitializeComponent();
            FileNameBox.Text = currentName;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = FileNameBox.Text;
            this.Hide();
        }

        private void FileNameBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ContentDialog_PrimaryButtonClick(this, null);
                e.Handled = true;
            }
        }
    }
}

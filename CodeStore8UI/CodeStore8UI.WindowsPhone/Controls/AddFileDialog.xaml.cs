using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CodeStore8UI.Controls
{
    public sealed partial class AddFileDialog : ContentDialog, INotifyPropertyChanged
    {
        private bool _submitEnabled = false;
        public bool SubmitEnabled
        {
            get { return _submitEnabled; }
            set
            {
                if(_submitEnabled == value)
                {
                    return;
                }
                _submitEnabled = value;
                RaisePropertyChanged();
            }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(AddFileDialog), new PropertyMetadata(""));
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(AddFileDialog), new PropertyMetadata(""));
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }
        
        public AddFileDialog()
        {
            this.InitializeComponent();
            ((FrameworkElement)this.Content).DataContext = this;
        }

        private void ContentDialog_PrimaryButtonClick(object sender, ContentDialogButtonClickEventArgs args) { }
        private void ContentDialog_SecondaryButtonClick(object sender, ContentDialogButtonClickEventArgs args) { }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public Task WhenDismissed()
        {
            var tcs = new TaskCompletionSource<bool>();
            TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> handler = null;
            handler = (s, args) =>
            {
                tcs.TrySetResult(true);
                this.Closed -= handler;
            };
            this.Closed += handler;
            return tcs.Task;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatedSubmitEnabled();
        }

        private void FileNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatedSubmitEnabled();
        }

        private void UpdatedSubmitEnabled()
        {
            if(!String.IsNullOrEmpty(FileNameBox.Text) && !String.IsNullOrEmpty(PasswordBox.Password))
            {
                SubmitEnabled = true;
            }
            else
            {
                SubmitEnabled = false;
            }
        }
    }
}

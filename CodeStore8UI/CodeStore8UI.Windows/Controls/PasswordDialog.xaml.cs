using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CodeStore8UI.Controls
{
    public sealed partial class PasswordDialog : CustomDialog, INotifyPropertyChanged
    {
        public enum Result { Ok, Cancel };
        public event EventHandler<PasswordDialogClosedEventArgs> PasswordDialogClosed;        

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordDialog), new PropertyMetadata(""));
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

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
        
        public PasswordDialog()
        {
            this.InitializeComponent();            
            ((FrameworkElement)this.Content).DataContext = this;                                                         
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {                        
            this.PasswordDialogClosed(this, new PasswordDialogClosedEventArgs(Result.Cancel));
            this.IsOpen = false;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            this.PasswordDialogClosed(this, new PasswordDialogClosedEventArgs(Result.Ok));
            this.IsOpen = false;
        }

        public Task<PasswordDialogClosedEventArgs> WhenClosed()
        {
            var tcs = new TaskCompletionSource<PasswordDialogClosedEventArgs>();
            EventHandler<PasswordDialogClosedEventArgs> handler = null;
            handler = (s, args) =>
            {
                tcs.TrySetResult(new PasswordDialogClosedEventArgs(args.DialogResult));
                this.PasswordDialogClosed -= handler;
            };
            this.PasswordDialogClosed += handler;
            return tcs.Task;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property="")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void FileNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSubmitEnabled();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateSubmitEnabled();
        }

        private void UpdateSubmitEnabled()
        {
            if(!String.IsNullOrEmpty(PasswordBox.Password))
            {
                SubmitEnabled = true;
            }
            else
            {
                SubmitEnabled = false;
            }
        }        

        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (SubmitEnabled)
                {
                    SubmitButton_Click(sender, null);
                }
            }
        }

        private async void PasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => PasswordBox.Focus(FocusState.Programmatic));
        }
    }

    public class PasswordDialogClosedEventArgs
    {        
        public PasswordDialog.Result DialogResult { get; set; }

        public PasswordDialogClosedEventArgs(PasswordDialog.Result dialogResult)
        {
            DialogResult = dialogResult;
        }
    }
}

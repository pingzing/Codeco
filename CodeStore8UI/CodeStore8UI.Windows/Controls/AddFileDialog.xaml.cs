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
    public sealed partial class AddFileDialog : Callisto.Controls.CustomDialog, INotifyPropertyChanged
    {
        public enum Result { Ok, Cancel };
        public event EventHandler<AddFileDialogClosedEventArgs> PasswordDialogClosed;        

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(AddFileDialog), new PropertyMetadata(""));
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(AddFileDialog), new PropertyMetadata(""));
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
        
        public AddFileDialog()
        {
            this.InitializeComponent();
            ((FrameworkElement)this.Content).DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {                        
            this.PasswordDialogClosed(this, new AddFileDialogClosedEventArgs(Result.Cancel));
            this.IsOpen = false;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            this.PasswordDialogClosed(this, new AddFileDialogClosedEventArgs(Result.Ok));
            this.IsOpen = false;
        }

        public Task<AddFileDialogClosedEventArgs> WhenClosed()
        {
            var tcs = new TaskCompletionSource<AddFileDialogClosedEventArgs>();
            EventHandler<AddFileDialogClosedEventArgs> handler = null;
            handler = (s, args) =>
            {
                tcs.TrySetResult(new AddFileDialogClosedEventArgs(args.DialogResult));
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

    public class AddFileDialogClosedEventArgs
    {        
        public AddFileDialog.Result DialogResult { get; set; }

        public AddFileDialogClosedEventArgs(AddFileDialog.Result dialogResult)
        {
            DialogResult = dialogResult;
        }
    }
}

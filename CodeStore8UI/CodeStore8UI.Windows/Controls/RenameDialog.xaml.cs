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

namespace Codeco.Controls
{
    public sealed partial class RenameDialog : CustomDialog, INotifyPropertyChanged
    {
        public enum Result { Ok, Cancel };
        public event EventHandler<RenameDialogClosedEventArgs> RenameDialogClosed;

        public static readonly DependencyProperty NewNameProperty = DependencyProperty.Register("NewName", typeof(string), typeof(RenameDialog), new PropertyMetadata(""));
        public string NewName
        {
            get { return (string)GetValue(NewNameProperty); }
            set { SetValue(NewNameProperty, value); }
        }

        private bool _submitEnabled = false;
        public bool SubmitEnabled
        {
            get { return _submitEnabled; }
            set
            {
                if (_submitEnabled == value)
                {
                    return;
                }
                _submitEnabled = value;
                RaisePropertyChanged();
            }
        }

        public RenameDialog()
        {
            this.InitializeComponent();
            ((FrameworkElement)this.Content).DataContext = this;
        }        

        private void NewNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!String.IsNullOrEmpty(NewNameBox.Text))
            {
                SubmitEnabled = true;
            }
            else
            {
                SubmitEnabled = false;
            }
        }

        private void NewNameBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter && SubmitEnabled)
            {
                SubmitButton_Click(sender, e);
            }
        }

        private async void NewNameBox_Loaded(object sender, RoutedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => NewNameBox.Focus(FocusState.Programmatic));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            this.RenameDialogClosed(this, new RenameDialogClosedEventArgs(Result.Ok));
            this.IsOpen = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.RenameDialogClosed(this, new RenameDialogClosedEventArgs(Result.Cancel));
            this.IsOpen = false;
        }

        public Task<RenameDialogClosedEventArgs> WhenClosed()
        {
            var tcs = new TaskCompletionSource<RenameDialogClosedEventArgs>();
            EventHandler<RenameDialogClosedEventArgs> handler = null;
            handler = (s, args) =>
            {
                tcs.TrySetResult(new RenameDialogClosedEventArgs(args.DialogResult));
                this.RenameDialogClosed -= handler;
            };
            this.RenameDialogClosed += handler;
            return tcs.Task;
        }
    }

    public class RenameDialogClosedEventArgs
    {
        public RenameDialog.Result DialogResult { get; set; }

        public RenameDialogClosedEventArgs(RenameDialog.Result dialogResult)
        {
            DialogResult = dialogResult;
        }
    }
}

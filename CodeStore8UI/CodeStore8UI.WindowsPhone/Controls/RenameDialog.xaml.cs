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
    public sealed partial class RenameDialog : ContentDialog, INotifyPropertyChanged
    {

        public static readonly DependencyProperty NewNameProperty = DependencyProperty.Register("NewName", typeof(string), typeof(RenameDialog), new PropertyMetadata(""));

        public string NewName
        {
            get { return (string)GetValue(NewNameProperty); }
            set { SetValue(NewNameProperty, value); }
        }

        public RenameDialog()
        {
            this.InitializeComponent();
            ((FrameworkElement)this.Content).DataContext = this;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }

        private void NewNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(NewNameBox.Text))
            {
                this.IsPrimaryButtonEnabled = true;
            }
            else
            {
                this.IsPrimaryButtonEnabled = false;
            }
        }

        private void NewNameBox_Loaded(object sender, RoutedEventArgs e)
        {
            NewNameBox.Focus(FocusState.Programmatic);
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}

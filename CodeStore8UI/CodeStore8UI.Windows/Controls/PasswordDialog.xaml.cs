using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class PasswordDialog : Callisto.Controls.CustomDialog
    {
        public enum Result { Ok, Cancel };
        public event EventHandler<PasswordDialogClosedEventArgs> PasswordDialogClosed;

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordDialog), new PropertyMetadata(""));


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

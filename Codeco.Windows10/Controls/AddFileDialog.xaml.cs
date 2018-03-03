using Codeco.Windows10.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Codeco.Windows10.Controls
{
    public sealed partial class AddFileDialog : ContentDialog, INotifyPropertyChanged
    {
        private bool _areBothBoxesFilled = false;
        public bool AreBothBoxesFilled
        {
            get { return _areBothBoxesFilled; }
            set
            {
                if(value != _areBothBoxesFilled)
                {
                    _areBothBoxesFilled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public AddFileDialogOutput Result { get; set; }

        public AddFileDialog()
        {
            this.InitializeComponent();
        }        

        public AddFileDialog(string fileName)
        {
            this.InitializeComponent();
            FilenameBox.Text = fileName;
            this.PasswordBox.Loaded += (s, e) =>
            {
                PasswordBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            };        
        }

        private void FilenameBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                if(AreBothBoxesFilled)
                {
                    ContentDialog_PrimaryButtonClick(this, null);
                    
                }
                else
                {
                    FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                }
                e.Handled = true;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {            
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (AreBothBoxesFilled)
                {
                    ContentDialog_PrimaryButtonClick(this, null);
                }
                e.Handled = true;
            }
        }

        private void FilenameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateBothBoxesFilled();
        }

        private void PasswordBox_PasswordChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UpdateBothBoxesFilled();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = new AddFileDialogOutput { FileName = FilenameBox.Text, Password = PasswordBox.Password };
            this.Hide();
        }

        private void UpdateBothBoxesFilled()
        {
            if(!String.IsNullOrWhiteSpace(PasswordBox.Password) && !String.IsNullOrWhiteSpace(FilenameBox.Text))
            {
                AreBothBoxesFilled = true;
            }
            else
            {
                AreBothBoxesFilled = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }       
    }
}

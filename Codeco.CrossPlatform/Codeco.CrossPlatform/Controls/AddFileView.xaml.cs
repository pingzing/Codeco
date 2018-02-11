using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Text.RegularExpressions;

using Xamarin.Forms;

namespace Codeco.CrossPlatform.Controls
{
    public partial class AddFileView : PopupPage
    {
        private static Regex _illegalCharsRegex = new Regex($"[{Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))}]", RegexOptions.Compiled);      

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public string FileNameText
        {
            get => (string)GetValue(FileNameTextProperty);
            set
            {
                SetValue(FileNameTextProperty, value);
                OnPropertyChanged(nameof(IsOkEnabled));
            }
        }

        public string PasswordText
        {
            get => (string)GetValue(PasswordTextProperty);
            set
            {
                SetValue(PasswordTextProperty, value);
                OnPropertyChanged(nameof(IsOkEnabled));
            }
        }

        public bool IsOkEnabled => !String.IsNullOrWhiteSpace(FileNameText) && !String.IsNullOrWhiteSpace(PasswordText);

        private RelayCommand _okCommand;
        public RelayCommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(OkPressed, 
            canExecute: () => IsOkEnabled));

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(CancelPressed));

        public AddFileView()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void OkPressed()
        {
            //TODO: Set result
            OnBackButtonPressed();
        }

        private async void CancelPressed()
        {
            await PopupNavigation.RemovePageAsync(this);
        }        

        public static readonly BindableProperty MessageProperty = BindableProperty.Create(
            nameof(Message),
            typeof(string),
            typeof(AddFileView),
            defaultValue: null);

        public static readonly BindableProperty FileNameTextProperty = BindableProperty.Create(
            nameof(FileNameText),
            typeof(string),
            typeof(AddFileView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,            
            propertyChanging: FileNameChanged);

        private static void FileNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var _this = bindable as AddFileView;
            string input = (string)newValue;
            _this.FileNameText = SanitizeFileName(input);
        }

        private static string SanitizeFileName(string inputString)
        {
            if (_illegalCharsRegex.IsMatch(inputString))
            {
                inputString = _illegalCharsRegex.Replace(inputString, "");
            }

            return inputString;
        }

        private static readonly BindableProperty PasswordTextProperty = BindableProperty.Create(
            nameof(PasswordText),
            typeof(string),
            typeof(AddFileView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);
    }
}
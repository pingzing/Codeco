using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Popups;
using Codeco.CrossPlatform.Services;
using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Codeco.CrossPlatform.ViewModels
{
    public class AddFileViewModel : NavigablePopupViewModelBase<(string fileName, string password)>
    {
        private readonly static Regex _illegalCharsRegex = new Regex($"[{Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))}]", RegexOptions.Compiled);
        private readonly IPopupNavigation _popupNavigation;

        private string _fileNameText;
        public string FileNameText
        {
            get => _fileNameText;
            set
            {
                Set(ref _fileNameText, value);
                RaisePropertyChanged(nameof(IsOkEnabled));
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        private string _passwordText;
        public string PasswordText
        {
            get => _passwordText;
            set
            {
                Set(ref _passwordText, value);
                RaisePropertyChanged(nameof(IsOkEnabled));
            }
        }

        public bool IsOkEnabled => !String.IsNullOrWhiteSpace(FileNameText)
            && !String.IsNullOrWhiteSpace(PasswordText);

        private RelayCommand _okCommand;
        public RelayCommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(OkPressed,
            canExecute: () => IsOkEnabled));

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(CancelPressed));

        public AddFileViewModel(INavigationService navService,
                                IPopupNavigation popupNavigation)
            : base(navService)
        {
            _popupNavigation = popupNavigation;
        }

        private async void OkPressed()
        {
            Result = new PopupResult<(string fileName, string password)>
            {
                PopupChoice = PopupChoice.Ok,
                Result = (fileName: FileNameText, password: PasswordText)
            };
            await _popupNavigation.PopAsync();
        }

        private async void CancelPressed()
        {
            Result = new PopupResult<(string fileName, string password)>
            {
                PopupChoice = PopupChoice.Cancel,
                Result = (null, null)
            };
            await _popupNavigation.PopAsync();
        }

        private static string SanitizeFileName(string inputString)
        {
            if (_illegalCharsRegex.IsMatch(inputString))
            {
                string oldInput = inputString;
                inputString = _illegalCharsRegex.Replace(inputString, "");
                Debug.WriteLine($"Sanitizing {oldInput} into {inputString}");
            }

            return inputString;
        }
    }
}

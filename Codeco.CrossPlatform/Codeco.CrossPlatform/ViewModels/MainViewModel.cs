using Acr.UserDialogs;
using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.ViewModels
{
    public class MainViewModel : NavigableViewModelBase
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IUserFileService _userFileService;

        private readonly NamedKeyboard _defaultKeyboard = new NamedKeyboard(Keyboard.Default, "Default");
        private readonly NamedKeyboard _numericKeyboard = new NamedKeyboard(Keyboard.Numeric, "Numeric");

        private IDictionary<string, string> _activeDictFile = ImmutableDictionary.Create<string, string>();

        private string _codeText;
        public string CodeText
        {
            get => _codeText;
            set => Set(ref _codeText, value);
        }

        private bool _isCopyButtonEnabled;
        public bool IsCopyButtonEnabled
        {
            get => _isCopyButtonEnabled;
            set => Set(ref _isCopyButtonEnabled, value);
        }

        private NamedKeyboard _currentInputKeyboard;
        public NamedKeyboard CurrentInputKeyboard
        {
            get => _currentInputKeyboard;
            set => Set(ref _currentInputKeyboard, value);            
        }        

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                if (Set(ref _inputText, value))
                {
                    LookupValue(_inputText);
                }                
            }
        }

        public ObservableCollection<NamedKeyboard> AvailableKeyboards = new ObservableCollection<NamedKeyboard>();

        private RelayCommand _copyCodeTextCommand;
        public RelayCommand CopyCodeTextCommand => _copyCodeTextCommand ?? (_copyCodeTextCommand = new RelayCommand(CopyCodeText));

        private RelayCommand _addFileCommand;
        public RelayCommand AddFileCommand => _addFileCommand ?? (_addFileCommand = new RelayCommand(AddFile));

        public MainViewModel(INavigationService navService,
                             IUserDialogs userDialogs,
                             IUserFileService userFileService) 
            : base(navService)
        {
            _userDialogs = userDialogs;
            _userFileService = userFileService;

            _currentInputKeyboard = _defaultKeyboard;
            AvailableKeyboards.Add(_defaultKeyboard);
            AvailableKeyboards.Add(_numericKeyboard);
        }

        public override async Task Activated(NavigationType navType)
        {
            await Task.CompletedTask;
        }

        public override Task Deactivated()
        {
            return Task.CompletedTask;
        }

        private void LookupValue(string inputText)
        {
            if (_activeDictFile.TryGetValue(inputText, out string foundValue))
            {
                CodeText = foundValue;
            }
            else
            {
                CodeText = "";
            }
        }

        private async Task<string> GetPassword()
        {
            var result = await _userDialogs.PromptAsync(new PromptConfig
            {
                CancelText = "Cancel",
                InputType = InputType.Default,
                IsCancellable = true,
                Message = "Password",
                OkText = "Ok",
                Title = "Enter password",
                OnTextChanged = args => {
                    args.IsValid = !String.IsNullOrWhiteSpace(args.Value);
                },
                
            });

            if (result.Ok)
            {                
                return result.Value;
            }

            return null;
        }

        private async void AddFile()
        {
            Random rand = new Random();
            await _userFileService.CreateUserFileAsync($"test-{rand.Next()}.txt");
        }

        private void CopyCodeText()
        {
            //todo: copy current value to clipboard
        }
    }
}

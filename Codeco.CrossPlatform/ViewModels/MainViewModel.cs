using Acr.UserDialogs;
using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Services;
using Codeco.Encryption;
using DynamicData;
using GalaSoft.MvvmLight.Command;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Xamarin.Forms;
using Codeco.CrossPlatform.Extensions;
using System.Diagnostics;
using DynamicData.Binding;
using Plugin.Clipboard.Abstractions;

namespace Codeco.CrossPlatform.ViewModels
{
    public class MainViewModel : NavigableViewModelBase
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IUserFileService _userFileService;
        private readonly IFilePicker _filePicker;
        private readonly IClipboard _clipboard;

        private readonly NamedKeyboard _defaultKeyboard = new NamedKeyboard(Keyboard.Default, "Default");
        private readonly NamedKeyboard _numericKeyboard = new NamedKeyboard(Keyboard.Numeric, "Numeric");

        private IDictionary<string, string> _activeDictFile = ImmutableDictionary.Create<string, string>();

        private string _codeText;
        public string CodeText
        {
            get => _codeText;
            set
            {
                Set(ref _codeText, value);
                RaisePropertyChanged(nameof(IsCopyButtonEnabled));
            }
        }

        public bool IsCopyButtonEnabled => !String.IsNullOrWhiteSpace(CodeText);

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

        private string _activeFileName = "None";
        public string ActiveFileName
        {
            get => _activeFileName;
            set => Set(ref _activeFileName, value);
        }

        public ObservableCollection<NamedKeyboard> AvailableKeyboards { get; } = new ObservableCollection<NamedKeyboard>();

        private ReadOnlyObservableCollection<SimpleFileInfoViewModel> _files;
        public ReadOnlyObservableCollection<SimpleFileInfoViewModel> Files => _files;        

        private RelayCommand _copyCodeTextCommand;
        public RelayCommand CopyCodeTextCommand => _copyCodeTextCommand ?? (_copyCodeTextCommand = new RelayCommand(CopyCodeText));

        private RelayCommand _addFileCommand;
        public RelayCommand AddFileCommand => _addFileCommand ?? (_addFileCommand = new RelayCommand(AddFile));

        private RelayCommand<SimpleFileInfoViewModel> _renameItemCommand;
        public RelayCommand<SimpleFileInfoViewModel> RenameItemCommand => _renameItemCommand ?? (_renameItemCommand = new RelayCommand<SimpleFileInfoViewModel>(RenameItem));

        private RelayCommand<SimpleFileInfoViewModel> _deleteItemCommand;
        public RelayCommand<SimpleFileInfoViewModel> DeleteItemCommand => _deleteItemCommand ?? (_deleteItemCommand = new RelayCommand<SimpleFileInfoViewModel>(DeleteItem));

        private RelayCommand<SimpleFileInfoViewModel> _switchItemLocationCommand;
        public RelayCommand<SimpleFileInfoViewModel> SwitchItemLocationCommand => _switchItemLocationCommand ?? (_switchItemLocationCommand = new RelayCommand<SimpleFileInfoViewModel>(SwitchItemLocation));

        private RelayCommand<SimpleFileInfoViewModel> _setActiveFileCommand;
        public RelayCommand<SimpleFileInfoViewModel> SetActiveFileCommand => _setActiveFileCommand ?? (_setActiveFileCommand = new RelayCommand<SimpleFileInfoViewModel>(SetActiveFile));        

        public MainViewModel(INavigationService navService,
                             IUserDialogs userDialogs,
                             IUserFileService userFileService,
                             IFilePicker filePicker,
                             IClipboard clipboard,
                             IChangeJournalService changeJournalService) // remove this once we have the remote stuff all set up
            : base(navService)
        {
            _userDialogs = userDialogs;
            _userFileService = userFileService;
            _filePicker = filePicker;
            _clipboard = clipboard;

            AvailableKeyboards.Add(_defaultKeyboard);
            AvailableKeyboards.Add(_numericKeyboard);
            CurrentInputKeyboard = _defaultKeyboard;
        }

        public override Task Activated(NavigationType navType)
        {
            if (_files == null)
            {
                _userFileService.FilesList                    
                         .Connect()       
                         .Sort(SortExpressionComparer<SimpleFileInfoViewModel>
                            .Descending(x => x.FileLocation)
                            .ThenByAscending(x => x.Name))
                         .Bind(out _files)
                         .Subscribe();

                RaisePropertyChanged(nameof(Files));
            }                                  

            return Task.CompletedTask;
        }

        public override Task Deactivated()
        {
            return Task.CompletedTask;
        }

        private void LookupValue(string inputText)
        {
            if (_activeDictFile != null && _activeDictFile.TryGetValue(inputText, out string foundValue))
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
                OnTextChanged = args =>
                {
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
            var pickedFile = await CrossFilePicker.Current.PickFile();
            if (pickedFile == null)
            {
                return;
            }

            bool isValid = await _userFileService.ValidateFileAsync(pickedFile.DataArray);
            if (!isValid)
            {
                await _userDialogs.AlertAsync(new AlertConfig
                {
                    Message = "The file you tried to add was not in a format Codeco understands. It will not be added.",
                    OkText = "Ok",
                    Title = "Invalid file"
                });
                return;
            }
            string pickedFileDataString = Encoding.UTF8.GetString(pickedFile.DataArray);

            var filePopupResult = await _navigationService.ShowPopupViewModelAsync<AddFileViewModel, (string, string)>();
            if (filePopupResult.PopupChoice != Popups.PopupChoice.Ok)
            {
                return;
            }

            var (pickedFileName, pickedPassword) = filePopupResult.Result;

            await _userFileService.CreateUserFileAsync(pickedFileName, FileLocation.Local, pickedPassword, pickedFileDataString);
        }

        private async void SetActiveFile(SimpleFileInfoViewModel obj)
        {
            // Prompt user for password
            var promptResult = await _userDialogs.PromptAsync(new PromptConfig
            {
                Message = "Enter this file's password",
                OkText = "Unlock",
                CancelText = "Cancel",
                Title = "Enter password",
                Placeholder = "Password",
                OnTextChanged = args =>
                {
                    args.IsValid = !String.IsNullOrEmpty(args.Value);
                }
            });

            if (!promptResult.Ok)
            {
                return;
            }

            // Use password to decrypt file
            var fileDict = await _userFileService.GetUserFileContentsAsync(obj.Name, obj.FileLocation, promptResult.Text);

            // Load dict into current
            _activeDictFile = fileDict;

            // Probably do some other stuff too--change title, etc etc
            ActiveFileName = obj.Name;

            // Switch active tab to "Active File" tab
            // No mechanism to do this yet. Maybe use MVVM Light's Messenger

        }

        private void CopyCodeText()
        {
            _clipboard.SetText(CodeText);            
        }

        private async void RenameItem(SimpleFileInfoViewModel obj)
        {
            // Get new name
            var promptResult = await _userDialogs.PromptAsync(new PromptConfig
            {
                Title = "Rename file",
                Text = obj.Name,
                Placeholder = "New file name",
                OnTextChanged = args => args.IsValid = !String.IsNullOrWhiteSpace(args.Value),
                OkText = "Rename",
                CancelText = "Cancel"
            });

            if (promptResult.Ok)
            {
                await _userFileService.RenameUserFile(obj.Name, obj.FileLocation, promptResult.Value);
            }            
        }

        private void DeleteItem(SimpleFileInfoViewModel obj)
        {
            _userFileService.DeleteUserFileAsync(obj.Name, obj.FileLocation);
        }

        private void SwitchItemLocation(SimpleFileInfoViewModel obj)
        {
            FileLocation destinationLocation = obj.FileLocation == FileLocation.Local ? FileLocation.Roamed : FileLocation.Local;
            _userFileService.ChangeUserFileLocationAsync(obj.Name, obj.FileLocation, destinationLocation);
        }
    }
}

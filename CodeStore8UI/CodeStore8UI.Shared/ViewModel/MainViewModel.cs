using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System;
using Windows.ApplicationModel.Activation;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Text;
using System.Threading.Tasks;
using CodeStore8UI.Common;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using CodeStore8UI.Controls;
using System.Linq;
using System.Diagnostics;

namespace CodeStore8UI.ViewModel
{    
    public class MainViewModel : ViewModelBase, INavigable
    {
        private string _sessionPassword;
        private Dictionary<string, string> _codeDictionary = new Dictionary<string, string>();

        private RelayCommand _addFileCommand;
        public RelayCommand AddFileCommand => _addFileCommand ?? (_addFileCommand = new RelayCommand(AddFile));

        private RelayCommand _changePasswordCommand;
        public RelayCommand ChangePasswordCommand => _changePasswordCommand ?? (_changePasswordCommand = new RelayCommand(ChangePassword));

        private RelayCommand _deleteCodesCommand;
        public RelayCommand DeleteCodesCommand => _deleteCodesCommand ?? (_deleteCodesCommand = new RelayCommand(DeleteCodes));


        private int _longestCode = int.MaxValue;        
        #region LongestCode Property
        public int LongestCode
        {
            get { return _longestCode; }
            set
            {
                if (_longestCode == value)
                {
                    return;
                }                
                _longestCode = value;               
                RaisePropertyChanged(nameof(LongestCode));
            }
        }
        #endregion

        private string _codeText = "";
        #region CodeText Property
        public string CodeText
        {
            get { return _codeText; }
            set
            {
                if (_codeText == value)
                {
                    return;
                }                
                _codeText = value;                
                RaisePropertyChanged(nameof(CodeText));
            }
        }
        #endregion

        private string _inputText = "";
        #region InputText Property
        public string InputText
        {
            get { return _inputText; }
            set
            {
                if(value == _inputText)
                {
                    return;
                }
                _inputText = value;
                Debug.WriteLine("New input value is: " + value);
                LookupCode(_inputText); //Binding is always a character behind if we just use EventToCommand behaviors, so we cheat.
                RaisePropertyChanged(nameof(InputText));
            }
        }
#endregion

        public bool AllowGoingBack { get; set; }

        public MainViewModel()
        {            
        }

        private async void AddFile()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.CommitButtonText = "Add code file";
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".csv");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
#if WINDOWS_PHONE_APP
            picker.PickMultipleFilesAndContinue();
#else
            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
            StringBuilder sb = new StringBuilder();
            foreach (var file in files)
            {
                if (_sessionPassword == null)
                {
                    return;
                }
                sb.Append(await FileIO.ReadTextAsync(file));
            }
            if (sb.Length > 0)
            {
                await FileManager.AppendToEncryptedFile(sb.ToString(), _sessionPassword);
            }
            _codeDictionary = await LoadCodes();
#endif
        }

#if WINDOWS_PHONE_APP
        public async Task AddFile_PhoneContinued(FileOpenPickerContinuationEventArgs args)
        {
            IReadOnlyList<StorageFile> files = args.Files;
            StringBuilder sb = new StringBuilder();
            foreach (var file in files)
            {
                if (_sessionPassword == null)
                {
                    return;
                }
                sb.Append(await FileIO.ReadTextAsync(file));
            }
            if (sb.Length > 0)
            {
                await FileManager.AppendToEncryptedFile(sb.ToString(), _sessionPassword);
            }
            _codeDictionary = await LoadCodes();
        }
#endif

        private async void ChangePassword()
        {
            await ShowPasswordDialog(force: true);
        }

        private async Task ShowPasswordDialog(bool force = false)
        {
#if WINDOWS_PHONE_APP
            PasswordDialog dialog = new PasswordDialog();
            dialog.Closed += PasswordDialog_Closed;
            await dialog.ShowAsync();  
#else
            PasswordDialog dialog = new PasswordDialog();
            dialog.PasswordDialogClosed += PasswordDialog_Closed;
            dialog.IsOpen = true;
#endif

        }

#if WINDOWS_PHONE_APP
        private async void PasswordDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary)
            {
                _sessionPassword = (sender as PasswordDialog).Password;
                //todo: verify password here
                _codeDictionary = await LoadCodes();
            }
        }
#else
        private async void PasswordDialog_Closed(object sender, PasswordDialogClosedEventArgs args)
        {
            if(args.DialogResult == PasswordDialog.Result.Ok)
            {
                _sessionPassword = (sender as PasswordDialog).Password;
                //todo: verify password here
                _codeDictionary = await LoadCodes();
            }
        }
#endif

        private async Task<Dictionary<string, string>> LoadCodes()
        {
            Dictionary<string, string> codeDict = new Dictionary<string, string>();
            string fileContents = await FileManager.RetrieveEncryptedFile(_sessionPassword);

            if (fileContents == null)
            {
                return codeDict;
            }
            string[] lines = fileContents.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] codePair = line
                    .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x = x.Trim())
                    .ToArray();
                codeDict.Add(codePair[0], codePair[1]);

                LongestCode = codePair[0].Length > LongestCode ? codePair[0].Length : LongestCode;
            }

            return codeDict;
        }

        private async void DeleteCodes()
        {
            await FileManager.ClearFile();
            _codeDictionary = new Dictionary<string, string>();
        }

        private void LookupCode(string newText)
        {
            string foundValue = "";            
            Debug.WriteLine("LookupCode fired with input: " + newText);
            if (_codeDictionary.TryGetValue(newText, out foundValue))
            {
                CodeText = foundValue;
            }
            else
            {
                CodeText = "";
            }
        }

        public async void Activate(object parameter, NavigationMode navigationMode)
        {
            if(_sessionPassword == null)
            {
                await ShowPasswordDialog();
            }
        }

        public void Deactivate(object parameter)
        {
            //
        }
    }
}
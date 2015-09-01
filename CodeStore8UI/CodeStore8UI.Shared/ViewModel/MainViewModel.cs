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
using System.Collections.ObjectModel;
using CodeStore8UI.Model;

namespace CodeStore8UI.ViewModel
{    
    public class MainViewModel : ViewModelBase, INavigable
    {        
        private Dictionary<string, string> _codeDictionary = new Dictionary<string, string>();        

        private RelayCommand _addFileCommand;
        public RelayCommand AddFileCommand => _addFileCommand ?? (_addFileCommand = new RelayCommand(AddFile));

        private RelayCommand _changePasswordCommand;
        public RelayCommand ChangePasswordCommand => _changePasswordCommand ?? (_changePasswordCommand = new RelayCommand(ChangePassword));

        private RelayCommand _deleteCodesCommand;
        public RelayCommand DeleteCodesCommand => _deleteCodesCommand ?? (_deleteCodesCommand = new RelayCommand(DeleteCodes));

        private RelayCommand<StorageFile> _changeActiveFileCommand;
        public RelayCommand<StorageFile> ChangeActiveFileCommand => _changeActiveFileCommand 
            ?? (_changeActiveFileCommand = new RelayCommand<StorageFile>(ChangeActiveFile));      

        private int _longestCode = 1;        
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

        private string _inputText = "5";
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
                LookupCode(_inputText); //Binding is always a character behind if we just use EventToCommand behaviors, so we cheat with Two-Way Binding and manually Setting.
                RaisePropertyChanged(nameof(InputText));
            }
        }
        #endregion

        private ObservableCollection<BindableStorageFile> _savedFiles = new ObservableCollection<BindableStorageFile>();
        public ObservableCollection<BindableStorageFile> SavedFiles
        {
            get { return _savedFiles; }
            set
            {
                if(_savedFiles == value)
                {
                    return;
                }
                _savedFiles = value;
                RaisePropertyChanged(nameof(SavedFiles));
            }
        }

        private StorageFile _activeFile;
        public StorageFile ActiveFile
        {
            get { return _activeFile; }
            set
            {
                if(_activeFile == value)
                {
                    return;
                }
                _activeFile = value;
                OpenFileText = value.Name;
                RaisePropertyChanged(nameof(ActiveFile));
            }
        }

        private string _openFileText = "No active file";
        public string OpenFileText
        {
            get { return _openFileText; }
            set
            {
                _openFileText = "Active File: " + value;
                RaisePropertyChanged(nameof(OpenFileText));
            }
        }

        public bool AllowGoingBack { get; set; }

        public MainViewModel()
        {
            //not much to do here            
        }

        private async void AddFile()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.CommitButtonText = "Add code file";
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".csv");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
#if WINDOWS_PHONE_APP
            picker.PickSingleFileAndContinue();
#else
            StorageFile file = await picker.PickSingleFileAsync();
            AddFileDialogOutput output = await ShowAddFileDialog();
            if(output == null)
            {
                return;
            }
            string contents = await FileIO.ReadTextAsync(file);
            string savedFileName = await FileManager.SaveAndEncryptFile(contents, output.FileName, output.Password);
            ActiveFile = await FileManager.GetEncryptedFile(savedFileName);
            BindableStorageFile bsf = await BindableStorageFile.Create(ActiveFile);
            SavedFiles.Add(bsf);
            _codeDictionary = await GetCodes(output.Password);
#endif
        }

#if WINDOWS_PHONE_APP
        public async Task AddFile_PhoneContinued(FileOpenPickerContinuationEventArgs args)
        {
            AddFileDialogOutput output = await ShowAddFileDialog();
            if(output == null)
            {
                return;
            }

            StorageFile file = args.Files[0];
            string contents = await FileIO.ReadTextAsync(file);
            string savedFiledName = await FileManager.SaveAndEncryptFile(contents, output.FileName, output.Password);
            ActiveFile = await FileManager.GetEncryptedFile(savedFiledName);
            BindableStorageFile bsf = await BindableStorageFile.Create(ActiveFile);
            SavedFiles.Add(bsf);
            _codeDictionary = await GetCodes(output.Password);
        }
#endif

        private async void ChangePassword()
        {
            //todo: show change password dialog (request old pass & new pass)
        }

        private async Task<AddFileDialogOutput> ShowAddFileDialog()
        {
#if WINDOWS_PHONE_APP
            AddFileDialog dialog = new AddFileDialog();            
            if ((await dialog.ShowAsync()) == ContentDialogResult.Primary)
            {
                return new AddFileDialogOutput
                {
                    FileName = dialog.FileName,
                    Password = dialog.Password
                };                
            }
            else
            {
                return null;
            }
#else
            AddFileDialog dialog = new AddFileDialog();            
            dialog.IsOpen = true;
            if ((await dialog.WhenClosed()).DialogResult == AddFileDialog.Result.Ok)
            {
                return new AddFileDialogOutput
                {
                    FileName = dialog.FileName,
                    Password = dialog.Password
                };
            }
            else
            {
                return null;
            }
#endif
        }

        private async Task<Dictionary<string, string>> GetCodes(string password)
        {
            Dictionary<string, string> codeDict = new Dictionary<string, string>();
            string fileContents = await FileManager.RetrieveFileContents(ActiveFile.Name, password);

            if (fileContents == null)
            {
                return codeDict;
            }
            string[] lines = fileContents.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            LongestCode = 1;
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
            await FileManager.ClearFileAsync(_activeFile.Name);
            _codeDictionary = new Dictionary<string, string>();
        }

        private void LookupCode(string newText)
        {
            string foundValue = "";                        
            if (_codeDictionary.TryGetValue(newText, out foundValue))
            {
                CodeText = foundValue;
            }
            else
            {
                CodeText = "";
            }
        }

        private async void ChangeActiveFile(StorageFile arg)
        {
            ActiveFile = arg;
            string password = await GetPassword();
            _codeDictionary = await GetCodes(password);
        }

        private async Task<string> GetPassword()
        {
#if WINDOWS_PHONE_APP
            PasswordDialog dialog = new PasswordDialog();
            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                return dialog.Password;
            }
            else
            {
                return null;
            }
#else
            PasswordDialog dialog = new PasswordDialog();
            dialog.IsOpen = true;
            if((await dialog.WhenClosed()).DialogResult == PasswordDialog.Result.Ok)
            {
                return dialog.Password;
            }
            else
            {
                return null;
            }
#endif
        }

        public async void Activate(object parameter, NavigationMode navigationMode)
        {
           var files = new ObservableCollection<StorageFile>(await FileManager.GetFiles());
           foreach(var file in files)
           {
                BindableStorageFile bsf = await BindableStorageFile.Create(file);
                SavedFiles.Add(bsf);
           }
        }

        public void Deactivate(object parameter)
        {
            //
        }
    }
}
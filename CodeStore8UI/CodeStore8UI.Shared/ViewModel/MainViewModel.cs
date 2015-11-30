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
using Windows.UI.Popups;
using CodeStore8UI.Services;
using GalaSoft.MvvmLight.Views;

namespace CodeStore8UI.ViewModel
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        private Dictionary<string, string> _codeDictionary = new Dictionary<string, string>();
        private FileService _fileService;
        public FileService FileService => _fileService;
        private NavigationServiceEx _navigationService;

        private RelayCommand _addFileCommand;
        public RelayCommand AddFileCommand => _addFileCommand ?? (_addFileCommand = new RelayCommand(AddFile));        

        private RelayCommand _deleteCodesCommand;
        public RelayCommand DeleteCodesCommand => _deleteCodesCommand ?? (_deleteCodesCommand = new RelayCommand(DeleteCodes));

        private RelayCommand<BindableStorageFile> _changeActiveFileCommand;
        public RelayCommand<BindableStorageFile> ChangeActiveFileCommand => _changeActiveFileCommand
            ?? (_changeActiveFileCommand = new RelayCommand<BindableStorageFile>(ChangeActiveFile));

        private RelayCommand<BindableStorageFile> _deleteFileCommand;
        public RelayCommand<BindableStorageFile> DeleteFileCommand => _deleteFileCommand
            ?? (_deleteFileCommand = new RelayCommand<BindableStorageFile>(DeleteFile));

        private RelayCommand<BindableStorageFile> _renameFileCommand;
        public RelayCommand<BindableStorageFile> RenameFileCommand => _renameFileCommand
            ?? (_renameFileCommand = new RelayCommand<BindableStorageFile>(RenameFile));

        private RelayCommand _goToSettingsCommand;
        public RelayCommand GoToSettingsCommand => _goToSettingsCommand ?? (_goToSettingsCommand = new RelayCommand(GoToSettings));

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

        private string _inputText = "";
        #region InputText Property
        public string InputText
        {
            get { return _inputText; }
            set
            {
                if (value == _inputText)
                {
                    return;
                }
                _inputText = value;
                LookupCode(_inputText); //Binding is always a character behind if we just use EventToCommand behaviors, so we cheat with Two-Way Binding and manually Setting.
                RaisePropertyChanged(nameof(InputText));
            }
        }
        #endregion

        private ObservableCollection<FileCollection> _fileGroups = new ObservableCollection<FileCollection>();
        public ObservableCollection<FileCollection> FileGroups
        {
            get { return _fileGroups; }
            set
            {
                if (_fileGroups == value)
                {
                    return;
                }
                _fileGroups = value;
                RaisePropertyChanged();
            }
        }

        private BindableStorageFile _activeFile;
        public BindableStorageFile ActiveFile
        {
            get { return _activeFile; }
            set
            {
                if (_activeFile == value)
                {
                    return;
                }
                _activeFile = value;
                OpenFileText = value.Name;
                RaisePropertyChanged(nameof(ActiveFile));
            }
        }

        private string _openFileText = "";
        public string OpenFileText
        {
            get { return _openFileText; }
            set
            {
                if (_openFileText == value)
                {
                    return;
                }
                _openFileText = value;
                RaisePropertyChanged(nameof(OpenFileText));
            }
        }

        public bool AllowGoingBack { get; set; }

        public MainViewModel(IService fileService, INavigationServiceEx navService)
        {
            _fileService = (fileService as FileService);
            _navigationService = navService as NavigationServiceEx;            
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
            if (file != null)
            {
                await OpenFile(file);
            }
#endif
        }

#if WINDOWS_PHONE_APP
        public async Task AddFile_PhoneContinued(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                StorageFile file = args.Files[0];
                await OpenFile(file);
            }
        }
#endif

        private async Task OpenFile(StorageFile file)
        {
            bool isValid = await FileUtilities.ValidateFileAsync(file);
            if (!isValid)
            {
                MessageDialog dialog = new MessageDialog("The file you tried to open was not formatted correctly. Are you sure it's a two-column comma-delimited file?", "Unable to read file");
                await dialog.ShowAsync();
                return;
            }
            AddFileDialogOutput output = await ShowAddFileDialog(file.Name);
            if (output == null)
            {
                return;
            }
            string contents = await FileIO.ReadTextAsync(file);
            ActiveFile = await _fileService.SaveAndEncryptFileAsync(contents, output.FileName, output.Password);
            FileGroups.Where(x => x.Location == FileService.FileLocation.Local).First().Files.Add(ActiveFile);
            _codeDictionary = await GetCodes(output.Password);
        }        

        private async Task<AddFileDialogOutput> ShowAddFileDialog(string fileName)
        {
#if WINDOWS_PHONE_APP
            AddFileDialog dialog = new AddFileDialog();    
            dialog.FileName = fileName;        
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
            dialog.FileName = fileName;
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
            FileService.FileLocation location = _fileService.GetFileLocation(ActiveFile);
            string fileContents = await _fileService.RetrieveFileContentsAsync(ActiveFile.Name, password, location);

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
            FileService.FileLocation location = _fileService.GetFileLocation(ActiveFile);
            await _fileService.ClearFileAsync(ActiveFile.Name, location);
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

        private async void ChangeActiveFile(BindableStorageFile arg)
        {
            string password = await GetPassword();
            if (password != null)
            {
                ActiveFile = arg;
                _codeDictionary = await GetCodes(password);
            }
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
            if ((await dialog.WhenClosed()).DialogResult == PasswordDialog.Result.Ok)
            {
                return dialog.Password;
            }
            else
            {
                return null;
            }
#endif
        }

        private async void DeleteFile(BindableStorageFile item)
        {
            FileService.FileLocation location = _fileService.GetFileLocation(item);
            await _fileService.DeleteFileAsync((StorageFile)item.BackingFile, location);
            FileGroups.Where(x => x.Location == location)
                .First()
                .Files
                .Remove(item);
        }

        private async void RenameFile(BindableStorageFile item)
        {
            string newName = await GetNewName();
            if (newName != null)
            {
                await _fileService.RenameFileAsync(item, newName);                
            }
        }

        private async Task<string> GetNewName()
        {
#if WINDOWS_PHONE_APP
            RenameDialog dialog = new RenameDialog();
            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                return dialog.NewName;
            }
            else
            {
                return null;
            }
#else
            RenameDialog dialog = new RenameDialog();
            dialog.IsOpen = true;
            if((await dialog.WhenClosed()).DialogResult == RenameDialog.Result.Ok)
            {
                return dialog.NewName;
            }
            else
            {
                return null;
            }
#endif
        }

        private void GoToSettings()
        {
            _navigationService.NavigateTo(nameof(SettingsPage));
        }

        public void Activate(object parameter, NavigationMode navigationMode)
        {
            _navigationService.BackButtonPressed += OnBackPressed;

            FileGroups.Add(new FileCollection(Constants.ROAMED_FILES_TITLE,
                new ObservableCollection<IBindableStorageFile>(_fileService.GetRoamedFiles()), FileService.FileLocation.Roamed));
            FileGroups.Add(new FileCollection(Constants.LOCAL_FILES_TITLE, 
                new ObservableCollection<IBindableStorageFile>(_fileService.GetLocalFiles()), FileService.FileLocation.Local));            
        }


        private void OnBackPressed(object sender, UniversalBackPressedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("MainPageVM BackPressed!");
            _navigationService.GoBack();
        }


        public void Deactivating(object parameter)
        {

        }

        public void Deactivated(object parameter)
        {
            _navigationService.BackButtonPressed -= OnBackPressed;
            FileGroups.Clear();
        }
    }
}
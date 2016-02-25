using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Codeco.Windows10.Services;
using Codeco.Windows10.Models;
using Codeco.Windows10.Common;
using Codeco.Windows10.Views;
using Codeco.Windows10.Controls;

namespace Codeco.Windows10.ViewModels
{
    public class MainViewModel : ViewModelBase, INavigable
    {        
        private Dictionary<string, string> _codeDictionary = new Dictionary<string, string>();
        private readonly FileService _fileService;
        //TODO: This is only used for a debug function. Probably best to find a way to remove it.
        public FileService FileService => _fileService;
        private readonly NavigationServiceEx _navigationService;        

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
                OpenFileText = value?.Name ?? "none";
                RaisePropertyChanged(nameof(ActiveFile));
            }
        }

        private string _openFileText = "none";
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

        public MainViewModel(IFileService fileService, INavigationServiceEx navService)
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
            FileGroups.First(x => x.Location == FileService.FileLocation.Local).Files.Add(ActiveFile);            
            _codeDictionary = await GetCodes(output.Password);
        }        

        private async Task<AddFileDialogOutput> ShowAddFileDialog(string fileName)
        {
            AddFileDialog dialog = new AddFileDialog(fileName);
            await dialog.ShowAsync();
            if(dialog.Result != null)
            {
                return dialog.Result;
            }
            else
            {
                return null;
            }
        }

        private async Task ShowErrorDialog(string title, string message)
        {
            MessageDialog errorDialog = new MessageDialog(message, title);
            await errorDialog.ShowAsync();
        }

        private async Task<Dictionary<string, string>> GetCodes(string password)
        {
            Dictionary<string, string> codeDict = new Dictionary<string, string>();
            FileService.FileLocation location = _fileService.GetFileLocation(ActiveFile);
            try
            {
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
            catch(Exception ex)
            {
                ActiveFile = null;
                await ShowErrorDialog("Could not decrypt file", "Unable to decrypt and open the file. Ensure that you've entered your password correctly, and try again.");
                return new Dictionary<string, string>();           
            }              
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
            PasswordDialog dialog = new PasswordDialog();            
            await dialog.ShowAsync();
            if (dialog.Result != null)
            {
                return dialog.Result;
            }
            else
            {
                return null;
            }
        }

        private async void DeleteFile(BindableStorageFile item)
        {
            FileService.FileLocation location = _fileService.GetFileLocation(item);
            await _fileService.DeleteFileAsync((StorageFile)item.BackingFile, location);
            FileGroups.First(x => x.Location == location)
                .Files
                .Remove(item);
            if (item == ActiveFile)
            {
                ActiveFile = null;
            }
        }

        private async void RenameFile(BindableStorageFile item)
        {
            string newName = await GetNewName();
            if (newName != null)
            {
                await _fileService.RenameFileAsync(item, newName);                
                if(ActiveFile == item)
                {
                    OpenFileText = item.Name;
                }
            }
        }

        private async Task<string> GetNewName()
        {
            throw new NotImplementedException("Need to create a GetName dialog for UWP");
        }


        private void GoToSettings()
        {
            _navigationService.NavigateTo(nameof(SettingsPage));
        }
       
        public async void Activate(object parameter, NavigationMode navigationMode)
        {
            _navigationService.BackButtonPressed += OnBackPressed;

            await FileService.IsInitialized.Task;

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
using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Services;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.ViewModels
{
    public class MainViewModel : NavigableViewModelBase
    {
        private NamedKeyboard DefaultKeyboard = new NamedKeyboard(Keyboard.Default, "Default");
        private NamedKeyboard NumericKeyboard = new NamedKeyboard(Keyboard.Numeric, "Numeric");

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
            set => Set(ref _inputText, value);
        }

        public ObservableCollection<NamedKeyboard> AvailableKeyboards = new ObservableCollection<NamedKeyboard>();

        private RelayCommand _copyCodeTextCommand;
        public RelayCommand CopyCodeTextCommand => _copyCodeTextCommand ?? (_copyCodeTextCommand = new RelayCommand(CopyCodeText));

        public MainViewModel(INavigationService navService) : base(navService)
        {
            _currentInputKeyboard = DefaultKeyboard;
            AvailableKeyboards.Add(DefaultKeyboard);
            AvailableKeyboards.Add(NumericKeyboard);
        }

        private void CopyCodeText()
        {
            //todo: copy current value to clipboard
        }
    }
}

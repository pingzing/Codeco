using CodeStore8UI.Common;
using CodeStore8UI.Controls;
using CodeStore8UI.ViewModel;
using Coding4Fun.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CodeStore8UI
{    
    public sealed partial class MainPage : BindablePage, IFileOpenPickerContinuable
    {       
        public MainPage()
        {
            this.InitializeComponent();            
        }               

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            await (DataContext as MainViewModel).AddFile_PhoneContinued(args);
        }
    }
}

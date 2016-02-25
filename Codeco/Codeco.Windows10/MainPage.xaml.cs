using Codeco.Common;
using Codeco.Controls;
using Codeco.Model;
using Codeco.ViewModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Codeco
{    
    public sealed partial class MainPage : BindablePage
    {
        private const string ACTIVE_INPUT_PREFIX_TEXT = "Input method:";

        public MainPage()
        {
            this.InitializeComponent();
            if (InputBoxNumbers.Visibility == Visibility.Visible)
            {
                InputScopeBlock.Text = GetNewActiveInputScopeText(InputScopeNameValue.Number);
            }
            else
            {
                InputScopeBlock.Text = GetNewActiveInputScopeText(InputScopeNameValue.Default);
            }
        }                               

        private void SavedFile_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            if(item != null)
            {
                FlyoutBase.ShowAttachedFlyout(item);
            }
        }

        private void SavedFile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedFile = (sender as StackPanel)?.Tag as BindableStorageFile;
            if (tappedFile == null)
            {
                return;
            }

            var context = (DataContext as MainViewModel);
            context?.ChangeActiveFileCommand.Execute(tappedFile);
        }        

        private string GetNewActiveInputScopeText(InputScopeNameValue currentScope)
        {
            switch (currentScope)
            {
                case InputScopeNameValue.Default:
                    return ACTIVE_INPUT_PREFIX_TEXT + " General";
                case InputScopeNameValue.Number:
                    return ACTIVE_INPUT_PREFIX_TEXT + " Numbers only";
                default:
                    return "";
            }
        }
    
        //This gets handled in code-behind by swapping text-boxes rather than having a single text box with a databound InputScope
        //becaue apparently, changing a TextBox's InputScope at runtime doesn't work on WP8.1 Universal. Ugh.
        private void CycleInputScopeButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Toggle visibility of both textboxes
            InputBoxNumbers.Visibility = InputBoxNumbers.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
            InputBoxGeneral.Visibility = InputBoxGeneral.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;

            if (InputBoxGeneral.Visibility == Visibility.Visible)
            {
                InputScopeBlock.Text = GetNewActiveInputScopeText(InputScopeNameValue.Default);
            }
            else
            {
                InputScopeBlock.Text = GetNewActiveInputScopeText(InputScopeNameValue.Number);
            }
        }        
    }
}

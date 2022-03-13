using Codeco.CrossPlatform.Models.Messages;
using Codeco.CrossPlatform.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Views
{
    public partial class MainPage : TabbedPage
    {
        private readonly IMessagingCenter _messagingCenter;

        public MainPage()
        {
            InitializeComponent();
            _messagingCenter = (BindingContext as MainViewModel)?.MessagingCenter;
            _messagingCenter.Subscribe<MainViewModel>(this, nameof(FileSetActiveMessage), FileSetActive);
        }

        public void FilesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null; //Disable selection entirely.
        }

        public void FilesList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as SimpleFileInfoViewModel;
            if (item != null)
            {
                var viewModel = BindingContext as MainViewModel;
                if (viewModel != null)
                {
                    viewModel.SetActiveFileCommand.Execute(item);
                }
            }
        }

        // All these context menu _Clicked methods that move or delete an item
        // clear the ViewCell's ContextActions, because on UWP, if that ViewCell 
        // gets recycled, it will use the old BindingContext for the ContextActions.
        // Clearing the ContextActions seems to force reevaluation, however.

        public void RenameItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewCell = (ViewCell)menuItem.BindingContext;

            var viewModel = BindingContext as MainViewModel;
            var fileInfoItem = viewCell.BindingContext as SimpleFileInfoViewModel;
            viewModel.RenameItemCommand.Execute(fileInfoItem);
        }

        public void DeleteItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewCell = (ViewCell)menuItem.BindingContext;
            viewCell.ContextActions.Clear();

            var viewModel = BindingContext as MainViewModel;
            var fileInfoItem = viewCell.BindingContext as SimpleFileInfoViewModel;
            viewModel.DeleteItemCommand.Execute(fileInfoItem);
        }

        public void SwitchLocation_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewCell = (ViewCell)menuItem.BindingContext;
            viewCell.ContextActions.Clear();

            var viewModel = BindingContext as MainViewModel;
            var fileInfoItem = viewCell.BindingContext as SimpleFileInfoViewModel;
            viewModel.SwitchItemLocationCommand.Execute(fileInfoItem);
        }

        private bool _copyEffectAnimating = false;
        public async void CopyButton_Clicked(object sender, EventArgs e)
        {
            if (!_copyEffectAnimating)
            {
                _copyEffectAnimating = true;
                await CopyEffectText.TranslateTo(0, 50, 0, null);
                CopyEffectText.FadeTo(1, length: 250);
                await CopyEffectText.TranslateTo(0, 0, 250, Easing.SpringOut);
                await Task.Delay(2000);
                await CopyEffectText.FadeTo(0);
                await CopyEffectText.TranslateTo(0, 0, 0, null);
                _copyEffectAnimating = false;
            }
        }

        // We're doing this in code, because data bindings on MenuItems are broken on UWP, due
        // to a ViewCell recycling bug.
        public void FileItemTemplateViewCell_BindingContextChanged(object sender, EventArgs e)
        {
            var viewCell = sender as ViewCell;
            if (viewCell != null)
            {
                var simpleFileInfoViewModel = viewCell.BindingContext as SimpleFileInfoViewModel;
                if (simpleFileInfoViewModel != null)
                {
                    var switchLocationMenuItem = viewCell.ContextActions[1];
                    switchLocationMenuItem.Text = simpleFileInfoViewModel.SwitchLocationText;
                }
            }
        }

        private void FileSetActive(MainViewModel sender)
        {
            if (CurrentPage != ActiveFileTab)
            {
                CurrentPage = ActiveFileTab;
            }
        }
    }
}
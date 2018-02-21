using Codeco.CrossPlatform.Models;
using Codeco.CrossPlatform.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Codeco.CrossPlatform.Views
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void FilesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null; //Disable selection entirely.
        }

        public void RenameItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewCell = (ViewCell)menuItem.BindingContext;            

            var viewModel = this.BindingContext as MainViewModel;
            var fileInfoItem = viewCell.BindingContext as SimpleFileInfoViewModel;
            viewModel.RenameItemCommand.Execute(fileInfoItem);
        }

        public void DeleteItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewCell = (ViewCell)menuItem.BindingContext;
            viewCell.ContextActions.Clear();

            var viewModel = this.BindingContext as MainViewModel;
            var fileInfoItem = viewCell.BindingContext as SimpleFileInfoViewModel;
            viewModel.DeleteItemCommand.Execute(fileInfoItem);
        }

        public void SwitchLocation_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewCell = (ViewCell)menuItem.BindingContext;
            viewCell.ContextActions.Clear();

            var viewModel = this.BindingContext as MainViewModel;
            var fileInfoItem = viewCell.BindingContext as SimpleFileInfoViewModel;
            viewModel.SwitchItemLocationCommand.Execute(fileInfoItem);
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
    }
}
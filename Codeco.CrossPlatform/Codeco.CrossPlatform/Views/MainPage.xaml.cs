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
            var viewModel = this.BindingContext as MainViewModel;
            var fileInfoItem = menuItem.CommandParameter as SimpleFileInfoViewModel;
            viewModel.RenameItemCommand.Execute(fileInfoItem);
        }

        public void DeleteItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewModel = this.BindingContext as MainViewModel;
            var fileInfoItem = menuItem.CommandParameter as SimpleFileInfoViewModel;
            viewModel.DeleteItemCommand.Execute(fileInfoItem);
        }

        public void SwitchLocation_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewModel = this.BindingContext as MainViewModel;
            var fileInfoItem = menuItem.CommandParameter as SimpleFileInfoViewModel;
            viewModel.SwitchItemLocationCommand.Execute(fileInfoItem);
        }
    }
}
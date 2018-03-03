using Codeco.CrossPlatform.ViewModels;
using Rg.Plugins.Popup.Pages;

namespace Codeco.CrossPlatform.Popups
{
    public partial class AddFileView : PopupPage
    {
        public AddFileView()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (this.BindingContext as AddFileViewModel)?.Opened();
        }

        protected override async void OnDisappearing()
        {
            await (this.BindingContext as AddFileViewModel)?.Closed();
            base.OnDisappearing();
        }
    }
}
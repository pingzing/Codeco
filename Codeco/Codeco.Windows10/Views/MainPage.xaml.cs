using System;
using Codeco.Windows10.Common;
using Codeco.Windows10.Models;
using Codeco.Windows10.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.Foundation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Codeco.Windows10.Views
{
    public sealed partial class MainPage : BindablePage
    {
        private static bool _isFlyoutOpen = false;
        private static bool _listeningToFlyoutState = false;

        public MainViewModel ViewModel { get; }

        public MainPage()
        {            
            this.InitializeComponent();
            ViewModel = DataContext as MainViewModel;
            Messenger.Default.Register<object>(this, Constants.SCROLL_PIVOT_MESSAGE, ScrollToMainPivot);
#if DEBUG
            if (ClearAllWideButton != null)
            {
                ClearAllWideButton.Visibility = Visibility.Visible;
            }
            if (ClearAllNarrowButton != null)
            {
                ClearAllNarrowButton.Visibility = Visibility.Visible;
            }
#endif
        }

        private void ScrollToMainPivot(object _)
        {
            if(MainPivot != null)
            {
                this.MainPivot.SelectedItem = ActivePivotItem;
            }            
        }

        private void SavedFile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedFile = (sender as StackPanel)?.DataContext as BindableStorageFile;
            if (tappedFile == null)
            {
                return;
            }

            var context = (DataContext as MainViewModel);
            context?.ChangeActiveFileCommand.Execute(tappedFile);
        }

        private async void Deubg_ClearAllTapped(object sender, TappedRoutedEventArgs e)
        {
            await ViewModel.ClearAllFiles();
        }

        private void SavedFile_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            ShowFileContextMenu(item, e.GetPosition(this));
        }

        private void ContainerStackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            ShowFileContextMenu(item, e.GetPosition(this));
        }

        private void ShowFileContextMenu(FrameworkElement item, Point position)
        {
            if (item != null)
            {
                MenuFlyout flyout = FlyoutBase.GetAttachedFlyout(item) as MenuFlyout;
                if (!_listeningToFlyoutState)
                {
                    flyout.Opened += (s, e) => _isFlyoutOpen = true;
                    flyout.Closed += (s, e) => _isFlyoutOpen = false;
                    _listeningToFlyoutState = true;
                }
                if (!_isFlyoutOpen)
                {
                    flyout?.ShowAt(this, position);
                }
            }
        }
    }
}

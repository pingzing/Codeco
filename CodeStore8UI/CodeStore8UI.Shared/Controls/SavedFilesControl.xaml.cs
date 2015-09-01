using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CodeStore8UI.Controls
{
    public sealed partial class SavedFilesControl : UserControl
    {

        public event EventHandler<SelectionChangedEventArgs> FileListSelectionChanged;        

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<object>), typeof(SavedFilesControl), new PropertyMetadata(null));
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(SavedFilesControl), new PropertyMetadata(null));
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(SavedFilesControl), new PropertyMetadata(Visibility.Visible));
        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register("DeleteCommand", typeof(RelayCommand), typeof(SavedFilesControl), new PropertyMetadata(null));
        public RelayCommand DeleteCommand
        {
            get { return (RelayCommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty RenameCommandProperty = DependencyProperty.Register("RenameCommand", typeof(RelayCommand), typeof(SavedFilesControl), new PropertyMetadata(null));
        public RelayCommand RenameCommand
        {
            get { return (RelayCommand)GetValue(RenameCommandProperty); }
            set { SetValue(RenameCommandProperty, value); }
        }
               
        public SavedFilesControl()
        {
            this.InitializeComponent();
            ((FrameworkElement)this.Content).DataContext = this;
            SavedFiledListView.SelectionMode = ListViewSelectionMode.Single;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var handler = FileListSelectionChanged;
            handler?.Invoke(sender, e);
        }        
    }
}

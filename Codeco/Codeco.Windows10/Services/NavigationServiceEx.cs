using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using GalaSoft.MvvmLight.Views;

namespace Codeco.Windows10.Services
{
    public class NavigationServiceEx : NavigationService, INavigationServiceEx
    {                             
        public int BackStackDepth
        {
            get
            {
                var frame = (Frame) Window.Current.Content;
                return frame.BackStackDepth;
            }
        }
      
        public bool CanGoBack
        {
            get
            {               
                var frame = (Frame) Window.Current.Content;            
                return frame.CanGoBack;
            }
        }

        public void ClearBackStack()
        {
            var frame = (Frame)Window.Current.Content;
            frame.BackStack.Clear();
            RaiseCanGoBackChanged();
        }

        public void BackStackRemoveAt(int index)
        {
            var frame = (Frame)Window.Current.Content;
            frame.BackStack.RemoveAt(index);
            RaiseCanGoBackChanged();
        }

        public void BackStackRemove(PageStackEntry entry)
        {
            var frame = (Frame)Window.Current.Content;
            frame.BackStack.Remove(entry);
            RaiseCanGoBackChanged();
        }

        public PageStackEntry BackStackGet(PageStackEntry entry)
        {
            var frame = (Frame)Window.Current.Content;
            return frame.BackStack[frame.BackStack.IndexOf(entry)];
        }

        public PageStackEntry BackStackGetAt(int index)
        {
            var frame = (Frame)Window.Current.Content;
            return frame.BackStack[index];
        }

        public override void NavigateTo(string pageKey, object parameter)
        {
            base.NavigateTo(pageKey, parameter);
            RaiseCanGoBackChanged();
        }

        public new void GoBack()
        {
            base.GoBack();
            RaiseCanGoBackChanged();                        
        }

        public event CanGoBackChangedHandler CanGoBackChanged;

        private void RaiseCanGoBackChanged()
        {
            CanGoBackChanged?.Invoke(this, new CanGoBackChangedHandlerArgs(CanGoBack));
        }      
    }

    public delegate void CanGoBackChangedHandler(object sender, CanGoBackChangedHandlerArgs args);

    public class CanGoBackChangedHandlerArgs
    {
        public bool NewCanGoBack { get; private set; }

        public CanGoBackChangedHandlerArgs(bool newCanGoBack)
        {
            NewCanGoBack = newCanGoBack;
        }
    }
}

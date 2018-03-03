using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Views;

namespace Codeco.Windows10.Services
{
    public interface INavigationServiceEx : INavigationService
    {
        new void GoBack();

        int BackStackDepth { get; }
        bool CanGoBack { get; }
        void ClearBackStack();
        void BackStackRemoveAt(int index);
        void BackStackRemove(PageStackEntry entry);        
        PageStackEntry BackStackGet(PageStackEntry entry);
        PageStackEntry BackStackGetAt(int index);        

        event CanGoBackChangedHandler CanGoBackChanged;
    }
}

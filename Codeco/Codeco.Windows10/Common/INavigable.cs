using Windows.UI.Xaml.Navigation;

namespace Codeco.Windows10.Common
{
    public interface INavigable
    {
        void Activate(object parameter, NavigationMode navigationMode);
        void Deactivating(object parameter);
        void Deactivated(object parameter);        
    }
}

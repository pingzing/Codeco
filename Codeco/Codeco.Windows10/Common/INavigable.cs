using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Codeco.Windows10.Common
{
    public interface INavigable
    {
        void Activate(object parameter, NavigationMode navigationMode);        
        void Deactivated(object parameter);        
    }
}

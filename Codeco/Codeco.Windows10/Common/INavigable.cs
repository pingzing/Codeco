using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Codeco.Windows10.Common
{
    public interface INavigable
    {
        Task ActivateAsync(object parameter, NavigationMode navigationMode);        
        Task DeactivatedAsync(object parameter);        
    }
}

using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Mvvm
{
    public interface INavigable
    {
        Task Activated(NavigationType navType);

        Task Deactivated();
    }
}

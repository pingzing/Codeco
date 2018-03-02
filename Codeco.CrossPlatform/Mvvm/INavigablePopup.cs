using Codeco.CrossPlatform.Popups;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Mvvm
{
    public interface INavigablePopup
    {
        Task Opened();
        Task Closed();
    }

    public interface INavigablePopup<TResult> : INavigablePopup
    {
        PopupResult<TResult> Result { get; }
    }
}

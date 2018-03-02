using Codeco.CrossPlatform.Popups;
using Codeco.CrossPlatform.Services;
using GalaSoft.MvvmLight;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Mvvm
{
    public class NavigablePopupViewModelBase : ViewModelBase, INavigablePopup
    {
        protected INavigationService _navigationService;

        public PopupResult Result { get; protected set; }

        public NavigablePopupViewModelBase(INavigationService navService)
        {
            _navigationService = navService;
        }

        public virtual Task Closed()
        {
            return Task.CompletedTask;
        }

        public virtual Task Opened()
        {
            return Task.CompletedTask;
        }
    }

    public class NavigablePopupViewModelBase<TResult> : NavigablePopupViewModelBase, INavigablePopup<TResult>
    {
        public new PopupResult<TResult> Result { get; protected set; }

        public NavigablePopupViewModelBase(INavigationService navService) : base(navService)
        {

        }
    }
}

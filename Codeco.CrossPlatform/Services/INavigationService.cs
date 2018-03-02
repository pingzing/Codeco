using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Popups;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Services
{

    //todo: xml doooooc
    public interface INavigationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task GoBackAsync(bool animated);

        Task NavigateToViewModelAsync<T>(bool animated = true) where T : INavigable;        
        Task NavigateToViewModelAsync(Type vmType, bool animated = true);
        Task NavigateToViewModelAsync(Type vmType, object parameter, bool animated = true);
        Task NavigateToPageAsync<T>(bool animated = true) where T : Page;
        Task NavigateToViewModelAsync<T>(object parameter, bool animated = true) where T : INavigable;
        Task NavigateToPageAsync<T>(object parameter, bool animated = true) where T : Page;

        Task<PopupResult> ShowPopupViewModelAsync<T>(bool animated = true) where T : INavigablePopup;
        Task<PopupResult> ShowPopupAsync(Type popupType, bool animated = true);
        Task<PopupResult<TResult>> ShowPopupViewModelAsync<TViewModel, TResult>(bool animated = true) where TViewModel : INavigablePopup<TResult>;
        Task<PopupResult<TResult>> ShowPopupAsync<TResult>(Type popupType, bool animated = true);

        void ClearBackStack();

        bool CanGoBack { get; }

        event EventHandler<CanGoBackChangedHandlerArgs> CanGoBackChanged;
    }
}

using Codeco.CrossPlatform.Popups;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.CrossPlatform.Mvvm
{
    public class PopupHost
    {
        private readonly IPopupNavigation _popupNavigation;

        public PopupHost(IPopupNavigation popupNavigation)
        {
            _popupNavigation = popupNavigation;
        }

        public async Task<PopupResult> ShowAsync(PopupPage pageToShow, bool animated = true)
        {
            TaskCompletionSource<PopupResult> _showTcs = new TaskCompletionSource<PopupResult>();

            void PageToShow_Disappearing(object sender, EventArgs e)
            {
                pageToShow.Disappearing -= PageToShow_Disappearing;

                var vm = pageToShow.BindingContext as NavigablePopupViewModelBase;
                if (vm != null)
                {
                    _showTcs.SetResult(vm.Result);
                }
                else
                {
                    _showTcs.SetResult(new PopupResult { PopupChoice = PopupChoice.Unknown });
                }
            }

            pageToShow.Disappearing += PageToShow_Disappearing;

            await _popupNavigation.PushAsync(pageToShow, animated);
            return await _showTcs.Task;
        }        

        public async Task<PopupResult<TResult>> ShowAsync<TResult>(PopupPage pageToShow, bool animated = true)
        {
            TaskCompletionSource<PopupResult<TResult>> _showTcs = new TaskCompletionSource<PopupResult<TResult>>();

            void PageToShow_Disappearing(object sender, EventArgs e)
            {
                var vm = pageToShow.BindingContext as NavigablePopupViewModelBase<TResult>;
                if (vm != null)
                {
                    _showTcs.SetResult(vm.Result);
                }
                else
                {
                    _showTcs.SetResult(new PopupResult<TResult>
                    {
                        Result = default(TResult),
                        PopupChoice = PopupChoice.Unknown
                    });
                }
            }

            pageToShow.Disappearing += PageToShow_Disappearing;

            await _popupNavigation.PushAsync(pageToShow, animated);
            return await _showTcs.Task;
        }
    }
}

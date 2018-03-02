using Codeco.CrossPlatform.Mvvm;
using Codeco.CrossPlatform.Popups;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Services
{
    public class NavigationService : INavigationService
    {
        private readonly List<Type> _knownPageTypes = new List<Type>();
        private readonly Dictionary<Type, Type> _viewmodelPageAssociations = new Dictionary<Type, Type>();
        private readonly NavigationHost _navHost;
        private readonly PopupHost _popupHost;

        private readonly SemaphoreSlim _knownPagesLock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _vmAssociationsLock = new SemaphoreSlim(1);

        public bool CanGoBack => _navHost.CanGoBack;

        public event EventHandler<CanGoBackChangedHandlerArgs> CanGoBackChanged;

        public NavigationService(NavigationHost navigation, PopupHost popupHost)
        {
            _navHost = navigation;
            _popupHost = popupHost;
            _navHost.CanGoBackChanged += navHost_CanGoBackChanged;
        }

        private void navHost_CanGoBackChanged(object sender, CanGoBackChangedHandlerArgs e)
        {
            CanGoBackChanged?.Invoke(sender, e);
        }

        public Type CurrentPageKey
        {
            get
            {
                lock (_knownPageTypes)
                {
                    if (_navHost.CurrentPage == null)
                    {
                        return null;
                    }

                    var pageType = _navHost.CurrentPage.GetType();

                    return _knownPageTypes.Contains(pageType)
                        ? _knownPageTypes.First(p => p == pageType)
                        : null;
                }
            }
        }

        public async Task GoBackAsync(bool animated = true)
        {
            await _navHost.GoBackAsync(animated);
        }

        /// <summary>
        /// Navigates to the <see cref="Page"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Page"/> to navigate to.</typeparam>
        /// <param name="animated"></param>
        public async Task NavigateToPageAsync<T>(bool animated = true) where T : Page
        {
            await NavigateToAsync(typeof(T), animated);
        }

        /// <summary>
        /// Navigates to the <see cref="Page"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Page"/> to navigate to.</typeparam>
        /// <param name="animated"></param>
        public async Task NavigateToPageAsync<T>(object parameter, bool animated = true) where T : Page
        {
            await NavigateToAsync(typeof(T), parameter, animated);
        }

        /// <summary>
        /// Navigates to the <see cref="INavigable"/> ViewModel.
        /// </summary>
        /// <typeparam name="T">The type of the ViewModel to navigate to.</typeparam>
        /// <param name="animated">Whether or not the page transition should be animated.</param>
        public async Task NavigateToViewModelAsync<T>(bool animated = true) where T : INavigable
        {
            Type vmType = typeof(T);
            await NavigateToViewModelAsync(vmType);
        }

        /// <summary>
        /// Navigates to the <see cref="INavigable"/> ViewModel.
        /// </summary>
        /// <typeparam name="T">The type of the ViewModel to navigate to.</typeparam>
        /// <param name="parameter">An optional parameter to send to the ViewModel.</param>
        /// <param name="animated">Whether or not the page transition should be animated.</param>
        public async Task NavigateToViewModelAsync<T>(object parameter, bool animated = true)
            where T : INavigable
        {
            Type vmType = typeof(T);
            await NavigateToViewModelAsync(vmType, parameter, animated);
        }

        /// <summary>
        /// Navigates to the <see cref="INavigable"/> ViewModel of <see cref="Type"/>.
        /// </summary>
        /// <param name="vmType">The type of the ViewModel to navigate to.</param>
        /// <param name="animated">Whether or not the page transition should be animated.</param>
        public async Task NavigateToViewModelAsync(Type vmType, bool animated = true)
        {
            await _vmAssociationsLock.WaitAsync();
            {
                Type destPage = null;
                if (!_viewmodelPageAssociations.TryGetValue(vmType, out destPage))
                {
                    throw new ArgumentException(
                        $"No such ViewModel: {vmType}. Did you forget to call NavigationService.Configure?",
                        nameof(vmType));
                }

                await NavigateToAsync(destPage, animated);
            }
            _vmAssociationsLock.Release();
        }

        /// <summary>
        /// Navigates to the <see cref="INavigable"/> ViewModel of <see cref="Type"/>.
        /// </summary>
        /// <param name="vmType">The type of the ViewModel to navigate to.</param>
        /// /// <param name="parameter">An optional parameter to send to the ViewModel.</param>
        /// <param name="animated">Whether or not the page transition should be animated.</param>
        public async Task NavigateToViewModelAsync(Type vmType, object parameter, bool animated = true)
        {
            await _vmAssociationsLock.WaitAsync();
            {
                Type destPage = null;
                if (!_viewmodelPageAssociations.TryGetValue(vmType, out destPage))
                {
                    throw new ArgumentException(
                        $"No such ViewModel: {vmType}. Did you forget to call NavigationService.Configure?",
                        nameof(vmType));
                }

                await NavigateToAsync(destPage, parameter, animated);
            }
            _vmAssociationsLock.Release();
        }

        private async Task NavigateToAsync(Type pageKey, object parameter, bool animated = true)
        {
            await _knownPagesLock.WaitAsync();
            {
                if (_knownPageTypes.Contains(pageKey))
                {
                    var type = pageKey;

                    if (parameter == null)
                    {
                        throw new ArgumentException("Navigational parameters cannot be null. Did you mean to use the method overload without a navigational parameter?");
                    }

                    ConstructorInfo constructor = type.GetTypeInfo()
                        .DeclaredConstructors
                        .FirstOrDefault(
                            c =>
                            {
                                var p = c.GetParameters();
                                return p.Length == 1 && p[0].ParameterType == parameter.GetType();
                            });

                    object[] parameters =
                    {
                        parameter
                    };

                    if (constructor == null)
                    {
                        throw new InvalidOperationException("No suitable constructor found for page " + pageKey);
                    }

                    var page = constructor.Invoke(parameters) as Page;
                    await _navHost.NavigateToAsync(page, animated);
                }
                else
                {
                    throw new ArgumentException(
                        $"No such page: {pageKey}. Did you forget to call NavigationService.Configure?",
                        nameof(pageKey));
                }
            }
            _knownPagesLock.Release();
        }

        private async Task NavigateToAsync(Type pageKey, bool animated = true)
        {
            await _knownPagesLock.WaitAsync();
            {
                if (_knownPageTypes.Contains(pageKey))
                {
                    var type = pageKey;

                    ConstructorInfo constructor = type.GetTypeInfo()
                        .DeclaredConstructors
                        .FirstOrDefault(c => !c.GetParameters().Any());

                    object[] parameters = { };

                    if (constructor == null)
                    {
                        throw new InvalidOperationException("No suitable constructor found for page " + pageKey);
                    }

                    var page = constructor.Invoke(parameters) as Page;
                    await _navHost.NavigateToAsync(page, animated);
                }
                else
                {
                    throw new ArgumentException(
                        $"No such page: {pageKey}. Did you forget to call NavigationService.Configure?",
                        nameof(pageKey));
                }
            }
            _knownPagesLock.Release();
        }

        public async Task<PopupResult> ShowPopupViewModelAsync<T>(bool animated = true) where T : INavigablePopup
        {
            await _vmAssociationsLock.WaitAsync();
            {
                if (!_viewmodelPageAssociations.TryGetValue(typeof(T), out Type pageType))
                {
                    _vmAssociationsLock.Release();

                    throw new ArgumentException(
                        $"No such ViewModel: {typeof(T)}. Did you forget to call NavigationService.Configure?",
                        nameof(T));
                }

                _vmAssociationsLock.Release();

                return await ShowPopupAsync(pageType, animated);
            }
        }

        public async Task<PopupResult> ShowPopupAsync(Type popupType, bool animated = true)
        {
            await _knownPagesLock.WaitAsync();
            {
                if (_knownPageTypes.Contains(popupType))
                {
                    ConstructorInfo constructor = popupType.GetTypeInfo()
                        .DeclaredConstructors
                        .FirstOrDefault(c => !c.GetParameters().Any());

                    if (constructor == null)
                    {
                        _knownPagesLock.Release();

                        throw new InvalidOperationException($"No suitable constructor found for popup {popupType}.");
                    }

                    var popup = constructor.Invoke(new object[] { }) as PopupPage;

                    _knownPagesLock.Release();

                    return await _popupHost.ShowAsync(popup, animated);
                }
                else
                {
                    _knownPagesLock.Release();

                    throw new ArgumentException(
                        $"No such page {popupType}. Did you forget to add it via NavigationService.Configure?",
                        nameof(popupType));
                }
            }
        }

        public async Task<PopupResult<TResult>> ShowPopupViewModelAsync<TViewModel, TResult>(bool animated = true)
            where TViewModel : INavigablePopup<TResult>
        {
            await _vmAssociationsLock.WaitAsync();
            {
                if (!_viewmodelPageAssociations.TryGetValue(typeof(TViewModel), out Type pageType))
                {
                    _vmAssociationsLock.Release();

                    throw new ArgumentException(
                       $"No such ViewModel: {typeof(TViewModel)}. Did you forget to call NavigationService.Configure?",
                       nameof(TViewModel));
                }

                _vmAssociationsLock.Release();

                return await ShowPopupAsync<TResult>(pageType, animated);
            }
        }

        public async Task<PopupResult<TResult>> ShowPopupAsync<TResult>(Type popupType, bool animated = true)
        {
            await _knownPagesLock.WaitAsync();
            {
                if (_knownPageTypes.Contains(popupType))
                {
                    ConstructorInfo constructor = popupType.GetTypeInfo()
                        .DeclaredConstructors
                        .FirstOrDefault(c => !c.GetParameters().Any());

                    if (constructor == null)
                    {
                        throw new InvalidOperationException($"No suitable constructor found for popup {popupType}.");
                    }

                    var popup = constructor.Invoke(new object[] { }) as PopupPage;

                    _knownPagesLock.Release();
                    return await _popupHost.ShowAsync<TResult>(popup, animated);
                }
                else
                {
                    _knownPagesLock.Release();

                    throw new ArgumentException(
                        $"No such page {popupType}. Did you forget to add it via NavigationService.Configure?",
                        nameof(popupType));
                }
            }
        }

        /// <summary>
        /// Associates ViewModel <see cref="Type"/>s with <see cref="Type"/>s of <see cref="Page"/>s.
        /// </summary>
        /// <param name="pageType">The Page type to associate with a viewmodel.</param>
        /// <param name="vmType">The viewmodel to associate the page with.</param>
        public NavigationService Configure(Type vmType, Type pageType)
        {
            lock (_knownPageTypes)
            {
                if (!_knownPageTypes.Contains(pageType))
                {
                    _knownPageTypes.Add(pageType);
                }
            }

            if (vmType != null)
            {
                lock (_viewmodelPageAssociations)
                {
                    if (!_viewmodelPageAssociations.ContainsKey(vmType))
                    {
                        _viewmodelPageAssociations.Add(vmType, pageType);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Removes all <see cref="Page"/>s from the NavigationStack except for the currently-displayed page.
        /// </summary>
        public void ClearBackStack()
        {
            _navHost.ClearBackStack();
        }
    }
}

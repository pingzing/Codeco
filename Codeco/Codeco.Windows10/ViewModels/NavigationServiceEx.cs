using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Codeco.Windows10.Common;

using GalaSoft.MvvmLight.Views;

namespace Codeco.Windows10.ViewModels
{
    public class NavigationServiceEx : NavigationService, INavigationServiceEx
    {                             
        public int BackStackDepth
        {
            get
            {
                var frame = (Frame) Window.Current.Content;
                return frame.BackStackDepth;
            }
        }

        private bool _oldCanGoBack = false;        
        public bool CanGoBack
        {
            get
            {               
                var frame = (Frame) Window.Current.Content;

                if (_oldCanGoBack != frame.CanGoBack)
                {
                                                        
                }
                _oldCanGoBack = frame.CanGoBack;

                return frame.CanGoBack;

            }
        }

        public void ClearBackStack()
        {
            var frame = (Frame)Window.Current.Content;
            frame.BackStack.Clear();
            RaiseCanGoBackChanged();
        }

        public void BackStackRemoveAt(int index)
        {
            var frame = (Frame)Window.Current.Content;
            frame.BackStack.RemoveAt(index);
            RaiseCanGoBackChanged();
        }

        public void BackStackRemove(PageStackEntry entry)
        {
            var frame = (Frame)Window.Current.Content;
            frame.BackStack.Remove(entry);
            RaiseCanGoBackChanged();
        }

        public PageStackEntry BackStackGet(PageStackEntry entry)
        {
            var frame = (Frame)Window.Current.Content;
            return frame.BackStack[frame.BackStack.IndexOf(entry)];
        }

        public PageStackEntry BackStackGetAt(int index)
        {
            var frame = (Frame)Window.Current.Content;
            return frame.BackStack[index];
        }

        public override void NavigateTo(string pageKey, object parameter)
        {
            base.NavigateTo(pageKey, parameter);
            RaiseCanGoBackChanged();
        }

        public event CanGoBackChangedHandler CanGoBackChanged;

        private void RaiseCanGoBackChanged()
        {
            CanGoBackChanged?.Invoke(this, new CanGoBackChangedHandlerArgs(CanGoBack));
        }      
    }

    public delegate void CanGoBackChangedHandler(object sender, CanGoBackChangedHandlerArgs args);

    public class CanGoBackChangedHandlerArgs
    {
        public bool NewCanGoBack { get; private set; }

        public CanGoBackChangedHandlerArgs(bool newCanGoBack)
        {
            NewCanGoBack = newCanGoBack;
        }
    }
}

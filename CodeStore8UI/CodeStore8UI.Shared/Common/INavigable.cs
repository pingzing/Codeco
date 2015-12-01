using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Navigation;

namespace Codeco.Common
{
    public interface INavigable
    {
        void Activate(object parameter, NavigationMode navigationMode);
        void Deactivating(object parameter);
        void Deactivated(object parameter);
        bool AllowGoingBack { get; set; }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Codeco.CrossPlatform.Mvvm
{

    public class CanGoBackChangedHandlerArgs
    {
        public bool NewCanGoBack { get; private set; }

        public CanGoBackChangedHandlerArgs(bool newCanGoBack)
        {
            NewCanGoBack = newCanGoBack;
        }
    }
}

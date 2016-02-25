using System;

namespace Codeco.Windows10.Common
{
    public class UniversalBackPressedEventArgs : EventArgs
    {
        public bool Handled { get; set; }

        public UniversalBackPressedEventArgs(bool isHandled)
        {
            Handled = isHandled;
        }
    }
}

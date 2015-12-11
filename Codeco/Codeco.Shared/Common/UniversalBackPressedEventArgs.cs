using System;
using System.Collections.Generic;
using System.Text;

namespace Codeco.Common
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

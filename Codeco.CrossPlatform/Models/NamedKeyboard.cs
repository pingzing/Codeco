using Xamarin.Forms;

namespace Codeco.CrossPlatform.Models
{
    public class NamedKeyboard
    {
        public Keyboard Keyboard { get; set; }
        public string Name { get; set; }

        public NamedKeyboard(Keyboard keyboard, string name)
        {
            Keyboard = keyboard;
            Name = name;
        }
    }
}

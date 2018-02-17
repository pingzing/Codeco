using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Converters
{
    public class ValidFileNameBehavior : Behavior<Entry>
    {
        private readonly static Regex _illegalCharsRegex = new Regex($"[{Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))}]", RegexOptions.Compiled);

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += Bindable_TextChanged;
        }

        private void Bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_illegalCharsRegex.IsMatch(e.NewTextValue))
            {
                ((Entry)sender).Text = _illegalCharsRegex.Replace(e.NewTextValue, "");
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= Bindable_TextChanged;
        }
    }
}

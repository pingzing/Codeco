using Codeco.CrossPlatform.Models;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Behaviors
{
    public class FileLocationToTextColorBehavior : Behavior<Label>
    {
        protected override void OnAttachedTo(Label bindable)
        {
            bindable.PropertyChanged += Bindable_PropertyChanged;
        }

        private void Bindable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Label label = (Label)sender;
            if (e.PropertyName == Label.TextProperty.PropertyName)
            {
                if (label.Text == FileLocation.Roamed.FolderName())
                {
                    label.SetDynamicResource(Label.TextColorProperty, "AccentColor");
                }
                else
                {
                    label.SetDynamicResource(Label.TextColorProperty, "ForegroundColor");
                }
            }
        }
    }
}

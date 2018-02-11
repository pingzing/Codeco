using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.Windows10.Services.DependencyServices;
using System.Reactive.Subjects;
using Windows.UI.ViewManagement;
using Xamarin.Forms;

[assembly: Dependency(typeof(AccentColorService))]
namespace Codeco.Windows10.Services.DependencyServices
{
    public class AccentColorService : IAccentColorService
    {

        private UISettings _uiSettings = new UISettings();

        private BehaviorSubject<Color> _platformAccentColor;
        public BehaviorSubject<Color> PlatformAccentColor => _platformAccentColor;        

        public AccentColorService()
        {
            _uiSettings.ColorValuesChanged += _uiSettings_ColorValuesChanged;
            var accentColor = _uiSettings.GetColorValue(UIColorType.Accent);            
            _platformAccentColor = new BehaviorSubject<Color>(new Color(
                (double)accentColor.R / byte.MaxValue,
                (double)accentColor.G / byte.MaxValue,
                (double)accentColor.B / byte.MaxValue,
                (double)accentColor.A / byte.MaxValue
            ));
        }

        private void _uiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            Windows.UI.Color accentColor = sender.GetColorValue(UIColorType.Accent);
            PlatformAccentColor.OnNext(new Color(
                (double)accentColor.R / byte.MaxValue,
                (double)accentColor.G / byte.MaxValue,
                (double)accentColor.B / byte.MaxValue,
                (double)accentColor.A / byte.MaxValue
            ));
        }
    }
}

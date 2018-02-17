using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Codeco.Windows10.Services.DependencyServices;
using System;
using System.Reactive.Subjects;
using Windows.UI.ViewManagement;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformColorService))]
namespace Codeco.Windows10.Services.DependencyServices
{
    public class PlatformColorService : IPlatformColorService
    {

        private UISettings _uiSettings = new UISettings();

        private BehaviorSubject<Color> _platformAccentColor;
        public IObservable<Color> PlatformAccentColor => _platformAccentColor;

        private BehaviorSubject<Color> _platformBackgroundColor;
        public IObservable<Color> PlatformBackgroundColor => _platformBackgroundColor;

        private BehaviorSubject<Color> _platformForegroundColor;
        public IObservable<Color> PlatformForegroundColor => _platformForegroundColor;

        public PlatformColorService()
        {
            _uiSettings.ColorValuesChanged += _uiSettings_ColorValuesChanged;

            // Initial values
            var accentColor = _uiSettings.GetColorValue(UIColorType.Accent);
            _platformAccentColor = new BehaviorSubject<Color>(accentColor.ToXamarinColor());

            var foregroundColor = _uiSettings.GetColorValue(UIColorType.Foreground);
            _platformForegroundColor = new BehaviorSubject<Color>(foregroundColor.ToXamarinColor());

            var backgroundColor = _uiSettings.GetColorValue(UIColorType.Background);
            _platformBackgroundColor = new BehaviorSubject<Color>(backgroundColor.ToXamarinColor());
        }

        // React to theme changes
        private void _uiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            var accentColor = sender.GetColorValue(UIColorType.Accent);
            _platformAccentColor.OnNext(accentColor.ToXamarinColor());

            var foregroundColor = _uiSettings.GetColorValue(UIColorType.Foreground);
            _platformForegroundColor.OnNext(foregroundColor.ToXamarinColor());

            var backgroundColor = _uiSettings.GetColorValue(UIColorType.Background);
            _platformBackgroundColor.OnNext(backgroundColor.ToXamarinColor());
        }        
    }

    public static class ColorExtensions
    {
        public static Color ToXamarinColor(this Windows.UI.Color color)
        {
            return new Color(
                (double)color.R / byte.MaxValue,
                (double)color.G / byte.MaxValue,
                (double)color.B / byte.MaxValue,
                (double)color.A / byte.MaxValue);
        }
    }
}

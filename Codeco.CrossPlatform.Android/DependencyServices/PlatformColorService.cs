using System.Reactive.Subjects;
using AndroidGraphics = Android.Graphics; //Avoid namespaace collision witih Codeco.CrossPlatform.Android
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Xamarin.Forms;
using static Android.Content.Res.Resources;
using Android.Util;
using System;

namespace Codeco.CrossPlatform.Droid.DependencyServices
{
    public class PlatformColorService : IPlatformColorService
    {
        public static Theme CurrentTheme { get; set; }

        private BehaviorSubject<Color> _platformAccentColor;
        public IObservable<Color> PlatformAccentColor => _platformAccentColor;

        private BehaviorSubject<Color> _platformBackgroundColor;
        public IObservable<Color> PlatformBackgroundColor => _platformBackgroundColor;

        private BehaviorSubject<Color> _platformForegroundColor;
        public IObservable<Color> PlatformForegroundColor => _platformForegroundColor;

        public PlatformColorService()
        {
            var themeAccentColor = new TypedValue();
            CurrentTheme.ResolveAttribute(Resource.Attribute.colorAccent, themeAccentColor, true);
            var accentColor = new AndroidGraphics.Color(themeAccentColor.Data);            
            _platformAccentColor = new BehaviorSubject<Color>(ToXamarinColor(accentColor));

            var themeBackground = new TypedValue();
            CurrentTheme.ResolveAttribute(Android.Resource.Attribute.WindowBackground, themeBackground, true);
            var backgroundColor = new AndroidGraphics.Color(themeBackground.Data);
            _platformBackgroundColor = new BehaviorSubject<Color>(ToXamarinColor(backgroundColor));

            var themeForeground = new TypedValue();
            CurrentTheme.ResolveAttribute(Resource.Attribute.editTextColor, themeForeground, true);
            var foregroundColor = new AndroidGraphics.Color(themeForeground.Data);
            _platformForegroundColor = new BehaviorSubject<Color>(ToXamarinColor(foregroundColor));
        }

        private Color ToXamarinColor(AndroidGraphics.Color color)
        {
            return new Color(
                color.R / byte.MaxValue,
                color.G / byte.MaxValue,
                color.B / byte.MaxValue,
                color.A / byte.MaxValue
            );
        }
    }    
}
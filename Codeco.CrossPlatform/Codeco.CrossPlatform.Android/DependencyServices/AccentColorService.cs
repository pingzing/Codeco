using System.Reactive.Subjects;
using AndroidGraphics = Android.Graphics; //Avoid namespaace collision witih Codeco.CrossPlatform.Android
using Codeco.CrossPlatform.Services.DependencyInterfaces;
using Xamarin.Forms;
using Codeco.CrossPlatform.Droid.DependencyServices;

[assembly: Dependency(typeof(AccentColorService))]
namespace Codeco.CrossPlatform.Droid.DependencyServices
{
    public class AccentColorService : IAccentColorService
    {
        public static AndroidGraphics.Color ContextAccentColor { get; set; }

        private BehaviorSubject<Color> _platformAccentColor;
        public BehaviorSubject<Color> PlatformAccentColor => _platformAccentColor;

        public AccentColorService()
        {
            var xamarinAccentColor = new Color(
                ContextAccentColor.R / byte.MaxValue,
                ContextAccentColor.G / byte.MaxValue,
                ContextAccentColor.B / byte.MaxValue,
                ContextAccentColor.A / byte.MaxValue
            );
            _platformAccentColor = new BehaviorSubject<Color>(xamarinAccentColor);
        }
    }
}
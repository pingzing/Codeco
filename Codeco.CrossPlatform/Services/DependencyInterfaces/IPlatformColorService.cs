using System;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Services.DependencyInterfaces
{
    public interface IPlatformColorService
    {
        IObservable<Color> PlatformAccentColor { get; }
        IObservable<Color> PlatformBackgroundColor { get; }        
        IObservable<Color> PlatformForegroundColor { get; }
    }
}

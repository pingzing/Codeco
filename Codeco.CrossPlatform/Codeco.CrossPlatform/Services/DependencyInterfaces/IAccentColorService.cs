using System.Reactive.Subjects;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Services.DependencyInterfaces
{
    public interface IAccentColorService
    {
        BehaviorSubject<Color> PlatformAccentColor { get; }
    }
}

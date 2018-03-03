using System.Globalization;

namespace Codeco.CrossPlatform.Services.DependencyInterfaces
{
    public interface ILocalizeService
    {
        CultureInfo GetCurrentCultureInfo();
    }
}

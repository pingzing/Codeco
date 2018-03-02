using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Codeco.Windows10.Common.Converters
{
    //A little hacky, but eh, who cares
    public class FilenameToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string filename = value as string;
            if (filename == null)
            {
                return null;
            }
            if (filename == "none")
            {
                return new SolidColorBrush(Colors.Gray);
            }
            else
            {                
                if (Application.Current.Resources.ContainsKey("SystemControlBackgroundAccentBrush"))
                {
                    return (SolidColorBrush) Application.Current.Resources["SystemControlBackgroundAccentBrush"];
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

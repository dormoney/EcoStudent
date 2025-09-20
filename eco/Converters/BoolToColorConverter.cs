using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace eco.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVerified)
            {
                return isVerified ? Brushes.Green : Brushes.Orange;
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

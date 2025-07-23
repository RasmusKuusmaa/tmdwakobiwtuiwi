using System;
using System.Globalization;
using System.Windows.Data;

namespace tmdwa.Views
{
    public class BoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? 1 : 0;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return intValue == 1;
            return false;
        }
    }
}
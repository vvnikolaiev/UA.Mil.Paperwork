using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Mil.Paperwork.WriteOff.Converters
{
    public class InversedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolValue))
                return DependencyProperty.UnsetValue;

            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolValue))
                return DependencyProperty.UnsetValue;

            return !boolValue;
        }
    }
}

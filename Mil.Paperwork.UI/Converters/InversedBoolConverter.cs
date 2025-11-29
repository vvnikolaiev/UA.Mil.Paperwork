using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Mil.Paperwork.UI.Converters
{
    public class InversedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolValue))
                return BindingOperations.DoNothing;

            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolValue))
                return BindingOperations.DoNothing;

            return !boolValue;
        }
    }
}

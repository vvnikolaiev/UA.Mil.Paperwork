using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Mil.Paperwork.UI.Converters
{
    public class ResourceKeyToGeometryConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Geometry? result = null;

            if (value is string resourceKey && Application.Current != null)
            {
                if (Application.Current.TryGetResource(resourceKey, Application.Current.ActualThemeVariant, out var resource))
                {
                    result = resource as Geometry;
                }
            }

            return result;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

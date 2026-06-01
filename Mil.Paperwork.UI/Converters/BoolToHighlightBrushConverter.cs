using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Mil.Paperwork.UI.Converters
{
    public class BoolToHighlightBrushConverter : IValueConverter
    {
        public IBrush HighlightBrush { get; set; } = new SolidColorBrush(Color.FromArgb(60, 100, 180, 255));
        public IBrush DefaultBrush { get; set; } = Brushes.Transparent;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && b ? HighlightBrush : DefaultBrush;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BindingOperations.DoNothing;
        }
    }
}

using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Mil.Paperwork.Infrastructure.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object TargetValue { get; set; }
        public bool IsReversed { get; set; }
        public bool IsHidden { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null && TargetValue != null)
            {
                return DependencyProperty.UnsetValue;
            }

            var hiddenValue = IsHidden ? Visibility.Hidden : Visibility.Collapsed;
            var isVisible = value == TargetValue || (value?.Equals(TargetValue) ?? false);

            if (IsReversed)
                isVisible = !isVisible;

            return isVisible ? Visibility.Visible : hiddenValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static readonly VisibilityConverter TrueToCollapsed = new VisibilityConverter { TargetValue = true };
        public static readonly VisibilityConverter FalseToCollapsed = new VisibilityConverter { TargetValue = false, IsReversed = true };
        public static readonly VisibilityConverter TrueToHidden = new VisibilityConverter { TargetValue = true, IsHidden = true };
        public static readonly VisibilityConverter FalseToHidden = new VisibilityConverter { TargetValue = false, IsHidden = true, IsReversed = true };

        public static readonly VisibilityConverter ZeroToCollapsed = new VisibilityConverter { TargetValue = 0, IsReversed = true };
    }
}

using Microsoft.Xaml.Behaviors;
using Mil.Paperwork.Common.Helpers;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Mil.Paperwork.WriteOff.Behaviors
{
    public class FlexibleDecimalInputBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.LostFocus += AssociatedObject_LostFocus;
            DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        }

        protected override void OnDetaching()
        {
            DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
            AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
            base.OnDetaching();
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
                // allow paste only if it parses (optional)
                if (!SimpleNumberParser.TryParse(text, out _))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = AssociatedObject;
            if (tb == null) return;

            var text = tb.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                // update source with empty string if binding accepts it, or do nothing
                var be = BindingOperations.GetBindingExpression(tb, TextBox.TextProperty);
                be?.UpdateSource();
                return;
            }

            if (SimpleNumberParser.TryParse(text, out decimal value))
            {
                // Format for display according to current UI culture
                // Use "G" or "N" depending on desired formatting; keep it simple here:

                string formatted = value.ToString(CultureInfo.InvariantCulture);

                // Set text only if it differs to avoid unnecessary changes while editing
                if (!string.Equals(tb.Text, formatted, StringComparison.Ordinal))
                    tb.Text = formatted;

                // Force update binding (Text -> source)
                var binding = BindingOperations.GetBindingExpression(tb, TextBox.TextProperty);
                binding?.UpdateSource();
            }
            else
            {
                // invalid input: leave it so user can fix; do not throw.
                // Optionally you might set validation errors here.
            }
        }
    }
}

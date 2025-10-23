using Microsoft.Xaml.Behaviors;
using Mil.Paperwork.WriteOff.Behaviors;
using System.Windows.Controls;

namespace Mil.Paperwork.WriteOff.Controls
{
    public class NumericTextBox : TextBox
    {
        public NumericTextBox()
        {
            // Attach behavior programmatically
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Add(new FlexibleDecimalInputBehavior());
        }
    }
}

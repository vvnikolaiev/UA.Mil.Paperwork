using Microsoft.Xaml.Behaviors;
using Mil.Paperwork.WriteOff.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Mil.Paperwork.WriteOff.Controls
{
    public class NumericTextColumn : DataGridTextColumn
    {
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var textBox = (TextBox)base.GenerateEditingElement(cell, dataItem);
            Interaction.GetBehaviors(textBox).Add(new FlexibleDecimalInputBehavior());
            return textBox;
        }
    }
}

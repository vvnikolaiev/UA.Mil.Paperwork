using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Mil.MVVM.Common;
using Mil.Paperwork.UI.ViewModels;
using System;

namespace Mil.Paperwork.UI
{
    public class ViewLocator : IDataTemplate
    {

        public Control? Build(object? param)
        {
            if (param is null)
            {
                return null;
            }

            var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                var control = (Control)Activator.CreateInstance(type)!;
                return control;
            }

            var result = new TextBlock { Text = "Not Found: " + name };
            return result;
        }

        public bool Match(object? data)
        {
            return data is ObservableItem;
        }
    }
}
